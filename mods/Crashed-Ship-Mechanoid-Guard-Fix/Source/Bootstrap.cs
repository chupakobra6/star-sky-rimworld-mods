using System;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace CrashedShipMechanoidGuardFix
{
    [StaticConstructorOnStartup]
    internal static class Bootstrap
    {
        internal const string HarmonyId = "igor.crashedshipmechanoidguardfix";
        internal const string ModName = "Crashed Ship Mechanoid Guard Fix";
        internal const string Version = "1.0.0";

        static Bootstrap()
        {
            try
            {
                new Harmony(HarmonyId).PatchAll(Assembly.GetExecutingAssembly());
                Log.Message("[" + ModName + "] " + Version + " loaded.");
            }
            catch (Exception exception)
            {
                Log.Error("[" + ModName + "] Failed to apply Harmony patches: " + exception);
            }
        }
    }
}

