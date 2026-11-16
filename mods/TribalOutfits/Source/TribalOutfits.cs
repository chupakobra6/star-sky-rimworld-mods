using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace StarSky.TribalOutfits
{
    [DefOf]
    internal static class TribalDefOf
    {
        public static ThingDef SSM_Tribal_BarbarianHandWraps = null;
        public static ThingDef SSM_Tribal_BarbarianFootWraps = null;

        static TribalDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(TribalDefOf));
        }
    }

    [StaticConstructorOnStartup]
    internal static class BarbarianBonuses
    {
        private const float BowRate = 1.10f;
        private const float HuntingStealthBonus = 0.10f;
        private const float MeleeHitBonus = 0.05f;

        static BarbarianBonuses()
        {
            new Harmony("starsky.rimworld.tribaloutfits").PatchAll();
        }

        private static bool IsWearing(Pawn pawn, ThingDef apparel)
        {
            if (pawn?.apparel == null)
                return false;

            foreach (Apparel item in pawn.apparel.WornApparel)
            {
                if (item.def == apparel)
                    return true;
            }

            return false;
        }

        private static bool HasBowBonus(Pawn pawn, Thing weapon)
        {
            // Core bows and the Odyssey greatbow share this prefix. Modded bows may use it too.
            return weapon != null && weapon.def.IsRangedWeapon &&
                weapon.def.defName.StartsWith("Bow_", StringComparison.Ordinal) &&
                IsWearing(pawn, TribalDefOf.SSM_Tribal_BarbarianHandWraps);
        }

        [HarmonyPatch(typeof(Verb), nameof(Verb.WarmupTime), MethodType.Getter)]
        private static class BowWarmup
        {
            private static void Postfix(Verb __instance, ref float __result)
            {
                if (__instance is Verb_Shoot && HasBowBonus(__instance.CasterPawn, __instance.EquipmentSource))
                    __result /= BowRate;
            }
        }

        [HarmonyPatch(typeof(VerbProperties), nameof(VerbProperties.AdjustedCooldown), new[] { typeof(Tool), typeof(Pawn), typeof(Thing) })]
        private static class BowCooldown
        {
            private static void Postfix(VerbProperties __instance, Pawn attacker, Thing equipment, ref float __result)
            {
                if (typeof(Verb_Shoot).IsAssignableFrom(__instance.verbClass) && HasBowBonus(attacker, equipment))
                    __result /= BowRate;
            }
        }

        [HarmonyPatch(typeof(StatWorker), nameof(StatWorker.FinalizeValue))]
        private static class BarbarianStatBonuses
        {
            private static void Postfix(StatDef ___stat, StatRequest req, ref float val, bool applyPostProcess)
            {
                if (!applyPostProcess || (___stat != StatDefOf.HuntingStealth && ___stat != StatDefOf.MeleeHitChance))
                    return;

                // Both vanilla stats use a post-process curve. Add the percentage points to the result of that curve.
                if (___stat == StatDefOf.HuntingStealth && IsWearing(req.Thing as Pawn, TribalDefOf.SSM_Tribal_BarbarianFootWraps))
                    val = Math.Min(1f, val + HuntingStealthBonus);
                else if (___stat == StatDefOf.MeleeHitChance && IsWearing(req.Thing as Pawn, TribalDefOf.SSM_Tribal_BarbarianHandWraps))
                    val = Math.Min(1f, val + MeleeHitBonus);
            }
        }
    }
}
