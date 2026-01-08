using HarmonyLib;
using OpenWood.Core.Events;
using UnityEngine;

namespace OpenWood.Core.Patches
{
    /// <summary>
    /// Harmony patches for game lifecycle events.
    /// These patches hook into Littlewood's GameScript to trigger OpenWood events.
    /// </summary>
    public static class GamePatches
    {
        /// <summary>
        /// Patch for when the game starts/loads.
        /// </summary>
        [HarmonyPatch(typeof(GameScript), "BeginGame")]
        public static class BeginGame_Patch
        {
            static void Postfix()
            {
                Plugin.Log.LogDebug("Game started - triggering OnGameStart");
                GameEvents.TriggerGameStart();
            }
        }

        /// <summary>
        /// Patch for when the player goes to sleep (end of day).
        /// </summary>
        [HarmonyPatch(typeof(GameScript), "GoToSleep", new System.Type[0])]
        public static class GoToSleep_Patch
        {
            static void Prefix()
            {
                Plugin.Log.LogDebug($"Day {GameScript.day} ending");
                GameEvents.TriggerDayEnd(GameScript.day);
            }
        }

        /// <summary>
        /// Patch for when a new day begins after sleeping.
        /// </summary>
        [HarmonyPatch(typeof(GameScript), "ContinueSleep")]
        public static class ContinueSleep_Patch
        {
            static void Postfix()
            {
                Plugin.Log.LogDebug($"Day {GameScript.day} started");
                GameEvents.TriggerDayStart(GameScript.day);
            }
        }

        /// <summary>
        /// Patch for fade out transitions.
        /// </summary>
        [HarmonyPatch(typeof(GameScript), "FadeOut")]
        public static class FadeOut_Patch
        {
            static void Postfix()
            {
                Plugin.Log.LogDebug("Fade out triggered");
            }
        }
    }

    /// <summary>
    /// Patches for the save/load system.
    /// </summary>
    public static class SavePatches
    {
        /// <summary>
        /// Patch for when the game is saved.
        /// </summary>
        [HarmonyPatch(typeof(SaveData), "Save")]
        public static class Save_Patch
        {
            static void Postfix()
            {
                Plugin.Log.LogDebug("Game saved");
                GameEvents.TriggerGameSave();
            }
        }

        /// <summary>
        /// Patch for when the game is loaded.
        /// </summary>
        [HarmonyPatch(typeof(SaveData), "Load")]
        public static class Load_Patch
        {
            static void Postfix()
            {
                Plugin.Log.LogDebug("Game loaded");
                GameEvents.TriggerGameLoad();
            }
        }
    }
}
