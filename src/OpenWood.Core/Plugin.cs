using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using OpenWood.Core.Cheats;
using System;
using System.Reflection;

namespace OpenWood.Core
{
    /// <summary>
    /// Main entry point for the OpenWood modding API.
    /// This plugin provides the foundation for all Littlewood mods.
    /// </summary>
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }
        public static ManualLogSource Log { get; private set; }
        
        private Harmony _harmony;

        private void Awake()
        {
            Instance = this;
            Log = Logger;
            
            Logger.LogInfo($"OpenWood {PluginInfo.PLUGIN_VERSION} is loading...");
            
            // Initialize Harmony for patching
            _harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
            
            // Initialize subsystems
            InitializeEventSystem();
            InitializeAssetManager();
            InitializeItemRegistry();
            InitializeCheatMenu();
            
            Logger.LogInfo("OpenWood loaded successfully!");
        }

        private void OnDestroy()
        {
            _harmony?.UnpatchSelf();
            Logger.LogInfo("OpenWood unloaded.");
        }

        private void InitializeEventSystem()
        {
            Logger.LogDebug("Initializing event system...");
            Events.GameEvents.Initialize();
        }

        private void InitializeAssetManager()
        {
            Logger.LogDebug("Initializing asset manager...");
            Assets.AssetManager.Initialize();
        }

        private void InitializeItemRegistry()
        {
            Logger.LogDebug("Initializing item registry...");
            Items.ItemRegistry.Initialize();
        }

        private void InitializeCheatMenu()
        {
            Logger.LogDebug("Initializing cheat menu...");
            CheatMenu.Initialize();
        }

        private void Update()
        {
            // Tick the event system
            Events.GameEvents.Tick();
        }

        private void OnGUI()
        {
            // Allow mods to render custom UI
            UI.ModUI.OnGUI();
        }
    }
}
