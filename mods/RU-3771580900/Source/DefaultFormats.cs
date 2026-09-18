using System;
using System.Collections.Generic;
using HarmonyLib;
using Verse;

namespace Igor.RU_3771580900
{
    [StaticConstructorOnStartup]
    public static class DefaultFormats
    {
        public static readonly Dictionary<string,string> Originals = new Dictionary<string,string>();
        private static readonly string[] Fields = {"UpperTextFormat","BottomTextFormat","MN_ReminderTextFormat","MN_OccasionTextFormat"};
        static DefaultFormats() { LongEventHandler.ExecuteWhenFinished(Apply); }
        private static void Apply()
        {
            if(LanguageDatabase.activeLanguage?.folderName.StartsWith("Russian",StringComparison.OrdinalIgnoreCase)!=true)return;
            var defaults=AccessTools.TypeByName("DawnNewDay.DawnDefault");
            foreach(var name in Fields)
            {
                var field=AccessTools.Field(defaults,name);
                Originals.Add(name,(string)field.GetValue(null));
                field.SetValue(null,("chupakobra6_3771580900_"+name).Translate().ToString());
            }
            TranslateSettings(AccessTools.Field(AccessTools.TypeByName("DawnNewDay.DawnMod"),"Settings").GetValue(null));
        }
        public static void TranslateSettings(object settings)
        {
            foreach(var name in Fields)
            {
                object owner=settings;
                string fieldName=name;
                if(name.StartsWith("MN_",StringComparison.Ordinal))
                {
                    owner=AccessTools.Field(settings.GetType(),name.Replace("TextFormat","")).GetValue(settings);
                    fieldName="TextFormat";
                }
                var field=AccessTools.Field(owner.GetType(),fieldName);
                if((string)field.GetValue(owner)==Originals[name])
                    field.SetValue(owner,("chupakobra6_3771580900_"+name).Translate().ToString());
            }
        }
    }
}
