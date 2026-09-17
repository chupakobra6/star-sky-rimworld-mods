using HarmonyLib;
using IsekaiLeveling;
using IsekaiLeveling.MobRanking;
using Verse;

namespace IsekaiBalancedStatsQuickQuests
{
    internal static class PetAutoStatDistributor
    {
        private static readonly int[] PhysicalWeights = { 2, 2, 2, 1, 1, 1 };
        private static readonly int[] BalancedWeights = { 1, 1, 1, 1, 1, 1 };

        public static void Distribute(MobRankComponent component)
        {
            if (!IsekaiBalancedStatsQuickQuestsMod.Settings.EnablePetAutoDistribution)
                return;

            Pawn pawn = component?.Pawn;
            if (pawn?.Faction == null || !pawn.Faction.IsPlayer || pawn.RaceProps.Humanlike
                || component.stats == null || component.stats.availableStatPoints <= 0)
                return;

            AutoStatModeGameComponent gameComponent = AutoStatModeGameComponent.Get();
            PetAutoStatState state = gameComponent?.GetPetState(pawn) ?? new PetAutoStatState();
            if (!state.Enabled)
                return;

            int[] weights = state.Mode == PetAutoStatMode.Balanced
                ? BalancedWeights
                : PhysicalWeights;
            int[] allocated = state.GetAllocationSnapshot();
            bool[] blocked = new bool[6];
            int maxStat = IsekaiStatAllocation.GetEffectiveMaxStat();

            for (int i = 0; i < blocked.Length; i++)
                blocked[i] = component.stats.GetStat((IsekaiStatType)i) >= maxStat;

            while (component.stats.availableStatPoints > 0)
            {
                int next = StatDistributionMath.ChooseNext(allocated, weights, blocked);
                if (next < 0)
                    break;

                IsekaiStatType stat = (IsekaiStatType)next;
                if (!component.stats.AllocatePoint(stat))
                {
                    blocked[next] = true;
                    continue;
                }

                allocated[next]++;
                state.RecordAllocation(next);
                blocked[next] = component.stats.GetStat(stat) >= maxStat;
            }
        }
    }

    [HarmonyPatch(typeof(MobRankComponent), "LevelUp")]
    public static class PetAutoStatsOnLevelUpPatch
    {
        [HarmonyPostfix]
        public static void Postfix(MobRankComponent __instance)
        {
            PetAutoStatDistributor.Distribute(__instance);
        }
    }

    [HarmonyPatch(typeof(MobRankComponent), nameof(MobRankComponent.CompTickRare))]
    public static class PetAutoStatsMigrationPatch
    {
        [HarmonyPostfix]
        public static void Postfix(MobRankComponent __instance)
        {
            // Handles unspent points in saves created before this patch and any
            // external mod that grants creature stat points outside LevelUp().
            PetAutoStatDistributor.Distribute(__instance);
        }
    }
}
