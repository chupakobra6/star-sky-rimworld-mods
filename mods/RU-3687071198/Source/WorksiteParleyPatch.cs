using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;
namespace Igor.WorksitesRussian
{
    public static class WorksiteParleyPatch
    {
        private static readonly MethodInfo ArrayConcat=typeof(string).GetMethod("Concat",new[]{typeof(string[])});
        private static readonly MethodInfo FourConcat=typeof(string).GetMethod("Concat",new[]{typeof(string),typeof(string),typeof(string),typeof(string)});
        private static readonly MethodInfo[] Formats=Enumerable.Range(1,3).Select(n=>typeof(string).GetMethod("Format",new[]{typeof(string)}.Concat(Enumerable.Repeat(typeof(object),n)).ToArray())).ToArray();
        private static bool Russian=>LanguageDatabase.activeLanguage?.folderName.StartsWith("Russian",StringComparison.OrdinalIgnoreCase)==true;
        public static void Install(Harmony harmony)
        {
            Patch(harmony,"MiningOutpost.Parley.Dialog_Parley:CacheHeaderInfo",nameof(RewriteGreeting),nameof(HeaderLabels));
            Patch(harmony,"MiningOutpost.Parley.Dialog_Parley:DrawSpecificsPanel",nameof(RewriteRepeat));
            Patch(harmony,"MiningOutpost.Parley.ParleySuccessCalculator:CalculateWithSpecifics",nameof(RewriteFactors));
            Patch(harmony,"MiningOutpost.Parley.ParleySuccessCalculator:GetSkillRoleName",null,nameof(SkillLabel));
        }
        private static void Patch(Harmony h,string name,string transpiler,string postfix=null)
        {
            var m=AccessTools.Method(name);if(m==null)throw new MissingMethodException(name);
            h.Patch(m,postfix:postfix==null?null:new HarmonyMethod(typeof(WorksiteParleyPatch),postfix),transpiler:transpiler==null?null:new HarmonyMethod(typeof(WorksiteParleyPatch),transpiler));
        }
        public static string GreetingArray(string[] p)=>Russian?WorksiteParleyText.ArrayGreeting(p):string.Concat(p);
        public static string GreetingFour(string a,string b,string c,string d)=>Russian?WorksiteParleyText.FourGreeting(a,b,c,d):string.Concat(a,b,c,d);
        public static string Repeat(string a,string b,string c,string d)=>Russian?WorksiteParleyText.RepeatGoodWord(a,b,c,d):string.Concat(a,b,c,d);
        public static string One(string f,object a)=>Russian?WorksiteParleyText.One(f,a):string.Format(f,a);
        public static string Two(string f,object a,object b)=>Russian?WorksiteParleyText.Two(f,a,b):string.Format(f,a,b);
        public static string Three(string f,object a,object b,object c)=>Russian?WorksiteParleyText.Three(f,a,b,c):string.Format(f,a,b,c);
        public static void SkillLabel(SkillDef __0,ref string __result)
        {if(Russian)__result=__0?.LabelCap.Resolve()??"Навык";}
        public static string AlignmentLabel(string source)
        {
            switch(source)
            {
                case "Lawful":return "Законопослушность";
                case "Good":return "Доброжелательность";
                case "Neutral":return "Нейтралитет";
                case "Chaotic":return "Хаотичность";
                case "Evil":return "Злонамеренность";
                default:throw new InvalidOperationException("Unreviewed Worksites alignment: "+source);
            }
        }
        public static void HeaderLabels(object __instance)
        {
            if(!Russian)return;
            var t=__instance.GetType();
            var alignment=AccessTools.Field(t,"alignment").GetValue(__instance);
            AccessTools.Field(t,"alignmentLabel").SetValue(__instance,AlignmentLabel(alignment.ToString()));
            var data=AccessTools.Field(t,"siteData").GetValue(__instance);
            string tag=data==null?null:(string)AccessTools.Field(data.GetType(),"outpostTag").GetValue(data);
            var def=string.IsNullOrEmpty(tag)?null:DefDatabase<SitePartDef>.GetNamedSilentFail(tag);
            if(def!=null)AccessTools.Field(t,"outpostType").SetValue(__instance,def.LabelCap.Resolve());
            else if(data==null)AccessTools.Field(t,"outpostType").SetValue(__instance,"Аванпост");
        }
        public static IEnumerable<CodeInstruction> RewriteGreeting(IEnumerable<CodeInstruction> input)
        {
            int arrays=0,fours=0;var result=new List<CodeInstruction>();
            foreach(var i in input){var c=new CodeInstruction(i);if(c.Calls(ArrayConcat)){c.operand=AccessTools.Method(typeof(WorksiteParleyPatch),nameof(GreetingArray));arrays++;}else if(c.Calls(FourConcat)){c.operand=AccessTools.Method(typeof(WorksiteParleyPatch),nameof(GreetingFour));fours++;}result.Add(c);}
            if(arrays!=1||fours!=1)throw new InvalidOperationException("Worksites greeting call sites changed.");return result;
        }
        public static IEnumerable<CodeInstruction> RewriteRepeat(IEnumerable<CodeInstruction> input)
        {
            int count=0;var result=new List<CodeInstruction>();
            foreach(var i in input){var c=new CodeInstruction(i);if(c.Calls(FourConcat)){c.operand=AccessTools.Method(typeof(WorksiteParleyPatch),nameof(Repeat));count++;}result.Add(c);}
            if(count!=1)throw new InvalidOperationException("Worksites repeated request call sites changed.");return result;
        }
        public static IEnumerable<CodeInstruction> RewriteFactors(IEnumerable<CodeInstruction> input)
        {
            int[] counts=new int[3];string[] names={nameof(One),nameof(Two),nameof(Three)};var result=new List<CodeInstruction>();
            foreach(var i in input){var c=new CodeInstruction(i);for(int n=0;n<3;n++)if(c.Calls(Formats[n])){c.operand=AccessTools.Method(typeof(WorksiteParleyPatch),names[n]);counts[n]++;break;}result.Add(c);}
            if(!counts.SequenceEqual(new[]{87,14,1}))throw new InvalidOperationException("Worksites factor format sites changed: "+string.Join(",",counts));return result;
        }
    }
}
