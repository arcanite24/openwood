using System;
using System.Reflection;
using UnityEngine;

namespace OpenWood.Core.API
{
    /// <summary>
    /// API for controlling and querying the player's inventory.
    /// Provides methods for adding, removing, and querying items.
    /// </summary>
    /// <example>
    /// <code>
    /// // Add 10 wood to inventory
    /// InventoryAPI.AddItem(ItemID.Wood, 10);
    /// 
    /// // Add items by category
    /// InventoryAPI.AddWood(ItemID.Wood, 10);
    /// 
    /// // Check if player has an item
    /// bool hasWood = InventoryAPI.HasItem(ItemID.Wood);
    /// </code>
    /// </example>
    public static class InventoryAPI
    {
        #region Constants - Item IDs

        /// <summary>
        /// Common item IDs organized by category.
        /// Use these constants for type-safe item manipulation.
        /// </summary>
        public static class ItemID
        {
            #region Wood (40-44)
            /// <summary>Basic wood</summary>
            public const int Wood = 40;
            /// <summary>Magicwood - rare wood type</summary>
            public const int Magicwood = 41;
            /// <summary>Goldenwood - rare wood type</summary>
            public const int Goldenwood = 42;
            /// <summary>Almwood - rare wood type</summary>
            public const int Almwood = 43;
            /// <summary>Leifwood - rare wood type</summary>
            public const int Leifwood = 44;
            #endregion

            #region Planks (60-64)
            /// <summary>Wooden Plank</summary>
            public const int WoodenPlank = 60;
            /// <summary>Fancy Plank</summary>
            public const int FancyPlank = 61;
            /// <summary>Perfect Plank</summary>
            public const int PerfectPlank = 62;
            /// <summary>Dusk Plank</summary>
            public const int DuskPlank = 63;
            /// <summary>Dawn Plank</summary>
            public const int DawnPlank = 64;
            #endregion

            #region Stone/Ore (80-84)
            /// <summary>Basic stone</summary>
            public const int Stone = 80;
            /// <summary>Magicite ore</summary>
            public const int Magicite = 81;
            /// <summary>Orichalcum ore</summary>
            public const int Orichalcum = 82;
            /// <summary>Wyvernite ore</summary>
            public const int Wyvernite = 83;
            /// <summary>Dragalium ore</summary>
            public const int Dragalium = 84;
            #endregion

            #region Bricks (100-104)
            /// <summary>Plain Brick</summary>
            public const int PlainBrick = 100;
            /// <summary>Fancy Brick</summary>
            public const int FancyBrick = 101;
            /// <summary>Perfect Brick</summary>
            public const int PerfectBrick = 102;
            /// <summary>Moonlight Orb</summary>
            public const int MoonlightOrb = 103;
            /// <summary>Sunlight Orb</summary>
            public const int SunlightOrb = 104;
            #endregion

            #region Fruit (120-132)
            /// <summary>Slimeapple fruit</summary>
            public const int Slimeapple = 120;
            /// <summary>Plumberry fruit</summary>
            public const int Plumberry = 121;
            /// <summary>Peachot fruit</summary>
            public const int Peachot = 122;
            /// <summary>Goldenbell fruit</summary>
            public const int Goldenbell = 123;
            /// <summary>Sourpuck fruit</summary>
            public const int Sourpuck = 124;
            /// <summary>Goop Melon fruit</summary>
            public const int GoopMelon = 125;
            /// <summary>Papayapa fruit</summary>
            public const int Papayapa = 126;
            /// <summary>Crescent Moon fruit</summary>
            public const int CrescentMoon = 127;
            #endregion

            #region Vegetables (140-152)
            /// <summary>Carrot vegetable</summary>
            public const int Carrot = 140;
            /// <summary>Cabbage vegetable</summary>
            public const int Cabbage = 141;
            /// <summary>Potato vegetable</summary>
            public const int Potato = 142;
            /// <summary>Corn vegetable</summary>
            public const int Corn = 143;
            /// <summary>Motato vegetable</summary>
            public const int Motato = 144;
            /// <summary>Eggplant vegetable</summary>
            public const int Eggplant = 145;
            /// <summary>Onion vegetable</summary>
            public const int Onion = 146;
            /// <summary>Golden Carrot vegetable</summary>
            public const int GoldenCarrot = 149;
            #endregion

            #region Fish (160-189)
            /// <summary>Minnow fish</summary>
            public const int Minnow = 160;
            /// <summary>Trout fish</summary>
            public const int Trout = 161;
            /// <summary>Fire Carp fish</summary>
            public const int FireCarp = 162;
            /// <summary>Golden Tuna fish</summary>
            public const int GoldenTuna = 176;
            #endregion

