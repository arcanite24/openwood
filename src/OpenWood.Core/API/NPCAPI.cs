using System;
using UnityEngine;

namespace OpenWood.Core.API
{
    /// <summary>
    /// API for controlling and querying NPC-related game state.
    /// Provides methods for managing friendships, romance, and NPC interactions.
    /// </summary>
    /// <example>
    /// <code>
    /// // Set friendship with Willow to max
    /// NPCAPI.SetFriendship(NPC.Willow, 10);
    /// 
    /// // Get friendship level
    /// int hearts = NPCAPI.GetFriendship(NPC.Dalton);
    /// 
    /// // Max all friendships
    /// NPCAPI.MaxAllFriendships();
    /// </code>
    /// </example>
    public static class NPCAPI
    {
        #region Enums

        /// <summary>
        /// Represents the townsfolk NPCs in Littlewood.
        /// IDs correspond to the game's internal townsfolk array indices.
        /// </summary>
        public enum NPC
        {
            /// <summary>Willow - The first villager</summary>
            Willow = 1,
            /// <summary>Dalton - The blacksmith</summary>
            Dalton = 2,
            /// <summary>Dudley - The wizard</summary>
            Dudley = 3,
            /// <summary>Laura - The coffee shop owner</summary>
            Laura = 4,
            /// <summary>Bubsy - The innkeeper</summary>
            Bubsy = 5,
            /// <summary>Ash - The adventurer</summary>
            Ash = 6,
            /// <summary>Lilith - The dark mage</summary>
            Lilith = 7,
            /// <summary>Zana - The musician</summary>
            Zana = 8,
            /// <summary>Eunice - Available later</summary>
            Eunice = 9,
            /// <summary>Clyde - Available later</summary>
            Clyde = 10,
            /// <summary>Wolfgang - Available later</summary>
            Wolfgang = 11,
            /// <summary>Iris - Available later</summary>
            Iris = 12,
            /// <summary>August - Available later</summary>
            August = 13,
            /// <summary>Mia - Available later</summary>
            Mia = 14,
            /// <summary>Theo - Available later</summary>
            Theo = 15
        }

        #endregion

        #region Constants

        /// <summary>
        /// Maximum friendship level.
        /// </summary>
        public const int MaxFriendshipLevel = 10;

        /// <summary>
        /// Maximum romance level.
        /// </summary>
        public const int MaxRomanceLevel = 10;

        /// <summary>
        /// Total number of potential NPCs in the game.
        /// </summary>
        public const int TotalNPCCount = 20;

        #endregion

        #region Public Methods - Friendship

        /// <summary>
        /// Gets the friendship level for an NPC.
        /// </summary>
        /// <param name="npc">The NPC to query.</param>
        /// <returns>Friendship level (0-10).</returns>
        public static int GetFriendship(NPC npc)
        {
            return GetFriendship((int)npc);
        }

        /// <summary>
        /// Gets the friendship level for an NPC by ID.
        /// </summary>
        /// <param name="npcId">The NPC ID (1-based index).</param>
        /// <returns>Friendship level (0-10).</returns>
        public static int GetFriendship(int npcId)
        {
            if (npcId >= 0 && npcId < GameScript.townsfolkLevel.Length)
            {
                return GameScript.townsfolkLevel[npcId];
            }
            return 0;
        }

        /// <summary>
        /// Sets the friendship level for an NPC.
        /// </summary>
        /// <param name="npc">The NPC to modify.</param>
        /// <param name="level">Friendship level (0-10).</param>
        public static void SetFriendship(NPC npc, int level)
        {
            SetFriendship((int)npc, level);
        }

