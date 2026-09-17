using System;
using HarmonyLib;
using Verse;

namespace IsekaiQuickQuests
{
    [StaticConstructorOnStartup]
    public static class Bootstrap
    {
        private const string HarmonyId = "starsky.rimworld.isekaiquickquests";

        static Bootstrap()
        {
            try
            {
                new Harmony(HarmonyId).PatchAll();
                Log.Message("[ISEKAI Quick Quests] Middle-click quest shortcut is active.");
            }
            catch (Exception exception)
            {
                Log.Error($"[ISEKAI Quick Quests] Could not install patches: {exception}");
            }
        }
    }
}
