# Decompiling Littlewood

This guide explains how to decompile the game's assemblies for reference.

## Option 1: Using dnSpy (Recommended for Windows)

dnSpy is a powerful .NET debugger and assembly editor.

### Download
- Download from: https://github.com/dnSpyEx/dnSpy/releases
- Get the `dnSpy-net-win64.zip` file

### Usage
1. Extract dnSpy
2. Run `dnSpy.exe`
3. File → Open → Navigate to `Littlewood/Littlewood_Data/Managed/`
4. Open `Assembly-CSharp.dll`
5. Browse the decompiled code in the tree view
6. To export: File → Export to Project → Select output folder

## Option 2: Using ILSpy

### GUI Version
1. Download from: https://github.com/icsharpcode/ILSpy/releases
2. Run ILSpy.exe
3. Open `Assembly-CSharp.dll`
4. File → Save Code to save decompiled source

### Command Line (requires .NET SDK)
```bash
dotnet tool install ilspycmd -g
ilspycmd "Littlewood/Littlewood_Data/Managed/Assembly-CSharp.dll" -p -o ./decompiled
```

## Option 3: Using JetBrains dotPeek (Free)

1. Download from: https://www.jetbrains.com/decompiler/
2. Install and run dotPeek
3. File → Open → `Assembly-CSharp.dll`
4. Right-click → Export to Project

## Key Files to Examine

After decompiling, look for these patterns:

### Game Managers
- `GameManager` - Main game logic
- `SaveManager` / `SaveSystem` - Save/load functionality
- `TimeManager` / `DayNightCycle` - Day/time system

### Player
- `PlayerController` - Player movement and actions
- `PlayerStats` / `PlayerData` - Player attributes
- `Inventory` / `InventoryManager` - Item management

### NPCs
- `NPC` / `NPCController` - NPC behavior
- `DialogueManager` - Conversations
- `RelationshipManager` - Friendship levels

### Items & Crafting
- `Item` / `ItemData` - Item definitions
- `CraftingManager` - Crafting system
- `ShopManager` - Buying/selling

### World
- `TileManager` / `WorldManager` - World/map system
- `BuildingManager` - Placeable structures
- `WeatherManager` - Weather system

## Updating Harmony Patches

After decompiling, update the patches in `OpenWood.Core/Patches/`:

1. Find the actual class names for game systems
2. Find the method names you want to hook
3. Update the `[HarmonyPatch]` attributes

Example - if you find the save manager is called `SaveDataManager`:

```csharp
[HarmonyPatch(typeof(SaveDataManager), "SaveGame")]
public static class SaveGame_Patch
{
    static void Postfix()
    {
        GameEvents.TriggerGameSave();
    }
}
```

## Legal Notice

⚠️ **Important**: Decompiled code is for reference only!

- Do NOT redistribute decompiled game code
- Do NOT include game code in your mods
- Use it only to understand game mechanics
- Always respect the game developer's intellectual property
