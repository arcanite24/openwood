# Littlewood Game Reference

This is a comprehensive reference for the Littlewood game internals, designed to help mod developers understand the game's systems without needing to decompile the code themselves.

## Table of Contents

1. [Game Architecture](./architecture.md)
2. [Game State & Variables](./game-state.md)
3. [Item System](./items.md)
4. [NPC & Townsfolk System](./npcs.md)
5. [Player System](./player.md)
6. [Save/Load System](./save-system.md)
7. [Time & Calendar](./time-calendar.md)
8. [Crafting & Recipes](./crafting.md)
9. [Building & Town](./building.md)
10. [Events & Hooks](./events-hooks.md)

## Quick Reference

### Main Classes

| Class | Purpose |
|-------|---------|
| `GameScript` | Main game controller (MonoBehaviour on main game object) |
| `PlayerController` | Player movement, input, and actions |
| `NPCScript` | NPC behavior, dialogue, and interactions |
| `SaveData` | Static class for save/load operations |
| `GameData` | Serializable save data container |
| `MenuScript` | Main menu and UI management |

### Key Static Fields (GameScript)

```csharp
// Time
GameScript.day          // Current day of the week (0-6)
GameScript.week         // Current week number
GameScript.season       // Current season (0-3: Spring, Summer, Fall, Winter)
GameScript.year         // Current year
GameScript.daysPlayed   // Total days played

// Currency
GameScript.dew          // Current dewdrops (money)
GameScript.actualDew    // Actual dewdrops (may differ from displayed)
GameScript.dayEXP       // Experience earned today

// Player State
GameScript.playerPos    // Player position (Vector2)
GameScript.facingDir    // Direction player is facing (0-3)
GameScript.isIndoor     // Whether player is indoors
GameScript.curLocation  // Current location name (string)

// Game State
GameScript.menuOpen     // Whether any menu is open
GameScript.talking      // Whether player is in dialogue
GameScript.interacting  // Whether player is interacting with something
GameScript.building     // Whether in build mode
GameScript.inEditMode   // Whether in edit/decoration mode
GameScript.fishing      // Whether player is fishing
GameScript.raining      // Whether it's raining
```

### Item ID Ranges

| Range | Category |
|-------|----------|
| 0-39 | Ground/Terrain items |
| 40-59 | Wood types |
| 60-79 | Planks |
| 80-99 | Ores/Stones |
| 100-119 | Bricks/Orbs |
| 120-139 | Fruits |
| 140-159 | Vegetables |
| 160-199 | Fish |
| 200-239 | Bugs |
| 240-279 | Farm products & gatherable items |
| 280-339 | Special/Quest items |
| 340-399 | Cooked food & recipes |

### Townsfolk IDs

| ID | Name | Romanceable |
|----|------|-------------|
| 1 | Willow | Yes |
| 2 | Dalton | Yes |
| 3 | Dudley | No |
| 4 | Laura | Yes |
| 5 | Bubsy | Yes |
| 6 | Ash | Yes |
| 7 | Lilith | Yes |
| 8 | Zana | No |
| 9 | Terric | Yes |
| 10 | Mel | No |
| 11 | Maximilian | Yes |
| 12 | Iris | Yes |
| 13 | Dark | No |
| 14 | Arpeggio | No |
| 15 | Toby | No |
| 24+ | Special NPCs (Gobby, Mayor, etc.) |

## Unity Version

- **Unity Version**: 2019.4.3f1
- **Scripting Backend**: Mono
- **Target Platform**: Windows x64
