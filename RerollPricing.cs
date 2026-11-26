using System;
using System.IO;
using System.Reflection;
using BepInEx.Logging;
using UnityEngine;

namespace ScalingRerolls
{
    internal static class RerollPricing
    {
        private const int MaxCost = 200;
        private const int MaxStoredRerolls = 10000;
        private const string StateFileName = "saveFile0.scalingrerolls";
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        private static readonly FieldInfo RerollsLeftField = typeof(UpgradeShopManager).GetField("rerollsLeft", Flags);
        private static readonly FieldInfo OptionRerollField = typeof(UpgradeShopManager).GetField("optionReroll", Flags);
        private static readonly FieldInfo SpriteRerollField = typeof(UpgradeShopManager).GetField("spriteReroll", Flags);
        private static readonly FieldInfo LoadedSessionField = typeof(UpgradeShopManager).GetField("wasPopulatedFromLoadedSession", Flags);
        private static readonly PropertyInfo IsStartShopProperty = typeof(UpgradeShopManager).GetProperty("IsStartShop", Flags);

        private static ManualLogSource log;
        private static int rerollsUsed;
        private static int pendingRestore;

        internal static void Initialize(ManualLogSource logger)
        {
            log = logger;

            if (RerollsLeftField == null || OptionRerollField == null || SpriteRerollField == null || IsStartShopProperty == null)
            {
                log.LogError("Could not find the upgrade shop fields. The game may have been updated, so rerolls will behave as in the base game.");
            }

            if (LoadedSessionField == null)
            {
                log.LogWarning("Could not find the loaded session flag. The reroll price may reset when a saved shop is loaded.");
            }
        }

        internal static bool IsLoadedRepeat(UpgradeShopManager manager)
        {
            try
            {
                return LoadedSessionField != null && (bool)LoadedSessionField.GetValue(manager);
            }
            catch (Exception e)
            {
                log.LogError(e);
                return false;
            }
        }

        internal static void PrepareRestore(GameSession session)
        {
            try
            {
                pendingRestore = session != null && session.wasUpgradeShop == 1 ? ReadState() : 0;
            }
            catch (Exception e)
            {
                pendingRestore = 0;
                log.LogError(e);
            }
        }

        internal static void ResetShop(UpgradeShopManager manager)
        {
            try
            {
                rerollsUsed = pendingRestore;
                pendingRestore = 0;

                if (RerollsLeftField != null)
                {
                    RerollsLeftField.SetValue(manager, int.MaxValue);
                }

                if (rerollsUsed > 0 && CanPrice() && !IsStartShop(manager))
                {
                    ApplyCost(manager);
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
                if (!CanPrice() || IsStartShop(manager))
                {
                    return;
                }

                if (GetRerollOption(manager) == null)
                {
                    return;
                }

                rerollsUsed++;
                ApplyCost(manager);
            }
            catch (Exception e)
            {
                log.LogError(e);
            }
        }

        internal static void SaveState(GameSession session)
        {
            try
            {
                int value = session != null && session.wasUpgradeShop == 1 ? rerollsUsed : 0;
                string path = StatePath();
                string directory = Path.GetDirectoryName(path);

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(path, value.ToString());
            }
            catch (Exception e)
            {
                log.LogError(e);
            }
        }

        private static int ReadState()
        {
            string path = StatePath();

            if (!File.Exists(path))
            {
                return 0;
            }

            int value;
            if (!int.TryParse(File.ReadAllText(path).Trim(), out value))
            {
                return 0;
            }

            return Math.Max(0, Math.Min(value, MaxStoredRerolls));
        }

        private static string StatePath()
        {
            return Path.Combine(Application.persistentDataPath, "Save", StateFileName);
        }

        private static bool CanPrice()
        {
            return OptionRerollField != null && SpriteRerollField != null && IsStartShopProperty != null;
        }

        private static bool IsStartShop(UpgradeShopManager manager)
        {
            return (bool)IsStartShopProperty.GetValue(manager, null);
        }

        private static UpgradeShopOption GetRerollOption(UpgradeShopManager manager)
        {
            return OptionRerollField.GetValue(manager) as UpgradeShopOption;
        }

        private static void ApplyCost(UpgradeShopManager manager)
        {
            UpgradeShopOption option = GetRerollOption(manager);
            if (option == null)
            {
                return;
            }

            option.SetupIconForReroll(CostFor(rerollsUsed), SpriteRerollField.GetValue(manager) as Sprite);
        }

        private static int CostFor(int rerollsAlreadyUsed)
        {
            int baseCost = Main.BalancingConfig.RerollShopCost;
            double cost = baseCost + Math.Ceiling(Plugin.Acceleration.Value * rerollsAlreadyUsed * (rerollsAlreadyUsed + 1) - 0.0001);
            return (int)Math.Min(cost, Math.Max(MaxCost, baseCost));
        }
    }
}
