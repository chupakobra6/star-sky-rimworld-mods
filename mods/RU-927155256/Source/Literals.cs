using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace Igor.RU_927155256
{
    public sealed class LiteralTranslations : Mod
    {
        private static readonly Dictionary<string,Dictionary<string,string>> Methods =
            new Dictionary<string,Dictionary<string,string>> {
            { "SimpleSidearms.rimworld.Gizmo_Brainscope:GizmoOnGUI_old", new Dictionary<string,string> {
                { "Pawn is null", "Пешка не задана" },
                { "none yet", "пока нет" },
                { "Idle:", "Простой:" },
                { "Job:", "Задание:" },
                { "Last job:", "Предыдущее задание:" },
                { "JobDriver:", "JobDriver:" },
                { "def. rng:", "Основное стрелковое:" },
                { "pref. mle:", "Предпочт. ближнего боя:" },
                { "forced:", "Принудительно:" }
            } },
            { "SimpleSidearms.rimworld.Gizmo_SidearmsList:GizmoOnGUI_New", new Dictionary<string,string> {
                { " (godmode)", " (режим бога)" }
            } },
            { "SimpleSidearms.rimworld.Gizmo_SidearmsList:GizmoOnGUI_old", new Dictionary<string,string> {
                { " (godmode)", " (режим бога)" }
            } },
            { "PeteTimesSix.SimpleSidearms.SimpleSidearms_Settings:DoSettingsWindowContents", new Dictionary<string,string> {
                { " kg", " кг" }
            } },
            { "PeteTimesSix.SimpleSidearms.SimpleSidearms_Settings:Limits", new Dictionary<string,string> {
                { " kg", " кг" }
            } },
            { "PeteTimesSix.SimpleSidearms.Utilities.StatCalculator:canUseSidearmType", new Dictionary<string,string> {
                { "No issue", "Можно использовать" }
            } },
            { "PeteTimesSix.SimpleSidearms.Utilities.WeaponAssingment:DoFumbleMote", new Dictionary<string,string> {
                { "% chance", "% вероятности" }
            } },
            { "PeteTimesSix.SimpleSidearms.UI.Verse.CurveEditorPublic:DoCurveEditor", new Dictionary<string,string> {
                { "Add point at [{0:F0} - {1}{2}]", "Добавить точку [{0:F0} — {1}{2}]" },
                { "Move point at [{0:F0} - {1}{2}] to [{3:F0} - {4}{5}]", "Переместить точку [{0:F0} — {1}{2}] в [{3:F0} — {4}{5}]" },
                { "Remove point at [{0:F0} - {1}{2}]", "Удалить точку [{0:F0} — {1}{2}]" }
            } },
            { "SimpleSidearms.rimworld.Gizmo_SidearmsList:DrawIconForWeapon", new Dictionary<string,string> {
                { "DrawSidearm_gizmoTooltipOffhandWhileDrafted", "chupakobra6_927155256_OffhandDrafted" },
                { "DrawSidearm_gizmoTooltipOffhand", "chupakobra6_927155256_Offhand" }
            } }
            };
        private static readonly Dictionary<string,string[]> Parameters = new Dictionary<string,string[]> {

        };
        public LiteralTranslations(ModContentPack content) : base(content) { }
        public static void Apply()
        {
            var harmony = new Harmony("igor.ru.927155256.literals");
            foreach (var target in Methods.Keys)
                harmony.Patch(ResolveTarget(target),
                    transpiler: new HarmonyMethod(typeof(LiteralTranslations), nameof(Transpile)));
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
