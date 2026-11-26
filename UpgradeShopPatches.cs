using HarmonyLib;

namespace ScalingRerolls
{
    [HarmonyPatch(typeof(UpgradeShopManager), "HandleUpgradeShopEnter")]
    internal static class ShopEnterPatch
    {
        private static void Prefix(UpgradeShopManager __instance, out bool __state)
        {
            __state = RerollPricing.IsLoadedRepeat(__instance);
        }

        private static void Postfix(UpgradeShopManager __instance, bool __state)
        {
            if (!__state)
            {
                RerollPricing.ResetShop(__instance);
            }
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

    [HarmonyPatch(typeof(UpgradeShopManager), nameof(UpgradeShopManager.ApplyLoadedGameSession))]
    internal static class LoadSessionPatch
    {
        private static void Prefix(GameSession loadedGameSession)
        {
            RerollPricing.PrepareRestore(loadedGameSession);
        }
    }

    [HarmonyPatch(typeof(GameSession), nameof(GameSession.SaveGameSession))]
    internal static class SaveSessionPatch
    {
        private static void Postfix(GameSession __instance)
        {
            RerollPricing.SaveState(__instance);
        }
    }
}
