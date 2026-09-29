using System;
using HarmonyLib;
using Verse;

namespace Igor.RU1541721856
{
    [StaticConstructorOnStartup]
    internal static class LivesLabel
    {
        static LivesLabel()
        {
            new Harmony("igor.ru.1541721856.lives").Patch(
                AccessTools.Method("AlphaBehavioursAndEvents.HediffComp_Resurrect:get_CompLabelInBracketsExtra"),
                postfix:new HarmonyMethod(typeof(LivesLabel),nameof(Postfix)));
        }
        private static void Postfix(int ___resurrectionsLeft,ref string __result)
        {
            if(LanguageDatabase.activeLanguage?.folderName.StartsWith("Russian",StringComparison.OrdinalIgnoreCase)==true)
                __result="Жизней: "+___resurrectionsLeft;
        }
    }
}
