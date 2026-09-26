using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using HarmonyLib;
using Verse;

namespace Igor.CharacterEditorFixes
{
    public sealed class ModEntry : Mod
    {
        public ModEntry(ModContentPack content) : base(content)
        {
            var harmony = new Harmony("igor.charactereditor.fixes");
            var predicate = AccessTools.Method("CharacterEditor.GeneTool+<>c:<get_ListBodySizeGenes>b__184_0");
            harmony.Patch(predicate, prefix: new HarmonyMethod(typeof(Fixes), nameof(Fixes.BodySizeGene)));
            harmony.Patch(AccessTools.Method("CharacterEditor.Label:AddNamesFromPath"),
                transpiler: new HarmonyMethod(typeof(Fixes), nameof(Fixes.RepairNames)));
        }
    }

    public static class Fixes
    {
        private static readonly Func<GeneDef,bool> IsBodySizeGene = (Func<GeneDef,bool>)
            Delegate.CreateDelegate(typeof(Func<GeneDef,bool>), AccessTools.Method("CharacterEditor.GeneTool:IsBodySizeGene"));

        public static bool BodySizeGene(GeneDef __0, ref bool __result)
        {
            __result = IsBodySizeGene(__0);
            return false;
        }

        public static string RepairNameData(string text)
        {
            return Regex.Replace(text.TrimStart('\uFEFF'), @"(?m)^Is,,Retarded(?=\r?\nKai,,Henriksen;)", "Is,,Retarded;");
        }

        public static IEnumerable<CodeInstruction> RepairNames(IEnumerable<CodeInstruction> input)
        {
            var decode = AccessTools.Method("CharacterEditor.Extension:AsStringUNICODE");
            int changed = 0;
            foreach (var instruction in input)
            {
                yield return new CodeInstruction(instruction);
                if (instruction.Calls(decode))
                {
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Fixes), nameof(RepairNameData)));
                    changed++;
                }
            }
            if (changed != 1) throw new InvalidOperationException("Character Editor name decoder changed: " + changed);
        }
    }
}
