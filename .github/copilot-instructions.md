# OpenWood - AI Coding Instructions

OpenWood is a BepInEx modding framework for [Littlewood](https://store.steampowered.com/app/894940/Littlewood/), a cozy Unity RPG.

## Architecture

### Design Principle: API-First

**All features must use the public modding API.** The API layer abstracts Harmony patches and game internals, providing a consistent interface for:
- Internal features (CheatMenu, debug tools)
- External mods (third-party developers)

```
┌─────────────────────────────────────────────────────────┐
│  Features (CheatMenu, ExampleMod, third-party mods)     │
├─────────────────────────────────────────────────────────┤
│  Public API Layer (PlayerAPI, TimeAPI, NPCAPI, etc.)    │
├─────────────────────────────────────────────────────────┤
│  Internal (Harmony Patches, ReflectionHelper, Events)   │
├─────────────────────────────────────────────────────────┤
│  Game (GameScript, PlayerController, SaveData)          │
└─────────────────────────────────────────────────────────┘
```

**Rules:**
1. Features like CheatMenu MUST use API classes, never direct `GameScript` access
2. Only API classes may use `ReflectionHelper` or access game internals
3. Harmony patches trigger events; APIs consume events and expose methods
4. New functionality = new/updated API methods, then feature uses API

### Core Structure
- **Plugin Entry**: [src/OpenWood.Core/Plugin.cs](../src/OpenWood.Core/Plugin.cs) - BepInEx plugin, initializes subsystems
- **Modding API**: [src/OpenWood.Core/API/](../src/OpenWood.Core/API/) - Public API for all game manipulation
- **Harmony Patches**: [src/OpenWood.Core/Patches/](../src/OpenWood.Core/Patches/) - Hook into game methods via `[HarmonyPatch]`
- **Event System**: [src/OpenWood.Core/Events/GameEvents.cs](../src/OpenWood.Core/Events/GameEvents.cs) - C# events triggered from patches
- **Cheat Menu**: [src/OpenWood.Core/Cheats/CheatMenu.cs](../src/OpenWood.Core/Cheats/CheatMenu.cs) - IMGUI overlay (F3 toggle), uses API only

### Modding API Classes

| API | Purpose | Key Methods |
|-----|---------|-------------|
| `PlayerAPI` | Money, experience, movement | `AddMoney()`, `Teleport()`, `SetSpeedMultiplier()` |
| `TimeAPI` | Day, season, weather | `AdvanceDay()`, `SetDate()`, `ToggleRain()` |
| `NPCAPI` | Friendships, romance | `SetFriendship()`, `MaxAllFriendships()` |
| `InventoryAPI` | Items | `AddItem()`, `AddStarterPack()` |
| `WorldAPI` | Tools, recipes, discoveries | `UnlockAllTools()`, `DiscoverAllItems()` |
| `GameAPI` | Game state, debug | `IsPaused`, `DumpGameState()` |

### Key Game Classes (from decompiled code)
- `GameScript` - God object with 64k+ lines, most game state as static fields (`GameScript.dew`, `GameScript.day`)
- `PlayerController` - Player movement and interaction
- `SaveData` - Save/load system
- `Dialogue` - NPC dialogue system

> ⚠️ **Never access these directly in features.** Use the API layer instead.

## Build & Development

```powershell
# Build (outputs to Littlewood/BepInEx/plugins/)
dotnet build src/OpenWood.Core/OpenWood.Core.csproj -c Release

# Decompile game code for reference
cd tools; .\decompile.ps1

# Extract game assets (textures, audio, prefabs)
cd tools; .\extract-assets.ps1
```

**Target**: .NET Framework 4.7.2 (Unity 2019.4.3f1 Mono backend)

## Using the Modding API

**Always use API classes for game manipulation.** This applies to CheatMenu, debug tools, and any new features.

```csharp
using OpenWood.Core.API;

// ✅ Correct - use API
PlayerAPI.AddMoney(1000);
TimeAPI.AdvanceDay();
NPCAPI.SetFriendship(NPCAPI.NPC.Willow, 10);
InventoryAPI.AddItem(InventoryAPI.ItemID.Wood, 10);

// ❌ Wrong - never do this in features
GameScript.dew += 1000;
GameScript.day++;
```

### Adding New Functionality

1. **Add API method** in appropriate API class (e.g., `PlayerAPI.cs`)
2. **Use reflection/game access** only inside the API method
3. **Feature calls API** - CheatMenu, mods, etc. use the new method
4. **Document** in `docs/MOD_DEVELOPER_GUIDE.md`

Example - adding a new player ability:
```csharp
// In PlayerAPI.cs
public static void SetInvincible(bool enabled)
{
    var player = GetPlayerController();
    if (player != null)
        ReflectionHelper.SetField(player, "invincible", enabled);
}

// In CheatMenu.cs - uses API only
if (GUILayout.Button("God Mode"))
    PlayerAPI.SetInvincible(true);
```

## Harmony Patching Pattern

Always patch game methods, never modify game DLLs:

```csharp
// In Patches/ folder - patches auto-discovered via PatchAll()
[HarmonyPatch(typeof(GameScript), "BeginGame")]
public static class BeginGame_Patch
{
    static void Postfix()  // After original method
    {
        GameEvents.TriggerGameStart();
    }
}

[HarmonyPatch(typeof(GameScript), "AddItem", typeof(int), typeof(int))]
public static class AddItem_Patch
{
    static void Prefix(ref int id, ref int q)  // Modify args before
    {
        Plugin.Log.LogDebug($"Adding item {id} x{q}");
    }
}
```

## Accessing Game State

> ⚠️ **Only API classes should access game state directly.** Features must use the API.

Game uses static fields extensively. For private members, use `ReflectionHelper`:

```csharp
// Inside API classes only:
GameScript.dew += 1000;              // Currency
GameScript.day                        // Current day
GameScript.curNPC                     // Current NPC being talked to

// Reflection for private fields
ReflectionHelper.GetField<int>(gameScript, "privateField");
ReflectionHelper.InvokeMethod(gameScript, "PrivateMethod", args);
```

## Event System Usage

Subscribe from mod's `Awake()`:

```csharp
GameEvents.OnDayStart += day => Logger.LogInfo($"Day {day}");
GameEvents.OnItemPickup += (itemId, count) => { /* handle */ };
GameEvents.OnNPCFriendshipChange += (npcId, level) => { /* handle */ };
```

## Item ID Ranges

| Category | Range | Example |
|----------|-------|---------|
| Wood/Planks | 40-64 | 40=Wood, 60=Wooden Planks |
| Stone/Bricks | 80-104 | 80=Stone, 100=Plain Bricks |
| Fish | 160-189 | 160=Minnow, 180=Fire Carp |
| Bugs | 200-229 | 200=Flutterfly |
| Flowers | 400-447 | Custom mod items start at 10000+ |

## UI (IMGUI)

CheatMenu demonstrates IMGUI patterns - use `OnGUI()` in MonoBehaviour:

```csharp
void OnGUI()
{
    if (!_visible) return;
    _windowRect = GUI.Window(_windowId, _windowRect, DrawWindow, "Title");
}
```

## File Conventions

- **Patches**: One file per system (`GamePatches.cs`, `ItemPatches.cs`, `PlayerPatches.cs`)
- **Events**: Internal trigger methods called from patches, public events for mods
- **Naming**: `[MethodName]_Patch` for Harmony patch classes
- **Logging**: Use `Plugin.Log.LogDebug/Info/Warning/Error()`

## Reference Materials

- Decompiled game code: `decompiled/Assembly-CSharp/` (generated by `tools/decompile.ps1`)
- Game reference docs: `docs/game-reference/` (items, NPCs, buildings, events)
- Extracted assets: `extracted-assets/` (generated by `tools/extract-assets.ps1`)
