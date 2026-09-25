using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Xml;
using HarmonyLib;
using Verse;

namespace Igor.CharacterEditorRussian
{
    public sealed class Bootstrap : Mod
    {
        public Bootstrap(ModContentPack content) : base(content) { EditorText.Install(content); }
    }

    public static class EditorText
    {
        const string Prefix = "chupakobra6_1874644848_";
        static Dictionary<string,string> english;
        static Dictionary<string,string> russian;
        static readonly Dictionary<string,string[]> Names = new Dictionary<string,string[]>();
        static readonly Dictionary<FieldInfo,string> Fields = new Dictionary<FieldInfo,string>();
        static Func<GeneDef,bool> isBodySizeGene;
        public static int AppliedFields { get; private set; }
        public static int TranslatedNames { get; private set; }
        public static bool Russian => LanguageDatabase.activeLanguage?.folderName.StartsWith("Russian", StringComparison.OrdinalIgnoreCase) == true;

        static Dictionary<string,string> Read(string path)
        {
            var xml = new XmlDocument(); xml.Load(path);
            return xml.DocumentElement.ChildNodes.OfType<XmlElement>().ToDictionary(e=>e.Name,e=>e.InnerText);
        }

        public static void Install(ModContentPack content)
        {
            english=Read(Path.Combine(content.RootDir,"Languages/English/Keyed/Additional.xml"));
            russian=Read(Path.Combine(content.RootDir,"Languages/Russian/Keyed/Translations.xml"));
            var label=AccessTools.TypeByName("CharacterEditor.Label");
            var source=PatchProcessor.GetOriginalInstructions(AccessTools.Method(label,"LangEN")).ToList();
            var assignments=new Dictionary<string,string>();
            for(int i=1;i<source.Count;i++)
            {
                var field=source[i].operand as FieldInfo;
                if(source[i].opcode==OpCodes.Stsfld && field?.DeclaringType==label && field.FieldType==typeof(string))
                {
                    if(source[i-1].opcode!=OpCodes.Ldstr)throw new InvalidOperationException("Character Editor label assignment changed: "+field.Name);
                    assignments.Add(field.Name,(string)source[i-1].operand);
                }
            }
            var expected=english.Where(p=>p.Key.StartsWith(Prefix+"Label_",StringComparison.Ordinal)).ToArray();
            if(assignments.Count!=534 || expected.Length!=534)throw new InvalidOperationException("Character Editor label count changed");
            foreach(var pair in expected)
            {
                string field=pair.Key.Substring((Prefix+"Label_").Length);
                if(!assignments.TryGetValue(field,out var text)||text!=pair.Value)
                    throw new InvalidOperationException("Unreviewed Character Editor source label: "+field);
                Fields.Add(AccessTools.Field(label,field),pair.Key);
                if(!russian.ContainsKey(pair.Key))throw new InvalidOperationException("Missing Russian Character Editor label: "+field);
            }
            foreach(var pair in english.Where(p=>p.Key.StartsWith(Prefix+"Name_",StringComparison.Ordinal)))
            {
                var en=ParseName(pair.Value); var ru=ParseName(russian[pair.Key]);
                string key=string.Join(",",en);
                if(Names.TryGetValue(key,out var previous) && !previous.SequenceEqual(ru))
                    throw new InvalidOperationException("Conflicting Character Editor name translation: "+key);
                Names[key]=ru;
            }
            var harmony=new Harmony("igor.ru.1874644848.editor");
            var bodySizePredicate=AccessTools.Method("CharacterEditor.GeneTool+<>c:<get_ListBodySizeGenes>b__184_0");
            var bodySizeCheck=AccessTools.Method("CharacterEditor.GeneTool:IsBodySizeGene");
            if(bodySizePredicate==null || bodySizeCheck==null)
                throw new InvalidOperationException("Character Editor body-size gene selector changed");
            isBodySizeGene=(Func<GeneDef,bool>)Delegate.CreateDelegate(typeof(Func<GeneDef,bool>),bodySizeCheck);
            harmony.Patch(bodySizePredicate,prefix:new HarmonyMethod(typeof(EditorText),nameof(BodySizeGene)));
            harmony.Patch(AccessTools.Method(label,"LangRU"),postfix:new HarmonyMethod(typeof(EditorText),nameof(ApplyFields)));
            harmony.Patch(AccessTools.Method(label,"AddNamesFromPath"),transpiler:new HarmonyMethod(typeof(EditorText),nameof(TranslateNameConstruction)));
            harmony.Patch(AccessTools.Method("CharacterEditor.HeadTool:GetHeadName"),postfix:new HarmonyMethod(typeof(EditorText),nameof(HeadName)));
            harmony.Patch(AccessTools.Method("CharacterEditor.ThingTool:CreateBuilding"),prefix:new HarmonyMethod(typeof(EditorText),nameof(BuildingLabel)));
            harmony.Patch(AccessTools.Method("CharacterEditor.DialogObjects:DrawLowerButtons"),transpiler:new HarmonyMethod(typeof(EditorText),nameof(RotationCall)));
            LongEventHandler.ExecuteWhenFinished(ApplyFields);
        }

