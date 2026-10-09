using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;
namespace Igor.WorksitesRussian
{
    public static class WorksiteWorkerPatch
    {
        private static readonly MethodInfo FormatOne=typeof(string).GetMethod("Format",new[]{typeof(string),typeof(object)});
        private static readonly FieldInfo Worker=AccessTools.Field("MiningOutpost.Parley.Dialog_Parley:preGeneratedWorker");
        public static void Install(Harmony harmony)
        {
            var target=AccessTools.Method("MiningOutpost.Parley.Dialog_Parley:PreGenerateWorker");
            if(target==null||Worker==null||Worker.FieldType!=typeof(Pawn))throw new MissingMemberException("Worksites worker flavor API changed.");
            harmony.Patch(target,transpiler:new HarmonyMethod(typeof(WorksiteWorkerPatch),nameof(Rewrite)));
        }
        public static string Format(string source,object name,object dialog)
        {
            bool russian=LanguageDatabase.activeLanguage?.folderName.StartsWith("Russian",StringComparison.OrdinalIgnoreCase)==true;
            return russian?WorksiteWorkerText.Format(source,(Pawn)Worker.GetValue(dialog)):string.Format(source,name);
        }
        public static IEnumerable<CodeInstruction> Rewrite(IEnumerable<CodeInstruction> input)
        {
            var output=new List<CodeInstruction>(); int count=0;
            foreach(var instruction in input)
            {
                var copy=new CodeInstruction(instruction);
                if(copy.Calls(FormatOne))
                {
                    var dialog=new CodeInstruction(OpCodes.Ldarg_0);
                    dialog.labels.AddRange(copy.labels);copy.labels.Clear();
                    dialog.blocks.AddRange(copy.blocks);copy.blocks.Clear();
                    output.Add(dialog);copy.operand=AccessTools.Method(typeof(WorksiteWorkerPatch),nameof(Format));count++;
                }
                output.Add(copy);
            }
            if(count!=1)throw new InvalidOperationException("Worksites worker flavor format sites changed: "+count);
            return output;
        }
    }
}
