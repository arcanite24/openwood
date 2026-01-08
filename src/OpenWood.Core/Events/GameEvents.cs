using System;
using System.Collections.Generic;

namespace OpenWood.Core.Events
{
    /// <summary>
    /// Central event system for game events that mods can subscribe to.
    /// </summary>
    public static class GameEvents
    {
        private static bool _initialized;
        private static readonly List<Action> _updateCallbacks = new List<Action>();

        // Game lifecycle events
        public static event Action OnGameStart;
        public static event Action OnGameSave;
        public static event Action OnGameLoad;
        public static event Action<int> OnDayStart;
        public static event Action<int> OnDayEnd;
        public static event Action<string> OnSeasonChange;

        // Player events
        public static event Action<float, float> OnPlayerMove;
        public static event Action<string> OnPlayerInteract;
        public static event Action<int> OnPlayerMoneyChange;
        public static event Action<int> OnPlayerLevelUp;

        // NPC events
        public static event Action<string> OnNPCSpawn;
        public static event Action<string, string> OnNPCDialogue;
        public static event Action<string, int> OnNPCFriendshipChange;

        // Item events
        public static event Action<string, int> OnItemPickup;
        public static event Action<string, int> OnItemDrop;
        public static event Action<string, int> OnItemCraft;
        public static event Action<string, int> OnItemSell;

        // Building events
        public static event Action<string, int, int> OnBuildingPlace;
        public static event Action<string, int, int> OnBuildingRemove;
        public static event Action<string> OnBuildingUpgrade;

        // World events
        public static event Action<int, int, string> OnTileChange;
        public static event Action<string> OnWeatherChange;

        internal static void Initialize()
        {
            if (_initialized) return;
            _initialized = true;
            Plugin.Log.LogDebug("GameEvents initialized");
        }

        internal static void Tick()
        {
            foreach (var callback in _updateCallbacks)
            {
                try
                {
                    callback?.Invoke();
                }
                catch (Exception ex)
                {
                    Plugin.Log.LogError($"Error in update callback: {ex}");
                }
            }
        }

        /// <summary>
        /// Register a callback to be called every frame.
        /// </summary>
        public static void RegisterUpdate(Action callback)
        {
            if (callback != null && !_updateCallbacks.Contains(callback))
            {
                _updateCallbacks.Add(callback);
            }
        }

        /// <summary>
        /// Unregister a previously registered update callback.
        /// </summary>
        public static void UnregisterUpdate(Action callback)
        {
            _updateCallbacks.Remove(callback);
        }

        // Internal trigger methods - called from Harmony patches
        internal static void TriggerGameStart() => OnGameStart?.Invoke();
        internal static void TriggerGameSave() => OnGameSave?.Invoke();
        internal static void TriggerGameLoad() => OnGameLoad?.Invoke();
        internal static void TriggerDayStart(int day) => OnDayStart?.Invoke(day);
        internal static void TriggerDayEnd(int day) => OnDayEnd?.Invoke(day);
        internal static void TriggerSeasonChange(string season) => OnSeasonChange?.Invoke(season);
        
        internal static void TriggerPlayerMove(float x, float y) => OnPlayerMove?.Invoke(x, y);
        internal static void TriggerPlayerInteract(string target) => OnPlayerInteract?.Invoke(target);
        internal static void TriggerPlayerMoneyChange(int amount) => OnPlayerMoneyChange?.Invoke(amount);
        internal static void TriggerPlayerLevelUp(int level) => OnPlayerLevelUp?.Invoke(level);
        
        internal static void TriggerNPCSpawn(string npcId) => OnNPCSpawn?.Invoke(npcId);
        internal static void TriggerNPCDialogue(string npcId, string dialogue) => OnNPCDialogue?.Invoke(npcId, dialogue);
        internal static void TriggerNPCFriendshipChange(string npcId, int level) => OnNPCFriendshipChange?.Invoke(npcId, level);
        
        internal static void TriggerItemPickup(string itemId, int count) => OnItemPickup?.Invoke(itemId, count);
        internal static void TriggerItemDrop(string itemId, int count) => OnItemDrop?.Invoke(itemId, count);
        internal static void TriggerItemCraft(string itemId, int count) => OnItemCraft?.Invoke(itemId, count);
        internal static void TriggerItemSell(string itemId, int count) => OnItemSell?.Invoke(itemId, count);
        
        internal static void TriggerBuildingPlace(string buildingId, int x, int y) => OnBuildingPlace?.Invoke(buildingId, x, y);
        internal static void TriggerBuildingRemove(string buildingId, int x, int y) => OnBuildingRemove?.Invoke(buildingId, x, y);
        internal static void TriggerBuildingUpgrade(string buildingId) => OnBuildingUpgrade?.Invoke(buildingId);
        
        internal static void TriggerTileChange(int x, int y, string tileType) => OnTileChange?.Invoke(x, y, tileType);
        internal static void TriggerWeatherChange(string weather) => OnWeatherChange?.Invoke(weather);
    }
}
