using HarmonyLib;

namespace ScalingRerolls
{
    [HarmonyPatch(typeof(UpgradeShopManager), "HandleUpgradeShopEnter")]
    internal static class ShopEnterPatch
    {
        private static void Postfix(UpgradeShopManager __instance)
        {
            RerollPricing.ResetShop(__instance);
        }
    }

    [HarmonyPatch(typeof(UpgradeShopManager), nameof(UpgradeShopManager.Reroll))]
    internal static class RerollPatch
    {
        private static void Prefix(UpgradeShopManager __instance)
        {
            RerollPricing.AdvanceCost(__instance);
        }
    }
}
