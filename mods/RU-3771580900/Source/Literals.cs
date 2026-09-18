using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace Igor.RU_3771580900
{
    public sealed class LiteralTranslations : Mod
    {
        private static readonly Dictionary<string,Dictionary<string,string>> Methods =
            new Dictionary<string,Dictionary<string,string>> {
            { "DawnNewDay.FormatContext:get_FactionName", new Dictionary<string,string> {
                { "Faction", "Фракция" }
            } },
            { "DawnNewDay.FormatContext:get_SettlementName", new Dictionary<string,string> {
                { "Settlement", "Поселение" }
            } },
            { "DawnNewDay.FormatContext:TimeRemaining", new Dictionary<string,string> {
                { "{0}h", "{0} ч" }
            } },
            { "DawnNewDay.DawnFormatterUtility+<>c:<.cctor>b__11_18", new Dictionary<string,string> {
                { "{0}m", "{0} м" }
            } },
            { "DawnNewDay.DawnFormatterUtility+<>c:<.cctor>b__11_19", new Dictionary<string,string> {
                { "{0:F2}km", "{0:F2} км" }
            } },
            { "DawnNewDay.DawnSettings:ShowAppearanceSection", new Dictionary<string,string> {
                { "{0}: {1}x", "{0}: ×{1}" },
                { "{0} X ({1} px)", "{0} X ({1} пикс.)" },
                { "{0} Y ({1} px)", "{0} Y ({1} пикс.)" },
                { "{0} ({1} px)", "{0} ({1} пикс.)" }
            } },
            { "DawnNewDay.DawnSettings:ShowDurationSection", new Dictionary<string,string> {
                { " (seconds)", " (секунды)" }
            } },
            { "DawnNewDay.DawnSettings:ShowExtraSection", new Dictionary<string,string> {
                { "{0} ({1:00}h)", "{0} ({1:00}:00)" }
            } },
            { "DawnNewDay.DawnSettings:<ShowAppearanceSection>b__56_0", new Dictionary<string,string> {
                { "{0}x", "×{0}" }
            } },
            { "DawnNewDay.DawnTranslation:get_Hints_LabelFormatPresets", new Dictionary<string,string> {
                { "DAY {DAY_SETTLE} <size=30>{HOUR_D2}:00</size>", "ДЕНЬ {DAY_SETTLE} <size=30>{HOUR_D2}:00</size>" },
                { "{DAY_QUADRUM_ORDINAL} of {QUADRUM}, {YEAR}", "День {DAY_QUADRUM_ORDINAL}; {QUADRUM}; {YEAR} год" },
                { "{SETTLEMENT} Y{YEAR}, somewhere in the <lower>{BIOME}</lower>", "{SETTLEMENT}, {YEAR} год; где-то поблизости. Биом: <lower>{BIOME}</lower>" }
            } },
            { "DawnNewDay.DawnTranslation:get_Hints_MN_LabelFormatPresets", new Dictionary<string,string> {
                { "<color={MN_REMINDER_COLOR}>-{MN_REMINDER_REMAINING} Remain-</color>", "<color={MN_REMINDER_COLOR}>Осталось: {MN_REMINDER_REMAINING}</color>" },
                { "<color={MN_REMINDER_SEVERITY_COLOR}>-{MN_REMINDER_REMAINING} Remain-</color>", "<color={MN_REMINDER_SEVERITY_COLOR}>Осталось: {MN_REMINDER_REMAINING}</color>" },
                { "<color={MN_OCCASION_REMAINING_COLOR}>-{MN_OCCASION_REMAINING} Remain-</color>", "<color={MN_OCCASION_REMAINING_COLOR}>Осталось: {MN_OCCASION_REMAINING}</color>" }
            } },
            { "DawnNewDay.Dialogs.Dialog_Hint:ShowHints", new Dictionary<string,string> {
                { "FormatText copied into the clipboard!", "Шаблон текста скопирован в буфер обмена!" }
            } }
            };
        private static readonly Dictionary<string,string[]> Parameters = new Dictionary<string,string[]> {

        };
        public LiteralTranslations(ModContentPack content) : base(content) { }
        public static void Apply()
        {
            var harmony = new Harmony("igor.ru.3771580900.literals");
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
