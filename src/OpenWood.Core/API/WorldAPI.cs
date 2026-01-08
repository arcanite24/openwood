using System;
using System.Reflection;
using UnityEngine;

namespace OpenWood.Core.API
{
    /// <summary>
    /// API for controlling and querying world-related game state.
    /// Provides methods for unlocking features, managing discoveries, and world info.
    /// </summary>
    /// <example>
    /// <code>
    /// // Unlock all tools
    /// WorldAPI.UnlockAllTools();
    /// 
    /// // Check if player is in build mode
    /// if (WorldAPI.IsInBuildMode) { ... }
    /// 
    /// // Discover all items
    /// WorldAPI.DiscoverAllItems();
    /// </code>
    /// </example>
    public static class WorldAPI
    {
        #region Enums

        /// <summary>
        /// Known location types in Littlewood.
        /// </summary>
        public enum Location
        {
            /// <summary>The main town area</summary>
            Town,
            /// <summary>Inside a building</summary>
            Indoors,
            /// <summary>The forest area</summary>
            Forest,
            /// <summary>The beach area</summary>
            Beach,
            /// <summary>The mine/cavern area</summary>
            Cavern
        }

        /// <summary>
        /// Tool types in Littlewood.
        /// </summary>
        public enum Tool
        {
            /// <summary>Axe for chopping wood</summary>
            Axe = 0,
            /// <summary>Pickaxe for mining</summary>
            Pickaxe = 1,
            /// <summary>Fishing rod</summary>
            FishingRod = 2,
            /// <summary>Bug net</summary>
            BugNet = 3,
            /// <summary>Watering can</summary>
            WateringCan = 4,
            /// <summary>Hoe for farming</summary>
            Hoe = 5
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets whether the player is currently indoors.
        /// </summary>
        public static bool IsIndoors => GameScript.isIndoor;

        /// <summary>
        /// Gets the player's current position.
        /// </summary>
        public static Vector2 PlayerPosition => GameScript.playerPos;

        /// <summary>
        /// Gets whether the player is in build/edit mode.
        /// </summary>
        public static bool IsInBuildMode => GameScript.building;

        /// <summary>
        /// Gets whether the player is in edit mode.
        /// </summary>
        public static bool IsInEditMode => GameScript.inEditMode;

        /// <summary>
        /// Gets the player's facing direction.
        /// </summary>
        public static int FacingDirection => GameScript.facingDir;

        #endregion

        #region Public Methods - Tools

        /// <summary>
        /// Unlocks a specific tool.
        /// </summary>
        /// <param name="tool">The tool to unlock.</param>
        public static void UnlockTool(Tool tool)
        {
            UnlockTool((int)tool);
        }

