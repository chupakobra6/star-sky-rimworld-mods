using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;
namespace Igor.WorksitesRussian
{
    public static class WorksiteVolunteerPatch
    {
        private static readonly MethodInfo FormatThree=typeof(string).GetMethod("Format",new[]{typeof(string),typeof(object),typeof(object),typeof(object)});
        public static void Install(Harmony harmony)
        {
            var target=AccessTools.Method("MiningOutpost.Orbital.Services.OrbitalVolunteerHall:DrawCandidateCard");
            if(target==null)throw new MissingMethodException("Worksites volunteer card method changed.");
            harmony.Patch(target,transpiler:new HarmonyMethod(typeof(WorksiteVolunteerPatch),nameof(Rewrite)));
        }
        public static string Format(string source,object name,object silver,object criminal,Pawn pawn)
        {
            bool russian=LanguageDatabase.activeLanguage?.folderName.StartsWith("Russian",StringComparison.OrdinalIgnoreCase)==true;
            return russian?WorksiteVolunteerText.Format(source,silver,(string)criminal,pawn):string.Format(source,name,silver,criminal);
        }
        public static IEnumerable<CodeInstruction> Rewrite(IEnumerable<CodeInstruction> input)
        {
            var output=new List<CodeInstruction>();int count=0;
            foreach(var instruction in input)
            {
                var copy=new CodeInstruction(instruction);
                if(copy.Calls(FormatThree))
                {
                    var pawn=new CodeInstruction(OpCodes.Ldarg_1);
                    pawn.labels.AddRange(copy.labels);copy.labels.Clear();
                    pawn.blocks.AddRange(copy.blocks);copy.blocks.Clear();
                    output.Add(pawn);copy.operand=AccessTools.Method(typeof(WorksiteVolunteerPatch),nameof(Format));count++;
                }
                output.Add(copy);
            }
            if(count!=1)throw new InvalidOperationException("Worksites volunteer lore format sites changed: "+count);
            return output;
        }
    }
}
