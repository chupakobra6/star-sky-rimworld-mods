using System;
using HarmonyLib;
using RimWorld;

namespace CrashedShipMechanoidGuardFix.Patches
{
    [HarmonyPatch(typeof(IncidentWorker_CrashedShipPart), "TryExecuteWorker")]
    internal static class CrashedShipIncidentContextPatch
    {
        [HarmonyPrefix]
        private static void Prefix(
            IncidentWorker_CrashedShipPart __instance,
            out bool __state)
        {
            string defName = __instance.def == null
                ? null
                : __instance.def.defName;
            __state = CrashedShipGuardGenerationContext.Begin(
                GuardGroupEligibility.IsAffectedIncident(defName));
        }

        [HarmonyFinalizer]
        private static Exception Finalizer(Exception __exception, bool __state)
        {
            CrashedShipGuardGenerationContext.End(__state);
            return __exception;
        }
    }

    [HarmonyPatch(
        typeof(PawnGroupKindWorker_Normal),
        nameof(PawnGroupKindWorker_Normal.CanGenerateFrom))]
    internal static class CrashedShipGroupEligibilityPatch
    {
        [HarmonyPostfix]
        private static void Postfix(
            PawnGroupMakerParms parms,
            PawnGroupMaker groupMaker,
            ref bool __result)
        {
            bool active = CrashedShipGuardGenerationContext.Active;
            if (!active || !__result) return;

            bool combatGroup = parms != null && groupMaker != null
                && parms.groupKind == PawnGroupKindDefOf.Combat
                && groupMaker.kindDef == PawnGroupKindDefOf.Combat;
            if (!GuardGroupEligibility.ShouldRecheck(active, __result, combatGroup))
            {
                return;
            }

            int optionCount = AffordablePawnGroupUtility.CountActuallyAffordableOptions(
                parms,
                groupMaker);
            __result = GuardGroupEligibility.Refine(active, __result, combatGroup, optionCount);
        }
    }
}

