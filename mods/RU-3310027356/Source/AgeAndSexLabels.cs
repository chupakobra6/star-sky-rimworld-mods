using System;
using HarmonyLib;
using Verse;

namespace Igor.RU_3310027356
{
    [StaticConstructorOnStartup]
    public static class AgeAndSexLabels
    {
        static AgeAndSexLabels()
        {
            LongEventHandler.ExecuteWhenFinished(()=>
                new Harmony("igor.ru.3310027356.grammar").Patch(
                    AccessTools.Method("ColonyManagerRedux.Managers.AgeAndSexExtensions:GetLabel"),
                    postfix:new HarmonyMethod(typeof(AgeAndSexLabels),nameof(After))));
        }
        private static void After(object __0,bool __1,ref string __result)
        {
            if(!LanguageDatabase.activeLanguage.folderName.StartsWith("Russian",StringComparison.OrdinalIgnoreCase))return;
            int value=Convert.ToInt32(__0);
            if(value<0 || value>3)throw new InvalidOperationException("Colony Manager Redux age/sex enum changed");
            __result=("chupakobra6_3310027356_AgeSex"+(__1?"Plural":"")+value).Translate().ToString();
        }
    }
}
