using System;
using HarmonyLib;
using Verse;

namespace Igor.RU_2975771801
{
    [StaticConstructorOnStartup]
    public static class BodyPartLabels
    {
        static BodyPartLabels() { LongEventHandler.ExecuteWhenFinished(Apply); }
        private static void Apply()
        {
            new Harmony("igor.ru.2975771801.bodylabels").Patch(
                AccessTools.Method("VREAndroids.HealthCardUtility_GenerateSurgeryOption_Patch:AndroidPartLabel"),
                prefix:new HarmonyMethod(typeof(BodyPartLabels),nameof(Label)));
        }
        private static bool Label(BodyPartRecord __0, HediffDef __1, ref string __result)
        {
            if(LanguageDatabase.activeLanguage?.folderName.StartsWith("Russian",StringComparison.OrdinalIgnoreCase)!=true)
                return true;
            // A custom name can have a different case, gender, side or anatomy.
            // Keep it intact; native English word replacement can leave {0} in UI.
            __result=__0.customLabel.NullOrEmpty() ? __1.label : __0.customLabel+" ("+__1.label+")";
            return false;
        }
    }
}
