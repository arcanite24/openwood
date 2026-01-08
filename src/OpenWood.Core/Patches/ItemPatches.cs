using HarmonyLib;
using OpenWood.Core.Events;

namespace OpenWood.Core.Patches
{
    /// <summary>
    /// Harmony patches for item-related events.
    /// </summary>
    public static class ItemPatches
    {
        /// <summary>
        /// Patch for when items are added to inventory.
        /// </summary>
        [HarmonyPatch(typeof(GameScript), "AddItem", typeof(int), typeof(int))]
        public static class AddItem_Patch
        {
            static void Postfix(int id, int q)
            {
                Plugin.Log.LogDebug($"Item added: ID={id}, Quantity={q}");
                GameEvents.TriggerItemPickup(id.ToString(), q);
            }
        }

        /// <summary>
        /// Patch for when dewdrops (currency) are added.
        /// </summary>
        [HarmonyPatch(typeof(GameScript), "AddDewdrops")]
        public static class AddDewdrops_Patch
        {
            static void Postfix(int a)
            {
                Plugin.Log.LogDebug($"Dewdrops added: {a}");
                GameEvents.TriggerPlayerMoneyChange(GameScript.dew);
            }
        }
    }

    /// <summary>
    /// Patches for crafting system.
    /// </summary>
    public static class CraftingPatches
    {
        /// <summary>
        /// Patch for when a recipe is cooked/crafted.
        /// </summary>
        [HarmonyPatch(typeof(GameScript), "InstantlyCookRecipe")]
        public static class CookRecipe_Patch
        {
            static void Postfix(int a)
            {
                Plugin.Log.LogDebug($"Recipe crafted: {a}");
                GameEvents.TriggerItemCraft(a.ToString(), 1);
            }
        }
    }
}
