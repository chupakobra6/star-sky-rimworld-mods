using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace Igor.WorksitesRussian
{
    public static class WorksiteServicePatch
    {
        private static bool Russian=>LanguageDatabase.activeLanguage?.folderName.StartsWith("Russian",StringComparison.OrdinalIgnoreCase)==true;
        public static string CyberParts(string a,string b,string c)=>Russian?WorksiteServiceText.CyberParts(a,b,c):string.Concat(a,b,c);
        public static string CyberPrompt(string[] p)=>Russian?WorksiteServiceText.CyberPrompt(p):string.Concat(p);
        public static string WinTitle(string a,string b,string c,string d)=>Russian?WorksiteServiceText.WinTitle(a,b,c,d):string.Concat(a,b,c,d);
        public static string WinPrompt(string[] p)=>Russian?WorksiteServiceText.WinPrompt(p):string.Concat(p);
        public static string Stranded(string f,object a,object b)=>Russian?WorksiteServiceText.Stranded(f,a,b):string.Format(f,a,b);
        public static string Laborer(string a,string b)=>Russian?WorksiteServiceText.Laborer(a,b):string.Concat(a,b);
        private static readonly Dictionary<string,Dictionary<string,Tuple<string,int>>> Bindings=new Dictionary<string,Dictionary<string,Tuple<string,int>>>
        {
            {"MiningOutpost.Orbital.Services.OrbitalCyberDoc:ConfirmDestructiveInstall",new Dictionary<string,Tuple<string,int>> {
                {"Concat(System.String,System.String,System.String)",Tuple.Create("CyberParts",2)}, {"Concat(System.String[])",Tuple.Create("CyberPrompt",1)}}},
            {"MiningOutpost.Orbital.Services.OrbitalGamblingTable:FlushRivalWinPrompts",new Dictionary<string,Tuple<string,int>> {
                {"Concat(System.String,System.String,System.String,System.String)",Tuple.Create("WinTitle",1)}, {"Concat(System.String[])",Tuple.Create("WinPrompt",1)}}},
            {"MiningOutpost.Orbital.Services.Stranded:Draw",new Dictionary<string,Tuple<string,int>> {
                {"Format(System.String,System.Object,System.Object)",Tuple.Create("Stranded",1)}}},
            {"MiningOutpost.GenStep_WorkerSpawns:SpawnPawnAtOutpost",new Dictionary<string,Tuple<string,int>> {
                {"Concat(System.String,System.String)",Tuple.Create("Laborer",1)}}}
        };
        public static void Install(Harmony h)
        {
            foreach(string key in Bindings.Keys)h.Patch(AccessTools.Method(key),transpiler:new HarmonyMethod(typeof(WorksiteServicePatch),nameof(Rewrite)));
        }
        public static IEnumerable<CodeInstruction> Rewrite(IEnumerable<CodeInstruction> input,MethodBase __originalMethod)
        {
            var specs=Bindings[__originalMethod.DeclaringType.FullName+":"+__originalMethod.Name];
            var seen=new Dictionary<string,int>();var output=new List<CodeInstruction>();
            foreach(var instruction in input)
            {
                var code=new CodeInstruction(instruction);
                if(code.operand is MethodInfo method&&method.DeclaringType==typeof(string))
                {
                    var signature=method.Name+"("+string.Join(",",method.GetParameters().Select(p=>p.ParameterType.FullName))+")";
                    if(specs.TryGetValue(signature,out var spec))
                    {
                        code.operand=AccessTools.Method(typeof(WorksiteServicePatch),spec.Item1);
                        seen[signature]=seen.TryGetValue(signature,out int n)?n+1:1;
                    }
                }
                output.Add(code);
            }
            if(specs.Any(s=>!seen.TryGetValue(s.Key,out int n)||n!=s.Value.Item2))throw new InvalidOperationException("Worksites service formatter changed: "+__originalMethod);
            return output;
        }
    }
}
