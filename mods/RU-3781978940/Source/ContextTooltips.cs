using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Igor.RU_3781978940
{
    [StaticConstructorOnStartup]
    internal static class ContextTooltips
    {
        static ContextTooltips()
        {
            LongEventHandler.ExecuteWhenFinished(()=>
            {
                var harmony=new Harmony("igor.ru.3781978940.grammar");
                harmony.Patch(AccessTools.Method("BioTabEnhanced.EnhancedCharacterCard:GetFavoriteColorTooltip"),
                    postfix:new HarmonyMethod(typeof(ContextTooltips),nameof(Favorite)));
                harmony.Patch(AccessTools.Method("BioTabEnhanced.EnhancedCharacterCard:GetUnwaveringlyLoyalTooltip"),
                    postfix:new HarmonyMethod(typeof(ContextTooltips),nameof(Loyal)));
            });
        }
        private static bool Russian => LanguageDatabase.activeLanguage?.folderName
            .StartsWith("Russian",StringComparison.OrdinalIgnoreCase)==true;
        private static void Favorite(Pawn __0,ref string __result)
        {
            if(!Russian)return;
            __result="chupakobra6_3781978940_Favorite".Translate(__0.Name?.ToStringShort ?? __0.LabelShortCap);
            if(ModsConfig.IdeologyActive)__result+="\n\n"+"chupakobra6_3781978940_FavoriteIdeology".Translate();
        }
        private static void Loyal(Pawn __0,ref string __result)
        {
            if(Russian)__result="chupakobra6_3781978940_Loyal".Translate(__0.Name?.ToStringShort ?? __0.LabelShortCap);
        }
    }
}
