using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Verse;
using MiningOutpost.Orbital.Services;

namespace Igor.WorksitesRussian
{
    public static class WorksitePokerPatch
    {
        private static bool Russian=>LanguageDatabase.activeLanguage?.folderName.StartsWith("Russian",StringComparison.OrdinalIgnoreCase)==true;
        public static string Action(string a,string b,string c)=>Russian?WorksitePokerText.Action(a,b,c):string.Concat(a,b,c);
        public static string Wager(string[] p)=>Russian?WorksitePokerText.Wager(p):string.Concat(p);
        public static string Result(string a,string b,string c)=>Russian?WorksitePokerText.Result(a,b,c):string.Concat(a,b,c);
        public static string Bank(string[] p)=>Russian?WorksitePokerText.Bank(p):string.Concat(p);
        public static string WagerResult(string a,string b,string c)=>Russian?WorksitePokerText.WagerResult(a,b,c):string.Concat(a,b,c);
        public static string GearResult(string[] p)=>Russian?WorksitePokerText.GearResult(p):string.Concat(p);
        public static string Loss(string a,string b,string c,string d)=>Russian?WorksitePokerText.Loss(a,b,c,d):string.Concat(a,b,c,d);
        public static string Relationship(string a,string b,string c)=>Russian?WorksitePokerText.Relationship(a,b,c):string.Concat(a,b,c);
        public static string DeathFour(string a,string b,string c,string d)=>Russian?WorksitePokerText.DeathFour(a,b,c,d):string.Concat(a,b,c,d);
        public static string DeathTwo(string a,string b)=>Russian?WorksitePokerText.DeathTwo(a,b):string.Concat(a,b);
        public static string DeathThree(string a,string b,string c)=>Russian?WorksitePokerText.DeathThree(a,b,c):string.Concat(a,b,c);
        public static string OrganSide(string a,string b)=>Russian?WorksitePokerText.OrganSide(a,b):string.Concat(a,b);
        public static void OrganLabel(ref RivalAssetCatalog.Pick __result)
        {if(Russian&&__result.def!=null)__result.label=__result.def.label??__result.label;}
        private static readonly Dictionary<string,Dictionary<string,Tuple<string,int>>> Bindings=new Dictionary<string,Dictionary<string,Tuple<string,int>>>
        {
            {"MiningOutpost.Orbital.Services.PokerSim:TryStakeOrgan",new Dictionary<string,Tuple<string,int>>{{"Concat(System.String[])",Tuple.Create("Bank",1)},{"Concat(System.String,System.String)",Tuple.Create("OrganSide",1)}}},
            {"MiningOutpost.Orbital.Services.PokerSim:TryStakeBionic",new Dictionary<string,Tuple<string,int>>{{"Concat(System.String[])",Tuple.Create("Bank",1)}}},
            {"MiningOutpost.Orbital.Services.PokerSim:TryStakeEquipmentOrItem",new Dictionary<string,Tuple<string,int>>{{"Concat(System.String[])",Tuple.Create("Bank",1)}}},
            {"MiningOutpost.Orbital.Services.PokerSim:ApplySeatAction",new Dictionary<string,Tuple<string,int>> {
                {"Concat(System.String,System.String,System.String)",Tuple.Create("Action",3)}}},
            {"MiningOutpost.Orbital.Services.PokerSim:PlayerWagerAsset",new Dictionary<string,Tuple<string,int>> {
                {"Concat(System.String[])",Tuple.Create("Wager",1)}}},
            {"MiningOutpost.Orbital.Services.PokerSim:EndHand",new Dictionary<string,Tuple<string,int>> {
                {"Concat(System.String,System.String,System.String)",Tuple.Create("Result",4)},
                {"Concat(System.String[])",Tuple.Create("Bank",3)}}},
            {"MiningOutpost.Orbital.Services.PokerSim:ResolvePlayerWagers",new Dictionary<string,Tuple<string,int>> {
                {"Concat(System.String,System.String,System.String)",Tuple.Create("WagerResult",4)},
                {"Concat(System.String[])",Tuple.Create("GearResult",1)},
                {"Concat(System.String,System.String,System.String,System.String)",Tuple.Create("Loss",1)}}},
            {"MiningOutpost.Orbital.Services.PokerSim:PrintRelationshipFlavor",new Dictionary<string,Tuple<string,int>> {
                {"Concat(System.String,System.String,System.String)",Tuple.Create("Relationship",4)}}},
            {"MiningOutpost.Orbital.Services.PokerSim:BuildDeathContextText",new Dictionary<string,Tuple<string,int>> {
                {"Concat(System.String,System.String,System.String,System.String)",Tuple.Create("DeathFour",1)},
                {"Concat(System.String,System.String)",Tuple.Create("DeathTwo",2)},
                {"Concat(System.String,System.String,System.String)",Tuple.Create("DeathThree",2)}}}
        };
        public static void Install(Harmony h)
        {
            h.Patch(AccessTools.Method("MiningOutpost.Orbital.Services.RivalAssetCatalog:PickOrgan"),postfix:new HarmonyMethod(typeof(WorksitePokerPatch),nameof(OrganLabel)));
            foreach(string key in Bindings.Keys)h.Patch(AccessTools.Method(key),transpiler:new HarmonyMethod(typeof(WorksitePokerPatch),nameof(Rewrite)));
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
                        code.operand=AccessTools.Method(typeof(WorksitePokerPatch),spec.Item1);
                        seen[signature]=seen.TryGetValue(signature,out int n)?n+1:1;
                    }
                }
                output.Add(code);
            }
            if(specs.Any(s=>!seen.TryGetValue(s.Key,out int n)||n!=s.Value.Item2))throw new InvalidOperationException("Worksites poker formatter changed: "+__originalMethod);
            return output;
        }
    }
}
