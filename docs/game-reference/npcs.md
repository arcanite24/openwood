# NPC & Townsfolk System

## Overview

NPCs in Littlewood are called "Townsfolk." Each NPC has:
- A unique ID (integer)
- Friendship/Heart level
- Romance level (for romanceable NPCs)
- Daily state (talked to, gifted, etc.)
- Position in the world

## Townsfolk IDs

### Main Villagers (1-15)
| ID | Name | Role | Romanceable |
|----|------|------|-------------|
| 1 | Willow | Starting villager | ✅ Yes |
| 2 | Dalton | Farmer | ✅ Yes |
| 3 | Dudley | Shop keeper | ❌ No |
| 4 | Laura | Explorer | ✅ Yes |
| 5 | Bubsy | Child | ✅ Yes |
| 6 | Ash | Mysterious | ✅ Yes |
| 7 | Lilith | Dark mage | ✅ Yes |
| 8 | Zana | Fortune teller | ❌ No |
| 9 | Terric | Knight | ✅ Yes |
| 10 | Mel | Inventor | ❌ No |
| 11 | Maximilian | Noble | ✅ Yes |
| 12 | Iris | Artist | ✅ Yes |
| 13 | Dark | Hero's companion | ❌ No |
| 14 | Arpeggio | Musician | ❌ No |
| 15 | Toby | Dog | ❌ No |

### Special NPCs (24+)
| ID | Name | Role |
|----|------|------|
| 24 | Gobby | Generic Goblin |
| 25 | Gobby Traveler | Traveling merchant |
| 26 | Gobby Merchant | Shop goblin |
| 27 | Mayor Wadsworth | Town mayor |
| 28 | Hamilton | Banker |
| 29 | Archmage Fiona | Magic teacher |
| 30 | Toad | Animal |
| 31 | Olivia | Librarian |
| 32 | Freddy | Following pet |
| 33 | Harold Pockets | Pockets game |
| 34 | Beeford | Bee keeper |
| 35 | Corvus | Crow merchant |
| 36 | Captain Georgie | Ship captain |
| 37 | Bug Expert Petunia | Bug collector |
| 38 | Gobby Duelist | Tarot opponent |
| 39 | Gobby On The Jobby | Worker |
| 40 | Bridgette | Special NPC |
| 41 | Maya | Special NPC |

## Friendship System

### Static Arrays in GameScript
```csharp
// Heart level (0-10)
public static int[] townsfolkLevel = new int[50];

// Heart EXP progress (towards next level)
public static int[] townsfolkHeartEXP = new int[50];
public static int[] townsfolkHeartEXPTemp = new int[50]; // Pending EXP

// Daily interaction tracking
public static int[] townsfolkTalkedTo = new int[50];

// Cutscene progress
public static int[] cutsceneLevel = new int[50];
```

### Romance System
```csharp
// Romance level (0-3: None, Dating, Engaged, Married)
public static int[] townsfolkRomanceLvl = new int[50];

// Romance dialogue progress
public static int[] townsfolkRomanceDialogueLvl = new int[50];

// Marriage state
public static bool isMarried;
public static bool isEngaged;
public static string engagedTownsfolkName;
```

## NPCScript Class

The `NPCScript` component is attached to each NPC GameObject.

### Key Fields
```csharp
public class NPCScript : MonoBehaviour
{
    public bool isNPC;           // Is this a main NPC
    public bool isFarmAnimal;    // Is this a farm animal
    public bool isGobby;         // Is this a goblin
    public bool isFreddy;        // Is this the pet Freddy
    public bool isPlayer;        // (unused)
    public bool emptyNPC;        // Placeholder NPC
    
    public Animator animator;    // NPC animator
    public GameObject follower;  // Following NPC (hangouts)
    public Transform target;     // Movement target
    
    public bool hangingOut;      // Is hanging out with player
    public bool cutsceneMoving;  // In cutscene movement
    
    // Equipment display
    public GameObject fishingPoleObj;
    public GameObject axeObj;
    public GameObject pickAxeObj;
    public GameObject bugNetObj;
    public GameObject exclaObj;   // Exclamation mark
    public GameObject wantObj;    // Want bubble
}
```

### Key Methods
```csharp
// Animation control
public void SetAnimation(string animName);
public void SetDirection(int dir); // 0=down, 1=left, 2=up, 3=right

// Movement
public void MoveTo(Vector2 target);
public void StopMoving();

// Emotes
public void ShowEmote(string emoteType);
public void HideEmote();
```

## Dialogue System

Dialogue is handled through `GameScript` methods:

```csharp
// Start dialogue with NPC by name
public void TalkToSpecificTownsfolk(string s);
public void TalkToSpecificTownsfolkRomance(string s);

// Get NPC name from ID
private string GetTownsfolkName(int a);

// Check if can talk today
private bool CanTalkToTownsfolk(int id);
```

### Dialogue Types
- Regular conversation
- Romance dialogue
- Gift response
- Event dialogue
- Cutscene dialogue

## NPC Schedules

NPCs have daily schedules stored in their positions and activities:

```csharp
// In GameData (save data)
public Vector2[] townsfolkPositions;  // Current positions

// Position updates handled in GameScript Update
```

## NPC Requests (Town Wishes)

NPCs can request items from the player:

```csharp
// Request system
public static int[] townWishLevel = new int[200];

// Quest arrays
public static int[] questNeedToActivate = new int[3];
public static int[] questType = new int[3];
public static int[] questTownsfolk = new int[3];
public static int[] questQ = new int[3];
public static int[] questQCompleted = new int[3];
public static int[] questId = new int[3];
```

## Modding NPCs

### Getting NPC Reference
```csharp
// Find NPC by name
var npcs = FindObjectsOfType<NPCScript>();
var willow = npcs.FirstOrDefault(n => n.name.Contains("Willow"));
```

### Checking Friendship
```csharp
// Get Willow's friendship level (ID 1)
int willowLevel = GameScript.townsfolkLevel[1];
int willowEXP = GameScript.townsfolkHeartEXP[1];
```

### Hook into Dialogue
Using Harmony:
```csharp
[HarmonyPatch(typeof(GameScript), "TalkToSpecificTownsfolk")]
class DialoguePatch
{
    static void Postfix(string s)
    {
        // s is the NPC name
        Debug.Log($"Player talked to {s}");
    }
}
```

## Farm Animals

Farm animals are a special type of NPC:

| Index | Animal | Product |
|-------|--------|---------|
| 0-2 | Chicken | Egg (241) / Golden Egg (242) |
| 3-5 | Cow | Milk (243) / Golden Milk (244) |
| 6-8 | Sheep | Fleece (247) / Golden Fleece (248) |
| 9-11 | Mushroom | Shroom (245) / Golden Shroom (246) |

```csharp
// Farm animal state
public static int[] farmAnimalLevel = new int[12];

// In GameData
public int[] farmAnimalWant;  // What animal wants to eat
public int[] farmAnimalDrop;  // What animal drops
public Vector2[] farmAnimalPositions;
```
