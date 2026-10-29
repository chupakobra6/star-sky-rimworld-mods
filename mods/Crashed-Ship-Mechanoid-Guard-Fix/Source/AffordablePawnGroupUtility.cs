using System.Collections.Generic;
using RimWorld;

namespace CrashedShipMechanoidGuardFix
{
    internal static class AffordablePawnGroupUtility
    {
        internal static int CountActuallyAffordableOptions(
            PawnGroupMakerParms parms,
            PawnGroupMaker groupMaker)
        {
            if (parms == null
                || parms.faction == null
                || parms.faction.def == null
                || groupMaker == null
                || groupMaker.options == null)
            {
                return 0;
            }

            List<PawnGenOptionWithXenotype> options = PawnGroupMakerUtility.GetOptions(
                parms,
                parms.faction.def,
                groupMaker.options,
                parms.points,
                parms.points,
                null,
                null,
                false);
            return options.Count;
        }
    }
}

