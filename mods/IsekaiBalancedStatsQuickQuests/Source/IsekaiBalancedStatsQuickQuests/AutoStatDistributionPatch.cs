using HarmonyLib;
using IsekaiLeveling;

namespace IsekaiBalancedStatsQuickQuests
{
    [HarmonyPatch(typeof(IsekaiComponent), nameof(IsekaiComponent.AutoDistributeByClass))]
    public static class AutoStatDistributionPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(IsekaiComponent __instance)
        {
            if (!IsekaiBalancedStatsQuickQuestsMod.Settings.EnablePawnAutoDistribution)
                return true;

            Distribute(__instance);
            return false;
        }

        internal static void Distribute(IsekaiComponent component)
        {
            if (component?.stats == null || component.stats.availableStatPoints <= 0)
                return;

            AutoStatModeGameComponent gameComponent = AutoStatModeGameComponent.Get();
            AutoStatMode requestedMode = gameComponent?.GetMode(component.Pawn)
                                         ?? AutoStatMode.Balanced;
            string className = component.passiveTree?.assignedTree;
            int[] weights = requestedMode == AutoStatMode.ClassWeighted && !string.IsNullOrEmpty(className)
                ? WeightsForClass(className)
                : new[] { 1, 1, 1, 1, 1, 1 };

            int maxStat = IsekaiStatAllocation.GetEffectiveMaxStat();
            PawnAutoStatProgress progress = gameComponent?.GetPawnAutoStatProgress(component.Pawn)
                                            ?? new PawnAutoStatProgress();
            int[] allocated = progress.GetAllocationSnapshot();
            bool[] blocked = new bool[6];

            for (int i = 0; i < 6; i++)
            {
                IsekaiStatType stat = (IsekaiStatType)i;
                int value = component.stats.GetStat(stat);
                blocked[i] = value >= maxStat;
            }

            while (component.stats.availableStatPoints > 0)
            {
                int next = StatDistributionMath.ChooseNext(allocated, weights, blocked);
                if (next < 0)
                    break;

                IsekaiStatType stat = (IsekaiStatType)next;
                int value = component.stats.GetStat(stat);
                if (value >= maxStat)
                {
                    blocked[next] = true;
                    continue;
                }

                component.stats.SetStat(stat, value + 1);
                component.stats.availableStatPoints--;
                allocated[next]++;
                progress.RecordAllocation(next);
                blocked[next] = component.stats.GetStat(stat) >= maxStat;
            }

            component.InvalidateStatCache();
        }

        internal static int[] WeightsForClass(string className)
        {
            int[] weights = { 1, 1, 1, 1, 1, 1 };
            int primary = -1;
            int secondary = -1;

            switch (className)
            {
                case "Knight":      primary = 0; secondary = 2; break;
                case "Berserker":   primary = 0; secondary = 0; break;
                case "Duelist":     primary = 1; secondary = 0; break;
                case "Ranger":      primary = 1; secondary = 4; break;
                case "Paladin":     primary = 2; secondary = 5; break;
                case "Survivor":    primary = 2; secondary = 1; break;
                case "Mage":        primary = 3; secondary = 4; break;
                case "Alchemist":   primary = 3; secondary = 4; break;
                case "Crafter":     primary = 3; secondary = 1; break;
                case "Sage":        primary = 4; secondary = 3; break;
                case "Beastmaster": primary = 4; secondary = 5; break;
                case "Leader":      primary = 5; secondary = 4; break;
            }

            if (primary >= 0)
                weights[primary] += 2;
            if (secondary >= 0)
                weights[secondary] += 1;

            return weights;
        }
    }
}
