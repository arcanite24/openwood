# Game Architecture

## Overview

Littlewood is a Unity game built with a relatively simple architecture. The game uses a single main controller pattern where `GameScript` handles most game logic.

## Main Classes

### GameScript (MonoBehaviour)

The central game controller attached to the main game object. This is a massive class (~65,000 lines) that handles:

- Game state management
- UI management
- Inventory system
- NPC interactions
- Building/decorating
- Save/Load coordination
- Event handling
- Crafting
- Shopping
- Quest management
- And much more...

**Singleton Access**: The GameScript instance is typically accessed through `FindObjectOfType<GameScript>()` or by storing a reference.

```csharp
// Most game state is accessed through static fields
int currentDay = GameScript.day;
int playerMoney = GameScript.dew;
bool isRaining = GameScript.raining;
```

### PlayerController (MonoBehaviour)

Handles player movement, input processing, and player actions.

**Key static fields**:
```csharp
PlayerController.pos         // Current position (Vector2)
PlayerController.moving      // Is player moving
PlayerController.hasInput    // Is there input this frame
PlayerController.isSprinting // Is player sprinting
PlayerController.speed       // Movement speed
```

### NPCScript (MonoBehaviour)

Attached to each NPC in the game. Handles NPC movement, animations, and dialogue triggers.

### SaveData (Static Class)

Static utility class for save/load operations. Uses JSON serialization.

```csharp
SaveData.Save(path, gameContainer);
SaveData.Load(path);
```

### GameData (Serializable Class)

The data container that gets serialized to JSON. Contains all persistent game state.

### MenuScript (MonoBehaviour)

Handles the main menu, settings, and initial game setup.

## Game Flow

```
┌─────────────────┐
│   Main Menu     │
│  (MenuScript)   │
└────────┬────────┘
         │ New Game / Load
         ▼
┌─────────────────┐
│   Game Scene    │
│  (GameScript)   │
│                 │
│ ┌─────────────┐ │
│ │PlayerControl│ │
│ └─────────────┘ │
│ ┌─────────────┐ │
│ │  NPCScript  │ │
│ │  (per NPC)  │ │
│ └─────────────┘ │
└─────────────────┘
```

## Scene Structure

The game has 3 main scenes:
- **Scene 0**: Logo/Splash
- **Scene 1**: Main Menu
- **Scene 2**: Game World

## Important GameObjects

| GameObject | Purpose |
|------------|---------|
| `Game` | Main game controller (GameScript attached) |
| `Player` | Player character (PlayerController attached) |
| `Townsfolk` | Container for NPC objects |
| `Map` | Tilemap and world objects |
| `Canvas` | All UI elements |

## Update Loop

The game uses Unity's standard MonoBehaviour lifecycle:

1. `Awake()` - Initial setup, load config
2. `Start()` - Initialize game state, load save if exists
3. `Update()` - Per-frame game logic
4. `LateUpdate()` - Camera follow, UI updates

### GameScript Update Flow

```csharp
void Update()
{
    // Handle input
    // Update game state
    // Process player actions
    // Update NPCs
    // Check for events/triggers
    // Update UI
}
```

## Memory Layout

### Static Arrays (GameScript)

The game uses many static arrays for game state:

```csharp
// Inventory - 500 slots, indexed by item ID
private int[] inventory = new int[500];

// Townsfolk relationships - 50 NPCs max
public static int[] townsfolkLevel = new int[50];
public static int[] townsfolkHeartEXP = new int[50];
public static int[] townsfolkRomanceLvl = new int[50];

// Hobby skills - 10 hobbies
public static int[] hobbyEXP = new int[10];

// Discoveries - 1000 possible discoveries
public static int[] discoverLevel = new int[1000];

// Calendar events - 112 days (4 seasons × 4 weeks × 7 days)
public static int[] calendarEvents = new int[112];
```

## Coroutines

The game uses coroutines extensively for:
- Fade in/out transitions
- Delayed actions
- Cutscenes
- Animations
- Sleep/day transition

```csharp
// Example: Going to sleep
private IEnumerator GoToSleep()
{
    goingToSleep = true;
    CloseAllMenus();
    FadeOut();
    yield return new WaitForSeconds(0.65f);
    // Continue to next day...
}
```

## Event System

The game doesn't use a formal event system. Instead, it uses:
- Direct method calls
- Static flags for state
- Coroutines for sequences

This is why OpenWood adds a proper event system via Harmony patches.
