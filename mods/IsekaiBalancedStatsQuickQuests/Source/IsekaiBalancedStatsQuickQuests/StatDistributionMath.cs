using System;

namespace IsekaiBalancedStatsQuickQuests
{
    internal static class StatDistributionMath
    {
        /// <summary>
        /// Chooses the stat that is furthest below its weighted fair share.
        /// Scores are integer numerators, so the result is deterministic and
        /// cannot drift because of floating-point rounding.
        /// </summary>
        public static int ChooseNext(int[] allocated, int[] weights, bool[] blocked)
        {
            if (allocated == null || weights == null || blocked == null
                || allocated.Length != weights.Length || weights.Length != blocked.Length)
                throw new ArgumentException("Allocation arrays must have the same length.");

            long totalAllocated = 0;
            long totalWeight = 0;
            for (int i = 0; i < allocated.Length; i++)
            {
                if (!blocked[i])
                {
                    totalAllocated += Math.Max(0, allocated[i]);
                    totalWeight += Math.Max(0, weights[i]);
                }
            }

            if (totalWeight <= 0)
                return -1;

            int bestIndex = -1;
            long bestScore = long.MinValue;
            long nextTotal = totalAllocated + 1;

            for (int i = 0; i < allocated.Length; i++)
            {
                if (blocked[i] || weights[i] <= 0)
                    continue;

                long score = nextTotal * weights[i] - (long)Math.Max(0, allocated[i]) * totalWeight;
                if (score > bestScore)
                {
                    bestScore = score;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }
    }
}