        /// <summary>
        /// Sets the friendship level for an NPC by ID.
        /// </summary>
        /// <param name="npcId">The NPC ID (1-based index).</param>
        /// <param name="level">Friendship level (0-10).</param>
        public static void SetFriendship(int npcId, int level)
        {
            try
            {
                if (npcId >= 0 && npcId < GameScript.townsfolkLevel.Length)
                {
                    GameScript.townsfolkLevel[npcId] = Mathf.Clamp(level, 0, MaxFriendshipLevel);
                    Plugin.Log.LogDebug($"Set NPC {npcId} friendship to {level}");
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Failed to set NPC friendship: {ex.Message}");
            }
        }

        /// <summary>
        /// Adds to the friendship level for an NPC.
        /// </summary>
        /// <param name="npc">The NPC to modify.</param>
        /// <param name="amount">Amount to add (can be negative).</param>
        public static void AddFriendship(NPC npc, int amount)
        {
            AddFriendship((int)npc, amount);
        }

        /// <summary>
        /// Adds to the friendship level for an NPC by ID.
        /// </summary>
        /// <param name="npcId">The NPC ID (1-based index).</param>
        /// <param name="amount">Amount to add (can be negative).</param>
        public static void AddFriendship(int npcId, int amount)
        {
            int current = GetFriendship(npcId);
            SetFriendship(npcId, current + amount);
        }

        /// <summary>
        /// Sets all NPCs to maximum friendship level.
        /// </summary>
        public static void MaxAllFriendships()
        {
            for (int i = 1; i <= TotalNPCCount; i++)
            {
                SetFriendship(i, MaxFriendshipLevel);
            }
            Plugin.Log.LogInfo("Maxed all NPC friendships");
        }

        /// <summary>
        /// Resets all NPC friendships to zero.
        /// </summary>
        public static void ResetAllFriendships()
        {
            for (int i = 1; i <= TotalNPCCount; i++)
            {
                SetFriendship(i, 0);
            }
            Plugin.Log.LogInfo("Reset all NPC friendships");
        }

        #endregion

        #region Public Methods - Romance

        /// <summary>
        /// Gets the romance level for an NPC.
        /// </summary>
        /// <param name="npc">The NPC to query.</param>
        /// <returns>Romance level (0-10).</returns>
        public static int GetRomance(NPC npc)
        {
            return GetRomance((int)npc);
        }

        /// <summary>
        /// Gets the romance level for an NPC by ID.
        /// </summary>
        /// <param name="npcId">The NPC ID (1-based index).</param>
        /// <returns>Romance level (0-10).</returns>
        public static int GetRomance(int npcId)
        {
            try
            {
                if (GameScript.townsfolkRomanceLvl != null && 
                    npcId >= 0 && npcId < GameScript.townsfolkRomanceLvl.Length)
                {
                    return GameScript.townsfolkRomanceLvl[npcId];
                }
            }
            catch { }
            return 0;
        }

        /// <summary>
        /// Sets the romance level for an NPC.
        /// </summary>
        /// <param name="npc">The NPC to modify.</param>
        /// <param name="level">Romance level (0-10).</param>
        public static void SetRomance(NPC npc, int level)
        {
            SetRomance((int)npc, level);
        }

        /// <summary>
        /// Sets the romance level for an NPC by ID.
        /// </summary>
        /// <param name="npcId">The NPC ID (1-based index).</param>
        /// <param name="level">Romance level (0-10).</param>
        public static void SetRomance(int npcId, int level)
        {
            try
            {
                if (GameScript.townsfolkRomanceLvl != null && 
                    npcId >= 0 && npcId < GameScript.townsfolkRomanceLvl.Length)
                {
                    GameScript.townsfolkRomanceLvl[npcId] = Mathf.Clamp(level, 0, MaxRomanceLevel);
                    Plugin.Log.LogDebug($"Set NPC {npcId} romance to {level}");
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Failed to set NPC romance: {ex.Message}");
            }
        }

        /// <summary>
        /// Sets all NPCs to maximum romance level.
        /// </summary>
        public static void MaxAllRomance()
        {
            for (int i = 1; i <= TotalNPCCount; i++)
            {
                SetRomance(i, MaxRomanceLevel);
            }
            Plugin.Log.LogInfo("Maxed all NPC romance levels");
        }

        #endregion

        #region Public Methods - Utility

        /// <summary>
        /// Gets the name of an NPC.
        /// </summary>
        /// <param name="npc">The NPC enum value.</param>
        /// <returns>The NPC's name as a string.</returns>
        public static string GetName(NPC npc)
        {
            return npc.ToString();
        }

        /// <summary>
        /// Gets an NPC enum from an ID.
        /// </summary>
        /// <param name="npcId">The NPC ID.</param>
        /// <returns>The NPC enum value, or Willow if invalid.</returns>
        public static NPC GetNPCFromId(int npcId)
        {
            if (Enum.IsDefined(typeof(NPC), npcId))
            {
                return (NPC)npcId;
            }
            return NPC.Willow;
        }

        /// <summary>
        /// Checks if an NPC ID is valid.
        /// </summary>
        /// <param name="npcId">The NPC ID to check.</param>
        /// <returns>True if the ID is valid.</returns>
        public static bool IsValidNPC(int npcId)
        {
            return npcId >= 1 && npcId <= TotalNPCCount;
        }

        /// <summary>
        /// Gets a summary of an NPC's relationship status.
        /// </summary>
        /// <param name="npc">The NPC to query.</param>
        /// <returns>A formatted string with friendship and romance info.</returns>
        public static string GetRelationshipSummary(NPC npc)
        {
            int friendship = GetFriendship(npc);
            int romance = GetRomance(npc);
            return $"{GetName(npc)}: ♥{friendship} ♡{romance}";
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Initializes the NPC API. Called automatically by the plugin.
        /// </summary>
        internal static void Initialize()
        {
            Plugin.Log.LogDebug("NPCAPI initialized");
        }

        #endregion
    }
}