        /// <summary>
        /// Unlocks a tool by index.
        /// </summary>
        /// <param name="toolIndex">The tool index.</param>
        public static void UnlockTool(int toolIndex)
        {
            try
            {
                var toolUnlocked = typeof(GameScript).GetField("toolUnlocked", 
                    BindingFlags.Static | BindingFlags.Public);
                
                if (toolUnlocked != null)
                {
                    var tools = toolUnlocked.GetValue(null) as int[];
                    if (tools != null && toolIndex >= 0 && toolIndex < tools.Length)
                    {
                        tools[toolIndex] = 1;
                        Plugin.Log.LogDebug($"Unlocked tool {toolIndex}");
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Failed to unlock tool: {ex.Message}");
            }
        }

        /// <summary>
        /// Unlocks all tools.
        /// </summary>
        public static void UnlockAllTools()
        {
            try
            {
                var toolUnlocked = typeof(GameScript).GetField("toolUnlocked", 
                    BindingFlags.Static | BindingFlags.Public);
                
                if (toolUnlocked != null)
                {
                    var tools = toolUnlocked.GetValue(null) as int[];
                    if (tools != null)
                    {
                        for (int i = 0; i < tools.Length; i++)
                        {
                            tools[i] = 1;
                        }
                        Plugin.Log.LogInfo("Unlocked all tools");
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Failed to unlock tools: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if a tool is unlocked.
        /// </summary>
        /// <param name="tool">The tool to check.</param>
        /// <returns>True if the tool is unlocked.</returns>
        public static bool IsToolUnlocked(Tool tool)
        {
            return IsToolUnlocked((int)tool);
        }

        /// <summary>
        /// Checks if a tool is unlocked by index.
        /// </summary>
        /// <param name="toolIndex">The tool index.</param>
        /// <returns>True if the tool is unlocked.</returns>
        public static bool IsToolUnlocked(int toolIndex)
        {
            try
            {
                var toolUnlocked = typeof(GameScript).GetField("toolUnlocked", 
                    BindingFlags.Static | BindingFlags.Public);
                
                if (toolUnlocked != null)
                {
                    var tools = toolUnlocked.GetValue(null) as int[];
                    if (tools != null && toolIndex >= 0 && toolIndex < tools.Length)
                    {
                        return tools[toolIndex] == 1;
                    }
                }
            }
            catch { }
            return false;
        }

        #endregion

        #region Public Methods - Discovery

        /// <summary>
        /// Discovers an item by ID (marks it as seen in the collection).
        /// </summary>
        /// <param name="itemId">The item ID to discover.</param>
        public static void DiscoverItem(int itemId)
        {
            try
            {
                if (itemId >= 0 && itemId < GameScript.discoverLevel.Length)
                {
                    GameScript.discoverLevel[itemId] = 1;
                    Plugin.Log.LogDebug($"Discovered item {itemId}");
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Failed to discover item: {ex.Message}");
            }
        }

        /// <summary>
        /// Discovers all items in the game.
        /// </summary>
        public static void DiscoverAllItems()
        {
            try
            {
                for (int i = 0; i < GameScript.discoverLevel.Length; i++)
                {
                    GameScript.discoverLevel[i] = 1;
                }
                Plugin.Log.LogInfo("Discovered all items");
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Failed to discover items: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if an item has been discovered.
        /// </summary>
        /// <param name="itemId">The item ID to check.</param>
        /// <returns>True if the item has been discovered.</returns>
        public static bool IsItemDiscovered(int itemId)
        {
            try
            {
                if (itemId >= 0 && itemId < GameScript.discoverLevel.Length)
                {
                    return GameScript.discoverLevel[itemId] > 0;
                }
            }
            catch { }
            return false;
        }

        #endregion

        #region Public Methods - Recipes

        /// <summary>
        /// Unlocks all crafting recipes.
        /// </summary>
        public static void UnlockAllRecipes()
        {
            try
            {
                var recipeUnlocked = typeof(GameScript).GetField("recipeUnlocked", 
                    BindingFlags.Static | BindingFlags.Public);
                
                if (recipeUnlocked != null)
                {
                    var recipes = recipeUnlocked.GetValue(null) as int[];
                    if (recipes != null)
                    {
                        for (int i = 0; i < recipes.Length; i++)
                        {
                            recipes[i] = 1;
                        }
                        Plugin.Log.LogInfo("Unlocked all recipes");
                    }
                }
                else
                {
                    Plugin.Log.LogWarning("Recipe field not found - may not be implemented in this game version");
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Failed to unlock recipes: {ex.Message}");
            }
        }

        #endregion

        #region Public Methods - Museum

        /// <summary>
        /// Unlocks all museum items/donations.
        /// </summary>
        public static void UnlockMuseum()
        {
            try
            {
                var museumDonated = typeof(GameScript).GetField("museumDonated", 
                    BindingFlags.Static | BindingFlags.Public);
                
                if (museumDonated != null)
                {
                    var museum = museumDonated.GetValue(null) as int[];
                    if (museum != null)
                    {
                        for (int i = 0; i < museum.Length; i++)
                        {
                            museum[i] = 1;
                        }
                        Plugin.Log.LogInfo("Unlocked all museum items");
                    }
                }
                else
                {
                    Plugin.Log.LogWarning("Museum field not found - may not be implemented in this game version");
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Failed to unlock museum: {ex.Message}");
            }
        }

        #endregion

        #region Public Methods - Teleportation

        /// <summary>
        /// Common teleport locations with their coordinates.
        /// </summary>
        public static class Locations
        {
            /// <summary>Town center coordinates.</summary>
            public static readonly Vector2 TownCenter = new Vector2(0, 0);
            
            /// <summary>Farm area coordinates.</summary>
            public static readonly Vector2 FarmArea = new Vector2(-3, -3);
            
            /// <summary>Forest entrance coordinates.</summary>
            public static readonly Vector2 ForestEntrance = new Vector2(5, 5);
            
            /// <summary>Beach coordinates.</summary>
            public static readonly Vector2 Beach = new Vector2(-5, 0);
        }

        /// <summary>
        /// Teleports the player to a predefined location.
        /// </summary>
        /// <param name="location">The location to teleport to.</param>
        public static void TeleportTo(Location location)
        {
            Vector2 target;
            switch (location)
            {
                case Location.Town:
                    target = Locations.TownCenter;
                    break;
                case Location.Forest:
                    target = Locations.ForestEntrance;
                    break;
                case Location.Beach:
                    target = Locations.Beach;
                    break;
                default:
                    target = Locations.TownCenter;
                    break;
            }
            
            PlayerAPI.Teleport(target);
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Initializes the World API. Called automatically by the plugin.
        /// </summary>
        internal static void Initialize()
        {
            Plugin.Log.LogDebug("WorldAPI initialized");
        }

        #endregion
    }
}
