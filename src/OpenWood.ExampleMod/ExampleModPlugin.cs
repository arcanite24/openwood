using BepInEx;
using BepInEx.Configuration;
using OpenWood.Core;
using OpenWood.Core.API;
using OpenWood.Core.Events;
using OpenWood.Core.Items;
using OpenWood.Core.UI;
using UnityEngine;

namespace OpenWood.ExampleMod
{
    /// <summary>
    /// Example mod demonstrating the OpenWood modding API.
    /// 
    /// This mod showcases:
    /// - Event subscriptions (GameEvents)
    /// - API usage (PlayerAPI, TimeAPI, NPCAPI, etc.)
    /// - Custom item registration
    /// - IMGUI window creation
    /// - BepInEx configuration
    /// </summary>
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInDependency(OpenWood.Core.PluginInfo.PLUGIN_GUID)]
    public class ExampleModPlugin : BaseUnityPlugin
    {
        public const string GUID = "com.openwood.examplemod";
        public const string NAME = "Example Mod";
        public const string VERSION = "1.0.0";

        #region Configuration

        private ConfigEntry<bool> _enableDebugWindow;
        private ConfigEntry<KeyCode> _debugWindowKey;
        private ConfigEntry<int> _bonusMoneyPerDay;

        #endregion

        #region UI

        private ModWindow _debugWindow;
        private bool _showItems;
        private bool _showPlayerStats;
        private bool _showNPCStats;

        #endregion

        #region Lifecycle

        private void Awake()
        {
            Logger.LogInfo($"{NAME} is loading...");

            // Setup configuration
            SetupConfiguration();

            // Subscribe to game events
            SubscribeToEvents();

            // Register custom items
            RegisterCustomItems();

            // Setup debug UI
            SetupUI();

            Logger.LogInfo($"{NAME} loaded successfully!");
        }

        private void OnDestroy()
        {
            // Unsubscribe from events to prevent memory leaks
            UnsubscribeFromEvents();
            
            // Unregister UI
            if (_debugWindow != null)
            {
                ModUI.UnregisterWindow(_debugWindow);
            }
        }

        private void Update()
        {
            // Toggle debug window
            if (Input.GetKeyDown(_debugWindowKey.Value))
            {
                _debugWindow?.Toggle();
            }
        }

        #endregion

        #region Configuration Setup

        private void SetupConfiguration()
        {
            _enableDebugWindow = Config.Bind(
                "General",
                "EnableDebugWindow",
                true,
                "Show the debug window (toggle with configured key)"
            );

            _debugWindowKey = Config.Bind(
                "General",
                "DebugWindowKey",
                KeyCode.F2,
                "Key to toggle the debug window"
            );

            _bonusMoneyPerDay = Config.Bind(
                "Cheats",
                "BonusMoneyPerDay",
                100,
                "Amount of bonus money to receive at the start of each day"
            );
        }

        #endregion

        #region Event Subscriptions

        private void SubscribeToEvents()
        {
            // Game lifecycle events
            GameEvents.OnGameStart += OnGameStart;
            GameEvents.OnGameSave += OnGameSave;
            GameEvents.OnGameLoad += OnGameLoad;
            
            // Time events
            GameEvents.OnDayStart += OnDayStart;
            GameEvents.OnDayEnd += OnDayEnd;
            
            // Player events
            GameEvents.OnItemPickup += OnItemPickup;
            GameEvents.OnPlayerLevelUp += OnPlayerLevelUp;
            
            // NPC events
            GameEvents.OnNPCFriendshipChange += OnNPCFriendshipChange;
        }

        private void UnsubscribeFromEvents()
        {
            GameEvents.OnGameStart -= OnGameStart;
            GameEvents.OnGameSave -= OnGameSave;
            GameEvents.OnGameLoad -= OnGameLoad;
            GameEvents.OnDayStart -= OnDayStart;
            GameEvents.OnDayEnd -= OnDayEnd;
            GameEvents.OnItemPickup -= OnItemPickup;
            GameEvents.OnPlayerLevelUp -= OnPlayerLevelUp;
            GameEvents.OnNPCFriendshipChange -= OnNPCFriendshipChange;
        }

        #endregion

        #region Custom Items

        private void RegisterCustomItems()
        {
            // Example: Register a custom fertilizer item
            var superFertilizer = new ModItem
            {
                Id = "example_super_fertilizer",
                Name = "Super Fertilizer",
                Description = "Makes crops grow twice as fast!",
                Category = "Tool",
                SellPrice = 500,
                MaxStack = 10,
                SpritePath = "items/super_fertilizer.png",
                OnUse = (item) =>
                {
                    Logger.LogInfo("Super Fertilizer used!");
                    ModUI.ShowNotification("Crops will grow faster!");
                }
            };

            ItemRegistry.Register(superFertilizer);

            // Example: Register a custom decoration
            var goldenStatue = new ModItem
            {
                Id = "example_golden_statue",
                Name = "Golden Statue",
                Description = "A magnificent golden statue for your town.",
                Category = "Decoration",
                SellPrice = 10000,
                MaxStack = 1,
                CanGift = false
            };

            ItemRegistry.Register(goldenStatue);
            
            Logger.LogDebug($"Registered {ItemRegistry.Items.Count} custom items");
        }

        #endregion

        #region UI Setup

        private void SetupUI()
        {
            if (!_enableDebugWindow.Value) return;

            _debugWindow = new ModWindow(
                "example_debug",
                "Example Mod Debug",
                new Rect(20, 20, 350, 450)
            );

            _debugWindow.DrawContent = DrawDebugWindow;
            ModUI.RegisterWindow(_debugWindow);
        }

        private void DrawDebugWindow(int windowId)
        {
            GUILayout.Label($"Example Mod v{VERSION}");
            GUILayout.Space(10);

            // Player Stats Section
            _showPlayerStats = GUILayout.Toggle(_showPlayerStats, "Show Player Stats");
            if (_showPlayerStats)
            {
                DrawPlayerStats();
            }

            GUILayout.Space(10);

            // NPC Stats Section
            _showNPCStats = GUILayout.Toggle(_showNPCStats, "Show NPC Stats");
            if (_showNPCStats)
            {
                DrawNPCStats();
            }

            GUILayout.Space(10);

            // Custom Items Section
            _showItems = GUILayout.Toggle(_showItems, "Show Registered Items");
            if (_showItems)
            {
                DrawRegisteredItems();
            }

            GUILayout.Space(10);

            // API Demo Buttons
            DrawAPIButtons();
        }

        private void DrawPlayerStats()
        {
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("Player Status:");
            GUILayout.Label($"  Money: {PlayerAPI.Money} Dew");
            GUILayout.Label($"  Day EXP: {PlayerAPI.DayExperience}/100");
            GUILayout.Label($"  Position: {PlayerAPI.Position}");
            GUILayout.Label($"  Indoors: {PlayerAPI.IsIndoors}");
            GUILayout.Label($"  Date: {TimeAPI.GetFormattedDate()}");
            GUILayout.EndVertical();
        }

        private void DrawNPCStats()
        {
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("NPC Friendships:");
            
            var npcs = new[] { 
                NPCAPI.NPC.Willow, NPCAPI.NPC.Dalton, NPCAPI.NPC.Dudley, NPCAPI.NPC.Laura 
            };
            
            foreach (var npc in npcs)
            {
                int hearts = NPCAPI.GetFriendship(npc);
                GUILayout.Label($"  {NPCAPI.GetName(npc)}: {hearts}/10 hearts");
            }
            GUILayout.EndVertical();
        }

        private void DrawRegisteredItems()
        {
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("Custom Items:");
            foreach (var item in ItemRegistry.Items.Values)
            {
                GUILayout.Label($"  • {item.Name} ({item.Id})");
                GUILayout.Label($"    Price: {item.SellPrice} | Stack: {item.MaxStack}");
            }
            GUILayout.EndVertical();
        }

        private void DrawAPIButtons()
        {
            GUILayout.Label("API Demonstrations:");

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add 1000 Dew"))
            {
                PlayerAPI.AddMoney(1000);
                Logger.LogInfo("Added 1000 dewdrops via API");
            }
            if (GUILayout.Button("Add Wood"))
            {
                InventoryAPI.AddItem(InventoryAPI.ItemID.Wood, 10);
                Logger.LogInfo("Added 10 wood via API");
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Next Day"))
            {
                TimeAPI.AdvanceDay();
                Logger.LogInfo("Advanced day via API");
            }
            if (GUILayout.Button("Toggle Rain"))
            {
                TimeAPI.ToggleRain();
                Logger.LogInfo($"Rain toggled: {TimeAPI.IsRaining}");
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Max Willow Hearts"))
            {
                NPCAPI.SetFriendship(NPCAPI.NPC.Willow, NPCAPI.MaxFriendshipLevel);
                Logger.LogInfo("Maxed Willow friendship via API");
            }
            if (GUILayout.Button("Dump State"))
            {
                GameAPI.DumpGameState();
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Show Notification"))
            {
                ModUI.ShowNotification("Hello from Example Mod!");
            }
        }

        #endregion

        #region Event Handlers

        private void OnGameStart()
        {
            Logger.LogInfo("Game started! Welcome to Littlewood.");
        }

        private void OnGameSave()
        {
            Logger.LogDebug("Game saved.");
        }

        private void OnGameLoad()
        {
            Logger.LogDebug("Game loaded.");
        }

        private void OnDayStart(int day)
        {
            Logger.LogInfo($"Day {day} has started!");
            
            // Give bonus money if configured
            if (_bonusMoneyPerDay.Value > 0)
            {
                PlayerAPI.AddMoney(_bonusMoneyPerDay.Value);
                Logger.LogDebug($"Gave {_bonusMoneyPerDay.Value} bonus dewdrops");
            }
        }

        private void OnDayEnd(int day)
        {
            Logger.LogDebug($"Day {day} ended. Good night!");
        }

        private void OnItemPickup(string itemId, int count)
        {
            Logger.LogDebug($"Picked up {count}x item ID {itemId}");
        }

        private void OnPlayerLevelUp(int level)
        {
            Logger.LogInfo($"Congratulations! You reached level {level}!");
            ModUI.ShowNotification($"Level Up! You are now level {level}!");
        }

        private void OnNPCFriendshipChange(string npcId, int level)
        {
            Logger.LogDebug($"NPC {npcId} friendship changed to {level}");
        }

        #endregion
    }
}
