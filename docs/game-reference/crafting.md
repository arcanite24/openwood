# Crafting System

## Overview

Littlewood has two main crafting systems:
1. **Cooking** - Food recipes using 3 cooking stations
2. **Crafting Workshop** - Item crafting (bars, planks, etc.)

## Cooking System

### Cooking Stations

| Station | Description |
|---------|-------------|
| Bubble Pot | Soups and stews |
| Sizzle Pan | Fried dishes |
| Chop Board | Salads and raw dishes |

### Recipe Structure

```csharp
public class Recipe : MonoBehaviour
{
    public int id;              // Recipe output item ID
    public int[] ingredientId;  // Array of 2 ingredient item IDs
}
```

### Recipe Storage

Each station has its own recipe list (40 slots each):

```csharp
private List<Recipe> bubblePotRecipe = new List<Recipe>();
private List<Recipe> sizzlePanRecipe = new List<Recipe>();
private List<Recipe> chopBoardRecipe = new List<Recipe>();

// Unlock tracking
private int[] chopBoardRecipeUnlocked = new int[40];
private int[] sizzlePanRecipeUnlocked = new int[40];
private int[] bubblePotRecipeUnlocked = new int[40];
```

### Recipe Tracking

```csharp
private int[] recipeTimesCooked = new int[120];       // Times each recipe was made
private int[] recipeMasterpiecesCooked = new int[120]; // Masterpiece versions made
private int recipeHintID;                              // Current recipe hint
```

### Recipe Examples

Recipes are created with `new Recipe(outputId, new int[] { ingredient1, ingredient2 })`:

**Bubble Pot Recipes:**
```csharp
bubblePotRecipe.Add(new Recipe(32, new int[2] { 126, 240 }));
bubblePotRecipe.Add(new Recipe(33, new int[2] { 126, 245 }));
// etc.
```

**Sizzle Pan Recipes:**
```csharp
sizzlePanRecipe.Add(new Recipe(32, new int[2] { 176, 241 }));
sizzlePanRecipe.Add(new Recipe(33, new int[2] { 176, 249 }));
// etc.
```

**Chop Board Recipes:**
```csharp
chopBoardRecipe.Add(new Recipe(32, new int[2] { 146, 243 }));
chopBoardRecipe.Add(new Recipe(33, new int[2] { 146, 250 }));
// etc.
```

### Recipe UI

```csharp
public GameObject menuRecipes;
public Image[] recipeImage = new Image[40];
public Image[] recipeIngredientsImage = new Image[2];
public TMP_Text txtRecipeName;
public TMP_Text txtRecipeIngredients;
public TMP_Text txtRecipesUnlocked;
public TMP_Text txtRecipePage;
public GameObject[] topRecipeButton = new GameObject[3];
private int curRecipePage;
private int lastRecipeButton;
```

### Cooking Methods

```csharp
private void PopulateValidRecipes();    // Generate all valid recipes
private void AddUniqueBubblePotRecipes();
private void AddUniqueSizzlePanRecipes();
private void AddUniqueChopBoardRecipes();
private void SetRecipeHint();           // Set current recipe hint
```

## Crafting Workshop

The Crafting Workshop converts raw materials into processed goods.

### Workshop Fields

```csharp
public GameObject menuCraftingWorkshop;
public TMP_Text txtCraftingWorkshopDesc;
public GameObject bCraftWorkshop;
public Image[] craftingIngredientImage = new Image[4];
public TMP_Text[] txtCraftingN = new TMP_Text[4];  // Ingredient names
public TMP_Text[] txtCraftingQ = new TMP_Text[4];  // Ingredient quantities
public TMP_Text txtCraftingName;
public Image craftingImage;
public GameObject craftingWorkshopFloatyObj;

private int curCraftingType;
private int craftingId;
private int itemsCrafted;  // Stats tracking
```

### Crafting Types

The workshop processes different material types:

| Type | Input | Output |
|------|-------|--------|
| Wood | Logs (Oak, Birch, etc.) | Planks |
| Ore | Raw ore | Bars |
| Gem | Raw gems | Cut gems |

### Crafting Bonuses

Hobbies provide crafting bonuses:

```csharp
// Woodcutting levels give plank bonuses
case 3: "15% chance to get a bonus plank when crafting.";
case 6: "25% chance to get a bonus plank when crafting.";
case 10: "50% chance to get a bonus plank when crafting.";
```

## Pantry System

The pantry stores ingredients:

```csharp
public GameObject menuPantryStock;
private int[] pantryItem;         // Items in pantry
```

## Save Data

Recipe progress is saved in `GameData`:

```csharp
public int[] bubblePotRecipeUnlocked;
public int[] sizzlePanRecipeUnlocked;
public int[] chopBoardRecipeUnlocked;
public int[] recipeTimesCooked;
public int[] recipeMasterpiecesCooked;
public bool[] isMasterpiece;
public int recipeHintID;
public int[] pantryItem;
public int itemsCrafted;
```

## Modding the Crafting System

### Hook Recipe Cooking

```csharp
// Find the cooking method and patch it
[HarmonyPatch(typeof(GameScript))]
class CookingPatch
{
    // Patch the cooking completion method
    static void Postfix(int recipeId)
    {
        // A recipe was cooked
    }
}
```

### Add Custom Recipes

Recipes are populated at game start in `PopulateValidRecipes()`:

```csharp
[HarmonyPatch(typeof(GameScript), "PopulateValidRecipes")]
class RecipePatch
{
    static void Postfix(GameScript __instance)
    {
        // Use reflection to access private recipe lists
        var bubblePot = typeof(GameScript)
            .GetField("bubblePotRecipe", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(__instance) as List<Recipe>;
        
        // Add custom recipe
        // bubblePot.Add(new Recipe(customOutputId, new int[2] { ing1, ing2 }));
    }
}
```

### Check Unlocked Recipes

```csharp
// Use reflection to access unlock arrays
var bubblePotUnlocked = typeof(GameScript)
    .GetField("bubblePotRecipeUnlocked", BindingFlags.NonPublic | BindingFlags.Instance)
    .GetValue(gameScriptInstance) as int[];

bool isUnlocked = bubblePotUnlocked[recipeIndex] == 1;
```

### Track Crafting Stats

```csharp
// itemsCrafted tracks total items crafted
// Access via reflection or hook the crafting method
```

## Recipe Hint System

The game provides recipe hints:

```csharp
private int recipeHintID;
public Image[] recipeHintImage = new Image[2];

private void SetRecipeHint();  // Set next recipe to discover
```

## Masterpiece System

Cooked dishes can be "masterpieces" (higher quality):

```csharp
private int[] recipeMasterpiecesCooked = new int[120];
private bool[] isMasterpiece;

// In GameData
public bool[] isMasterpiece;
```

Masterpiece status affects dish value and gifting.

## Ingredient Categories

**Bubble Pot ingredients** (IDs 120-131):
- Various vegetables and herbs

**Sizzle Pan ingredients** (IDs 176-179):
- Meats and fish

**Chop Board ingredients** (IDs 146-149):
- Fruits and vegetables

**Condiments/extras** (IDs 240-256):
- Seasonings, sauces, etc.

## Dish Values

```csharp
public TMP_Text txtDishValue;
public int[] ingredientIds = new int[8];
```

Dish value is calculated based on:
- Base ingredients
- Recipe rarity
- Masterpiece bonus
- Cooking hobby level
