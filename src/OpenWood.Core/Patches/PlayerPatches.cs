using HarmonyLib;
using OpenWood.Core.Events;
using UnityEngine;

namespace OpenWood.Core.Patches
{
    /// <summary>
    /// Harmony patches for player-related events.
    /// </summary>
    public static class PlayerPatches
    {
        private static Vector2 _lastPosition;
        private static bool _lastSprintState;

        /// <summary>
        /// Patch for player movement updates.
        /// </summary>
        [HarmonyPatch(typeof(PlayerController), "Update")]
        public static class PlayerUpdate_Patch
        {
            static void Postfix(PlayerController __instance)
            {
                // Track position changes
                Vector2 currentPos = PlayerController.pos;
                if (currentPos != _lastPosition)
                {
                    _lastPosition = currentPos;
                    GameEvents.TriggerPlayerMove(currentPos.x, currentPos.y);
                }

                // Track sprint state changes
                bool currentSprint = PlayerController.isSprinting;
                if (currentSprint != _lastSprintState)
                {
                    _lastSprintState = currentSprint;
                    Plugin.Log.LogDebug($"Player sprinting: {currentSprint}");
                }
            }
        }
    }

    /// <summary>
    /// Patches for NPC interactions.
    /// </summary>
    public static class NPCPatches
    {
        /// <summary>
        /// Patch for when player starts talking to a specific NPC.
        /// </summary>
        [HarmonyPatch(typeof(GameScript), "TalkToSpecificTownsfolk")]
        public static class TalkToTownsfolk_Patch
        {
            static void Postfix(string s)
            {
                Plugin.Log.LogDebug($"Talking to townsfolk: {s}");
                GameEvents.TriggerNPCDialogue(s, "");
            }
        }
    }
}
