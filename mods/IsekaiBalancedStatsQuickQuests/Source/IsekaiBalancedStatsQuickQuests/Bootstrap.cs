using System;
using HarmonyLib;
using Verse;

namespace IsekaiBalancedStatsQuickQuests
{
    [StaticConstructorOnStartup]
    public static class Bootstrap
    {
        private const string HarmonyId = "chupakobra6.isekai.balancedstatsquickquests";

        static Bootstrap()
        {
            try
            {
                new Harmony(HarmonyId).PatchAll();
                Log.Message("[ISEKAI Balanced Stats] Colonist and pet allocation patches are active.");
            }
            catch (Exception exception)
            {
                Log.Error($"[ISEKAI Balanced Stats] Could not install patches: {exception}");
            }
        }
    }
}
