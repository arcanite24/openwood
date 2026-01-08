# Building & Town System

## Overview

The building system allows players to decorate their town, place structures, and customize interiors.

## Build Mode State

```csharp
public static bool building;       // In build mode
public static bool inEditMode;     // In edit/decoration mode
public static bool placingObject;  // Currently placing an object
public static bool pickingUpObject; // Picking up an object
public static int curBuildPage;    // Current build category page
```

## Build Menu

### Build Options UI

```csharp
public GameObject menuBuildOptions;
public GameObject menuBuildOptionsTop;
public GameObject buildOptionBar;
public TMP_Text txtBuildOptionName;
public Image[] buildOptionIcon = new Image[7];
public TMP_Text[] txtBuildOption = new TMP_Text[7];
public GameObject[] bBuildOption = new GameObject[7];
public GameObject[] buildOptionTopButton = new GameObject[7];
private int curBuildOption;
```

### Build Page Structure

Build items are organized by category and page:

| Category | Page 0 | Page 1 | Page 2 | Page 3 | Page 4 |
|----------|--------|--------|--------|--------|--------|
| Category 2 | 240-279 | 280-319 | - | - | - |
| Category 3 | 40-79 | 80-119 | 800+ | - | - |
| Category 4 | 120-159 | 160-199 | 200-239 | 640-679 | 680-719 |

Indoor categories:
| Category | Page 0 | Page 1 | Page 2 | Page 3 |
|----------|--------|--------|--------|--------|
| Category 3 | 320-359 | 360-399 | 400-439 | 440-479 |
| Category 4 | 480-519 | 520-559 | 560-599 | 600-639 |

## Object Placement

### Placement Methods

```csharp
private void PlaceObject();           // Start object placement
private IEnumerator PlaceObject2();   // Placement coroutine
private void PlaceObjectCancel();     // Cancel placement
```

### Placement State

```csharp
public GameObject tileHoverObj;       // Hover indicator
private bool canPlaceTile;            // Can place at current position
private bool overEdge;                // Over town edge
private bool hoveringOverObj;         // Hovering over existing object
private Vector3 lastCheckedPos;       // Last checked position
public GameObject pickedUpObj;        // Currently held object
private Vector2 pickupLastPos;        // Position before pickup
```

### Placement Validation

The game checks:
- Collision with existing objects
- Edge boundaries
- Indoor/outdoor restrictions
- Elevation levels
- Adjacent tiles

## Town Objects

### Object Storage

Objects placed in town are saved in arrays:

```csharp
// In GameData
public int[] objects;              // Object type IDs
public Vector2[] objectLocations;  // Object positions
public int[] floorObjects;         // Floor tile IDs
public Vector2[] floorObjectLocations; // Floor positions
```

### Object Sprites

```csharp
private Sprite[] buildIconSprite;      // Build menu icons
private Sprite[] buildTopSprite;       // Top category icons
private Sprite[] buildOptionTopSprite; // Build option sprites
```

## Structures

Structures are special buildings that can be upgraded:

```csharp
// In GameData
public int[] structureList;         // Structure type IDs
public Vector2[] structurePositions; // Structure positions
public int[] structureLevel;        // Upgrade levels
```

### Structure Types

Major structures include:
- Player House
- NPC Houses
- Shops (Cafe, General Store, etc.)
- Community buildings (Museum, Library)
- Farming structures (Barn, Coop)

## Town Customization

### Town Name

```csharp
// Access via method
private string TownName();  // Returns current town name
```

### Town Wishes

Town development goals:

```csharp
public static int[] townWishLevel = new int[200];
private int[] townWish = new int[4];
private int townWishesMade;
private Sprite[] townWishesSprite;
```

## Player House

### House Customization

```csharp
// In GameData
public int playerHouseWall;    // Wall style
public int playerHouseRoof;    // Roof style
public int[] wallpaper;        // Interior wallpaper per room
public int[] flooring;         // Interior flooring per room
```

### Interior Objects

Indoor placement uses different rules:

```csharp
// Indoor checks in PlaceObject2
if (isIndoor && building)
{
    // Different collision detection
    // Room-specific placement rules
}
```

## Build Categories

The build menu has multiple categories:

```csharp
// Method to get item ID for build slot
private int GetBuildItemID(int slotIndex);

// Method to get build item display name
private string GetBuildItemName(int itemId);

// Hover handler for build options
public void HoverBuildOption(int a);
```

## Terrain/Flooring

### Flooring Types

```csharp
public Tilemap tilemapFlooring;
public TileBase bridgeTileWood;
public TileBase bridgeTileStone;
```

### Terrain Modification

Flooring can be changed to create paths and bridges.

## Modding the Building System

### Hook Object Placement

```csharp
[HarmonyPatch(typeof(GameScript), "PlaceObject")]
class PlaceObjectPatch
{
    static void Postfix()
    {
        // Object was placed
    }
}
```

### Check Build Mode

```csharp
bool isBuilding = GameScript.building;
bool isEditMode = GameScript.inEditMode;
bool isPlacing = GameScript.placingObject;
```

### Hook Build Menu

```csharp
[HarmonyPatch(typeof(GameScript), "HoverBuildOption")]
class BuildHoverPatch
{
    static void Postfix(int a)
    {
        // Player hovering over build option 'a'
    }
}
```

### Access Placed Objects

Use reflection to access object arrays:

```csharp
// Get placed objects from current save
var gameData = MenuScript.gameData[MenuScript.curSave];
int[] objects = gameData.objects;
Vector2[] positions = gameData.objectLocations;
```

### Modify Structure Levels

```csharp
// Access structure levels
var gameData = MenuScript.gameData[MenuScript.curSave];
int[] structures = gameData.structureList;
int[] levels = gameData.structureLevel;
```

## Indoor vs Outdoor

The game tracks indoor/outdoor state:

```csharp
public static bool isIndoor;      // Currently indoors
public static string curLocation; // Current location name
```

Indoor locations have:
- Different lighting
- Room-based placement
- Wallpaper/flooring customization
- Different collision rules

## Edit Mode

Edit mode allows rearranging placed objects:

```csharp
public static bool inEditMode;
public GameObject menuEditMode;
public GameObject editRoomSelector;
```

### Edit Mode Features

- Pick up placed objects
- Rotate objects
- Move objects
- Delete objects

## Build Costs

Building items have costs displayed:

```csharp
public GameObject menuBuildCost;
public TMP_Text txtBuildPage;  // Shows item name/cost
```

## Discoveries

New decorations are "discovered" when first placed:

```csharp
public static int[] discoverLevel = new int[1000];

// Discovery check during building
if (discoverLevel[itemIndex] == 0 && !building)
{
    Unlock(itemIndex);  // Show unlock notification
}
```
