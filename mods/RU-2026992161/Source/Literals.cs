using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace Igor.RU_2026992161
{
    public sealed class LiteralTranslations : Mod
    {
        private static readonly Dictionary<string,Dictionary<string,string>> Methods =
            new Dictionary<string,Dictionary<string,string>> {
            { "DD.AbilityCompProperties_AbilityControl:.ctor", new Dictionary<string,string> {
                { "Breath Disabled", "Дыхание запрещено" },
                { "Breath Enabled", "Дыхание разрешено" },
                { "Toggle Dragon Breath Usage", "Переключить использование драконьего дыхания" }
            } },
            { "DD.CompEggIncubator+<CompGetGizmosExtra>d__30:MoveNext", new Dictionary<string,string> {
                { "Accelerated Growth", "Ускоренное развитие" },
                { "Debug: Force Tick", "Отладка: выполнить такт" },
                { "Debug: Hatch Now", "Отладка: вылупить сейчас" },
                { "Debug: Reset Progress", "Отладка: сбросить прогресс" },
                { "Revert egg growth rate to normal speed", "Вернуть обычную скорость развития яйца." },
                { "Speeds up the growth rate of the egg", "Ускорить развитие яйца." },
                { "Vanilla Growth", "Обычное развитие" }
            } },
            { "DD.CompEggIncubator:CompInspectStringExtra", new Dictionary<string,string> {
                { "Device has no fuel.\n", "В устройстве нет топлива.\n" },
                { "Device is inactive.\n", "Устройство выключено.\n" },
                { "Egg: ", "Яйцо: " }
            } },
            { "DD.CompRitualAltar+<CompGetGizmosExtra>d__12:MoveNext", new Dictionary<string,string> {
                { " Activation Count", " к счётчику активаций" },
                { "Debug: +", "Отладка: +" },
                { "Debug: Add Abilities", "Отладка: добавить способности" },
                { "Debug: Add Favor", "Отладка: добавить благосклонность" },
                { "Debug: Fill Favor", "Отладка: заполнить благосклонность" },
                { "Debug: Finish Cooldowns", "Отладка: завершить перезарядку" },
                { "Debug: Increment Activation Count", "Отладка: увеличить счётчик активаций" },
                { "Debug: Reset Activation Count", "Отладка: сбросить счётчик активаций" }
            } },
            { "DD.DisabledAbilityExtension:IsEnabledForPawn", new Dictionary<string,string> {
                { "Hidden/disabled ability - you should not be able to see this.", "Скрытая или отключённая способность. Она не должна отображаться." }
            } },
            { "DD.Gizmo_RitualInfo:GizmoOnGUI", new Dictionary<string,string> {
                { "Favor", "Благосклонность" },
                { "Rituals\nUnsupported", "Ритуалы\nнедоступны" }
            } },
            { "DD.HediffComp_GrowthSeverityScaling:CompDebugString", new Dictionary<string,string> {
                { "\nApply on Completion: ", "\nПрименить по завершении: " }
            } },
            { "DD.HediffComp_ModifyAge:CompDebugString", new Dictionary<string,string> {
                { "Duration: ", "Длительность: " }
            } },
            { "DD.HediffComp_ModifyAge:get_CompTipStringExtra", new Dictionary<string,string> {
                { "\n-Duration: ", "\nВремя: " },
                { "Remaining:\n-Age Adjustment: ", "Осталось:\nИзменение возраста: " }
            } }
            };
        private static readonly Dictionary<string,string[]> Parameters = new Dictionary<string,string[]> {

        };
        public LiteralTranslations(ModContentPack content) : base(content) { }
        public static void Apply()
        {
            var harmony = new Harmony("igor.ru.2026992161.literals");
            foreach (var target in Methods.Keys.ToArray())
            {
                if (!IsApplicable(target)) { Methods.Remove(target); continue; }
                harmony.Patch(ResolveTarget(target),
                    transpiler: new HarmonyMethod(typeof(LiteralTranslations), nameof(Transpile)));
            }
        }
        public static bool IsApplicable(string target)
        {
            switch (target)
            {

                default: return true;
            }
        }
        public static MethodBase ResolveTarget(string target)
        {
            int split = target.LastIndexOf(':');
            var type = AccessTools.TypeByName(target.Substring(0, split));
            string name = target.Substring(split + 1);
            if (type == null) throw new MissingMethodException("Exact translation type was not found: " + target);
            const BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.Instance | BindingFlags.Static;
            MethodBase method;
            if (Parameters.TryGetValue(target, out var parameters))
            {
                var signature = parameters.Select(AccessTools.TypeByName).ToArray();
                if (signature.Any(parameter => parameter == null))
                    throw new MissingMethodException("Translation parameter type was not found: " + target);
                method = name == ".ctor"
                    ? (MethodBase)type.GetConstructor(flags, null, signature, null)
                    : type.GetMethod(name, flags, null, signature, null);
            }
            else method = name == ".ctor"
                ? (MethodBase)type.GetConstructor(flags, null, Type.EmptyTypes, null)
                : type.GetMethod(name, flags);
            if (method == null || method.DeclaringType != type)
                throw new MissingMethodException("Exact translation target was not found: " + target);
            return method;
        }
        public static string Select(string original, string russian)
        {
            return LanguageDatabase.activeLanguage?.folderName.StartsWith("Russian", StringComparison.OrdinalIgnoreCase) == true
                ? russian : original;
        }
        public static IEnumerable<CodeInstruction> Transpile(IEnumerable<CodeInstruction> input, MethodBase __originalMethod)
        {
            var translations = Methods[__originalMethod.DeclaringType.FullName + ":" + __originalMethod.Name];
            var found = new HashSet<string>();
            var output = new List<CodeInstruction>();
            foreach (var instruction in input)
            {
                output.Add(instruction);
                if (instruction.opcode != OpCodes.Ldstr || !(instruction.operand is string text)
                    || !translations.TryGetValue(text, out var russian)) continue;
                found.Add(text);
                output.Add(new CodeInstruction(OpCodes.Ldstr, russian));
                output.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(LiteralTranslations), nameof(Select))));
            }
            if (found.Count != translations.Count)
                throw new InvalidOperationException("Source strings changed in " + __originalMethod + ": "
                    + string.Join(", ", translations.Keys.Where(k => !found.Contains(k))));
            return output;
        }
    }
    [StaticConstructorOnStartup]
    internal static class DeferredLiterals
    {
        static DeferredLiterals() { LongEventHandler.ExecuteWhenFinished(LiteralTranslations.Apply); }
    }
}
