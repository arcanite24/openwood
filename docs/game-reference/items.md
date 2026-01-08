# Item System

## Overview

Items in Littlewood are identified by integer IDs. The inventory is a simple array where the index is the item ID and the value is the quantity owned.

```csharp
// GameScript internal inventory array
private int[] inventory = new int[500];

// Access: inventory[itemId] = quantity
```

## Item Categories by ID Range

### Ground/Terrain (0-39)
| ID | Name | Notes |
|----|------|-------|
| 0 | Grassy Earth | Ground tile |
| 1 | Dirt | Ground tile |
| 2 | Tilled Soil | For planting |

### Wood Types (40-59)
| ID | Name | Description |
|----|------|-------------|
| 40 | Wood | Basic wood, makes Wooden Planks |
| 41 | Magicwood | Purple glow, makes Fancy Planks |
| 42 | Goldenwood | Shiny, makes Perfect Planks |
| 43 | Almwood | Rare, makes Dusk Planks at Master Forge |
| 44 | Leifwood | Rare, makes Dawn Planks at Master Forge |

### Planks (60-79)
| ID | Name | Made From |
|----|------|-----------|
| 60 | Wooden Plank | Wood |
| 61 | Fancy Plank | Magicwood |
| 62 | Perfect Plank | Goldenwood |
| 63 | Dusk Plank | Almwood |
| 64 | Dawn Plank | Leifwood |

### Ores & Stones (80-99)
| ID | Name | Description |
|----|------|-------------|
| 80 | Stone | Basic, makes Plain Bricks |
| 81 | Magicite | Magical, makes Fancy Bricks |
| 82 | Orichalcum | Golden, makes Perfect Bricks |
| 83 | Wyvernite | Purple, makes Moonlight Orb |
| 84 | Dragalium | Hot, makes Sunlight Orb |

### Bricks & Orbs (100-119)
| ID | Name | Made From |
|----|------|-----------|
| 100 | Plain Brick | Stone |
| 101 | Fancy Brick | Magicite |
| 102 | Perfect Brick | Orichalcum |
| 103 | Moonlight Orb | Wyvernite |
| 104 | Sunlight Orb | Dragalium |

### Fruits (120-139)
| ID | Name | Description |
|----|------|-------------|
| 120 | Slimeapple | Most popular fruit, sweet & crunchy |
| 121 | Plumberry | Small, sweet and tangy |
| 122 | Peachot | Juicy and sweet |
| 123 | Goldenbell | Soft and sweet, fancy |
| 124 | Sourpuck | Sour fruit |
| 125 | Goop Melon | Melon variety |
| 126 | Papayapa | Tropical fruit |
| 127 | Crescent Moon | Rare fruit |
| 128 | Ghost Fruit | Rare fruit |
| 129 | Perfect Slimeapple | Golden version |
| 130 | Jello Melon | Special melon |
| 131 | Duriot | Special fruit |
| 132 | Wizberry | Magical berry |

### Vegetables (140-159)
| ID | Name |
|----|------|
| 140 | Carrot |
| 141 | Cabbage |
| 142 | Potato |
| 143 | Corn |
| 144 | Motato |
| 145 | Eggplant |
| 146 | Onion |
| 147 | Kacoomba |
| 148 | Bloccoli |
| 149 | Golden Carrot |
| 150 | Radish |
| 151 | Punkin |
| 152 | Turnops |

### Fish (160-199)
| ID | Name | Rarity |
|----|------|--------|
| 160 | Minnow | Common |
| 161 | Trout | Common |
| 162 | Fire Carp | Uncommon |
| 163 | Plum Loach | Uncommon |
| 164 | Tetra | Common |
| 165 | Gobblenook | Uncommon |
| 166 | Cod | Common |
| 167 | Spikey Barb | Uncommon |
| 168 | Gemfish (Sapphire) | Rare |
| 169 | Gemfish (Peridot) | Rare |
| 170 | Gemfish (Ruby) | Rare |
| 171 | Gemfish (Amethyst) | Rare |
| 172 | Leviath | Rare |
| 173 | Cuttlefish | Uncommon |
| 174 | Octopoot | Uncommon |
| 175 | Giant Laserfin | Rare |
| 176 | Golden Tuna | Rare |
| 177 | Rockfish | Uncommon |
| 178 | Sea Slug | Uncommon |
| 179 | Pleurodon | Rare |
| 180 | Mystical Tarpon | Rare |
| 181 | Silver Poppy | Rare |
| 182 | Cruncher | Uncommon |
| 183 | Bloomfish | Rare |
| 184 | Crusher Dragon | Legendary |
| 185 | Blobble | Uncommon |
| 186 | Lavafish | Rare |
| 187 | Snootsnoot | Uncommon |
| 188 | King Octopoot | Legendary |
| 189 | Golden Pleurodon | Legendary |

