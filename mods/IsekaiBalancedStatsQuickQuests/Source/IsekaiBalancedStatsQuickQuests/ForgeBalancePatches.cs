using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using IsekaiLeveling.Forge;

namespace IsekaiBalancedStatsQuickQuests
{
    [HarmonyPatch]
    public static class ForgeCombatBonusPatch
    {
        public static IEnumerable<MethodBase> TargetMethods()
        {
            foreach (string name in new[] { "GetMeleeDamageBonus", "GetMeleeSpeedBonus", "GetRangedDamageBonus",
                "GetRangedCooldownReduction", "GetRangedAccuracyBonus", "GetArmorBonus" })
                yield return AccessTools.Method(typeof(ForgeUtility), name, new[] { typeof(int) });
        }

        [HarmonyPostfix]
        public static void Postfix(ref float __result)
        {
            var settings = IsekaiBalancedStatsQuickQuestsMod.Settings;
            if (settings.EnableForgeBalance) __result *= settings.ForgeCombatMultiplier;
        }
    }

    [HarmonyPatch(typeof(ForgeUtility), nameof(ForgeUtility.GetArmorMoveSpeedBonus))]
    public static class ForgeMoveSpeedPatch
    {
        [HarmonyPostfix]
        public static void Postfix(ref float __result)
        {
            var settings = IsekaiBalancedStatsQuickQuestsMod.Settings;
            if (settings.EnableForgeBalance) __result *= settings.ForgeMoveSpeedMultiplier;
        }
    }

    // Both the actual stat and its explanation call this upstream aggregation.
    // Cap only the forge contribution; DEX, genes, runes and other mods retain
    // their own movement effects. No item refinementLevel or rune is rewritten.
    [HarmonyPatch(typeof(StatPart_RefineArmorMoveSpeed), "GetTotalMoveSpeedBonus")]
    public static class ForgeTotalMoveSpeedPatch
    {
        [HarmonyPostfix]
        public static void Postfix(ref float __result)
        {
            var settings = IsekaiBalancedStatsQuickQuestsMod.Settings;
            if (settings.EnableForgeBalance && settings.ForgeMoveSpeedCap > 0f)
                __result = Math.Min(__result, settings.ForgeMoveSpeedCap);
        }
    }

    // The same returned structure feeds the displayed cost, availability check
    // and material consumption. Never patch the UI alone or charge twice.
    [HarmonyPatch(typeof(ForgeUtility), nameof(ForgeUtility.GetRefineCost))]
    public static class ForgeRefineCostPatch
    {
        internal static int Scale(int cost, float multiplier)
        {
            if (cost <= 0) return cost;
            // The UI's two-decimal multiplier is a fixed-point price. Binary32
            // 1.1f is slightly above 1.1; ceiling that float would charge 23 for
            // 20 * 1.10 instead of 22. Integer hundredths keep display and charge aligned.
            long hundredths = (long)Math.Round(multiplier * 100d, MidpointRounding.AwayFromZero);
            return (int)Math.Min(int.MaxValue, ((long)cost * hundredths + 99L) / 100L);
        }

        [HarmonyPostfix]
        public static void Postfix(ref ForgeUtility.RefineCost __result)
        {
            var settings = IsekaiBalancedStatsQuickQuestsMod.Settings;
            if (!settings.EnableForgeBalance || settings.ForgeCostMultiplier == 1f) return;
            __result.coreCount = Scale(__result.coreCount, settings.ForgeCostMultiplier);
            __result.secondaryCoreCount = Scale(__result.secondaryCoreCount, settings.ForgeCostMultiplier);
            __result.steel = Scale(__result.steel, settings.ForgeCostMultiplier);
            __result.components = Scale(__result.components, settings.ForgeCostMultiplier);
        }
    }
}
