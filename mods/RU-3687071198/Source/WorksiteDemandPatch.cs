using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace Igor.WorksitesRussian
{
    public static class WorksiteDemandPatch
    {
        private static bool Russian=>LanguageDatabase.activeLanguage?.folderName.StartsWith("Russian",StringComparison.OrdinalIgnoreCase)==true;
        public static string RewardTwo(string f,object a,object b)=>Russian?WorksiteDemandText.RewardTwo(f,a,b):string.Format(f,a,b);
        public static string RewardThree(string f,object a,object b,object c)=>Russian?WorksiteDemandText.RewardThree(f,a,b,c):string.Format(f,a,b,c);
        public static string WorkerReward(string a,string b,string c)=>Russian?WorksiteDemandText.WorkerReward(a,b,c):string.Concat(a,b,c);
        public static string Missing(string a,string b,string c,string d)=>Russian?WorksiteDemandText.Missing(a,b,c,d):string.Concat(a,b,c,d);
        public static string MissingTwo(string a,string b)=>Russian?WorksiteDemandText.MissingTwo(a,b):string.Concat(a,b);
        public static string RepeatMissing(string a,string b,string c,string d,object dialog)
        {
            if(!Russian)return string.Concat(a,b,c,d);
            var specifics=AccessTools.Field(dialog.GetType(),"demandSpecifics").GetValue(dialog);
            string role=specifics==null?null:(string)AccessTools.Field(specifics.GetType(),"specificRole").GetValue(specifics);
            string defName=specifics==null?null:(string)AccessTools.Field(specifics.GetType(),"specificItemDefName").GetValue(specifics);
            string subject=role!=null?WorksiteDemandText.Role(role):!string.IsNullOrEmpty(defName)?DefDatabase<ThingDef>.GetNamedSilentFail(defName)?.LabelCap.Resolve()??c:"запрошенное";
            return WorksiteDemandText.RepeatMissing(a,b,subject,d);
        }
        public static string Counter(string f,object a,object b)=>Russian?WorksiteDemandText.Counter(f,a,b):string.Format(f,a,b);
        public static string SpecificThree(string a,string b,string c)=>Russian?WorksiteDemandText.SpecificThree(a,b,c):string.Concat(a,b,c);
        public static string SpecificArray(string[] p)=>Russian?WorksiteDemandText.SpecificArray(p):string.Concat(p);
        public static string Bounty(string a,string b,string c)=>Russian?WorksiteDemandText.Bounty(a,b,c):string.Concat(a,b,c);
        private static readonly Dictionary<string,Dictionary<string,Tuple<string,int>>> Bindings=new Dictionary<string,Dictionary<string,Tuple<string,int>>>
        {
            {"MiningOutpost.Parley.Dialog_Parley:BuildRewardDescription",new Dictionary<string,Tuple<string,int>>{{"Format(System.String,System.Object,System.Object)",Tuple.Create("RewardTwo",5)},{"Format(System.String,System.Object,System.Object,System.Object)",Tuple.Create("RewardThree",2)},{"Concat(System.String,System.String,System.String)",Tuple.Create("WorkerReward",2)}}},
            {"MiningOutpost.Parley.Dialog_Parley:GetUnavailableMessage",new Dictionary<string,Tuple<string,int>>{{"Concat(System.String,System.String,System.String,System.String)",Tuple.Create("Missing",2)},{"Concat(System.String,System.String)",Tuple.Create("MissingTwo",1)}}},
            {"MiningOutpost.Parley.Dialog_Parley:ResolveDemand",new Dictionary<string,Tuple<string,int>>{{"Concat(System.String,System.String,System.String,System.String)",Tuple.Create("RepeatMissing",1)},{"Format(System.String,System.Object,System.Object)",Tuple.Create("Counter",1)}}},
            {"MiningOutpost.Parley.ParleySuccessCalculator:GetSpecificityLabel",new Dictionary<string,Tuple<string,int>>{{"Concat(System.String,System.String,System.String)",Tuple.Create("SpecificThree",2)},{"Concat(System.String[])",Tuple.Create("SpecificArray",1)}}},
            {"MiningOutpost.Parley.Dialog_Parley:TriggerBountyRaid",new Dictionary<string,Tuple<string,int>>{{"Concat(System.String,System.String,System.String)",Tuple.Create("Bounty",1)}}}
        };
        public static void Install(Harmony h)
        {
            foreach(string key in Bindings.Keys)h.Patch(AccessTools.Method(key),transpiler:new HarmonyMethod(typeof(WorksiteDemandPatch),nameof(Rewrite)));
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
                        if(spec.Item1=="RepeatMissing")
                        {
                            var context=new CodeInstruction(System.Reflection.Emit.OpCodes.Ldarg_0);
                            context.MoveLabelsFrom(code);context.MoveBlocksFrom(code);output.Add(context);
                        }
                        code.operand=AccessTools.Method(typeof(WorksiteDemandPatch),spec.Item1);
                        seen[signature]=seen.TryGetValue(signature,out int n)?n+1:1;
                    }
                }
                output.Add(code);
            }
            if(specs.Any(s=>!seen.TryGetValue(s.Key,out int n)||n!=s.Value.Item2))throw new InvalidOperationException("Worksites demand formatter changed: "+__originalMethod);
            return output;
        }
    }
}
