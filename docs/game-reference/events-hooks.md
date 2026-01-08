# Events & Hooks Reference

## Overview

This document lists all the key methods and events in Littlewood that mod developers can hook using Harmony patches.

## Save System Events

### SaveData Events

```csharp
// Built-in events
public static event SerializeAction OnLoaded;
public static event SerializeAction OnBeforeSave;
```

Subscribe to these for save/load notifications:

```csharp
SaveData.OnLoaded += () => {
    // Game just loaded
};

SaveData.OnBeforeSave += () => {
    // About to save
};
```

## Game Lifecycle Hooks

### Game Initialization

```csharp
// GameScript.Start() - Game initialization
[HarmonyPatch(typeof(GameScript), "Start")]
class GameStartPatch
{
    static void Postfix(GameScript __instance) { }
}

// GameScript.Update() - Every frame
[HarmonyPatch(typeof(GameScript), "Update")]
class GameUpdatePatch
{
    static void Postfix() { }
}
```

### Day Cycle

```csharp
// New day started
[HarmonyPatch(typeof(GameScript), "NewDay")]
class NewDayPatch
{
    static void Postfix() { }
}

// Day date incremented
[HarmonyPatch(typeof(GameScript), "IncreaseDate")]
class IncreaseDatePatch
{
    static void Postfix() { }
}

// End of day processing
// Method: NewDayMenu (IEnumerator)
// Method: SecondPartOfNewDay (IEnumerator)
```

## Player Hooks

### Movement

```csharp
// Player movement update
[HarmonyPatch(typeof(PlayerController), "Update")]
class PlayerUpdatePatch
{
    static void Postfix() 
    {
        if (PlayerController.moving)
        {
            Vector2 pos = PlayerController.pos;
        }
    }
}
```

### Player Actions

```csharp
// Player interacting with objects
[HarmonyPatch(typeof(GameScript), "Interact")]
class InteractPatch
{
    static void Prefix() { }  // Before interaction
    static void Postfix() { } // After interaction
}

// Player fishing
[HarmonyPatch(typeof(GameScript), "StartFishing")]
class FishingPatch
{
    static void Postfix() { }
}
```

## Inventory Hooks

### Adding Items

```csharp
// Item added to inventory
[HarmonyPatch(typeof(GameScript), "AddItem")]
class AddItemPatch
{
    static void Postfix(int itemId)
    {
        // itemId was added to inventory
    }
}

// Multiple items added
[HarmonyPatch(typeof(GameScript), "AddItems")]
class AddItemsPatch
{
    static void Postfix(int itemId, int count) { }
}
```

### Currency

```csharp
// Dewdrops added
[HarmonyPatch(typeof(GameScript), "AddDewdrops")]
class AddDewdropsPatch
{
    static void Postfix(int a)
    {
        // 'a' dewdrops were added
        // GameScript.dew has new total
    }
}

// Dewdrops removed
[HarmonyPatch(typeof(GameScript), "RemoveDewdrops")]
class RemoveDewdropsPatch
{
    static void Postfix(int a) { }
}
```

### Experience

```csharp
// Day experience added
[HarmonyPatch(typeof(GameScript), "AddDayEXP")]
class AddDayEXPPatch
{
    static void Postfix(int a)
    {
        // Experience added
        // GameScript.dayEXP has new total
    }
}
```

## NPC Hooks

### Dialogue

```csharp
// Talking to NPC
[HarmonyPatch(typeof(GameScript), "TalkToSpecificTownsfolk")]
class TalkPatch
{
    static void Prefix(int townsfolkId) { }
    static void Postfix(int townsfolkId) { }
}

// Dialogue shown
[HarmonyPatch(typeof(GameScript), "ShowDialogue")]
class DialoguePatch
{
    static void Prefix(string text) { }
}
```

### Relationships

```csharp
// Heart EXP added to townsfolk
[HarmonyPatch(typeof(GameScript), "AddHeartEXP")]
class HeartEXPPatch
{
    static void Postfix(int townsfolkId, int amount) { }
}
```

## Crafting Hooks

### Recipes

```csharp
// Recipe population (game start)
[HarmonyPatch(typeof(GameScript), "PopulateValidRecipes")]
class RecipePopulatePatch
{
    static void Postfix(GameScript __instance)
    {
        // Add custom recipes here
    }
}

// Cooking completed
// Find the cooking method and patch
```

### Crafting Workshop

```csharp
// Item crafted
// Hook the crafting completion method
```

## Building Hooks

### Object Placement

