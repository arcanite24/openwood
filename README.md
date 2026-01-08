# OpenWood

A modding framework for [Littlewood](https://store.steampowered.com/app/894940/Littlewood/) built on BepInEx.

**Status**: Preview  
**Unity**: 2019.4.3f1  
**BepInEx**: 5.4.23  

---

## Features

### Cheat Menu (F3)
- **Player**: Add/set Dewdrops, manage EXP, speed multiplier (1x-5x), freeze fatigue
- **Items**: Add items by ID, quick-add buttons for common materials
- **Time**: Change day/season/year, toggle rain, advance day
- **NPCs**: Set friendship levels
- **World**: Weather and season control
- **Teleport**: Quick travel to locations
- **Debug**: Runtime state inspection

### Modding API
- Harmony-based patching
- Reflection utilities for game internals
- Event hooks for game systems
- Save data helpers

### Documentation
- Complete item ID reference (400+ items)
- NPC and relationship system
- Building and crafting data
- Event triggers
- Save file format

---

## Requirements

- Littlewood (Steam)
- .NET SDK 8.0+ (for building)
- Windows 10/11

## Installation

1. Download the latest release from [Releases](https://github.com/your-username/openwood/releases)

2. Extract to your Littlewood folder:
   ```
   Steam/steamapps/common/Littlewood/
   ├── BepInEx/
   │   ├── core/
   │   ├── plugins/
   │   │   └── OpenWood.Core.dll
   │   └── config/
   ├── Littlewood.exe
   └── ...
   ```

3. Run the game. Press **F3** to open the cheat menu.

## Building from Source

```powershell
git clone https://github.com/your-username/openwood.git
cd openwood
dotnet build src/OpenWood.Core/OpenWood.Core.csproj -c Release
```

The DLL is copied to `Littlewood/BepInEx/plugins/` automatically.

---

## Documentation

| Document | Description |
|----------|-------------|
| [Creating Mods](docs/creating-mods.md) | Mod creation guide |
| [Items](docs/game-reference/items.md) | Item ID reference (400+ items) |
| [NPCs](docs/game-reference/npcs.md) | Villagers and friendship |
| [Buildings](docs/game-reference/buildings.md) | Recipes and construction |
| [Time](docs/game-reference/time.md) | Calendar and weather |
| [Events](docs/game-reference/events.md) | Cutscenes and triggers |
| [Save Data](docs/game-reference/save-data.md) | Save format |
| [Player](docs/game-reference/player.md) | Player controller |
| [UI](docs/game-reference/ui.md) | Interface hooks |

---

## Item ID Reference

| Category | Range | Examples |
|----------|-------|----------|
| Wood | 40-44 | Wood, Magicwood, Goldenwood, Almwood, Leifwood |
| Planks | 60-64 | Wooden, Fancy, Perfect, Dusk, Dawn |
| Stone | 80-84 | Stone, Magicite, Orichalcum, Wyvernite, Dragalium |
| Bricks | 100-104 | Plain, Fancy, Perfect, Moonlight Orb, Sunlight Orb |
| Fruits | 120-132 | Slimeapple, Plumberry, Peachot, Goldenbell |
| Vegetables | 140-152 | Carrot, Cabbage, Potato, Corn |
| Fish | 160-189 | Minnow, Trout, Fire Carp, Golden Tuna |
| Bugs | 200-229 | Flutterfly, Monarch, Ladybug, Golden Titan |
| Misc | 240-296 | Egg, Milk, Fleece, Dust, Honeycomb |
| Flowers | 400-447 | Lollypops, Zigzags, Dewcaps, Floofs |

Full list: [items.md](docs/game-reference/items.md)

---

## Cheat Menu Controls

| Key | Action |
|-----|--------|
| F3 | Toggle menu |
| Q / E | Switch tabs |
| Up / Down | Scroll |
| 1 / 2 / 3 | Quick actions |

Quick actions vary by tab:
- **Player**: +1000 Dew, +10000 Dew, Reset EXP
- **Items**: +10 Wood, +10 Stone, +10 Planks
- **Time**: Next Day, Toggle Rain

---

## Creating Mods

```csharp
using BepInEx;
using HarmonyLib;
using UnityEngine;

[BepInPlugin("com.yourname.mymod", "My Mod", "1.0.0")]
public class MyMod : BaseUnityPlugin
{
    void Awake()
    {
        Logger.LogInfo("Mod loaded");
        new Harmony("com.yourname.mymod").PatchAll();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            GameScript.dew += 1000;
        }
    }
}
```

See [Creating Mods](docs/creating-mods.md) for details.

---

## Project Structure

```
openwood/
├── src/
│   ├── OpenWood.Core/           # Core framework
│   │   ├── Plugin.cs            # Entry point
│   │   ├── Cheats/              # Cheat menu
│   │   ├── Patches/             # Harmony patches
│   │   └── Utilities/           # Helpers
│   └── OpenWood.ExampleMod/     # Example mod
├── docs/
│   ├── creating-mods.md
│   └── game-reference/
├── tools/                       # Development tools
│   ├── decompile.ps1            # Decompile game DLLs
│   ├── extract-assets.ps1       # Extract game assets
│   └── setup-bepinex.ps1        # Setup BepInEx
├── lib/                         # Game DLLs (not included)
├── decompiled/                  # Decompiled source (not included)
└── extracted-assets/            # Extracted assets (not included)
```

---

## Development Tools

### Asset Extraction

Extract game assets (textures, audio, prefabs, etc.) for reference:

```powershell
cd tools
.\extract-assets.ps1
```

This downloads [AssetRipper](https://github.com/AssetRipper/AssetRipper) and launches it to extract:
- **Textures**: Sprites, UI elements, tiles, character art
- **Audio**: Music, sound effects
- **Prefabs**: Game objects, NPCs, items
- **Scenes**: Game levels and maps
- **ScriptableObjects**: Game data (items, recipes, dialogues)
- **Animations**: Character and object animations

### Decompilation

Decompile game code for reference:

```powershell
cd tools
.\decompile.ps1
```

---

## Technical Details

| Property | Value |
|----------|-------|
| Game | Littlewood |
| Developer | Sean Young (SmashGames) |
| Steam App ID | 894940 |
| Unity | 2019.4.3f1 |
| Scripting | Mono |
| Main Assembly | Assembly-CSharp.dll (~65,000 lines) |
| Key Classes | GameScript, PlayerController, SaveData, Dialogue |

---

## Contributing

Bug reports, feature requests, and pull requests are welcome.

---

## Disclaimer

This is an unofficial fan project. Not affiliated with or endorsed by SmashGames.

---

## License

MIT License. See [LICENSE](LICENSE).

---

## Acknowledgments

Thanks to **Sean Young** for creating Littlewood. This project exists because the game resonated with so many players. If you enjoy using OpenWood, consider supporting the developer by [buying the game](https://store.steampowered.com/app/894940/Littlewood/).