        internal static string Value(string key) => russian[Prefix+key];

        // Russian GeneDef labels do not contain the English word "bodysize"
        // used by Character Editor's original filter. Keep the translated
        // gene list working even when the separate fixes mod is not installed.
        public static bool BodySizeGene(GeneDef __0, ref bool __result)
        {
            if(!Russian)return true;
            __result=isBodySizeGene(__0);
            return false;
        }

        static string[] ParseName(string raw)
        {
            string[] fields=raw.Split(',');
            if(fields.Length!=3)throw new InvalidOperationException("Malformed reviewed Character Editor name: "+raw);
            if(fields[1].Length==0)fields[1]=fields[0];
            return fields;
        }

        public static void ApplyFields()
        {
            if(!Russian)return;
            foreach(var pair in Fields)pair.Key.SetValue(null,russian[pair.Value]);
            AppliedFields=Fields.Count;
        }

        public static NameTriple MakeName(string first,string nick,string last)
        {
            if(Russian && Names.TryGetValue(first+","+nick+","+last,out var translated))
            {
                TranslatedNames++;
                return new NameTriple(translated[0],translated[1],translated[2]);
            }
            return new NameTriple(first,nick,last);
        }

        public static IEnumerable<CodeInstruction> TranslateNameConstruction(IEnumerable<CodeInstruction> input)
        {
            var constructor=AccessTools.Constructor(typeof(NameTriple),new[]{typeof(string),typeof(string),typeof(string)});
            var decode=AccessTools.Method("CharacterEditor.Extension:AsStringUNICODE");
            int changed=0,repaired=0;
            foreach(var original in input)
            {
                var instruction=new CodeInstruction(original);
                if(instruction.opcode==OpCodes.Newobj && Equals(instruction.operand,constructor))
                {
                    instruction.opcode=OpCodes.Call;
                    instruction.operand=AccessTools.Method(typeof(EditorText),nameof(MakeName));
                    changed++;
                }
                yield return instruction;
                if(instruction.Calls(decode))
                {
                    yield return new CodeInstruction(OpCodes.Call,AccessTools.Method(typeof(EditorText),nameof(RepairNameData)));
                    repaired++;
                }
            }
            if(changed!=1)throw new InvalidOperationException("Character Editor name construction changed: "+changed);
            if(repaired!=1)throw new InvalidOperationException("Character Editor name decoder changed: "+repaired);
        }

        public static string RepairNameData(string text)
        {
            return Regex.Replace(text.TrimStart('\uFEFF'),@"(?m)^Is,,Retarded(?=\r?\nKai,,Henriksen;)","Is,,Retarded;");
        }

        public static void HeadName(ref string __result)
        {
            if(!Russian||string.IsNullOrEmpty(__result))return;
            __result=Regex.Replace(__result,@"\b(Average|Narrow|Wide|Pointy|Normal)\b",m=>russian[Prefix+"Head"+m.Value]);
        }

        public static void BuildingLabel(string __0,ref string __1)
        {
            if(Russian && (__0=="Zombrella" || __0=="Zombgrella")) __1=russian[Prefix+__0];
        }

        public static string RotationLabel(Rot4 rotation)
        {
            string direction=rotation==Rot4.South?"South":rotation==Rot4.East?"East":rotation==Rot4.West?"West":"North";
            return Russian?russian[Prefix+direction]:"_"+direction.ToLowerInvariant();
        }

        public static IEnumerable<CodeInstruction> RotationCall(IEnumerable<CodeInstruction> input)
        {
            var target=AccessTools.Method("CharacterEditor.TextureTool:Rot4ToString"); int changed=0;
            foreach(var original in input)
            {
                var instruction=new CodeInstruction(original);
                if(instruction.Calls(target)) { instruction.operand=AccessTools.Method(typeof(EditorText),nameof(RotationLabel)); changed++; }
                yield return instruction;
            }
            if(changed!=1)throw new InvalidOperationException("Character Editor rotation display changed: "+changed);
        }
    }
}
