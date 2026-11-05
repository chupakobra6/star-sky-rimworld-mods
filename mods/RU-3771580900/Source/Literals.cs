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
            { "DawnNewDay.DawnSettings:ShowAppearanceSection", new Dictionary<string,string> {
                { "{0} X ({1} px)", "{0} X ({1} пикс.)" },
                { "{0} Y ({1} px)", "{0} Y ({1} пикс.)" },
                { "{0}: {1}x", "{0}: ×{1}" }
            } },
            { "DawnNewDay.DawnSettings:ShowDurationSection", new Dictionary<string,string> {
                { " (seconds)", " (секунды)" }
            } },
            { "DawnNewDay.DawnSettings:ShowExtraSection", new Dictionary<string,string> {
                { "{0} ({1:00}h)", "{0} ({1:00}:00)" }
            } },
            { "DawnNewDay.DawnTranslation:get_Hints_LabelFormatPresets", new Dictionary<string,string> {
                { "{DAY_QUADRUM_ORDINAL} of {QUADRUM}, {YEAR}", "День {DAY_QUADRUM_ORDINAL}; {QUADRUM}; {YEAR} год" },
                { "{SETTLEMENT} Y{YEAR}, somewhere in the <lower>{BIOME}</lower>", "{SETTLEMENT}, {YEAR} год; где-то в биоме <lower>{BIOME}</lower>" },
                { "DAY {DAY_SETTLE} <size={FONTSIZE / 2}>{HOUR_D2}:00</size>", "ДЕНЬ {DAY_SETTLE} <size={FONTSIZE / 2}>{HOUR_D2}:00</size>" }
            } },
            { "DawnNewDay.DawnTranslation:get_Hints_MN_LabelFormatPresets", new Dictionary<string,string> {
                { "<color={MN_OCCASION_REMAINING_COLOR}>-{MN_OCCASION_REMAINING} Remain-</color>", "<color={MN_OCCASION_REMAINING_COLOR}>Осталось: {MN_OCCASION_REMAINING}</color>" },
                { "<color={MN_REMINDER_COLOR}>-{MN_REMINDER_REMAINING} Remain-</color>", "<color={MN_REMINDER_COLOR}>Осталось: {MN_REMINDER_REMAINING}</color>" },
                { "<color={MN_REMINDER_SEVERITY_COLOR}>-{MN_REMINDER_REMAINING} Remain-</color>", "<color={MN_REMINDER_SEVERITY_COLOR}>Осталось: {MN_REMINDER_REMAINING}</color>" }
            } },
            { "DawnNewDay.Dialogs.Dialog_Hint:ShowHints", new Dictionary<string,string> {
                { "FormatText copied into the clipboard!", "Шаблон текста скопирован в буфер обмена!" }
            } },
            { "DawnNewDay.FormatContext:TimeRemaining", new Dictionary<string,string> {
                { "{0}h", "{0} ч" }
            } },
            { "DawnNewDay.DawnFormatterRegex+<>c:<.cctor>b__5_21", new Dictionary<string,string> {
                { "{0}m", "{0} м" }
            } },
            { "DawnNewDay.DawnFormatterRegex+<>c:<.cctor>b__5_22", new Dictionary<string,string> {
                { "{0:F2}km", "{0:F2} км" }
            } },
            { "DawnNewDay.DawnSettings:<ShowAppearanceSection>b__51_0", new Dictionary<string,string> {
                { "{0}x", "×{0}" }
            } },
            { "DawnNewDay.DawnSeparator:DoContents", new Dictionary<string,string> {
                { "{0} ({1} px)", "{0} ({1} пикс.)" }
            } }
            };
        private static readonly Dictionary<string,string[]> Parameters = new Dictionary<string,string[]> {
            { "DawnNewDay.DawnFormatterRegex+<>c:<.cctor>b__5_21", new string[] { "DawnNewDay.FormatContext" } },
            { "DawnNewDay.DawnFormatterRegex+<>c:<.cctor>b__5_22", new string[] { "DawnNewDay.FormatContext" } },
            { "DawnNewDay.DawnSettings:<ShowAppearanceSection>b__51_0", new string[] { "System.Single" } },
            { "DawnNewDay.DawnSeparator:DoContents", new string[] { "Verse.Listing_Standard" } },
            { "DawnNewDay.DawnSettings:ShowAppearanceSection", new string[] { "Verse.Listing_Standard" } },
            { "DawnNewDay.DawnTranslation:get_Hints_LabelFormatPresets", new string[] {  } }
        };
        public LiteralTranslations(ModContentPack content) : base(content) { }
        public static void Apply()
        {
            var harmony = new Harmony("igor.ru.3771580900.literals");
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
            IEnumerable<MethodBase> candidates = name == ".ctor"
                ? type.GetConstructors(flags).Where(m => m.Name == name).Cast<MethodBase>()
                : type.GetMethods(flags).Where(m => m.Name == name).Cast<MethodBase>();
            if (Parameters.TryGetValue(target, out var parameters))
                candidates = candidates.Where(m => m.GetParameters()
                    .Select(p => CecilParameterName(p.ParameterType)).SequenceEqual(parameters));
            else if (name == ".ctor")
                candidates = candidates.Where(m => m.GetParameters().Length == 0);
            var matches = candidates.ToArray();
            if (matches.Length != 1)
                throw new MissingMethodException("Expected one exact translation overload, found " + matches.Length + ": " + target);
            var method = matches[0];
            if (method.ContainsGenericParameters)
                throw new MissingMethodException("Open generic translation target requires an isolated call-site hook: " + target);
            return method;
        }
        private static string CecilParameterName(Type type)
        {
            if (type.HasElementType)
            {
                string element = CecilParameterName(type.GetElementType());
                if (element == null) return null;
                if (type.IsByRef) return element + "&";
                if (type.IsPointer) return element + "*";
                // Only vectors are supported. Never alias a non-SZ or
                // multidimensional array to a different parameter shape.
                return type.IsArray && type == type.GetElementType().MakeArrayType() ? element + "[]" : null;
            }
            if (!type.IsGenericType) return type.FullName;
            var arguments = type.GetGenericArguments().Select(CecilParameterName).ToArray();
            if (arguments.Any(argument => argument == null)) return null;
            return type.GetGenericTypeDefinition().FullName + "<" + string.Join(",", arguments) + ">";
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
