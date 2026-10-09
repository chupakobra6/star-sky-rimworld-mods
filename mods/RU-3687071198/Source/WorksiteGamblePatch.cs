using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace Igor.WorksitesRussian
{
    public static class WorksiteGamblePatch
    {
        private static bool Russian=>LanguageDatabase.activeLanguage?.folderName.StartsWith("Russian",StringComparison.OrdinalIgnoreCase)==true;
        public static string Prompt(string a,string b,string c)=>Russian?WorksiteGambleText.Prompt(a,b,c):string.Concat(a,b,c);
        public static string Title(string a,string b)=>Russian?WorksiteGambleText.Title(a,b):string.Concat(a,b);
        public static string Wager(string[] p)=>Russian?WorksiteGambleText.Wager(p):string.Concat(p);
        public static string Award(string[] p)=>Russian?WorksiteGambleText.Award(p):string.Concat(p);
        private static readonly Dictionary<string,Dictionary<string,Tuple<string,int>>> Bindings=new Dictionary<string,Dictionary<string,Tuple<string,int>>>
        {
            {"MiningOutpost.Orbital.Services.OrbitalGamblingTable:ConfirmWagerOrgan",new Dictionary<string,Tuple<string,int>>{{"Concat(System.String,System.String,System.String)",Tuple.Create("Prompt",1)},{"Concat(System.String,System.String)",Tuple.Create("Title",1)}}},
            {"MiningOutpost.Orbital.Services.OrbitalGamblingTable:ConfirmWagerOrganInner",new Dictionary<string,Tuple<string,int>>{{"Concat(System.String[])",Tuple.Create("Wager",1)}}},
            {"MiningOutpost.Orbital.Services.OrbitalGamblingTable:ConfirmWagerGear",new Dictionary<string,Tuple<string,int>>{{"Concat(System.String[])",Tuple.Create("Wager",1)}}},
            {"MiningOutpost.Orbital.Services.OrbitalGamblingTable:ConfirmAllIn",new Dictionary<string,Tuple<string,int>>{{"Concat(System.String,System.String,System.String)",Tuple.Create("Prompt",1)}}},
            {"MiningOutpost.Orbital.Services.OrbitalGamblingTable:OpenWagerOrganMenu",new Dictionary<string,Tuple<string,int>>{{"Concat(System.String,System.String,System.String)",Tuple.Create("Prompt",2)}}},
            {"MiningOutpost.Orbital.Services.OrbitalGamblingTable:OpenWagerGearMenu",new Dictionary<string,Tuple<string,int>>{{"Concat(System.String,System.String,System.String)",Tuple.Create("Prompt",3)}}},
            {"MiningOutpost.Orbital.Services.OrbitalGamblingTable:HandleAnestheticPromptIfNeeded",new Dictionary<string,Tuple<string,int>>{{"Concat(System.String,System.String)",Tuple.Create("Title",1)}}},
            {"MiningOutpost.Orbital.Services.OrbitalGamblingTable:GiveRivalOrganToCaravan",new Dictionary<string,Tuple<string,int>>{{"Concat(System.String,System.String,System.String)",Tuple.Create("Prompt",2)},{"Concat(System.String[])",Tuple.Create("Award",1)}}},
            {"MiningOutpost.Orbital.Services.OrbitalGamblingTable:SellRivalOrganIntoBankroll",new Dictionary<string,Tuple<string,int>>{{"Concat(System.String[])",Tuple.Create("Award",1)}}}
        };
        public static void Install(Harmony h)
        {
            foreach(string key in Bindings.Keys)h.Patch(AccessTools.Method(key),transpiler:new HarmonyMethod(typeof(WorksiteGamblePatch),nameof(Rewrite)));
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
                        code.operand=AccessTools.Method(typeof(WorksiteGamblePatch),spec.Item1);
                        seen[signature]=seen.TryGetValue(signature,out int n)?n+1:1;
                    }
                }
                output.Add(code);
            }
            if(specs.Any(s=>!seen.TryGetValue(s.Key,out int n)||n!=s.Value.Item2))throw new InvalidOperationException("Worksites gambling prompt formatter changed: "+__originalMethod);
            return output;
        }
    }
}
