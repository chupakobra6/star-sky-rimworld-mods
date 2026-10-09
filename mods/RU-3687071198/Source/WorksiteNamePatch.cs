using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace Igor.WorksitesRussian
{
    public static class WorksiteNamePatch
    {
        private static readonly MethodInfo ArrayConcat = typeof(string).GetMethod("Concat",new[]{typeof(string[])});
        private static readonly MethodInfo FourConcat = typeof(string).GetMethod("Concat",new[]{typeof(string),typeof(string),typeof(string),typeof(string)});
        public static void Install(Harmony harmony)
        {
            var target=AccessTools.Method("MiningOutpost.QuestNode_GetMiningFaction:GenerateProceduralName");
            if(target==null)throw new MissingMethodException("Worksites faction name generator was not found.");
            harmony.Patch(target,transpiler:new HarmonyMethod(typeof(WorksiteNamePatch),nameof(Rewrite)));
        }
        private static bool Russian => LanguageDatabase.activeLanguage?.folderName.StartsWith("Russian",StringComparison.OrdinalIgnoreCase)==true;
        public static string FromArray(string[] parts) => Russian ? WorksiteNames.FromArray(parts) : string.Concat(parts);
        public static string FromFour(string a,string b,string c,string d) => Russian ? WorksiteNames.FromFour(a,b,c,d) : string.Concat(a,b,c,d);
        public static IEnumerable<CodeInstruction> Rewrite(IEnumerable<CodeInstruction> input)
        {
            int arrays=0,fours=0;
            var output=new List<CodeInstruction>();
            foreach(var instruction in input)
            {
                var copy=new CodeInstruction(instruction);
                if(copy.Calls(ArrayConcat)) {copy.operand=AccessTools.Method(typeof(WorksiteNamePatch),nameof(FromArray));arrays++;}
                else if(copy.Calls(FourConcat)) {copy.operand=AccessTools.Method(typeof(WorksiteNamePatch),nameof(FromFour));fours++;}
                output.Add(copy);
            }
            // Do not modify choices, RNG, saved identities, or the early medieval naming-provider return.
            if(arrays!=5||fours!=2)throw new InvalidOperationException("Worksites name-generation call sites changed: "+arrays+" arrays, "+fours+" four-part calls.");
            return output;
        }
    }
}
