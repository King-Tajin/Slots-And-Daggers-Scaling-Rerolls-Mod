using System;
using System.Reflection;
using BepInEx.Logging;
using UnityEngine;

namespace ScalingRerolls
{
    internal static class RerollPricing
    {
        private const int MaxCost = 200;
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        private static readonly FieldInfo RerollsLeftField = typeof(UpgradeShopManager).GetField("rerollsLeft", Flags);
        private static readonly FieldInfo OptionRerollField = typeof(UpgradeShopManager).GetField("optionReroll", Flags);
        private static readonly FieldInfo SpriteRerollField = typeof(UpgradeShopManager).GetField("spriteReroll", Flags);
        private static readonly PropertyInfo IsStartShopProperty = typeof(UpgradeShopManager).GetProperty("IsStartShop", Flags);

        private static ManualLogSource log;
        private static int rerollsUsed;

        internal static void Initialize(ManualLogSource logger)
        {
            log = logger;

            if (RerollsLeftField == null || OptionRerollField == null || SpriteRerollField == null || IsStartShopProperty == null)
            {
                log.LogError("Could not find the upgrade shop fields. The game may have been updated, so rerolls will behave as in the base game.");
            }
        }

        internal static void ResetShop(UpgradeShopManager manager)
        {
            try
            {
                rerollsUsed = 0;

                if (RerollsLeftField != null)
                {
                    RerollsLeftField.SetValue(manager, int.MaxValue);
                }
            }
            catch (Exception e)
            {
                log.LogError(e);
            }
        }

        internal static void AdvanceCost(UpgradeShopManager manager)
        {
            try
            {
                if (OptionRerollField == null || SpriteRerollField == null || IsStartShopProperty == null)
                {
                    return;
                }

                if ((bool)IsStartShopProperty.GetValue(manager, null))
                {
                    return;
                }

                UpgradeShopOption option = OptionRerollField.GetValue(manager) as UpgradeShopOption;
                if (option == null)
                {
                    return;
                }

                rerollsUsed++;
                option.SetupIconForReroll(CostFor(rerollsUsed), SpriteRerollField.GetValue(manager) as Sprite);
            }
            catch (Exception e)
            {
                log.LogError(e);
            }
        }

        private static int CostFor(int rerollsAlreadyUsed)
        {
            int baseCost = Math.Max(1, Main.BalancingConfig.RerollShopCost);
            double cost = baseCost + Math.Ceiling(Plugin.Acceleration.Value * rerollsAlreadyUsed * (rerollsAlreadyUsed + 1) - 0.0001);
            return (int)Math.Min(cost, MaxCost);
        }
    }
}
