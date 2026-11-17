using System;
using System.Collections.Generic;
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
        public static ThingDef SSM_Tribal_MechSalvage = null;
        public static ThingDef ChunkMechanoidSlag = null;
        public static ThingDef UnfinishedTechArmor = null;

        static TribalDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(TribalDefOf));
        }
    }

    public sealed class RetiredSalvageRecipe : RecipeWorker
    {
        public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null) => false;
    }

    public sealed class MechSalvageMigration : GameComponent
    {
        private bool migrated;

        public MechSalvageMigration(Game game) { }

        public override void ExposeData() => Scribe_Values.Look(ref migrated, "mechSalvageMigrated");

        public override void StartedNewGame() => migrated = true;

        public override void LoadedGame()
        {
            if (migrated)
                return;

            var legacy = TribalDefOf.SSM_Tribal_MechSalvage;
            var items = new List<Thing>();
            migrated = true;
            foreach (Map map in Find.Maps)
            {
                foreach (Thing building in map.listerThings.ThingsInGroup(ThingRequestGroup.BuildingArtificial))
                {
                    if (building is IBillGiver station)
                    {
                        var bills = station.BillStack;
                        for (int i = bills.Count - 1; i >= 0; i--)
                        {
                            Bill bill = bills[i];
                            if (bill.recipe.workerClass == typeof(RetiredSalvageRecipe))
                                bills.Delete(bill);
                            else if (bill is Bill_Production production &&
                                bill.recipe.defName.StartsWith("SSM_Tribal_", StringComparison.Ordinal) &&
                                bill.recipe.IsIngredient(TribalDefOf.ChunkMechanoidSlag) &&
                                production.ingredientFilter.Allows(legacy))
                            {
                                production.ingredientFilter.SetAllow(legacy, false);
                                production.ingredientFilter.SetAllow(TribalDefOf.ChunkMechanoidSlag, true);
                            }
                        }
                    }
                }

                ThingOwnerUtility.GetAllThingsRecursively(map, ThingRequest.ForDef(legacy), items,
                    false, null, true);
                foreach (Thing item in items)
                    migrated &= SwapPlating(item);

                ThingOwnerUtility.GetAllThingsRecursively(map, ThingRequest.ForDef(TribalDefOf.UnfinishedTechArmor),
                    items, false, null, true);
                foreach (Thing item in items)
                    SwapUnfinishedPlating((UnfinishedThing)item);
            }

            foreach (var worldObject in Find.WorldObjects.AllWorldObjects)
            {
                if (!(worldObject is IThingHolder holder))
                    continue;

                ThingOwnerUtility.GetAllThingsRecursively(holder, items, false, null);
                foreach (Thing item in items)
                {
                    if (item.def == legacy)
                        migrated &= SwapPlating(item);
                    else if (item is UnfinishedThing unfinished && item.def == TribalDefOf.UnfinishedTechArmor)
                        SwapUnfinishedPlating(unfinished);
                }
            }
        }

        private static bool SwapPlating(Thing old)
        {
            // Just-loaded map objects still have a temporary Map-owned container.
            var owner = old.Spawned || old.holdingOwner?.Owner is Map ? null : old.holdingOwner;
            Map map = old.MapHeld;
            IntVec3 position = old.PositionHeld;
            bool forbidden = old.Spawned && old.IsForbidden(Faction.OfPlayer);
            while (!old.Destroyed)
            {
                Thing chunk = ThingMaker.MakeThing(TribalDefOf.ChunkMechanoidSlag);
                if (!(owner != null && owner.TryAdd(chunk)) &&
                    (map == null || !GenPlace.TryPlaceThing(chunk, position, map, ThingPlaceMode.Near)))
                {
                    chunk.Destroy();
                    return false;
                }

                if (forbidden)
                    chunk.SetForbidden(true, false);
                if (old.stackCount > 1)
                    old.stackCount--;
                else
                    old.Destroy();
            }
            return true;
        }

        private static void SwapUnfinishedPlating(UnfinishedThing unfinished)
        {
            var ingredients = unfinished.ingredients;
            if (ingredients == null)
                return;

            for (int i = ingredients.Count - 1; i >= 0; i--)
            {
                Thing old = ingredients[i];
                if (old.def != TribalDefOf.SSM_Tribal_MechSalvage)
                    continue;

                ingredients.RemoveAt(i);
                for (int count = 0; count < old.stackCount; count++)
                    ingredients.Insert(i, ThingMaker.MakeThing(TribalDefOf.ChunkMechanoidSlag));
                old.Destroy();
            }
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
