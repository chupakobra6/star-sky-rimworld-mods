using System;
using HarmonyLib;
using Verse;

namespace Igor.RU_1635901197
{
    [StaticConstructorOnStartup]
    public static class EditorLabels
    {
        static EditorLabels()
        {
            var harmony=new Harmony("igor.ru.1635901197.editorlabels");
            var callbacks=AccessTools.TypeByName("FacialAnimation.NL_SelectPartWindow+<>c");
            foreach(string name in new[]{
                "<DrawAnimationPawnParamVanilla>b__42_1","<DrawAnimationPawnParamVanilla>b__42_3",
                "<DrawAnimationPawnParamVanilla>b__42_5","<DrawAnimationPawnParamFacial>b__43_0",
                "<DrawAnimationPawnParamFacial>b__43_2","<DrawAnimationPawnParamFacial>b__43_4",
                "<DrawAnimationPawnParamFacial>b__43_6","<DrawAnimationPawnParamFacial>b__43_8",
                "<DrawAnimationPawnParamFacial>b__43_10"})
                harmony.Patch(AccessTools.DeclaredMethod(callbacks,name),
                    postfix:new HarmonyMethod(typeof(EditorLabels),nameof(DefLabel)));
            harmony.Patch(AccessTools.DeclaredMethod(callbacks,"<DrawAnimationPlayer>b__38_1"),
                postfix:new HarmonyMethod(typeof(EditorLabels),nameof(JobLabel)));
        }

        static bool Russian => LanguageDatabase.activeLanguage?.folderName.StartsWith("Russian",StringComparison.OrdinalIgnoreCase)==true;

        // Only the selector's display callback changes. Selection and serialized
        // references continue to receive the original Def object or job identifier.
        static void DefLabel(Def __0,ref string __result)
        {
            if(!Russian||__0==null)return;
            string key="chupakobra6_1635901197_Def_"+__0.defName;
            if(key.CanTranslate())__result=key.Translate().Resolve();
            else if(!string.IsNullOrEmpty(__0.label))__result=__0.LabelCap.ToString();
        }

        static void JobLabel(string __0,ref string __result)
        {
            if(!Russian)return;
            string key="chupakobra6_1635901197_Job_"+__0;
            if(key.CanTranslate())__result=key.Translate().Resolve();
        }
    }
}
