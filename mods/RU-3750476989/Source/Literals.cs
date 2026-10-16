using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace Igor.RU_3750476989
{
    public sealed class LiteralTranslations : Mod
    {
        private static readonly Dictionary<string,Dictionary<string,string>> Methods =
            new Dictionary<string,Dictionary<string,string>> {
            { "RimtoChess.ArenaUnitRegistry:ResolveBlKeyLabel", new Dictionary<string,string> {
                { "(빈 키)", "(пустой ключ)" },
                { "(세력 def 없음)", "(определение фракции отсутствует)" },
                { "(유닛 def 없음)", "(определение бойца отсутствует)" },
                { "(종족 def 없음)", "(определение вида отсутствует)" },
                { "(형식 불명)", "(неизвестный формат)" },
                { "세력 {0} [{1}]", "Фракция: {0} [{1}]" },
                { "유닛 {0} [{1}]", "Боец: {0} [{1}]" },
                { "종족 {0} [{1}]", "Вид: {0} [{1}]" }
            } },
            { "RimtoChess.Dialog_AutoBattler:DrawShop", new Dictionary<string,string> {
                { "T", "Р" }
            } },
            { "RimtoChess.Dialog_AutoBattler:TierOddsLine", new Dictionary<string,string> {
                { "T{0} {1:0}%", "Р{0} {1:0}%" }
            } },
            { "RimtoChess.Dialog_Reward:BuildUnitSubInner", new Dictionary<string,string> {
                { "T", "Ранг " }
            } },
            { "RimtoChess.Dialog_RtcSynergyGuide:DrawBlock", new Dictionary<string,string> {
                { "T", "Р" }
            } },
            { "RimtoChess.Dialog_RtcSynergyGuide:ModSigLine", new Dictionary<string,string> {
                { " 전체강제</color>", " — принудительно для всех</color>" },
                { "<color=#FFD27F>[DEV] ", "<color=#FFD27F>[ОТЛАДКА] " }
            } },
            { "RimtoChess.RtcRosterBlacklist:DrawTab", new Dictionary<string,string> {
                { "T", "Р" }
            } },
            { "RimtoChess.RtcStatStore:DrawRankSection", new Dictionary<string,string> {
                { " LP", " ОЛ" }
            } },
            { "RimtoChess.RtcTip:UnitInfo", new Dictionary<string,string> {
                { "  (T", "  (ранг " }
            } },
            { "RimtoChess.Window_RtcRankedHub:DoWindowContents", new Dictionary<string,string> {
                { " LP", " ОЛ" }
            } },
            { "RimtoChess.Window_RtcRunResult:DrawRankedPanel", new Dictionary<string,string> {
                { " LP", " ОЛ" }
            } },
            { "RimtoChess.Window_RtcUnitChoice:DoWindowContents", new Dictionary<string,string> {
                { "T", "Р" }
            } }
            };
        private static readonly Dictionary<string,string[]> Parameters = new Dictionary<string,string[]> {

        };
        public LiteralTranslations(ModContentPack content) : base(content) { }
        public static void Apply()
        {
            var harmony = new Harmony("igor.ru.3750476989.literals");
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
