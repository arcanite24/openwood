# Save System

## Overview

Littlewood uses Unity's `JsonUtility` for serialization. Save data is stored in a JSON file with a container structure.

## Core Classes

### SaveData (Static)

The `SaveData` class handles loading/saving operations:

```csharp
public class SaveData
{
    // Current game container
    public static GameContainer gameContainer = new GameContainer();

    // Events for save/load hooks
    public static event SerializeAction OnLoaded;
    public static event SerializeAction OnBeforeSave;
}
```

### GameContainer

Wrapper for multiple save slots:

```csharp
[Serializable]
public class GameContainer
{
    public List<GameData> games = new List<GameData>();
}
```

### GameData

The main save data class containing all game state. All fields are serializable.

## Save/Load Methods

### Loading
```csharp
public static void Load(string path)
{
    gameContainer = LoadGames(path);  // Deserialize JSON
    // Triggers OnLoaded event
    SaveData.OnLoaded();
    ClearGameList();
}

private static GameContainer LoadGames(string path)
{
    return JsonUtility.FromJson<GameContainer>(File.ReadAllText(path));
}
```

### Saving
```csharp
public static void Save(string path, GameContainer games)
{
    SaveData.OnBeforeSave();  // Triggers before save event
    SaveGames(path, games);
    ClearGameList();
}

private static void SaveGames(string path, GameContainer games)
{
    string contents = JsonUtility.ToJson(games);
    File.CreateText(path).Close();
    File.WriteAllText(path, contents);
}
```

## Save File Location

Default save path (Windows):
```
%USERPROFILE%/AppData/LocalLow/SmashGames/Littlewood/
```

Save file: `savegame.sav` (JSON format)

## GameData Fields Reference

### Player Identity
```csharp
public string playerName;    // Character name
public string townName;      // Town name
public int skin;             // Skin color
public int hair;             // Hair style
public int color;            // Hair color
public int outfit;           // Outfit style
public int type;             // Character type
```

### Time
```csharp
public int day;              // Day of week (0-6)
public int week;             // Week of season
public int season;           // Season (0-3)
public int year;             // Current year
public int daysPlayed;       // Total days played
```

### Currency & Progress
```csharp
public int dew;              // Dewdrops (money)
public int blankCards;       // Blank cards owned
public int dayEXP;           // Today's experience
public int totalCards;       // Total cards collected
public int unlockedPowers;   // Unlocked powers
public int maxDayEXP;        // Max daily XP
```

### Inventory (500 slots)
```csharp
public int[] inventory;         // Item IDs (see items.md)
public int[] inventorySOLD;     // Items sold count
```

### Town Building
```csharp
public int[] objects;           // Placed object IDs
public Vector2[] objectLocations;  // Object positions
public int[] floorObjects;      // Floor tile IDs
public Vector2[] floorObjectLocations; // Floor positions
```

### Structures & Upgrades
```csharp
public int[] structureList;     // Structure types
public Vector2[] structurePositions; // Structure positions
public int[] structureLevel;    // Upgrade levels
```

### NPC Relationships (50 slots)
```csharp
public int[] townsfolkLevel;    // Friendship level (0-10)
public int[] townsfolkRomanceLvl;   // Romance level
public int[] townsfolkHeartEXP;     // EXP to next level
public bool[] townsfolkDateable;    // Can be dated
public Vector2[] townsfolkPositions; // NPC positions
```

### Quest System
```csharp
public int[] questId;           // Active quest IDs
public int[] questValue;        // Quest progress
```

### Unlocks & Collections
```csharp
public int[] museumFish;        // Fish donated
public int[] museumBug;         // Bugs donated
public int[] museumFlower;      // Flowers donated
public int[] museumGem;         // Gems donated
public int[] cardUnlocked;      // Card collection
public int[] recipeUnlocked;    // Unlocked recipes
```

### Player House
```csharp
public int playerHouseWall;     // Wall style
public int playerHouseRoof;     // Roof style
public int[] wallpaper;         // Wallpaper per room
public int[] flooring;          // Floor per room
```

### Pets
```csharp
public int[] adoptedPet;        // Pet types
public string[] petName;        // Pet names
public Vector2[] adoptedPetPositions; // Pet locations
public int initAdoptedPet;      // Starting pet
```

### Farm Animals
```csharp
public int[] farmAnimal;        // Animal types
public int[] farmAnimalLevel;   // Animal levels
public string[] farmAnimalName; // Animal names
public bool[] farmAnimalHeart;  // Daily heart status
```

### Marriage
```csharp
public bool isMarried;
public bool isEngaged;
public string engagedTownsfolkName;
```

## Modding the Save System

### Hook Save Events
```csharp
[HarmonyPatch(typeof(SaveData))]
class SavePatch
{
    [HarmonyPatch("Load")]
    [HarmonyPostfix]
    static void AfterLoad(string path)
    {
        // Game just loaded
        var games = SaveData.gameContainer.games;
        if (games.Count > 0)
        {
            var data = games[0];
            // Access save data
        }
    }

    [HarmonyPatch("Save")]
    [HarmonyPrefix]
    static void BeforeSave(string path, GameContainer games)
    {
        // About to save
    }
}
```

### Access Current Save Data

During gameplay, most data is copied to `GameScript` static fields:
```csharp
// Get current money
int money = GameScript.dew;

// Get current day
int day = GameScript.day;
int season = GameScript.season;
int year = GameScript.year;
```

### Modify Inventory
```csharp
// Hook the AddItem method
[HarmonyPatch(typeof(GameScript), "AddItem")]
class InventoryPatch
{
    static void Postfix(int itemId)
    {
        // Item was added
    }
}
```

## Save Data Lifecycle

1. **Game Start**: `SaveData.Load()` called with save path
2. **OnLoaded Event**: Systems read from `GameData` into static fields
3. **During Play**: Game modifies static fields in `GameScript`
4. **Before Save**: `OnBeforeSave` event - systems write back to `GameData`
5. **Save**: `SaveData.Save()` serializes to JSON

## Custom Save Data (Mods)

To add custom mod data, you can:

1. **Separate File**: Save mod data in a separate JSON file
2. **Hook Events**: Use `OnLoaded` and `OnBeforeSave` events

```csharp
// Example: Custom mod save
public class MyModData
{
    public int customValue;
    public List<string> customList = new List<string>();
}

// In your mod plugin:
void OnEnable()
{
    SaveData.OnLoaded += LoadModData;
    SaveData.OnBeforeSave += SaveModData;
}

void LoadModData()
{
    string modPath = Path.Combine(Application.persistentDataPath, "mymod.json");
    if (File.Exists(modPath))
    {
        modData = JsonUtility.FromJson<MyModData>(File.ReadAllText(modPath));
    }
}

void SaveModData()
{
    string modPath = Path.Combine(Application.persistentDataPath, "mymod.json");
    File.WriteAllText(modPath, JsonUtility.ToJson(modData));
}
```

## Warning: Direct GameData Modification

Modifying `GameData` fields directly is risky because:
- Data is copied to/from `GameScript` at load/save
- Changes might be overwritten
- Always prefer hooking `GameScript` methods
