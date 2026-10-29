using System;
using Verse;

namespace EnhancedCarryingCapacitySaveLoadFix
{
    [StaticConstructorOnStartup]
    internal static class SettingsPreload
    {
        static SettingsPreload()
        {
            if (Scribe.mode != LoadSaveMode.Inactive)
            {
                Log.Error(
                    "[Enhanced Carrying Capacity - Save Loading Fix] "
                    + "Cannot preload settings while Scribe mode is " + Scribe.mode + ".");
                return;
            }

            try
            {
                EnhancedCarryingCapacity.EnhancedCarryingCapacitySettings settings =
                    EnhancedCarryingCapacity.EnhancedCarryingCapacitySettings.Instance;

                if (settings == null)
                {
                    Log.Error(
                        "[Enhanced Carrying Capacity - Save Loading Fix] "
                        + "The original mod returned null settings.");
                    return;
                }

                Log.Message(
                    "[Enhanced Carrying Capacity - Save Loading Fix] "
                    + "Settings preloaded safely.");
            }
            catch (Exception exception)
            {
                Log.Error(
                    "[Enhanced Carrying Capacity - Save Loading Fix] "
                    + "Failed to preload the original mod settings: " + exception);
            }
        }
    }
}
