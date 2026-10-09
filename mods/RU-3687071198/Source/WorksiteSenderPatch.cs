using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld.QuestGen;
using Verse;
namespace Igor.WorksitesRussian
{
    public static class WorksiteSenderPatch
    {
        private static readonly MethodInfo SetString = typeof(Slate).GetMethods().Single(m => m.Name == "Set" && m.IsGenericMethodDefinition && m.GetParameters().Length == 3).MakeGenericMethod(typeof(string));
        public static void Install(Harmony harmony)
        {
            var target=AccessTools.Method("MiningOutpost.QuestNode_GetMiningFaction:SetFirstQuestSenderVars");
            if (target==null) throw new MissingMethodException("Worksites first-quest sender method changed.");
            harmony.Patch(target,transpiler:new HarmonyMethod(typeof(WorksiteSenderPatch),nameof(Rewrite)));
        }
        public static void SetTranslated(Slate slate,string key,string value,bool global)
        {
            bool russian=LanguageDatabase.activeLanguage?.folderName.StartsWith("Russian",StringComparison.OrdinalIgnoreCase)==true;
            slate.Set(key,russian?WorksiteSenderText.Translate(key,value):value,global);
        }
        public static IEnumerable<CodeInstruction> Rewrite(IEnumerable<CodeInstruction> input)
        {
            var output=new List<CodeInstruction>(); int count=0;
            foreach(var instruction in input)
            {
                var copy=new CodeInstruction(instruction);
                if(copy.Calls(SetString)) {copy.opcode=OpCodes.Call;copy.operand=AccessTools.Method(typeof(WorksiteSenderPatch),nameof(SetTranslated));count++;}
                output.Add(copy);
            }
            if(count!=3)throw new InvalidOperationException("Worksites sender Slate writes changed: "+count);
            // This is before MedievalSenderFlavor.TryApply; external providers retain their own output.
            return output;
        }
    }
}
