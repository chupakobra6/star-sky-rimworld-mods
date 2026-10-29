using System;

namespace CrashedShipMechanoidGuardFix
{
    internal static class GuardGroupEligibility
    {
        internal const string DefoliatorIncident = "DefoliatorShipPartCrash";
        internal const string PsychicIncident = "PsychicEmanatorShipPartCrash";

        internal static bool IsAffectedIncident(string incidentDefName)
        {
            return string.Equals(
                       incidentDefName,
                       DefoliatorIncident,
                       StringComparison.Ordinal)
                   || string.Equals(
                       incidentDefName,
                       PsychicIncident,
                       StringComparison.Ordinal);
        }

        internal static bool ShouldRecheck(bool contextActive, bool originalResult, bool combatGroup)
        {
            return contextActive && originalResult && combatGroup;
        }

        internal static bool Refine(
            bool contextActive,
            bool originalResult,
            bool combatGroup,
            int actuallyAffordableOptionCount)
        {
            if (!ShouldRecheck(contextActive, originalResult, combatGroup))
            {
                return originalResult;
            }

            return actuallyAffordableOptionCount > 0;
        }
    }
}

