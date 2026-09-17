using System;
using System.Collections.Generic;
using HarmonyLib;
using Verse;

namespace Igor.RU_3033901359
{
    public sealed class CommandLabels : Mod
    {
        private static readonly Dictionary<string,string> Labels=new Dictionary<string,string>{
            {"AddStack","Добавить стопку"},
            {"EditGraphics","Редактировать графику"},
            {"UpdateGraphics","Обновить графику"}
        };
        public CommandLabels(ModContentPack content) : base(content)
        {
            new Harmony("igor.ru.3033901359.commands").Patch(
                AccessTools.Constructor(AccessTools.TypeByName("AdaptiveStorage.GodModeGizmos"),
                    new[]{AccessTools.TypeByName("AdaptiveStorage.ThingClass")}),
                postfix:new HarmonyMethod(typeof(CommandLabels),nameof(Translate)));
        }
        private static void Translate(object __instance)
        {
            if(LanguageDatabase.activeLanguage?.folderName.StartsWith("Russian",StringComparison.OrdinalIgnoreCase)!=true)return;
            foreach(var pair in Labels)
                ((Command)AccessTools.Field(__instance.GetType(),pair.Key).GetValue(__instance)).defaultLabel=pair.Value;
        }
    }
}
