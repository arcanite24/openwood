# OpenWood Mod Developer Guide

Welcome to the OpenWood modding framework for Littlewood! This guide will help you create your first mod.

## Prerequisites

- Visual Studio 2022 or VS Code with C# extension
- .NET SDK 4.7.2 or compatible
- BepInEx installed in your Littlewood game folder

## Project Setup

### 1. Create a New Class Library

Create a new .NET Framework 4.7.2 Class Library project.

### 2. Add References

Add references to:
- `BepInEx/core/BepInEx.dll`
- `BepInEx/core/0Harmony.dll`
- `Littlewood_Data/Managed/Assembly-CSharp.dll`
- `Littlewood_Data/Managed/UnityEngine.dll`
- `Littlewood_Data/Managed/UnityEngine.CoreModule.dll`

### 3. Create Plugin Class

```csharp
using BepInEx;
using HarmonyLib;

namespace MyMod
{
    [BepInPlugin("com.yourname.mymod", "My Mod", "1.0.0")]
    [BepInDependency("com.openwood.core", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        private Harmony harmony;

        void Awake()
        {
            Logger.LogInfo("My Mod loaded!");
            
            harmony = new Harmony("com.yourname.mymod");
            harmony.PatchAll();
        }

        void OnDestroy()
        {
            harmony?.UnpatchSelf();
        }
    }
}
```

## Basic Harmony Patches

### Add Items to Player

```csharp
using HarmonyLib;

[HarmonyPatch(typeof(GameScript), "Update")]
class DebugItemsPatch
{
    static void Postfix(GameScript __instance)
    {
        // Press F5 to add items
        if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.F5))
        {
            // Use reflection to call AddItem
            var addItem = typeof(GameScript).GetMethod("AddItem", 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance);
            
            addItem.Invoke(__instance, new object[] { 100 }); // Add item ID 100
        }
    }
}
```

### Modify Money

```csharp
[HarmonyPatch(typeof(GameScript), "AddDewdrops")]
class DoubleMoney
{
    static void Prefix(ref int a)
    {
        a *= 2; // Double all money earned
    }
}
```

### Hook NPC Dialogue

```csharp
[HarmonyPatch(typeof(GameScript), "TalkToSpecificTownsfolk")]
class CustomDialogue
{
    static void Postfix(int townsfolkId)
    {
        // Log who we talked to
        UnityEngine.Debug.Log($"Talked to townsfolk {townsfolkId}");
    }
}
```

## Accessing Game State

### Read Static Fields

```csharp
// Current date
int day = GameScript.day;
int season = GameScript.season;
int year = GameScript.year;

// Currency
int money = GameScript.dew;
int exp = GameScript.dayEXP;

// Player state
bool inMenu = GameScript.menuOpen;
bool isTalking = GameScript.talking;
bool isBuilding = GameScript.building;

// Weather
bool isRaining = GameScript.raining;
```

### Access Private Fields

```csharp
using System.Reflection;

// Get private inventory
var inventoryField = typeof(GameScript)
    .GetField("inventory", BindingFlags.NonPublic | BindingFlags.Instance);
int[] inventory = inventoryField.GetValue(gameScriptInstance) as int[];
```

## Using OpenWood.Core API

If OpenWood.Core is installed, you can use its utilities:

### Event Subscriptions

```csharp
using OpenWood.Core.Events;

void Start()
{
    GameEvents.OnDayStart += OnNewDay;
    GameEvents.OnItemPickup += OnItemPickup;
}

void OnNewDay(int day, int season, int year)
{
    Logger.LogInfo($"New day: {day}/{season}/{year}");
}

void OnItemPickup(int itemId)
{
    Logger.LogInfo($"Picked up item {itemId}");
}
```

### Custom Items

```csharp
using OpenWood.Core.Items;

void RegisterItems()
{
    ItemRegistry.RegisterItem(new CustomItem
    {
        Id = 1000,
        Name = "Magic Potion",
        Description = "A mysterious potion",
        Value = 500
    });
}
```

### Asset Loading

