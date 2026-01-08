using System;
using System.Collections.Generic;

namespace OpenWood.Core.Items
{
    /// <summary>
    /// Represents a custom item that can be added to the game.
    /// </summary>
    public class ModItem
    {
        /// <summary>
        /// Unique identifier for this item.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Display name shown in-game.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Item description shown in tooltips.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Category of the item (e.g., "Tool", "Seed", "Fish", "Material").
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Base sell price in Dewdrops.
        /// </summary>
        public int SellPrice { get; set; }

        /// <summary>
        /// Maximum stack size.
        /// </summary>
        public int MaxStack { get; set; } = 99;

        /// <summary>
        /// Path to the item's sprite texture.
        /// </summary>
        public string SpritePath { get; set; }

        /// <summary>
        /// Whether this item can be gifted to NPCs.
        /// </summary>
        public bool CanGift { get; set; } = true;

        /// <summary>
        /// Whether this item can be sold.
        /// </summary>
        public bool CanSell { get; set; } = true;

        /// <summary>
        /// Custom data storage for mod-specific properties.
        /// </summary>
        public Dictionary<string, object> CustomData { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Action called when the item is used.
        /// </summary>
        public Action<ModItem> OnUse { get; set; }

        /// <summary>
        /// Action called when the item is picked up.
        /// </summary>
        public Action<ModItem, int> OnPickup { get; set; }

        /// <summary>
        /// Action called when the item is dropped.
        /// </summary>
        public Action<ModItem, int> OnDrop { get; set; }
    }

    /// <summary>
    /// Registry for custom mod items.
    /// </summary>
    public static class ItemRegistry
    {
        private static bool _initialized;
        private static readonly Dictionary<string, ModItem> _items = new Dictionary<string, ModItem>();
        private static int _nextInternalId = 10000; // Start high to avoid conflicts

        /// <summary>
        /// All registered mod items.
        /// </summary>
        public static IReadOnlyDictionary<string, ModItem> Items => _items;

        internal static void Initialize()
        {
            if (_initialized) return;
            _initialized = true;
            Plugin.Log.LogDebug("ItemRegistry initialized");
        }

        /// <summary>
        /// Register a new custom item.
        /// </summary>
        /// <param name="item">The item to register</param>
        /// <returns>True if registration was successful</returns>
        public static bool Register(ModItem item)
        {
            if (item == null)
            {
                Plugin.Log.LogError("Cannot register null item");
                return false;
            }

            if (string.IsNullOrEmpty(item.Id))
            {
                Plugin.Log.LogError("Item must have an Id");
                return false;
            }

            if (_items.ContainsKey(item.Id))
            {
                Plugin.Log.LogWarning($"Item with Id '{item.Id}' already registered, overwriting");
            }

            _items[item.Id] = item;
            Plugin.Log.LogInfo($"Registered item: {item.Id} ({item.Name})");
            return true;
        }

        /// <summary>
        /// Unregister an item by its Id.
        /// </summary>
        public static bool Unregister(string itemId)
        {
            if (_items.Remove(itemId))
            {
                Plugin.Log.LogInfo($"Unregistered item: {itemId}");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get an item by its Id.
        /// </summary>
        public static ModItem Get(string itemId)
        {
            return _items.TryGetValue(itemId, out var item) ? item : null;
        }

        /// <summary>
        /// Check if an item with the given Id is registered.
        /// </summary>
        public static bool Exists(string itemId)
        {
            return _items.ContainsKey(itemId);
        }

        /// <summary>
        /// Get all items in a specific category.
        /// </summary>
        public static IEnumerable<ModItem> GetByCategory(string category)
        {
            foreach (var item in _items.Values)
            {
                if (item.Category == category)
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Get the next available internal ID for game integration.
        /// </summary>
        internal static int GetNextInternalId()
        {
            return _nextInternalId++;
        }
    }
}
