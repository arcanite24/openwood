# Player System

## Overview

Player state is managed through a combination of `PlayerController` (movement/input) and `GameScript` (stats/inventory).

## PlayerController

The `PlayerController` component handles player movement, input, and animations.

### Static Fields

```csharp
public class PlayerController : MonoBehaviour
{
    // Position
    public static Vector2 pos;              // Current position
    public static Vector2 lastPlayerPos;    // Previous position
    
    // Movement state
    public static bool moving;              // Is player moving
    public static bool hasInput;            // Has input this frame
    public static bool isSprinting;         // Is sprinting
    public static float speed = 0.55f;      // Current speed
    
    // Phase/teleport
    public static float phaseTimerSeconds;  // Phase cooldown
    public static bool knockingOut;         // Being knocked out
}
```

### Instance Fields

```csharp
// Speed settings
public float walkSpeed = 1.6f;
public float runSpeed = 2f;
public float editSpeed = 1.6f;
public float boostSpeed = 2f;
public float LERPSPEED = 25f;

// References
public Camera mainCamera;
public GameObject shadow;
public GameObject parent;
public Transform cursorObjTransform;
public Transform debugSquare;

// Animators (player appearance)
public Animator animatorOutfit;
public Animator animatorHair;
public Animator animatorSkin;

// NPC following
public GameObject follower;

// State
public bool inCutscene;
private bool canMove;
private bool hiding;
```

### Key Methods

```csharp
// Visibility
public void HidePlayer();          // Toggle visibility

// Hangout system
public void BeginHangout(GameObject g);  // Start hanging out with NPC
public void StopHangout();               // Stop hangout

// Direction control
public void FaceDown();   // Face direction 0
public void FaceLeft();   // Face direction 1
public void FaceUp();     // Face direction 2
public void FaceRight();  // Face direction 3

// Location check
private bool IsPlayerInTown();
```

## Player Stats (GameScript)

Most player stats are stored as static fields in `GameScript`:

### Currency & XP
```csharp
public static int dew;          // Dewdrops (money)
public static int actualDew;    // Actual dew (for animations)
public static int dayEXP;       // Experience earned today
public static int maxDayEXP = 100; // Max XP per day
```

### Position & Facing
```csharp
public static Vector2 playerPos;  // Player position (mirror of PlayerController.pos)
public static int facingDir;      // Direction (0=down, 1=left, 2=up, 3=right)
public static int elevation;      // Current elevation level
```

### Inventory Access
```csharp
// Private inventory array (use reflection)
private int[] inventory = new int[500];
private int[] inventorySOLD = new int[500];  // Sold items tracker

// Inventory state
private bool inventoryFull;
private int curInventoryPage;
```

### Player State Flags
```csharp
public static bool menuOpen;        // Any menu is open
public static bool talking;         // In dialogue
public static bool interacting;     // Interacting with object
public static bool fishing;         // Currently fishing
public static bool building;        // In build mode
public static bool inEditMode;      // In edit/decoration mode
public static bool placingObject;   // Placing an object
public static bool pickingUpObject; // Picking up object
```

### Marriage State
```csharp
public static bool isMarried;
public static bool isEngaged;
public static string engagedTownsfolkName;
public static bool finishedDate;
public static bool talkedToSinceMarried;
```

## Player Appearance

Player customization is stored in `GameData`:

```csharp
public int skin;    // Skin color index
public int hair;    // Hair style index
public int color;   // Hair color index
public int outfit;  // Outfit index
public int type;    // Character type
```

Unlocked customization options:
```csharp
public int[] hairsUnlocked;
public int[] outfitUnlocked;
public int[] skinUnlocked;
```

## Player Activities

### Hobby EXP
```csharp
// Hobby levels (10 hobbies)
public static int[] hobbyEXP = new int[10];
public static int[] hobbyEXPTemp = new int[10];
public static HeartExpPackage[] hobbyEXPPackage = new HeartExpPackage[10];
```

### Hobby IDs
| ID | Hobby |
|----|-------|
| 0 | Woodcutting |
| 1 | Mining |
| 2 | Fishing |
| 3 | Bug Catching |
| 4 | Farming |
| 5 | Cooking |
| 6 | Building |
| 7 | Exploration |
| 8 | Socializing |
| 9 | Collecting |

### Tools
```csharp
// Tool unlock state
public static int[] toolUnlocked = new int[40];

// In GameData
public int[] toolLevel;  // Tool upgrade levels
```

## Player House

```csharp
// Current house being viewed/edited
public static int curHouse;

// House customization
public int playerHouseWall;   // Wall style
public int playerHouseRoof;   // Roof style

// Interior
public int[] wallpaper;       // Wallpaper per room
public int[] flooring;        // Floor per room
```

## Movement System

### Direction Values
| Value | Direction |
|-------|-----------|
| 0 | Down |
| 1 | Left |
| 2 | Up |
| 3 | Right |

### Movement in Build Mode
```csharp
private void MoveInBuildMode(int dir)
{
    // Grid-based movement (0.1 units)
    // Checks for collisions with walls/objects
}
```

### Phase System (Teleport)
Players can "phase" through obstacles with the phase ability:
```csharp
private void PhaseCheck(Vector2 mod);  // Check if can phase
public static bool phasing;             // Currently phasing
```

## Pets

```csharp
// Adopted pets
public int[] adoptedPet;
public string[] petName;
public Vector2[] adoptedPetPositions;

// Initial pet
public int initAdoptedPet;

// Unique pets
public int[] uniquePetUnlock;
public Vector2[] uniquePetPositions;
```

## Modding the Player

### Get Player Position
```csharp
Vector2 playerPos = PlayerController.pos;
// or
Vector2 playerPos = GameScript.playerPos;
```

### Check Player State
```csharp
bool isInMenu = GameScript.menuOpen;
bool isTalking = GameScript.talking;
bool isBuilding = GameScript.building;
bool isFishing = GameScript.fishing;
```

### Hook Player Movement
```csharp
[HarmonyPatch(typeof(PlayerController), "Update")]
class PlayerMovePatch
{
    static void Postfix()
    {
        if (PlayerController.moving)
        {
            // Player is moving
            var pos = PlayerController.pos;
        }
    }
}
```

### Modify Player Money
```csharp
// Use reflection or find the AddDewdrops method
[HarmonyPatch(typeof(GameScript), "AddDewdrops")]
class MoneyPatch
{
    static void Postfix(int a)
    {
        // a is the amount added
        // GameScript.dew is the new total
    }
}
```