```csharp
using OpenWood.Core.Assets;

void LoadAssets()
{
    Texture2D myTexture = AssetManager.LoadTexture("mymod", "item.png");
    Sprite mySprite = AssetManager.CreateSprite(myTexture);
}
```

### Custom UI

```csharp
using OpenWood.Core.UI;

void CreateUI()
{
    ModUI.CreateWindow("My Window", 300, 200, (windowId) =>
    {
        GUI.Label(new Rect(10, 20, 280, 30), "Hello from my mod!");
        
        if (GUI.Button(new Rect(10, 60, 100, 30), "Click Me"))
        {
            // Button clicked
        }
    });
}
```

## Common Patterns

### Toggle Feature with Hotkey

```csharp
class MyMod : BaseUnityPlugin
{
    private bool featureEnabled = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F6))
        {
            featureEnabled = !featureEnabled;
            Logger.LogInfo($"Feature: {featureEnabled}");
        }
    }
}
```

### Config File

```csharp
using BepInEx.Configuration;

class MyMod : BaseUnityPlugin
{
    private ConfigEntry<bool> enableFeature;
    private ConfigEntry<int> multiplier;

    void Awake()
    {
        enableFeature = Config.Bind("General", "EnableFeature", true, 
            "Enable the main feature");
        multiplier = Config.Bind("General", "Multiplier", 2, 
            "Multiplier value");
    }
}
```

### Save Custom Data

```csharp
using System.IO;
using UnityEngine;

[Serializable]
class MyModData
{
    public int customValue;
    public List<string> items = new List<string>();
}

void SaveModData()
{
    string path = Path.Combine(Application.persistentDataPath, "mymod.json");
    string json = JsonUtility.ToJson(modData);
    File.WriteAllText(path, json);
}

void LoadModData()
{
    string path = Path.Combine(Application.persistentDataPath, "mymod.json");
    if (File.Exists(path))
    {
        string json = File.ReadAllText(path);
        modData = JsonUtility.FromJson<MyModData>(json);
    }
}
```

## Building and Installing

### Build

1. Build your project in Release mode
2. Copy the DLL to `Littlewood/BepInEx/plugins/`

### Test

1. Launch the game
2. Check `BepInEx/LogOutput.log` for your mod's messages
3. Enable console in `BepInEx/config/BepInEx.cfg` if needed

## Debugging

### Enable BepInEx Console

Edit `BepInEx/config/BepInEx.cfg`:
```ini
[Logging.Console]
Enabled = true
```

### Log Messages

```csharp
Logger.LogInfo("Info message");
Logger.LogWarning("Warning message");
Logger.LogError("Error message");
Logger.LogDebug("Debug message");
```

### Unity Debug

```csharp
UnityEngine.Debug.Log("Message");
```

## Reference Documentation

- [Game Architecture](./game-reference/architecture.md)
- [Items System](./game-reference/items.md)
- [NPC System](./game-reference/npcs.md)
- [Player System](./game-reference/player.md)
- [Save System](./game-reference/save-system.md)
- [Time & Calendar](./game-reference/time-calendar.md)
- [Crafting System](./game-reference/crafting.md)
- [Building System](./game-reference/building.md)
- [Events & Hooks](./game-reference/events-hooks.md)

## Example Mods

See the `OpenWood.ExampleMod` project for a complete example including:
- Custom item registration
- Debug UI window (F2 key)
- Event subscriptions
- Harmony patches

## Tips

1. **Always use Harmony for patches** - Don't modify game files directly
2. **Handle null references** - Game state may not be ready in early hooks
3. **Test save compatibility** - Ensure your mod doesn't corrupt saves
4. **Use soft dependencies** - Don't require other mods unless necessary
5. **Log your actions** - Makes debugging easier
6. **Clean up on destroy** - Unpatch Harmony and unsubscribe events

## Getting Help

- Check the decompiled source in `decompiled/Assembly-CSharp/`
- Review `GameScript.cs` for most game logic
- Use dnSpy for runtime debugging

Happy modding!
