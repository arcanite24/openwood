# OpenWood Mod Developer Guide

Welcome to the OpenWood modding framework for Littlewood! This guide will help you create mods using the OpenWood API.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Project Setup](#project-setup)
- [OpenWood API Overview](#openwood-api-overview)
- [API Reference](#api-reference)
  - [PlayerAPI](#playerapi)
  - [TimeAPI](#timeapi)
  - [NPCAPI](#npcapi)
  - [InventoryAPI](#inventoryapi)
  - [WorldAPI](#worldapi)
  - [GameAPI](#gameapi)
  - [UIAPI](#uiapi)
- [Event System](#event-system)
- [Custom Items](#custom-items)
- [Custom UI](#custom-ui)
- [Configuration](#configuration)
- [Building and Testing](#building-and-testing)
- [Example Mod](#example-mod)

---

## Prerequisites

- Visual Studio 2022 or VS Code with C# extension
- .NET Framework 4.7.2 SDK
- BepInEx 5.x installed in your Littlewood game folder
- OpenWood.Core installed in `BepInEx/plugins/`

## Project Setup

### 1. Create a New Class Library

Create a new .NET Framework 4.7.2 Class Library project.

### 2. Add References

Add references to:
- `BepInEx/core/BepInEx.dll`
- `BepInEx/core/0Harmony.dll`
- `BepInEx/plugins/OpenWood.Core.dll`
- `Littlewood_Data/Managed/Assembly-CSharp.dll`
- `Littlewood_Data/Managed/UnityEngine.dll`
- `Littlewood_Data/Managed/UnityEngine.CoreModule.dll`

### 3. Create Plugin Class

```csharp
using BepInEx;
using OpenWood.Core;
using OpenWood.Core.API;
using OpenWood.Core.Events;

namespace MyMod
{
    [BepInPlugin("com.yourname.mymod", "My Mod", "1.0.0")]
    [BepInDependency(OpenWood.Core.PluginInfo.PLUGIN_GUID)]
    public class MyModPlugin : BaseUnityPlugin
    {
        void Awake()
        {
            Logger.LogInfo("My Mod loaded!");
            
            // Subscribe to events
            GameEvents.OnDayStart += OnDayStart;
        }

        void OnDestroy()
        {
            // Unsubscribe from events
            GameEvents.OnDayStart -= OnDayStart;
        }

        private void OnDayStart(int day)
        {
            Logger.LogInfo($"Day {day} started!");
            
            // Use the API
            PlayerAPI.AddMoney(100);
        }
    }
}
```

---

## OpenWood API Overview

OpenWood provides a clean API layer that abstracts away the game's internal implementation. Instead of using reflection and Harmony patches directly, you can use these APIs:

| API | Description |
|-----|-------------|
| `PlayerAPI` | Money, experience, movement, hobbies |
| `TimeAPI` | Day, season, year, weather |
| `NPCAPI` | Friendships, romance |
| `InventoryAPI` | Adding/removing items |
| `WorldAPI` | Tools, recipes, discoveries |
| `GameAPI` | Game state, menus, debugging |
| `GameEvents` | Subscribe to game events |

---

## API Reference

### PlayerAPI

The PlayerAPI provides control over player-related game state.

#### Properties

```csharp
// Currency
int money = PlayerAPI.Money;          // Get current money
PlayerAPI.Money = 5000;               // Set money directly

// Experience/Fatigue
int exp = PlayerAPI.DayExperience;    // Get day experience (0-100)
Vector2 pos = PlayerAPI.Position;     // Get player position
bool indoors = PlayerAPI.IsIndoors;   // Check if indoors
bool sprinting = PlayerAPI.IsSprinting;
int facing = PlayerAPI.FacingDirection;

// Toggle cheats
PlayerAPI.InfiniteMoney = true;       // Enable infinite money
PlayerAPI.FreezeTime = true;          // Prevent fatigue
PlayerAPI.SpeedHackEnabled = true;    // Enable speed hack
PlayerAPI.SpeedMultiplier = 2.0f;     // Set speed (0.5-5.0)
```

#### Methods

```csharp
// Money
PlayerAPI.AddMoney(1000);             // Add money
PlayerAPI.SetMoney(5000);             // Set money

// Experience
PlayerAPI.AddDayExperience(10);       // Add fatigue
PlayerAPI.ResetDayExperience();       // Reset to 0
PlayerAPI.MaxDayExperience();         // Set to 100 (end day)

// Movement
PlayerAPI.Teleport(5.0f, 3.0f);       // Teleport to coordinates
PlayerAPI.Teleport(position);         // Teleport to Vector2
PlayerAPI.SetSpeedMultiplier(2.0f);   // Set movement speed

// Hobbies
int exp = PlayerAPI.GetHobbyExperience(0);  // 0=Woodcutting, 1=Mining, etc.
PlayerAPI.SetHobbyExperience(0, 1000);
PlayerAPI.MaxAllHobbies();            // Max all hobby levels
```

---

### TimeAPI

The TimeAPI provides control over the game's calendar and weather.

#### Enums

```csharp
public enum Season
{
    Spring = 0,
    Summer = 1,
    Autumn = 2,
    Winter = 3
}
```

#### Properties

```csharp
int day = TimeAPI.Day;                // Current day (1-7)
int week = TimeAPI.Week;              // Current week (0-3)
int seasonIdx = TimeAPI.SeasonIndex;  // Season as int (0-3)
TimeAPI.Season season = TimeAPI.CurrentSeason;  // Season enum
int year = TimeAPI.Year;              // Current year
int played = TimeAPI.DaysPlayed;      // Total days played
bool raining = TimeAPI.IsRaining;     // Weather state
string name = TimeAPI.SeasonName;     // "Spring", "Summer", etc.
```

#### Methods

```csharp
// Advance time
TimeAPI.AdvanceDay();                 // Next day
TimeAPI.AdvanceDays(5);               // Skip 5 days
TimeAPI.AdvanceWeek();                // Skip 7 days
TimeAPI.AdvanceSeason();              // Skip 28 days

// Set date
TimeAPI.SetDate(3, TimeAPI.Season.Summer, 2);  // Day 3, Summer, Year 2
TimeAPI.SetDate(5, 1, 1);             // Day 5, Season 1, Year 1
TimeAPI.CurrentSeason = TimeAPI.Season.Winter;

// Weather
TimeAPI.ToggleRain();
TimeAPI.IsRaining = true;

// Formatting
string date = TimeAPI.GetFormattedDate();  // "Day 3, Summer, Year 2"
string short = TimeAPI.GetShortDate();     // "3/2/2"
string name = TimeAPI.GetSeasonName(1);    // "Summer"
```

---

### NPCAPI

The NPCAPI provides control over NPC relationships.

#### Enums

```csharp
public enum NPC
{
    Willow = 1,
    Dalton = 2,
    Dudley = 3,
    Laura = 4,
    Bubsy = 5,
    Ash = 6,
    Lilith = 7,
    Zana = 8,
    Eunice = 9,
    Clyde = 10,
    Wolfgang = 11,
    Iris = 12,
    August = 13,
    Mia = 14,
    Theo = 15
}
```

#### Constants

```csharp
NPCAPI.MaxFriendshipLevel  // 10
NPCAPI.MaxRomanceLevel     // 10
NPCAPI.TotalNPCCount       // 20
```

#### Methods

```csharp
// Friendship
int hearts = NPCAPI.GetFriendship(NPCAPI.NPC.Willow);
NPCAPI.SetFriendship(NPCAPI.NPC.Willow, 10);
NPCAPI.AddFriendship(NPCAPI.NPC.Dalton, 2);
NPCAPI.MaxAllFriendships();
NPCAPI.ResetAllFriendships();

// Romance
int romance = NPCAPI.GetRomance(NPCAPI.NPC.Ash);
NPCAPI.SetRomance(NPCAPI.NPC.Ash, 5);
NPCAPI.MaxAllRomance();

// Utilities
string name = NPCAPI.GetName(NPCAPI.NPC.Lilith);  // "Lilith"
bool valid = NPCAPI.IsValidNPC(5);
string summary = NPCAPI.GetRelationshipSummary(NPCAPI.NPC.Zana);
```

---

### InventoryAPI

The InventoryAPI provides methods for managing the player's inventory.

#### Item ID Constants

```csharp
// Wood (40-44)
InventoryAPI.ItemID.Wood          // 40
InventoryAPI.ItemID.Magicwood     // 41
InventoryAPI.ItemID.Goldenwood    // 42
InventoryAPI.ItemID.Almwood       // 43
InventoryAPI.ItemID.Leifwood      // 44

// Planks (60-64)
InventoryAPI.ItemID.WoodenPlank   // 60
InventoryAPI.ItemID.FancyPlank    // 61
InventoryAPI.ItemID.PerfectPlank  // 62
InventoryAPI.ItemID.DuskPlank     // 63
InventoryAPI.ItemID.DawnPlank     // 64

// Stone/Ore (80-84)
InventoryAPI.ItemID.Stone         // 80
InventoryAPI.ItemID.Magicite      // 81
InventoryAPI.ItemID.Orichalcum    // 82
InventoryAPI.ItemID.Wyvernite     // 83
InventoryAPI.ItemID.Dragalium     // 84

// Bricks (100-104)
InventoryAPI.ItemID.PlainBrick    // 100
InventoryAPI.ItemID.FancyBrick    // 101
InventoryAPI.ItemID.PerfectBrick  // 102
InventoryAPI.ItemID.MoonlightOrb  // 103
InventoryAPI.ItemID.SunlightOrb   // 104

// And more: Fruit (120+), Vegetables (140+), Fish (160+), Bugs (200+)
```

#### Methods

```csharp
// Add items
InventoryAPI.AddItem(40);                    // Add 1 wood
InventoryAPI.AddItem(40, 10);                // Add 10 wood
InventoryAPI.AddItem(InventoryAPI.ItemID.Stone, 5);

// Bulk add
InventoryAPI.AddAllWoodTypes(10);   // 10 of each wood type
InventoryAPI.AddAllStoneTypes(10);  // 10 of each stone type
InventoryAPI.AddAllPlankTypes(10);  // 10 of each plank type
InventoryAPI.AddAllBrickTypes(10);  // 10 of each brick type
InventoryAPI.AddStarterPack();      // Basic materials + 5000 dew

// Item info
string category = InventoryAPI.GetItemCategory(40);  // "Wood"
bool valid = InventoryAPI.IsValidItemId(40);
bool modItem = InventoryAPI.IsModItem(10001);
```

---

### WorldAPI

The WorldAPI provides control over world features and unlocks.

#### Enums

```csharp
public enum Tool
{
    Axe = 0,
    Pickaxe = 1,
    FishingRod = 2,
    BugNet = 3,
    WateringCan = 4,
    Hoe = 5
}
```

#### Properties

```csharp
bool indoors = WorldAPI.IsIndoors;
Vector2 pos = WorldAPI.PlayerPosition;
bool building = WorldAPI.IsInBuildMode;
bool editing = WorldAPI.IsInEditMode;
int facing = WorldAPI.FacingDirection;
```

#### Location Constants

```csharp
Vector2 town = WorldAPI.Locations.TownCenter;
Vector2 farm = WorldAPI.Locations.FarmArea;
Vector2 forest = WorldAPI.Locations.ForestEntrance;
Vector2 beach = WorldAPI.Locations.Beach;
```

#### Methods

```csharp
// Tools
WorldAPI.UnlockTool(WorldAPI.Tool.FishingRod);
WorldAPI.UnlockAllTools();
bool unlocked = WorldAPI.IsToolUnlocked(WorldAPI.Tool.Axe);

// Discovery
WorldAPI.DiscoverItem(40);         // Discover wood
WorldAPI.DiscoverAllItems();       // Discover all items
bool found = WorldAPI.IsItemDiscovered(40);

// Unlocks
WorldAPI.UnlockAllRecipes();
WorldAPI.UnlockMuseum();

// Teleportation
WorldAPI.TeleportTo(WorldAPI.Location.Town);
```

---

### GameAPI

The GameAPI provides access to general game state and debugging.

#### Properties

```csharp
// Menu states
bool menuOpen = GameAPI.IsMenuOpen;
bool talking = GameAPI.IsTalking;
bool interacting = GameAPI.IsInteracting;
bool fishing = GameAPI.IsFishing;
bool building = GameAPI.IsBuilding;
bool paused = GameAPI.IsPaused;

// Stats
int days = GameAPI.DaysPlayed;
int money = GameAPI.TotalMoney;

// Version
string version = GameAPI.OpenWoodVersion;
string guid = GameAPI.OpenWoodGUID;
```

#### Methods

```csharp
// Menu control
GameAPI.CloseAllMenus();
GameAPI.Pause();
GameAPI.Resume();
GameAPI.TogglePause();

// Input
bool canInput = GameAPI.CanPlayerReceiveInput();

// Debug
GameAPI.DumpGameState();           // Log all state to console
string summary = GameAPI.GetGameStateSummary();
GameAPI.LogVersionInfo();
```

### UIAPI

The UIAPI provides factory methods for creating native game-styled UI elements.

#### Availability

```csharp
// Check if UI API is ready (after game start)
if (UIAPI.IsReady)
{
    // Safe to create UI elements
}
```

#### Factory Methods

```csharp
// Windows
UIWindow window = UIAPI.CreateWindow("My Window", 400, 300);
UIAPI.DestroyWindow(window);
var allWindows = UIAPI.GetWindows();

// Panels
UIPanel panel = UIAPI.CreatePanel(parent, 200, 100);

// Buttons
UIButton button = UIAPI.CreateButton(parent, "Click Me", () => {
    // Click handler
});

// Labels
UILabel label = UIAPI.CreateLabel(parent, "Hello World");

// Toggles/Checkboxes
UIToggle toggle = UIAPI.CreateToggle(parent, "Enable", false, (value) => {
    // Value changed handler
});

// Sliders
UISlider slider = UIAPI.CreateSlider(parent, "Speed", 0.5f, 5f, 1f);

// Input fields
UIInputField input = UIAPI.CreateInputField(parent, "Enter text...", "");

// Scroll views
UIScrollView scroll = UIAPI.CreateScrollView(parent, 300, 200);

// Item slots
UIItemSlot slot = UIAPI.CreateItemSlot(parent, 50);

// Layout helpers
GameObject vLayout = UIAPI.CreateVerticalLayout(parent, spacing: 5);
GameObject hLayout = UIAPI.CreateHorizontalLayout(parent, spacing: 10);
```

#### Utility Methods

```csharp
// Set keyboard/controller focus
UIAPI.SetSelected(button.GameObject);

// Get game sprites
Sprite itemSprite = UIAPI.GetItemSprite(40);  // Wood
Sprite portrait = UIAPI.GetNPCPortrait(1);    // Willow

// Create solid color sprites
Sprite solid = UIAPI.CreateSolidSprite(32, 32, Color.red);
```

---

## Event System

Subscribe to game events to react to gameplay changes.

### Available Events

```csharp
// Game lifecycle
GameEvents.OnGameStart          // Action
GameEvents.OnGameSave           // Action
GameEvents.OnGameLoad           // Action

// Time
GameEvents.OnDayStart           // Action<int>  (day)
GameEvents.OnDayEnd             // Action<int>  (day)
GameEvents.OnSeasonChange       // Action<string>  (season name)

// Player
GameEvents.OnPlayerMove         // Action<float, float>  (x, y)
GameEvents.OnPlayerInteract     // Action<string>  (target)
GameEvents.OnPlayerMoneyChange  // Action<int>  (new amount)
GameEvents.OnPlayerLevelUp      // Action<int>  (new level)

// NPCs
GameEvents.OnNPCSpawn           // Action<string>  (npc id)
GameEvents.OnNPCDialogue        // Action<string, string>  (npc id, text)
GameEvents.OnNPCFriendshipChange // Action<string, int>  (npc id, level)

// Items
GameEvents.OnItemPickup         // Action<string, int>  (item id, count)
GameEvents.OnItemDrop           // Action<string, int>  (item id, count)
GameEvents.OnItemCraft          // Action<string, int>  (item id, count)
GameEvents.OnItemSell           // Action<string, int>  (item id, count)

// Buildings
GameEvents.OnBuildingPlace      // Action<string, int, int>  (id, x, y)
GameEvents.OnBuildingRemove     // Action<string, int, int>  (id, x, y)
GameEvents.OnBuildingUpgrade    // Action<string>  (building id)

// World
GameEvents.OnTileChange         // Action<int, int, string>  (x, y, type)
GameEvents.OnWeatherChange      // Action<string>  (weather)
```

### Example Usage

```csharp
public class MyMod : BaseUnityPlugin
{
    void Awake()
    {
        GameEvents.OnDayStart += OnDayStart;
        GameEvents.OnItemPickup += OnItemPickup;
        GameEvents.OnNPCFriendshipChange += OnFriendshipChange;
    }

    void OnDestroy()
    {
        // Always unsubscribe!
        GameEvents.OnDayStart -= OnDayStart;
        GameEvents.OnItemPickup -= OnItemPickup;
        GameEvents.OnNPCFriendshipChange -= OnFriendshipChange;
    }

    private void OnDayStart(int day)
    {
        Logger.LogInfo($"Good morning! It's day {day}");
        PlayerAPI.AddMoney(100);  // Daily allowance
    }

    private void OnItemPickup(string itemId, int count)
    {
        Logger.LogDebug($"Picked up {count}x item {itemId}");
    }

    private void OnFriendshipChange(string npcId, int level)
    {
        if (level >= 10)
        {
            Logger.LogInfo($"Max friendship with NPC {npcId}!");
        }
    }
}
```

### Update Callbacks

Register per-frame callbacks:

```csharp
void Awake()
{
    GameEvents.RegisterUpdate(MyUpdateLoop);
}

void OnDestroy()
{
    GameEvents.UnregisterUpdate(MyUpdateLoop);
}

void MyUpdateLoop()
{
    // Called every frame
}
```

---

## Custom Items

Register custom items using the ItemRegistry.

```csharp
using OpenWood.Core.Items;

void RegisterItems()
{
    var myItem = new ModItem
    {
        Id = "mymod_super_seed",
        Name = "Super Seed",
        Description = "A seed that grows any crop!",
        Category = "Seed",
        SellPrice = 1000,
        MaxStack = 10,
        CanGift = true,
        CanSell = true,
        
        OnUse = (item) =>
        {
            Logger.LogInfo("Super Seed planted!");
        },
        
        OnPickup = (item, count) =>
        {
            Logger.LogDebug($"Picked up {count} super seeds");
        }
    };

    ItemRegistry.Register(myItem);
}

// Query items
ModItem item = ItemRegistry.Get("mymod_super_seed");
bool exists = ItemRegistry.Exists("mymod_super_seed");
var seeds = ItemRegistry.GetByCategory("Seed");
```

---

## Custom UI

OpenWood provides two approaches for creating UI:

### Native UI API (Recommended)

The `UIAPI` creates UI elements that match the game's native look and feel, using the game's own sprites and styling. This is the recommended approach for a polished mod experience.

```csharp
using OpenWood.Core.API;
using OpenWood.Core.UI;

private UIWindow _myWindow;

void SetupUI()
{
    // Create a native-styled window
    _myWindow = UIAPI.CreateWindow("My Mod", 400, 300);
    
    // Add a vertical layout to the content area
    _myWindow.AddVerticalLayout();
    
    // Add UI elements
    UIAPI.CreateLabel(_myWindow.ContentTransform, "Hello from My Mod!")
        .AsHeader()
        .Centered();
    
    UIAPI.CreateButton(_myWindow.ContentTransform, "Add Money", () => {
        PlayerAPI.AddMoney(1000);
    });
    
    UIAPI.CreateToggle(_myWindow.ContentTransform, "Enable Feature", false, (enabled) => {
        Plugin.Log.LogInfo($"Feature: {enabled}");
    });
    
    UIAPI.CreateSlider(_myWindow.ContentTransform, "Speed", 0.5f, 5f, 1f)
        .OnValueChanged(speed => {
            PlayerAPI.SetSpeedMultiplier(speed);
        });
    
    // Hide initially
    _myWindow.Hide();
}

void Update()
{
    if (Input.GetKeyDown(KeyCode.F2))
    {
        _myWindow?.Toggle();
    }
}

void OnDestroy()
{
    UIAPI.DestroyWindow(_myWindow);
}
```

### Native UI Elements Reference

| Element | Factory Method | Description |
|---------|---------------|-------------|
| `UIWindow` | `UIAPI.CreateWindow(title, width, height)` | Draggable window with title bar |
| `UIPanel` | `UIAPI.CreatePanel(parent, width, height)` | Panel container with game styling |
| `UIButton` | `UIAPI.CreateButton(parent, text, onClick)` | Button with native styling |
| `UILabel` | `UIAPI.CreateLabel(parent, text)` | Text label with game font |
| `UIToggle` | `UIAPI.CreateToggle(parent, label, value, onChange)` | Checkbox toggle |
| `UISlider` | `UIAPI.CreateSlider(parent, label, min, max, value)` | Value slider |
| `UIInputField` | `UIAPI.CreateInputField(parent, placeholder)` | Text input |
| `UIScrollView` | `UIAPI.CreateScrollView(parent, width, height)` | Scrollable container |
| `UIItemSlot` | `UIAPI.CreateItemSlot(parent, size)` | Inventory slot with item display |

### Layout Helpers

```csharp
// Vertical layout (stacks children top to bottom)
var vertLayout = UIAPI.CreateVerticalLayout(parent, spacing: 5);

// Horizontal layout (stacks children left to right)
var horzLayout = UIAPI.CreateHorizontalLayout(parent, spacing: 10);

// Scroll view with content layout
var scrollView = UIAPI.CreateScrollView(parent, 300, 200)
    .WithVerticalLayout(spacing: 5, padding: 10);
```

### Styling Examples

```csharp
// Label styles
UIAPI.CreateLabel(parent, "Title").AsHeader().Centered();
UIAPI.CreateLabel(parent, "Value: 100").AsValue().RightAligned();
UIAPI.CreateLabel(parent, "Description").SetFontSize(12);

// Button styles
UIAPI.CreateButton(parent, "Click Me", onClick).AsSmall();
UIAPI.CreateButton(parent, "Big Button", onClick).AsLarge();

// Slider for integers
UIAPI.CreateSlider(parent, "Level", 1, 10, 5).AsWholeNumbers();

// Input validation
UIAPI.CreateInputField(parent, "Enter number").AsInteger();
```

### Item Slot Grid Example

```csharp
var scrollView = UIAPI.CreateScrollView(_myWindow.ContentTransform, 380, 200)
    .WithGridLayout(cellWidth: 50, cellHeight: 50, spacing: 5);

// Add item slots
for (int i = 0; i < 20; i++)
{
    var slot = UIAPI.CreateItemSlot(scrollView.ContentTransform)
        .SetItem(40 + i, 10)  // Item ID, count
        .OnClick((itemId, count) => {
            Plugin.Log.LogInfo($"Clicked item {itemId} x{count}");
        });
}
```

### IMGUI Approach (Legacy)

For quick prototyping, you can still use IMGUI. Create custom IMGUI windows using ModUI.

```csharp
using OpenWood.Core.UI;

private ModWindow _myWindow;

void SetupUI()
{
    _myWindow = new ModWindow(
        "mymod_debug",           // Unique ID
        "My Mod Window",         // Title
        new Rect(20, 20, 300, 200)  // Position & size
    );

    _myWindow.DrawContent = DrawMyWindow;
    _myWindow.IsDraggable = true;
    
    ModUI.RegisterWindow(_myWindow);
}

void DrawMyWindow(int windowId)
{
    GUILayout.Label($"Player Money: {PlayerAPI.Money}");
    GUILayout.Label($"Day: {TimeAPI.Day}");

    if (GUILayout.Button("Add Money"))
    {
        PlayerAPI.AddMoney(1000);
    }

    // Helper widgets
    bool toggle = ModUI.Toggle("Enable Feature", _featureEnabled);
    float slider = ModUI.Slider("Speed", _speed, 0.5f, 5f);
    string text = ModUI.TextField("Name:", _name);
}

void Update()
{
    if (Input.GetKeyDown(KeyCode.F2))
    {
        _myWindow?.Toggle();
    }
}

void OnDestroy()
{
    ModUI.UnregisterWindow(_myWindow);
}
```

### Notifications

```csharp
ModUI.ShowNotification("Item collected!", 3f);  // 3 second duration
```

---

## Configuration

Use BepInEx configuration for mod settings.

```csharp
using BepInEx.Configuration;

public class MyMod : BaseUnityPlugin
{
    private ConfigEntry<bool> _enableFeature;
    private ConfigEntry<int> _bonusAmount;
    private ConfigEntry<KeyCode> _toggleKey;

    void Awake()
    {
        _enableFeature = Config.Bind(
            "General",           // Section
            "EnableFeature",     // Key
            true,                // Default
            "Enable the main feature"  // Description
        );

        _bonusAmount = Config.Bind(
            "Gameplay",
            "BonusAmount",
            100,
            new ConfigDescription(
                "Amount of bonus money per day",
                new AcceptableValueRange<int>(0, 10000)
            )
        );

        _toggleKey = Config.Bind(
            "Controls",
            "ToggleKey",
            KeyCode.F5,
            "Key to toggle the feature"
        );
    }

    void Update()
    {
        if (Input.GetKeyDown(_toggleKey.Value))
        {
            _enableFeature.Value = !_enableFeature.Value;
        }
    }
}
```

Configuration is saved to `BepInEx/config/com.yourname.mymod.cfg`.

---

## Building and Testing

### Build

1. Build your project in Release mode
2. Copy the DLL to `Littlewood/BepInEx/plugins/`

### Enable Debug Console

Edit `BepInEx/config/BepInEx.cfg`:
```ini
[Logging.Console]
Enabled = true
```

### Logging

```csharp
Logger.LogInfo("Info message");
Logger.LogWarning("Warning message");
Logger.LogError("Error message");
Logger.LogDebug("Debug message");  // Only shows with debug enabled
```

### Check Logs

- Console: Shows in game window if enabled
- File: `BepInEx/LogOutput.log`

---

## Example Mod

See `src/OpenWood.ExampleMod/` for a complete example demonstrating:

- Plugin setup with BepInEx dependency
- Configuration with hotkeys and values
- Event subscriptions (OnDayStart, OnItemPickup, etc.)
- Custom item registration
- IMGUI debug window
- API usage for all subsystems

### Quick Start Template

```csharp
using BepInEx;
using OpenWood.Core;
using OpenWood.Core.API;
using OpenWood.Core.Events;

namespace MyAwesomeMod
{
    [BepInPlugin("com.me.awesomemod", "Awesome Mod", "1.0.0")]
    [BepInDependency(OpenWood.Core.PluginInfo.PLUGIN_GUID)]
    public class AwesomeMod : BaseUnityPlugin
    {
        void Awake()
        {
            Logger.LogInfo("Awesome Mod loaded!");
            
            // Subscribe to events
            GameEvents.OnDayStart += day => 
            {
                PlayerAPI.AddMoney(500);
                Logger.LogInfo($"Day {day}: Here's 500 dew!");
            };
            
            // Log current state
            GameAPI.LogVersionInfo();
        }
    }
}
```

---

## Tips & Best Practices

1. **Use the APIs** - Don't use reflection unless necessary
2. **Unsubscribe from events** - In OnDestroy() to prevent memory leaks
3. **Handle null states** - Game state may not be ready early in lifecycle
4. **Use configuration** - Let users customize your mod
5. **Log appropriately** - Info for important events, Debug for details
6. **Test save compatibility** - Ensure your mod doesn't corrupt saves
7. **Document your mod** - Include a README with features and hotkeys

---

## Reference Materials

- **Decompiled Code**: `decompiled/Assembly-CSharp/`
- **Game Reference Docs**: `docs/game-reference/`
- **Extracted Assets**: `extracted-assets/`
- **Example Mod**: `src/OpenWood.ExampleMod/`

---

## Getting Help

- Review the CheatMenu source as a reference implementation
- Check `GameScript.cs` in the decompiled code for game logic
- Use `GameAPI.DumpGameState()` to debug current state

Happy modding! 🎮
