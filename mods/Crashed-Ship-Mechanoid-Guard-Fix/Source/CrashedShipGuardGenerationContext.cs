using System;

namespace CrashedShipMechanoidGuardFix
{
    internal static class CrashedShipGuardGenerationContext
    {
        [ThreadStatic]
        private static bool active;

        internal static bool Active
        {
            get { return active; }
        }

        internal static bool Begin(bool activate)
        {
            bool previous = active;
            active = previous || activate;
            return previous;
        }

        internal static void End(bool previous)
        {
            active = previous;
        }
    }
}

