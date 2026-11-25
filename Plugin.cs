using System;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace ScalingRerolls
{
    [BepInPlugin("com.king_tajin.scalingrerolls", "Scaling Rerolls", "0.2.0")]
    [BepInProcess("Slots and Daggers.exe")]
    public class Plugin : BaseUnityPlugin
    {
        internal static ConfigEntry<float> Acceleration;

        private void Awake()
        {
            Acceleration = Config.Bind("Pricing", "Acceleration", 0.5f, new ConfigDescription("How quickly the reroll price climbs. Each reroll in a shop costs the base price plus this value times k x (k + 1), where k is the number of rerolls already. Use 0 to keep the price flat", new AcceptableValueRange<float>(0f, 5f)));

            RerollPricing.Initialize(Logger);

            Harmony harmony = new Harmony("com.king_tajin.scalingrerolls");
            foreach (Type patch in new[] { typeof(ShopEnterPatch), typeof(RerollPatch) })
            {
                try
                {
                    harmony.PatchAll(patch);
                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                }
            }

            Logger.LogInfo("Scaling Rerolls loaded");
        }
    }
}