```csharp
// Object placed
[HarmonyPatch(typeof(GameScript), "PlaceObject")]
class PlaceObjectPatch
{
    static void Postfix() { }
}

// Object placement cancelled
[HarmonyPatch(typeof(GameScript), "PlaceObjectCancel")]
class PlaceCancelPatch
{
    static void Postfix() { }
}
```

### Build Mode

```csharp
// Build menu option hovered
[HarmonyPatch(typeof(GameScript), "HoverBuildOption")]
class BuildHoverPatch
{
    static void Postfix(int a) { }
}
```

## Weather Hooks

```csharp
// Rain started
[HarmonyPatch(typeof(GameScript), "RainStart")]
class RainStartPatch
{
    static void Postfix() { }
}

// Rain stopped
[HarmonyPatch(typeof(GameScript), "RainStop")]
class RainStopPatch
{
    static void Postfix() { }
}
```

## Calendar Hooks

```csharp
// Calendar events set
[HarmonyPatch(typeof(GameScript), "SetCalendarEvents")]
class CalendarPatch
{
    static void Postfix()
    {
        // Modify GameScript.calendarEvents[]
    }
}

// Get today's event
[HarmonyPatch(typeof(GameScript), "GetTodaysEvent")]
class TodayEventPatch
{
    static void Postfix(ref int __result)
    {
        // __result contains the event ID
        // Modify to change today's event
    }
}
```

## UI Hooks

### Menus

```csharp
// Notification shown
[HarmonyPatch(typeof(GameScript), "NotificationMessage")]
class NotificationPatch
{
    static void Prefix(string text)
    {
        // Notification about to show
    }
}

// Menu opened/closed
// Hook specific menu methods
```

## Useful Harmony Patterns

### Prefix - Modify Input

```csharp
[HarmonyPatch(typeof(GameScript), "AddItem")]
class ModifyItemPatch
{
    static void Prefix(ref int itemId)
    {
        // Change the item being added
        if (itemId == 5) itemId = 10;
    }
}
```

### Prefix - Cancel Method

```csharp
[HarmonyPatch(typeof(GameScript), "RemoveDewdrops")]
class PreventSpendingPatch
{
    static bool Prefix(int a)
    {
        // Return false to skip original method
        return a < 1000;  // Only allow small purchases
    }
}
```

### Postfix - Modify Output

```csharp
[HarmonyPatch(typeof(GameScript), "GetItemName")]
class ItemNamePatch
{
    static void Postfix(int itemId, ref string __result)
    {
        // Modify returned item name
        __result = "Custom: " + __result;
    }
}
```

### Accessing Private Fields

```csharp
[HarmonyPatch(typeof(GameScript), "Update")]
class InventoryAccessPatch
{
    static void Postfix(GameScript __instance)
    {
        // Access private inventory
        var inventoryField = typeof(GameScript)
            .GetField("inventory", BindingFlags.NonPublic | BindingFlags.Instance);
        int[] inventory = inventoryField.GetValue(__instance) as int[];
    }
}
```

### Transpiler - IL Modification

```csharp
[HarmonyPatch(typeof(GameScript), "AddDayEXP")]
class DoubleXPPatch
{
    static IEnumerable<CodeInstruction> Transpiler(
        IEnumerable<CodeInstruction> instructions)
    {
        // Advanced: Modify IL directly
        foreach (var instruction in instructions)
        {
            yield return instruction;
        }
    }
}
```

## Key Static Fields to Monitor

```csharp
// Player State
GameScript.menuOpen
GameScript.talking
GameScript.interacting
GameScript.building
GameScript.inEditMode
GameScript.fishing

// Time
GameScript.day
GameScript.week
GameScript.season
GameScript.year

// Location
GameScript.isIndoor
GameScript.curLocation

// Weather
GameScript.raining

// Currency
GameScript.dew
GameScript.dayEXP
```

## Common Hook Combinations

### Track All Item Acquisitions

```csharp
[HarmonyPatch(typeof(GameScript), "AddItem")]
class ItemTracker1 { static void Postfix(int id) { } }

[HarmonyPatch(typeof(GameScript), "AddItems")]
class ItemTracker2 { static void Postfix(int id, int count) { } }
```

### Complete Day Cycle Hook

```csharp
// End of day
[HarmonyPatch(typeof(GameScript), "IncreaseDate")]
class DayEnd { static void Prefix() { } }

// Start of day
[HarmonyPatch(typeof(GameScript), "NewDay")]
class DayStart { static void Postfix() { } }
```

### Full Save/Load Tracking

```csharp
void OnEnable()
{
    SaveData.OnLoaded += OnGameLoaded;
    SaveData.OnBeforeSave += OnBeforeSave;
}

void OnGameLoaded() { }
void OnBeforeSave() { }
```
