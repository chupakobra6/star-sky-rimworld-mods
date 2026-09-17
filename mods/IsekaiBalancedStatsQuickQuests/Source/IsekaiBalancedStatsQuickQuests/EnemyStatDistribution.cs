using System;
using HarmonyLib;
using IsekaiLeveling;
using IsekaiLeveling.MobRanking;
using RimWorld;
using Verse;

namespace IsekaiBalancedStatsQuickQuests
{
    public enum EnemyStatMode { Original = 0, Balanced = 1, CombatRole = 2 }

    internal static class EnemyStatDistribution
    {
        internal static bool IsHostileFaction(Pawn pawn)
        {
            var player = Find.FactionManager?.OfPlayer;
            return pawn?.Faction != null && player != null && !pawn.Faction.IsPlayer
                && pawn.Faction.HostileTo(player);
        }

        // Used only inside upstream generation/role-shaping hooks, never by a
        // world sweep, load migration, recruitment hook or periodic stat refresh.
        internal static bool Redistribute(IsekaiStatAllocation stats, int[] weights)
        {
            if (stats == null || weights == null || weights.Length != 6) return false;
            int baseValue = IsekaiStatAllocation.BASE_STAT_VALUE;
            int cap = IsekaiStatAllocation.GetEffectiveMaxStat();
            long total = 0;
            for (int i = 0; i < 6; i++)
            {
                int value = stats.GetStat((IsekaiStatType)i);
                // Preserve authored/irregular stat blocks rather than silently
                // clamping or destroying investments that do not fit this model.
                if (value < baseValue || value > cap || weights[i] <= 0) return false;
                total += value - baseValue;
            }
            if (total > int.MaxValue) return false;
            int[] allocated = new int[6];
            bool[] blocked = new bool[6];
            for (int point = 0; point < total; point++)
            {
                for (int i = 0; i < 6; i++) blocked[i] = allocated[i] >= cap - baseValue;
                int next = StatDistributionMath.ChooseNext(allocated, weights, blocked);
                if (next < 0) return false; // No writes until all points fit.
                allocated[next]++;
            }
            for (int i = 0; i < 6; i++) stats.SetStat((IsekaiStatType)i, baseValue + allocated[i]);
            return true;
        }

        internal static int[] RoleWeights(IsekaiPawnRole role)
        {
            var pair = PawnRoleStats.StatsFor(role);
            int[] weights = { 1, 1, 1, 1, 1, 1 };
            weights[(int)pair.primary] += 2;
            weights[(int)pair.secondary] += 1;
            return weights;
        }
    }

    [HarmonyPatch(typeof(PawnRoleStats), nameof(PawnRoleStats.ApplyRoleDistribution))]
    public static class EnemyHumanStatsPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(Pawn pawn, IsekaiComponent comp)
        {
            EnemyStatMode mode = IsekaiBalancedStatsQuickQuestsMod.Settings.EnemyHumanMode;
            if (mode == EnemyStatMode.Original || comp == null || comp.forgeLocked
                || !EnemyStatDistribution.IsHostileFaction(pawn) || !PawnRoleStats.ShouldApply(pawn, comp))
                return true;
            int[] weights = mode == EnemyStatMode.CombatRole
                ? EnemyStatDistribution.RoleWeights(PawnRoleStats.DetectRole(pawn))
                : new[] { 1, 1, 1, 1, 1, 1 };
            if (!EnemyStatDistribution.Redistribute(comp.stats, weights)) return true;
            comp.InvalidateStatCache();
            return false;
        }
    }

    [HarmonyPatch(typeof(MobRankComponent), "EnsureStatsInitialized")]
    public static class EnemyCreatureStatsPatch
    {
        [HarmonyPrefix]
        public static void Prefix(MobRankComponent __instance, out bool __state)
        {
            __state = IsekaiBalancedStatsQuickQuestsMod.Settings.EnemyCreatureMode != EnemyStatMode.Original
                && !__instance.statsInitialized && !__instance.IsInitialized
                && EnemyStatDistribution.IsHostileFaction(__instance.Pawn);
        }

        [HarmonyPostfix]
        public static void Postfix(MobRankComponent __instance, bool __state)
        {
            if (!__state || !__instance.statsInitialized) return;
            EnemyStatMode mode = IsekaiBalancedStatsQuickQuestsMod.Settings.EnemyCreatureMode;
            EnemyStatDistribution.Redistribute(__instance.stats, mode == EnemyStatMode.CombatRole
                ? new[] { 2, 2, 2, 1, 1, 1 } : new[] { 1, 1, 1, 1, 1, 1 });
        }
    }
}
