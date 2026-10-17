using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace Igor.RU_2891845502
{
    // MVCF's AdditionalVerbProps.label is an equality key, not display text.
    // Keep this one reviewed pair equal after DefInjected changes the verb.
    public sealed class FleshSpitterLabelSync : Mod
    {
        private const string DefName = "AG_FleshSpitter";
        private const string OriginalLabel = "acid spit";
        private const string CompType = "MVCF.Comps.HediffCompProperties_ExtendedVerbGiver";
        private static bool installed;
        private static HediffDef capturedDef;
        private static VerbProperties verb;
        private static object additional;
        private static FieldInfo additionalLabel;

        public FleshSpitterLabelSync(ModContentPack content) : base(content)
        {
            if (installed) return;
            var harmony = new Harmony("igor.ru.2891845502.flesh-spitter-label-pair");
            harmony.Patch(AccessTools.Method(typeof(LoadedLanguage), "InjectIntoData_BeforeImpliedDefs"),
                prefix: new HarmonyMethod(typeof(FleshSpitterLabelSync), nameof(BeforeInjection)),
                postfix: new HarmonyMethod(typeof(FleshSpitterLabelSync), nameof(AfterInjection)));
            harmony.Patch(AccessTools.Method(typeof(LoadedLanguage), "InjectIntoData_AfterImpliedDefs"),
                postfix: new HarmonyMethod(typeof(FleshSpitterLabelSync), nameof(AfterInjection)));
            harmony.Patch(AccessTools.Method(typeof(LanguageDatabase), "SelectLanguage"),
                prefix: new HarmonyMethod(typeof(FleshSpitterLabelSync), nameof(BeforeLanguageSelect)));
            installed = true;
        }

        public static void Capture(HediffDef def)
        {
            if (def == null || ReferenceEquals(capturedDef, def)) return;
            if (def.defName != DefName || !string.Equals(def.modContentPack?.PackageId,
                "sarg.alphagenes", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("AG_FleshSpitter owner changed.");
            var matches = def.comps?.Where(c => c.GetType().FullName == CompType).ToArray();
            if (matches == null || matches.Length != 1)
                throw new InvalidOperationException("Expected exactly one MVCF extended verb giver.");
            var giver = matches[0] as HediffCompProperties_VerbGiver;
            var extrasField = AccessTools.Field(matches[0].GetType(), "verbProps");
            var extras = extrasField?.GetValue(matches[0]) as IList;
            if (giver?.verbs == null || giver.verbs.Count != 1 || extras == null || extras.Count != 1
                || giver.verbs[0].label != OriginalLabel
                || giver.verbs[0].defaultProjectile?.defName != "Bullet_Shell_AcidSpit"
                || extras[0]?.GetType().FullName != "MVCF.Comps.AdditionalVerbProps")
                throw new InvalidOperationException("AG_FleshSpitter acid-spit structure changed.");
            var label = AccessTools.Field(extras[0].GetType(), "label");
            if (label == null || label.FieldType != typeof(string)
                || (string)label.GetValue(extras[0]) != OriginalLabel)
                throw new InvalidOperationException("MVCF acid-spit matching key changed.");
            capturedDef = def;
            verb = giver.verbs[0];
            additional = extras[0];
            additionalLabel = label;
        }

        public static void Apply(bool russian)
        {
            if (capturedDef == null) return;
            if (string.IsNullOrEmpty(verb.label))
                throw new InvalidOperationException("AG_FleshSpitter translated verb label is empty.");
            if (!russian) verb.label = OriginalLabel;
            additionalLabel.SetValue(additional, russian ? verb.label : OriginalLabel);
        }

        private static void BeforeInjection()
        {
            Guard(() => Capture(DefDatabase<HediffDef>.GetNamedSilentFail(DefName)));
        }

        private static void AfterInjection(LoadedLanguage __instance)
        {
            if (__instance.folderName.StartsWith("Russian", StringComparison.OrdinalIgnoreCase))
                Guard(() => Apply(true));
            else if (__instance.folderName.StartsWith("English", StringComparison.OrdinalIgnoreCase))
                Guard(() => Apply(false));
        }

        private static void BeforeLanguageSelect()
        {
            // SelectLanguage queues ClearAllPlayData + LoadAllPlayData. Restore
            // old objects now; the next injection captures the fresh EN objects.
            Guard(() => Apply(false));
            capturedDef = null;
            verb = null;
            additional = null;
            additionalLabel = null;
        }

        private static void Guard(Action action)
        {
            try { action(); }
            catch (Exception error) { Log.Error("[RU Alpha Genes] FleshSpitter MVCF label pair: " + error); }
        }
    }
}