            #region Bugs (200-229)
            /// <summary>Flutterfly bug</summary>
            public const int Flutterfly = 200;
            /// <summary>Monarch bug</summary>
            public const int Monarch = 202;
            /// <summary>Ladybug</summary>
            public const int Ladybug = 205;
            /// <summary>Golden Titan bug</summary>
            public const int GoldenTitan = 214;
            #endregion

            #region Animal Products (241-263)
            /// <summary>Egg</summary>
            public const int Egg = 241;
            /// <summary>Golden Egg</summary>
            public const int GoldenEgg = 242;
            /// <summary>Milk</summary>
            public const int Milk = 243;
            /// <summary>Golden Milk</summary>
            public const int GoldenMilk = 244;
            /// <summary>Fleece</summary>
            public const int Fleece = 247;
            /// <summary>Golden Fleece</summary>
            public const int GoldenFleece = 248;
            /// <summary>Honeycomb</summary>
            public const int Honeycomb = 263;
            #endregion

            #region Misc
            /// <summary>Dust item</summary>
            public const int Dust = 296;
            #endregion
        }

        #endregion

        #region Private Fields

        private static bool _initialized;
        private static GameScript _gameScript;
        private static MethodInfo _addItemMethod;
        private static MethodInfo _removeItemMethod;

        #endregion

        #region Public Methods - Add Items

