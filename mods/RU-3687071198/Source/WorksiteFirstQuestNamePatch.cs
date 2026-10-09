using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld.QuestGen;
using Verse;
using Verse.Grammar;
namespace Igor.WorksitesRussian
{
    // Installed only with the complete translation, after the original source is loaded.
    public static class WorksiteFirstQuestNamePatch
    {
        private static readonly MethodInfo AddRules = AccessTools.Method(typeof(QuestGen), nameof(QuestGen.AddQuestNameRules), new[] { typeof(List<Rule>) });
        public static void Install(Harmony harmony)
        {
            var target = AccessTools.Method("MiningOutpost.QuestNode_GetMiningFaction:InjectFirstQuestNameRules", new[] { typeof(bool) });
            if (target == null || AddRules == null) throw new MissingMethodException("Worksites first-quest naming API changed.");
            harmony.Patch(target, transpiler: new HarmonyMethod(typeof(WorksiteFirstQuestNamePatch), nameof(Rewrite)));
        }
        public static void AddTranslatedRules(List<Rule> original, bool industrial)
        {
            bool russian = LanguageDatabase.activeLanguage?.folderName.StartsWith("Russian", StringComparison.OrdinalIgnoreCase) == true;
            QuestGen.AddQuestNameRules(russian ? WorksiteFirstQuestNames.Create(industrial) : original);
        }
        public static IEnumerable<CodeInstruction> Rewrite(IEnumerable<CodeInstruction> input)
        {
            var output = new List<CodeInstruction>();
            int count = 0;
            foreach (var instruction in input)
            {
                var copy = new CodeInstruction(instruction);
                if (copy.Calls(AddRules))
                {
                    var family = new CodeInstruction(OpCodes.Ldarg_1);
                    family.labels.AddRange(copy.labels); copy.labels.Clear();
                    family.blocks.AddRange(copy.blocks); copy.blocks.Clear();
                    output.Add(family);
                    copy.operand = AccessTools.Method(typeof(WorksiteFirstQuestNamePatch), nameof(AddTranslatedRules));
                    count++;
                }
                output.Add(copy);
            }
            if (count != 1) throw new InvalidOperationException("Worksites first-quest rule call sites changed: " + count);
            return output;
        }
    }
}
