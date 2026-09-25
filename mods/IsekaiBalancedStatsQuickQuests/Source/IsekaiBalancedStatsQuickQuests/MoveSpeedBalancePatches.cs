using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using IsekaiLeveling;
using IsekaiLeveling.Stats;
using RimWorld;
using Verse;

namespace IsekaiBalancedStatsQuickQuests
{
    internal static class MoveSpeedBalance
    {
        private const float OriginalBonusPerLevel = 0.005f;

        public static float BonusPerLevel()
        {
            return OriginalBonusPerLevel * IsekaiBalancedStatsQuickQuestsMod.Settings.LevelMoveSpeedMultiplier;
        }

        public static float Limit(float value, float cap)
        {
            return Math.Min(value, cap);
        }

        public static IEnumerable<CodeInstruction> ReplaceLevelRate(IEnumerable<CodeInstruction> instructions)
        {
            int replaced = 0;
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldc_R4 && instruction.operand is float rate
                    && rate == OriginalBonusPerLevel)
                {
                    replaced++;
                    instruction.opcode = OpCodes.Call;
                    instruction.operand = AccessTools.Method(typeof(MoveSpeedBalance), nameof(BonusPerLevel));
                    yield return instruction;
                }
                else yield return instruction;
            }

            if (replaced != 1)
                throw new InvalidOperationException("ISEKAI MoveSpeed level-rate IL changed; expected exactly one 0.005 constant.");
        }
    }

    // ISEKAI computes its flat level bonus inside the stat part, then applies
    // Berserker speed effects. Replace the rate in both calculation and tooltip
    // so the order and displayed contribution remain the same.
    [HarmonyPatch]
    public static class IsekaiLevelMoveSpeedPatch
    {
        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(StatPart_IsekaiMoveSpeed), "TransformValueCore");
            yield return AccessTools.Method(typeof(StatPart_IsekaiMoveSpeed), "ExplanationPartCore");
        }

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return MoveSpeedBalance.ReplaceLevelRate(instructions);
        }
    }

    // FinalizeValue runs after stat parts, equipment, genes and other factors.
    // Keep this limited to pawns actually using ISEKAI; ordinary pawn movement
    // and abstract race stats retain their original values.
    [HarmonyPatch(typeof(StatWorker), nameof(StatWorker.FinalizeValue))]
    public static class IsekaiPawnMoveSpeedCapPatch
    {
        [HarmonyPostfix]
        public static void Postfix(StatDef ___stat, StatRequest req, ref float val)
        {
            var settings = IsekaiBalancedStatsQuickQuestsMod.Settings;
            if (!settings.EnablePawnMoveSpeedCap || ___stat != StatDefOf.MoveSpeed || !req.HasThing
                || !(req.Thing is Pawn pawn)
                || pawn.Dead || pawn.Destroyed || IsekaiComponent.GetCached(pawn) == null)
                return;
            val = MoveSpeedBalance.Limit(val, settings.PawnMoveSpeedCap);
        }
    }

    [HarmonyPatch(typeof(StatWorker), nameof(StatWorker.GetExplanationFinalizePart))]
    public static class IsekaiPawnMoveSpeedCapExplanationPatch
    {
        [HarmonyPostfix]
        public static void Postfix(StatDef ___stat, StatRequest req, ref string __result)
        {
            var settings = IsekaiBalancedStatsQuickQuestsMod.Settings;
            if (!settings.EnablePawnMoveSpeedCap || ___stat != StatDefOf.MoveSpeed || !req.HasThing
                || !(req.Thing is Pawn pawn)
                || pawn.Dead || pawn.Destroyed || IsekaiComponent.GetCached(pawn) == null)
                return;
            __result += "\n" + "IsekaiFixes_PawnSpeedCapExplanation".Translate(settings.PawnMoveSpeedCap.ToString("F1"));
        }
    }
}
