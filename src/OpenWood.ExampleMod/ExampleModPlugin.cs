using BepInEx;
using BepInEx.Configuration;
using OpenWood.Core;
using OpenWood.Core.Events;
using OpenWood.Core.Items;
using OpenWood.Core.UI;
using UnityEngine;

namespace OpenWood.ExampleMod
{
    /// <summary>
    /// Example mod demonstrating OpenWood API usage.
    /// </summary>
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInDependency(OpenWood.Core.PluginInfo.PLUGIN_GUID)]
    public class ExampleModPlugin : BaseUnityPlugin
    {
        public const string GUID = "com.openwood.examplemod";
        public const string NAME = "Example Mod";
        public const string VERSION = "1.0.0";

        // Config entries
        private ConfigEntry<bool> _enableDebugWindow;
        private ConfigEntry<KeyCode> _debugWindowKey;

        // UI
        private ModWindow _debugWindow;
        private bool _showItems;

        private void Awake()
        {
            Logger.LogInfo($"{NAME} is loading...");

            // Setup configuration
            _enableDebugWindow = Config.Bind("General", "EnableDebugWindow", true, "Show the debug window");
            _debugWindowKey = Config.Bind("General", "DebugWindowKey", KeyCode.F2, "Key to toggle debug window");

            // Subscribe to events
            GameEvents.OnDayStart += OnDayStart;
            GameEvents.OnItemPickup += OnItemPickup;
            GameEvents.OnPlayerLevelUp += OnPlayerLevelUp;

            // Register custom items
            RegisterCustomItems();

            // Setup UI
            SetupUI();

            Logger.LogInfo($"{NAME} loaded successfully!");
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            GameEvents.OnDayStart -= OnDayStart;
            GameEvents.OnItemPickup -= OnItemPickup;
            GameEvents.OnPlayerLevelUp -= OnPlayerLevelUp;
        }

        private void Update()
        {
            // Toggle debug window
            if (Input.GetKeyDown(_debugWindowKey.Value))
            {
                _debugWindow?.Toggle();
            }
        }

        private void RegisterCustomItems()
        {
            // Example: Register a custom item
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
        }

        private void SetupUI()
        {
            if (!_enableDebugWindow.Value) return;

            _debugWindow = new ModWindow(
                "example_debug",
                "Example Mod Debug",
                new Rect(20, 20, 300, 400)
            );

            _debugWindow.DrawContent = DrawDebugWindow;
            ModUI.RegisterWindow(_debugWindow);
        }

        private void DrawDebugWindow(int windowId)
        {
            GUILayout.Label($"Example Mod v{VERSION}");
            GUILayout.Space(10);

            GUILayout.Label("Registered Items:");
            _showItems = GUILayout.Toggle(_showItems, "Show Items");

            if (_showItems)
            {
                GUILayout.BeginVertical(GUI.skin.box);
                foreach (var item in ItemRegistry.Items.Values)
                {
                    GUILayout.Label($"• {item.Name} ({item.Id})");
                }
                GUILayout.EndVertical();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Test Notification"))
            {
                ModUI.ShowNotification("Hello from Example Mod!");
            }

            if (GUILayout.Button("Trigger Day Start Event"))
            {
                // This would normally be called by the game
                Logger.LogInfo("Manually triggering day start event");
            }
        }

        // Event handlers
        private void OnDayStart(int day)
        {
            Logger.LogInfo($"Day {day} has started!");
        }

        private void OnItemPickup(string itemId, int count)
        {
            Logger.LogDebug($"Picked up {count}x {itemId}");
        }

        private void OnPlayerLevelUp(int level)
        {
            Logger.LogInfo($"Congratulations! You reached level {level}!");
            ModUI.ShowNotification($"Level Up! You are now level {level}!");
        }
    }
}