        /// <summary>
        /// Adds an item to the player's inventory.
        /// </summary>
        /// <param name="itemId">The item ID to add.</param>
        /// <param name="quantity">The quantity to add (default 1).</param>
        /// <returns>True if successful.</returns>
        public static bool AddItem(int itemId, int quantity = 1)
        {
            EnsureInitialized();

            try
            {
                if (_gameScript != null && _addItemMethod != null)
                {
                    for (int i = 0; i < quantity; i++)
                    {
                        _addItemMethod.Invoke(_gameScript, new object[] { itemId, 1 });
                    }
                    Plugin.Log.LogDebug($"Added {quantity}x item {itemId}");
                    return true;
                }
                else
                {
                    Plugin.Log.LogWarning("Could not add item - GameScript not found");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Failed to add item: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Adds multiple items of the same type.
        /// </summary>
        /// <param name="itemId">The item ID to add.</param>
        /// <param name="quantity">The quantity to add.</param>
        /// <returns>True if successful.</returns>
        public static bool AddItems(int itemId, int quantity)
        {
            return AddItem(itemId, quantity);
        }

        #endregion

        #region Public Methods - Bulk Add by Category

        /// <summary>
        /// Adds a set of all wood types to the inventory.
        /// </summary>
        /// <param name="quantityEach">Quantity of each wood type.</param>
        public static void AddAllWoodTypes(int quantityEach = 10)
        {
            AddItem(ItemID.Wood, quantityEach);
            AddItem(ItemID.Magicwood, quantityEach);
            AddItem(ItemID.Goldenwood, quantityEach);
            AddItem(ItemID.Almwood, quantityEach);
            AddItem(ItemID.Leifwood, quantityEach);
            Plugin.Log.LogInfo($"Added {quantityEach}x of each wood type");
        }

        /// <summary>
        /// Adds a set of all stone/ore types to the inventory.
        /// </summary>
        /// <param name="quantityEach">Quantity of each stone type.</param>
        public static void AddAllStoneTypes(int quantityEach = 10)
        {
            AddItem(ItemID.Stone, quantityEach);
            AddItem(ItemID.Magicite, quantityEach);
            AddItem(ItemID.Orichalcum, quantityEach);
            AddItem(ItemID.Wyvernite, quantityEach);
            AddItem(ItemID.Dragalium, quantityEach);
            Plugin.Log.LogInfo($"Added {quantityEach}x of each stone type");
        }

        /// <summary>
        /// Adds a set of all plank types to the inventory.
        /// </summary>
        /// <param name="quantityEach">Quantity of each plank type.</param>
        public static void AddAllPlankTypes(int quantityEach = 10)
        {
            AddItem(ItemID.WoodenPlank, quantityEach);
            AddItem(ItemID.FancyPlank, quantityEach);
            AddItem(ItemID.PerfectPlank, quantityEach);
            AddItem(ItemID.DuskPlank, quantityEach);
            AddItem(ItemID.DawnPlank, quantityEach);
            Plugin.Log.LogInfo($"Added {quantityEach}x of each plank type");
        }

        /// <summary>
        /// Adds a set of all brick types to the inventory.
        /// </summary>
        /// <param name="quantityEach">Quantity of each brick type.</param>
        public static void AddAllBrickTypes(int quantityEach = 10)
        {
            AddItem(ItemID.PlainBrick, quantityEach);
            AddItem(ItemID.FancyBrick, quantityEach);
            AddItem(ItemID.PerfectBrick, quantityEach);
            AddItem(ItemID.MoonlightOrb, quantityEach);
            AddItem(ItemID.SunlightOrb, quantityEach);
            Plugin.Log.LogInfo($"Added {quantityEach}x of each brick type");
        }

        /// <summary>
        /// Adds a starter pack of common materials.
        /// </summary>
        public static void AddStarterPack()
        {
            AddItem(ItemID.Wood, 50);
            AddItem(ItemID.Stone, 50);
            AddItem(ItemID.WoodenPlank, 25);
            AddItem(ItemID.PlainBrick, 25);
            PlayerAPI.AddMoney(5000);
            Plugin.Log.LogInfo("Added starter pack");
        }

        #endregion

        #region Public Methods - Query

        /// <summary>
        /// Checks if the player has at least one of an item.
        /// </summary>
        /// <param name="itemId">The item ID to check.</param>
        /// <returns>True if the player has the item.</returns>
        public static bool HasItem(int itemId)
        {
            return GetItemCount(itemId) > 0;
        }

        /// <summary>
        /// Gets the count of an item in the player's inventory.
        /// Note: This may require additional game research to implement fully.
        /// </summary>
        /// <param name="itemId">The item ID to count.</param>
        /// <returns>The quantity of the item, or 0 if not found.</returns>
        public static int GetItemCount(int itemId)
        {
            // TODO: Implement inventory querying
            // This requires research into how GameScript.inventory works
            Plugin.Log.LogWarning("GetItemCount not yet fully implemented");
            return 0;
        }

        #endregion

        #region Public Methods - Item Information

        /// <summary>
        /// Gets the category name for an item ID.
        /// </summary>
        /// <param name="itemId">The item ID.</param>
        /// <returns>The category name.</returns>
        public static string GetItemCategory(int itemId)
        {
            if (itemId >= 40 && itemId <= 44) return "Wood";
            if (itemId >= 60 && itemId <= 64) return "Planks";
            if (itemId >= 80 && itemId <= 84) return "Stone/Ore";
            if (itemId >= 100 && itemId <= 104) return "Bricks";
            if (itemId >= 120 && itemId <= 132) return "Fruit";
            if (itemId >= 140 && itemId <= 152) return "Vegetables";
            if (itemId >= 160 && itemId <= 189) return "Fish";
            if (itemId >= 200 && itemId <= 229) return "Bugs";
            if (itemId >= 241 && itemId <= 263) return "Animal Products";
            if (itemId >= 400 && itemId <= 447) return "Flowers";
            if (itemId >= 10000) return "Mod Items";
            return "Unknown";
        }

        /// <summary>
        /// Checks if an item ID is in the valid range.
        /// </summary>
        /// <param name="itemId">The item ID to check.</param>
        /// <returns>True if the ID is valid.</returns>
        public static bool IsValidItemId(int itemId)
        {
            return itemId > 0 && itemId < 10000;
        }

        /// <summary>
        /// Checks if an item ID is for a mod item.
        /// </summary>
        /// <param name="itemId">The item ID to check.</param>
        /// <returns>True if this is a mod item.</returns>
        public static bool IsModItem(int itemId)
        {
            return itemId >= 10000;
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Initializes the Inventory API. Called automatically by the plugin.
        /// </summary>
        internal static void Initialize()
        {
            if (_initialized) return;
            _initialized = true;
            
            CacheReferences();
            Plugin.Log.LogDebug("InventoryAPI initialized");
        }

        #endregion

        #region Private Methods

        private static void EnsureInitialized()
        {
            if (!_initialized)
            {
                Initialize();
            }

            if (_gameScript == null)
            {
                CacheReferences();
            }
        }

        private static void CacheReferences()
        {
            try
            {
                _gameScript = UnityEngine.Object.FindObjectOfType<GameScript>();

                if (_gameScript != null)
                {
                    var type = typeof(GameScript);
                    
                    // Get AddItem method with specific signature
                    _addItemMethod = type.GetMethod("AddItem", 
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        null, new Type[] { typeof(int), typeof(int) }, null);
                    
                    // Try to get RemoveItem if it exists
                    _removeItemMethod = type.GetMethod("RemoveItem", 
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        null, new Type[] { typeof(int), typeof(int) }, null);
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Failed to cache inventory references: {ex.Message}");
            }
        }

        #endregion
    }
}
