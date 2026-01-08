using System;
using UnityEngine;

namespace OpenWood.Core.API
{
    /// <summary>
    /// API for controlling and querying general game state.
    /// Provides methods for game state management, debugging, and menu control.
    /// </summary>
    /// <example>
    /// <code>
    /// // Check if player is in a menu
    /// if (GameAPI.IsMenuOpen) { ... }
    /// 
    /// // Close all menus
    /// GameAPI.CloseAllMenus();
    /// 
    /// // Log game state for debugging
    /// GameAPI.DumpGameState();
    /// </code>
    /// </example>
    public static class GameAPI
    {
        #region Properties - Menu State

        /// <summary>
        /// Gets whether any menu is currently open.
        /// </summary>
        public static bool IsMenuOpen => GameScript.menuOpen;

        /// <summary>
        /// Gets whether the player is currently talking to an NPC.
        /// </summary>
        public static bool IsTalking => GameScript.talking;

        /// <summary>
        /// Gets whether the player is currently interacting with something.
        /// </summary>
        public static bool IsInteracting => GameScript.interacting;

        /// <summary>
        /// Gets whether the player is currently fishing.
        /// </summary>
        public static bool IsFishing => GameScript.fishing;

        /// <summary>
        /// Gets whether the player is in building mode.
        /// </summary>
        public static bool IsBuilding => GameScript.building;

        /// <summary>
        /// Gets whether the game is paused.
        /// </summary>
        public static bool IsPaused => Time.timeScale == 0f;

        #endregion

        #region Properties - Statistics

        /// <summary>
        /// Gets the total days played in this save.
        /// </summary>
        public static int DaysPlayed => GameScript.daysPlayed;

        /// <summary>
        /// Gets the total dewdrops (money) the player has.
        /// </summary>
        public static int TotalMoney => GameScript.dew;

        #endregion

        #region Public Methods - Menu Control

        /// <summary>
        /// Closes all open menus and dialogs.
        /// </summary>
        public static void CloseAllMenus()
        {
            GameScript.menuOpen = false;
            GameScript.talking = false;
            GameScript.interacting = false;
            Plugin.Log.LogDebug("Closed all menus");
        }

        /// <summary>
        /// Pauses the game.
        /// </summary>
        public static void Pause()
        {
            Time.timeScale = 0f;
            Plugin.Log.LogDebug("Game paused");
        }

        /// <summary>
        /// Resumes the game from a paused state.
        /// </summary>
        public static void Resume()
        {
            Time.timeScale = 1f;
            Plugin.Log.LogDebug("Game resumed");
        }

        /// <summary>
        /// Toggles the game's paused state.
        /// </summary>
        public static void TogglePause()
        {
            if (IsPaused)
                Resume();
            else
                Pause();
        }

        #endregion

        #region Public Methods - Debug

        /// <summary>
        /// Dumps the current game state to the log.
        /// Useful for debugging and understanding game state.
        /// </summary>
        public static void DumpGameState()
        {
            Plugin.Log.LogInfo("=== GAME STATE DUMP ===");
            Plugin.Log.LogInfo($"Date: {TimeAPI.GetFormattedDate()}");
            Plugin.Log.LogInfo($"Days Played: {DaysPlayed}");
            Plugin.Log.LogInfo($"Dewdrops: {TotalMoney}");
            Plugin.Log.LogInfo($"Day EXP: {PlayerAPI.DayExperience}/100");
            Plugin.Log.LogInfo($"Position: {PlayerAPI.Position}");
            Plugin.Log.LogInfo($"Facing: {PlayerAPI.FacingDirection}");
            Plugin.Log.LogInfo($"Indoors: {PlayerAPI.IsIndoors}");
            Plugin.Log.LogInfo($"Raining: {TimeAPI.IsRaining}");
            Plugin.Log.LogInfo("--- State Flags ---");
            Plugin.Log.LogInfo($"Menu Open: {IsMenuOpen}");
            Plugin.Log.LogInfo($"Talking: {IsTalking}");
            Plugin.Log.LogInfo($"Interacting: {IsInteracting}");
            Plugin.Log.LogInfo($"Fishing: {IsFishing}");
            Plugin.Log.LogInfo($"Building: {IsBuilding}");
            Plugin.Log.LogInfo("======================");
        }

        /// <summary>
        /// Gets a formatted summary of the current game state.
        /// </summary>
        /// <returns>A multi-line string with game state information.</returns>
        public static string GetGameStateSummary()
        {
            return $"Date: {TimeAPI.GetFormattedDate()}\n" +
                   $"Dewdrops: {TotalMoney}\n" +
                   $"Position: {PlayerAPI.Position}\n" +
                   $"Day EXP: {PlayerAPI.DayExperience}/100";
        }

        /// <summary>
        /// Checks if the player is able to receive input (not in menus, dialogs, etc.).
        /// </summary>
        /// <returns>True if the player can receive input.</returns>
        public static bool CanPlayerReceiveInput()
        {
            return !IsMenuOpen && !IsTalking && !IsInteracting && !IsPaused;
        }

        #endregion

        #region Public Methods - Version Info

        /// <summary>
        /// Gets the OpenWood version string.
        /// </summary>
        public static string OpenWoodVersion => PluginInfo.PLUGIN_VERSION;

        /// <summary>
        /// Gets the OpenWood GUID.
        /// </summary>
        public static string OpenWoodGUID => PluginInfo.PLUGIN_GUID;

        /// <summary>
        /// Logs OpenWood version information.
        /// </summary>
        public static void LogVersionInfo()
        {
            Plugin.Log.LogInfo($"OpenWood v{OpenWoodVersion}");
            Plugin.Log.LogInfo($"GUID: {OpenWoodGUID}");
            Plugin.Log.LogInfo($"Unity Version: {Application.unityVersion}");
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Initializes the Game API. Called automatically by the plugin.
        /// </summary>
        internal static void Initialize()
        {
            Plugin.Log.LogDebug("GameAPI initialized");
        }

        #endregion
    }
}
