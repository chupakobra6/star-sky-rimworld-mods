using System;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Igor.RU_3033901359
{
    // Text-only assignments to existing ASF-produced definitions. Never creates
    // a Def or changes the producer's conditions, collections or mechanics.
    public sealed class RuntimeDefFields : Mod
    {
        private static bool installed;
        private interface ISlot { void Capture(Def def); void Find(); void Apply(bool russian); void Reset(); }
        private sealed class Slot<T> : ISlot where T : Def
        {
            private readonly string name, field, russian;
            private readonly Func<T, string> get;
            private readonly Action<T, string> set;
            private T captured;
            private string original;
            public Slot(string name, string field, Func<T, string> get, Action<T, string> set, string russian)
            {
                var metadata = typeof(T).GetField(field, BindingFlags.Instance | BindingFlags.Public);
                if (metadata == null || metadata.FieldType != typeof(string) || metadata.IsInitOnly)
                    throw new InvalidOperationException("ASF public string field changed: " + typeof(T).FullName + "." + field);
                this.name = name; this.field = field; this.get = get; this.set = set; this.russian = russian;
            }
            public void Find() { Capture(DefDatabase<T>.GetNamedSilentFail(name)); }
            public void Capture(Def def)
            {
                if (def == null || def.defName != name || ReferenceEquals(captured, def)) return;
                var typed = def as T;
                if (typed == null) throw new InvalidOperationException("ASF definition type changed: " + name + "." + field);
                captured = typed; original = get(typed);
            }
            public void Apply(bool useRussian) { if (captured != null) set(captured, useRussian ? russian : original); }
            public void Reset() { captured = null; original = null; }
        }
        private static readonly ISlot[] slots = {
            new Slot<DesignationCategoryDef>("ASFstorage", "label", d => d.label, (d, v) => d.label = v, "хранение"),
            new Slot<KeyBindingCategoryDef>("Architect_ASFstorage", "label", d => d.label, (d, v) => d.label = v, "вкладка хранения"),
            new Slot<KeyBindingCategoryDef>("Architect_ASFstorage", "description", d => d.description, (d, v) => d.description = v, "Клавиши раздела «Хранение» в меню строительства."),
            new Slot<StuffCategoryDef>("ASFStoneChunks", "label", d => d.label, (d, v) => d.label = v, "каменные обломки"),
            new Slot<StuffCategoryDef>("ASFStoneChunks", "noun", d => d.noun, (d, v) => d.noun = v, "каменные обломки")
        };
        public RuntimeDefFields(ModContentPack content) : base(content)
        {
            if (installed) return;
            var harmony = new Harmony("igor.ru.3033901359.runtime-def-fields");
            var after = new HarmonyMethod(typeof(RuntimeDefFields), nameof(AfterInjection));
            after.before = new[] { "igor.preset.validation" };
            foreach (var method in new[] { "InjectIntoData_BeforeImpliedDefs", "InjectIntoData_AfterImpliedDefs" })
                harmony.Patch(AccessTools.Method(typeof(LoadedLanguage), method),
                    prefix: new HarmonyMethod(typeof(RuntimeDefFields), nameof(BeforeInjection)), postfix: after);
            harmony.Patch(AccessTools.Method(typeof(LanguageDatabase), "SelectLanguage"),
                prefix: new HarmonyMethod(typeof(RuntimeDefFields), nameof(BeforeLanguageSelect)));
            installed = true;
        }
        public static void Capture(Def def) { foreach (var slot in slots) slot.Capture(def); }
        public static void Apply(bool russian) { foreach (var slot in slots) slot.Apply(russian); }
        public static void RestoreAndReset() { Apply(false); foreach (var slot in slots) slot.Reset(); }
        private static void Find() { foreach (var slot in slots) slot.Find(); }
        private static bool Russian(LoadedLanguage language) { return language != null && language.folderName.StartsWith("Russian", StringComparison.OrdinalIgnoreCase); }
        private static void BeforeInjection() { Guard(Find); }
        private static void AfterInjection(LoadedLanguage __instance) { Guard(() => { Find(); if (Russian(__instance)) Apply(true); }); }
        private static void BeforeLanguageSelect() { Guard(RestoreAndReset); }
        internal static void Late() { Guard(() => { Find(); if (Russian(LanguageDatabase.activeLanguage)) Apply(true); }); }
        private static void Guard(Action action)
        {
            try { action(); }
            catch (Exception error) { Log.Error("[RU ASF] Runtime definition text: " + error); }
        }
    }
    [StaticConstructorOnStartup]
    public static class RuntimeDefFieldsStartup
    {
        static RuntimeDefFieldsStartup() { LongEventHandler.ExecuteWhenFinished(RuntimeDefFields.Late); }
    }
}
