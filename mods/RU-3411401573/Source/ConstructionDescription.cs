// Own integration code, licensed under MIT (see Licenses/MIT.txt).
using System;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Igor.RU_3411401573
{
    // Custom source: translations generate owns Overrides.cs and Literals.cs,
    // not this producer integration.
    [StaticConstructorOnStartup]
    public static class ConstructionDescription
    {
        private static readonly FieldInfo Instance;
        private static readonly FieldInfo Inventor;

        static ConstructionDescription()
        {
            var component = AccessTools.TypeByName("VanillaQuestsExpandedTheGenerator.Genetron_GameComponent");
            Instance = AccessTools.Field(component, "Instance");
            Inventor = AccessTools.Field(component, "inventor");
            if (Instance == null || Inventor == null || Inventor.FieldType != typeof(Pawn))
            {
                Log.Error("[Star Sky Mods] Generator inventor description producer changed; substitution not installed.");
                return;
            }
            new Harmony("igor.ru.3411401573.construction-description").Patch(
                AccessTools.PropertyGetter(typeof(Thing), "DescriptionDetailed"),
                postfix: new HarmonyMethod(typeof(ConstructionDescription), nameof(ResolveInventor)));
        }

        public static void ResolveInventor(Thing __instance, ref string __result)
        {
            var language = LanguageDatabase.activeLanguage;
            if (language == null || !language.folderName.StartsWith("Russian", StringComparison.OrdinalIgnoreCase)) return;
            if (!(__instance is Blueprint) && !(__instance is Frame)) return;
            if (__instance.def.entityDefToBuild?.modContentPack?.PackageIdPlayerFacing
                != "vanillaquestsexpanded.generator") return;
            if (__result == null || !__result.Contains("{InventorFullName}")) return;
            var component = Instance?.GetValue(null);
            var inventor = component == null ? null : Inventor?.GetValue(component) as Pawn;
            if (inventor != null)
                __result = __result.Replace("{InventorFullName}", inventor.NameFullColored);
        }
    }
}
