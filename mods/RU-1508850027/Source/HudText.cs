using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml;
using HarmonyLib;
using Verse;

namespace Igor.RimHudRussian
{
    public sealed class Bootstrap : Mod
    {
        public Bootstrap(ModContentPack content) : base(content) { HudText.Install(content); }
    }

    public static class HudText
    {
        const string Prefix="chupakobra6_1508850027_";
        static readonly Dictionary<string,string> Credits=new Dictionary<string,string>();
        static readonly Dictionary<string,string> PresetNames=new Dictionary<string,string>();
        static Dictionary<string,string> translated;
        static MethodInfo adjectiveNoun;
        static MethodInfo presetName;
        static MethodInfo corePreset;
        public static bool Russian => LanguageDatabase.activeLanguage?.folderName.StartsWith("Russian",StringComparison.OrdinalIgnoreCase)==true;

        static Dictionary<string,string> Read(string file)
        {
            var xml=new XmlDocument();xml.Load(file);
            return xml.DocumentElement.ChildNodes.OfType<XmlElement>().ToDictionary(x=>x.Name,x=>x.InnerText);
        }
        public static void Install(ModContentPack content)
        {
            var original=Read(Path.Combine(content.RootDir,"Languages/English/Keyed/Additional.xml"));
            translated=Read(Path.Combine(content.RootDir,"Languages/Russian/Keyed/Translations.xml"));
            foreach(var pair in original.Where(p=>p.Key.StartsWith(Prefix+"Credit_",StringComparison.Ordinal)))
                Credits.Add(pair.Value,translated[pair.Key]);
            foreach(var pair in original.Where(p=>p.Key.StartsWith(Prefix+"Preset_",StringComparison.Ordinal)))
                PresetNames.Add(pair.Value,translated[pair.Key]);
            var harmony=new Harmony("igor.ru.1508850027.display");
            // Credits are parsed during RimHUD's startup callback. Register the
            // narrow attribute hooks first, without asking the language loader.
            foreach(string target in new[]{"RimHUD.Engine.Credits+Group:FromXml","RimHUD.Engine.Credits+Entry:FromXml"})
                harmony.Patch(AccessTools.Method(target),transpiler:new HarmonyMethod(typeof(HudText),nameof(CreditAttributes)));
            LongEventHandler.ExecuteWhenFinished(() => {
                adjectiveNoun=AccessTools.Method("RimHUD.Engine.Lang:AdjectiveNoun");
                presetName=AccessTools.Method("RimHUD.Interface.Hud.Layout.LayoutPreset:get_Name");
                corePreset=AccessTools.Method("RimHUD.Interface.Hud.Layout.LayoutPreset:get_IsCore");
                foreach(string target in new[]{"RimHUD.Interface.Hud.Models.Values.RelationKindAndFactionValue:GetValue",
                    "RimHUD.Interface.Hud.Models.Values.GenderRaceAndAgeValue:GetValue"})
                    harmony.Patch(AccessTools.Method(target),transpiler:new HarmonyMethod(typeof(HudText),nameof(JoinDisplayValues)));
                foreach(string target in new[]{"RimHUD.Interface.Hud.Layout.LayoutPreset:get_Label","RimHUD.Interface.Hud.Layout.LayoutPreset:get_FullLabel"})
                    harmony.Patch(AccessTools.Method(target),transpiler:new HarmonyMethod(typeof(HudText),nameof(PresetLabels)));
            });
        }

        public static string Credit(string value) => Russian&&value!=null&&Credits.TryGetValue(value,out var result) ? result : value;
        public static string WithQualifier(string qualifier,string noun)
        {
            if(!Russian)return (string)adjectiveNoun.Invoke(null,new object[]{qualifier,noun});
            if(string.IsNullOrWhiteSpace(qualifier))return noun?.Trim() ?? "";
            if(string.IsNullOrWhiteSpace(noun))return qualifier.Trim();
            return noun.Trim()+" ("+qualifier.Trim()+")";
        }
        public static string PresetName(object preset)
        {
            string name=(string)presetName.Invoke(preset,null);
            return Russian&&(bool)corePreset.Invoke(preset,null)&&PresetNames.TryGetValue(name,out var result) ? result : name;
        }
        static IEnumerable<CodeInstruction> JoinDisplayValues(IEnumerable<CodeInstruction> input)
        {
            int changed=0;
            foreach(var original in input)
            {
                var instruction=new CodeInstruction(original);
                if(instruction.Calls(adjectiveNoun)){instruction.operand=AccessTools.Method(typeof(HudText),nameof(WithQualifier));changed++;}
                yield return instruction;
            }
            if(changed!=1)throw new InvalidOperationException("RimHUD description join changed: "+changed);
        }
        static IEnumerable<CodeInstruction> PresetLabels(IEnumerable<CodeInstruction> input)
        {
            int changed=0;
            foreach(var original in input)
            {
                var instruction=new CodeInstruction(original);
                if(instruction.Calls(presetName)){instruction.opcode=OpCodes.Call;instruction.operand=AccessTools.Method(typeof(HudText),nameof(PresetName));changed++;}
                yield return instruction;
            }
            if(changed!=1)throw new InvalidOperationException("RimHUD preset label changed: "+changed);
        }
        static IEnumerable<CodeInstruction> CreditAttributes(IEnumerable<CodeInstruction> input,MethodBase __originalMethod)
        {
            var source=input.ToList();int changed=0;
            for(int i=0;i<source.Count;i++)
            {
                yield return new CodeInstruction(source[i]);
                var method=source[i].operand as MethodInfo;
                if(i>0&&method?.DeclaringType.FullName=="RimHUD.Extensions.XmlExtensions"&&method.Name=="GetAttribute"
                    &&source[i-1].opcode==OpCodes.Ldstr&&new[]{"Label","Quote"}.Contains((string)source[i-1].operand))
                {
                    yield return new CodeInstruction(OpCodes.Call,AccessTools.Method(typeof(HudText),nameof(Credit)));changed++;
                }
            }
            int expected=__originalMethod.DeclaringType.Name=="Group" ? 1 : 2;
            if(changed!=expected)throw new InvalidOperationException("RimHUD credit attributes changed: "+__originalMethod+" / "+changed);
        }
    }
}
