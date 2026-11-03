using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace Igor.RU_3771580900
{
    [StaticConstructorOnStartup]
    public static class DefaultFormats
    {
        public static readonly Dictionary<string,string> Originals = new Dictionary<string,string>();
        private static readonly string[] Fields = {"UpperTextFormat","BottomTextFormat","MN_ReminderTextFormat","MN_OccasionTextFormat"};
        private static bool Russian => LanguageDatabase.activeLanguage?.folderName.StartsWith("Russian",StringComparison.OrdinalIgnoreCase)==true;
        static DefaultFormats() { LongEventHandler.ExecuteWhenFinished(Apply); }
        private static void Apply()
        {
            if(!Russian)return;
            var defaults=AccessTools.TypeByName("DawnNewDay.DawnDefault");
            var harmony=new Harmony("igor.ru.3771580900.defaults");
            foreach(var name in Fields)
            {
                var field=AccessTools.Field(defaults,name);
                var getter=field==null?AccessTools.PropertyGetter(defaults,name):null;
                Originals.Add(name,(string)(field!=null?field.GetValue(null):getter.Invoke(null,null)));
                if(field!=null)
                    field.SetValue(null,("chupakobra6_3771580900_"+name).Translate().ToString());
                else
                    harmony.Patch(getter,postfix:new HarmonyMethod(typeof(DefaultFormats),nameof(TranslateDefault)));
            }
            TranslateSettings(AccessTools.Field(AccessTools.TypeByName("DawnNewDay.DawnMod"),"Settings").GetValue(null));
        }
        private static void TranslateDefault(MethodBase __originalMethod,ref string __result)
        {
            string name=__originalMethod.Name.Substring(4);
            if(Russian && __result==Originals[name])
                __result=("chupakobra6_3771580900_"+name).Translate().ToString();
        }
        public static void TranslateSettings(object settings)
        {
            if(!Russian)return;
            var widgets=(IEnumerable)AccessTools.Field(settings.GetType(),"OverlayWidgets").GetValue(settings);
            foreach(var widget in widgets)
            {
                if(widget==null || widget.GetType().FullName!="DawnNewDay.DawnText")continue;
                var field=AccessTools.Field(widget.GetType(),"Format");
                foreach(var name in Fields)
                    if((string)field.GetValue(widget)==Originals[name])
                        field.SetValue(widget,("chupakobra6_3771580900_"+name).Translate().ToString());
            }
        }
    }
}
