using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;
namespace Igor.WorksitesRussian
{
    public static class WorksiteOrbitalSocialPatch
    {
        private static bool Russian=>LanguageDatabase.activeLanguage?.folderName.StartsWith("Russian",StringComparison.OrdinalIgnoreCase)==true;
        public static void Install(Harmony h)
        {
            h.Patch(AccessTools.Method("MiningOutpost.Orbital.OrbitalStrings:RandomStationName"),postfix:new HarmonyMethod(typeof(WorksiteOrbitalSocialPatch),nameof(Station)));
            h.Patch(AccessTools.Method("MiningOutpost.Orbital.OrbitalStrings:RandomRecruitmentHubName"),postfix:new HarmonyMethod(typeof(WorksiteOrbitalSocialPatch),nameof(Hub)));
            h.Patch(AccessTools.Method("MiningOutpost.Orbital.OrbitalPatronage:GenerateBrief"),transpiler:new HarmonyMethod(typeof(WorksiteOrbitalSocialPatch),nameof(RewriteBrief)));
        }
        public static void Station(ref string __result){if(Russian)__result=WorksiteOrbitalSocialText.Station(__result);}
        public static void Hub(ref string __result){if(Russian)__result=WorksiteOrbitalSocialText.Hub(__result);}
        public static string Brief(string format,object station,object opening,object originalList,List<string> selected)
        {
            return Russian?WorksiteOrbitalSocialText.Brief((string)station,(string)opening,selected):string.Format(format,station,opening,originalList);
        }
        public static IEnumerable<CodeInstruction> RewriteBrief(IEnumerable<CodeInstruction> input,MethodBase __originalMethod)
        {
            if(__originalMethod.GetMethodBody().LocalVariables[1].LocalType!=typeof(List<string>))throw new InvalidOperationException("Patronage selected-list contract changed");
            var target=AccessTools.Method(typeof(string),nameof(string.Format),new[]{typeof(string),typeof(object),typeof(object),typeof(object)});
            int count=0;var result=new List<CodeInstruction>();
            foreach(var i in input)
            {
                var c=new CodeInstruction(i);
                if(c.Calls(target))
                {
                    var load=new CodeInstruction(OpCodes.Ldloc_1);load.labels.AddRange(c.labels);c.labels.Clear();load.blocks.AddRange(c.blocks);c.blocks.Clear();result.Add(load);
                    c.operand=AccessTools.Method(typeof(WorksiteOrbitalSocialPatch),nameof(Brief));count++;
                }
                result.Add(c);
            }
            if(count!=1)throw new InvalidOperationException("Patronage final formatter count changed: "+count);
            return result;
        }
    }
}
