using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace Igor.WorksitesRussian
{
    public static class WorksiteEventPatch
    {
        // Counts describe only display operations. Original random selection,
        // timers, progress, saved values and static source tables remain intact.
        private static readonly Dictionary<string,Tuple<int,int,int>> Bindings=new Dictionary<string,Tuple<int,int,int>>
        {
            {"MiningOutpost.WorkSiteRevengeTracker:GameComponentTick",Tuple.Create(1,0,0)},
            {"MiningOutpost.WorkSiteRevengeTracker:GetRaidLetter",Tuple.Create(4,0,0)},
            {"MiningOutpost.WorkSiteRevengeTracker:SendWarningLetter",Tuple.Create(1,0,2)},
            {"MiningOutpost.WorkSiteRevengeTracker:CompleteRevenge",Tuple.Create(1,0,0)},
            {"MiningOutpost.PulseEmitter.CompPulseEmitter:OnReachPercent",Tuple.Create(1,0,0)},
            {"MiningOutpost.PulseEmitter.CompPulseEmitter:FireQuest",Tuple.Create(2,0,0)},
            {"MiningOutpost.PulseEmitter.CompPulseEmitter:CompInspectStringExtra",Tuple.Create(0,1,0)},
            {"MiningOutpost.PulseEmitter.CompPulseEmitter+<CompGetGizmosExtra>d__55:MoveNext",Tuple.Create(0,1,0)},
            {"MiningOutpost.PulseEmitter.JobDriver_OperatePulseEmitter:GetReport",Tuple.Create(0,2,0)}
        };
        public static void Install(Harmony harmony)
        {
            foreach(string key in Bindings.Keys)harmony.Patch(AccessTools.Method(key),transpiler:new HarmonyMethod(typeof(WorksiteEventPatch),nameof(Rewrite)));
        }
        public static IEnumerable<CodeInstruction> Rewrite(IEnumerable<CodeInstruction> input,MethodBase __originalMethod)
        {
            var expected=Bindings[__originalMethod.DeclaringType.FullName+":"+__originalMethod.Name];
            int formats=0,elements=0,selections=0;
            var output=new List<CodeInstruction>();
            var format=typeof(string).GetMethod("Format",new[]{typeof(string),typeof(object)});
            foreach(var instruction in input)
            {
                var code=new CodeInstruction(instruction);
                if(expected.Item1>0&&code.Calls(format))
                {
                    code.operand=AccessTools.Method(typeof(WorksiteEventText),nameof(WorksiteEventText.FormatOne));formats++;
                }
                output.Add(code);
                if(expected.Item2>0&&code.opcode==OpCodes.Ldelem_Ref)
                {
                    output.Add(new CodeInstruction(OpCodes.Call,AccessTools.Method(typeof(WorksiteEventText),nameof(WorksiteEventText.Translate))));elements++;
                }
                if(expected.Item3>0&&code.operand is MethodInfo method&&method.DeclaringType.FullName=="Verse.GenCollection"
                    &&method.Name=="RandomElement"&&method.IsGenericMethod&&method.GetGenericArguments().SequenceEqual(new[]{typeof(string)}))
                {
                    output.Add(new CodeInstruction(OpCodes.Call,AccessTools.Method(typeof(WorksiteEventText),nameof(WorksiteEventText.Translate))));selections++;
                }
            }
            if(formats!=expected.Item1||elements!=expected.Item2||selections!=expected.Item3)
                throw new InvalidOperationException("Worksites event display source changed: "+__originalMethod);
            return output;
        }
    }
}