### Bugs (200-239)
| ID | Name | Rarity |
|----|------|--------|
| 200 | Flutterfly | Common |
| 201 | Swallowtail | Common |
| 202 | Monarch | Common |
| 203 | Nightwing | Uncommon |
| 204 | Dragonwasp | Uncommon |
| 205 | Ladybug | Common |
| 206 | Mighty Moth | Uncommon |
| 207 | Shell Beetle | Uncommon |
| 208 | Emperorfly | Rare |
| 209 | Chimaera | Rare |
| 210 | Kaiser | Rare |
| 211 | Glasswing | Rare |
| 212 | Leaf Stag | Uncommon |
| 213 | Red Goliath | Rare |
| 214 | Golden Titan | Legendary |
| 215 | Blue Hercules | Legendary |
| 216 | Pricklypillar | Common |
| 217 | Wyvernfly | Rare |
| 218 | Dusk Cicada | Rare |
| 219 | Queen Lycaeides | Legendary |
| 220 | Rootbug | Common |
| 221 | Elder Dragonwasp | Rare |
| 222 | Wiggly Drone | Uncommon |
| 223 | Treasure Glider | Rare |
| 224 | Mookimooki | Uncommon |
| 225 | Steel Beetle | Rare |
| 226 | Ghostwick | Rare |
| 227 | Fellbug | Rare |
| 228 | Rainbow Slug | Legendary |
| 229 | Goldencrest | Legendary |

### Farm Products & Gatherable (240-279)
| ID | Name | Source |
|----|------|--------|
| 240 | Weeds | Gathering |
| 241 | Egg | Chicken |
| 242 | Golden Egg | Happy Chicken |
| 243 | Milk | Cow |
| 244 | Golden Milk | Happy Cow |
| 245 | Shroom | Gathering |
| 246 | Golden Shroom | Rare |
| 247 | Fleece | Sheep |
| 248 | Golden Fleece | Happy Sheep |
| 249 | Lily Pad | Water |
| 250 | Golden Frog | Rare |
| 251 | Floating Petal | Water |
| 252 | River Crystal | Water |
| 253 | Cavern Wart | Cavern |
| 254 | Dandelion | Gathering |
| 255 | Old Leaf | Gathering |
| 256 | Acorn | Trees |
| 257 | Golden Acorn | Rare |
| 258 | Heart Clover | Rare |
| 259 | Yucky Goo | Monsters |
| 260 | Wiggly Worm | Digging |
| 261 | Seaweed | Fishing |
| 262 | Primal Tooth | Boss drop |
| 263 | Honeycomb | Bees |

### Special/Quest Items (280-339)
| ID | Name | Use |
|----|------|-----|
| 280 | Deluca Coin | Deluca currency |
| 281 | Heroic Merit | Achievement reward |
| 282 | Sky Rock | Special material |
| 283 | Meteorite | Rare material |
| 284 | Smooth Rock | Decoration |
| 285 | Shiny Shell | Beach |
| 286 | Sea Star | Beach |
| 287 | Ancient Isopod | Rare |
| 288 | Twig | Common |
| 289 | Purple Sludge | Monster drop |
| 290 | Black Pearl | Rare |
| 291 | Large Feather | Bird |
| 292 | Light Fairy | Rare |
| 293 | Wicked Fairy | Rare |
| 294 | Dark Matter | Rare |
| 295 | Dragon Fossil | Rare |
| 296 | Dust | Common |
| 297 | Soul Ribbon | Quest |
| 298 | Nature Ribbon | Quest |
| 299 | Duelist Badge | Tarot |
| 300 | Deluca Present | Gift |
| 320 | Golden Key | Unlock |
| 321 | Laura's Notebook | Quest |
| 322-323 | Womper Whistle | Pet |
| 340 | Moon Present | Special gift |
| 341 | Solemn Crystal | Quest |
| 342 | Supreme Wizard | Quest |
| 343 | World Fragment | Main quest |

### Flowers (400-447)
Flowers come in 6 types, each with 8 colors:
- **Types**: Lollypop, Zigzag, Dewcap, Wickid, Tootsie, Floof
- **Colors**: White, Red, Yellow, Blue, Pink, Orange, Purple, Rare

| ID Range | Flower Type |
|----------|-------------|
| 400-407 | Lollypop |
| 408-415 | Zigzag |
| 416-423 | Dewcap |
| 424-431 | Wickid |
| 432-439 | Tootsie |
| 440-447 | Floof |

## Accessing Items (Modding)

### Reading Inventory
```csharp
// Using reflection to access private inventory
var gameScript = FindObjectOfType<GameScript>();
var inventory = ReflectionHelper.GetField<int[]>(gameScript, "inventory");
int woodCount = inventory[40]; // Get wood count
```

### Adding Items
The game uses `AddItem(int id, int quantity)` method:
```csharp
// Via Harmony patch or reflection
// AddItem(40, 5); // Add 5 wood
```

### Key Methods in GameScript

| Method | Signature | Purpose |
|--------|-----------|---------|
| `GetItemName` | `private string GetItemName(int id)` | Get item display name |
| `GetItemNamePlural` | `private string GetItemNamePlural(int id)` | Get plural item name |
| `GetMuseumDesc` | `private string GetMuseumDesc(int id)` | Get item description |
| `AddItem` | `private void AddItem(int id, int q)` | Add items to inventory |
| `AddItemRefund` | `private void AddItemRefund(int id, int q)` | Add items (refund) |

## Inventory UI

The inventory UI uses these arrays in GameScript:
```csharp
public GameObject[] inventorySlotObj = new GameObject[70]; // Slot objects
public TMP_Text[] txtInventoryQ = new TMP_Text[70];        // Quantity text
public Image[] inventorySprite = new Image[70];             // Item sprites
```

The inventory is paginated with 70 visible slots per page.
