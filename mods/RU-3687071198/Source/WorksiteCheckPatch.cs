using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Igor.WorksitesRussian
{
    public static class WorksiteCheckPatch
    {
        private static bool Russian => LanguageDatabase.activeLanguage?.folderName.StartsWith("Russian", StringComparison.OrdinalIgnoreCase) == true;
        public static string Skill(string id) => string.IsNullOrEmpty(id) ? id : DefDatabase<SkillDef>.GetNamedSilentFail(id)?.LabelCap.ToString() ?? id;
        private static string DisplayName(string value) => Russian ? WorksiteCheckText.Name(value, Skill) : value;
        public static void CheckName(ref string __result) { __result = DisplayName(__result); }
        public static void StatName(string __0, ref string __result) { if (Russian) __result = WorksiteCheckText.Stat(__0) == __0 ? "Способность" : WorksiteCheckText.Stat(__0); }
        public static void StatFull(string __0, ref string __result) { if (Russian) __result = WorksiteCheckText.Stat(__0); }
        public static string PawnCaption(Pawn pawn) => Russian ? pawn?.NameShortColored.Resolve() ?? "Экипаж" : (string)AccessTools.Method("MiningOutpost.Orbital.Services.OpportunityRunner:ColoredPossessive").Invoke(null, new object[] { pawn });
        public static string SubOne(string f, object a) => Russian ? WorksiteCheckText.SubOne(f, a) : string.Format(f, a);
        public static string SubThree(string f, object a, object b, object c) => Russian ? WorksiteCheckText.SubThree(f, a, b, c) : string.Format(f, a, b, c);
        public static string Formula(string a, string b, string c, string d) => Russian ? WorksiteCheckText.Formula(a, b, c, d) : string.Concat(a, b, c, d);
        public static string Math(string f, object a, object b, object c) => Russian ? WorksiteCheckText.Math(f, a, b, c) : string.Format(f, a, b, c);
        public static string TipFactor(string f, object a, object b, object c) => Russian ? WorksiteCheckText.TipFactor(f, a, b, c) : string.Format(f, a, b, c);
        public static string TipOdds(string f, object a, object b) => Russian ? WorksiteCheckText.TipOdds(f, a, b) : string.Format(f, a, b);
        public static string SkillFactor(string f, object a, object b, object c) => Russian ? WorksiteCheckText.SkillFactor(f, a, b, c) : string.Format(f, a, b, c);
        public static string Explain(string f, object a, object b) => Russian ? WorksiteCheckText.Explain(f, a, b, Skill) : string.Format(f, a, b);
        public static string SkillTipTwo(string f, object a, object b) => Russian ? WorksiteCheckText.SkillTipTwo(f, a, b, Skill) : string.Format(f, a, b);
        public static string SkillTipThree(string f, object a, object b, object c) => Russian ? WorksiteCheckText.SkillTipThree(f, a, b, c, Skill) : string.Format(f, a, b, c);
        public static string Resolve(string f, object[] a) => Russian ? WorksiteCheckText.Resolve(f, a, Skill) : string.Format(f, a);
        public static string Refusal(string a, string b) => Russian ? WorksiteCheckText.Refusal(a, b) : string.Concat(a, b);
        public static string Required(string a, string b, string c) => Russian ? WorksiteCheckText.Required(a, b, c) : string.Concat(a, b, c);
        public static string ShortStat(string value) => Russian ? WorksiteCheckText.Stat(value, true) : value;

        private static readonly Dictionary<string, Dictionary<string, Tuple<string, int>>> Bindings = new Dictionary<string, Dictionary<string, Tuple<string, int>>>
        {
            {"MiningOutpost.Orbital.Services.OpportunityRunner:CheckSub", new Dictionary<string, Tuple<string, int>> {
                {"Format(System.String,System.Object)", Tuple.Create("SubOne", 1)}, {"Format(System.String,System.Object,System.Object,System.Object)", Tuple.Create("SubThree", 1)}}},
            {"MiningOutpost.Orbital.Services.OpportunityRunner:FullLine", new Dictionary<string, Tuple<string, int>> {
                {"Concat(System.String,System.String,System.String,System.String)", Tuple.Create("Formula", 1)}}},
            {"MiningOutpost.Orbital.Services.OpportunityRunner:CheckMath", new Dictionary<string, Tuple<string, int>> {
                {"Format(System.String,System.Object,System.Object,System.Object)", Tuple.Create("Math", 1)}}},
            {"MiningOutpost.Orbital.Services.OpportunityRunner:BuildCheckTip", new Dictionary<string, Tuple<string, int>> {
                {"Format(System.String,System.Object,System.Object,System.Object)", Tuple.Create("TipFactor", 2)}, {"Format(System.String,System.Object,System.Object)", Tuple.Create("TipOdds", 1)}}},
            {"MiningOutpost.Orbital.Services.StatLab:SkillFactorLabel", new Dictionary<string, Tuple<string, int>> {
                {"Format(System.String,System.Object,System.Object,System.Object)", Tuple.Create("SkillFactor", 1)}}},
            {"MiningOutpost.Orbital.Services.StatLab:Explain", new Dictionary<string, Tuple<string, int>> {
                {"Format(System.String,System.Object,System.Object)", Tuple.Create("Explain", 1)}}},
            {"MiningOutpost.Orbital.Services.SquadSheetUI:SkillTip", new Dictionary<string, Tuple<string, int>> {
                {"Format(System.String,System.Object,System.Object)", Tuple.Create("SkillTipTwo", 2)}, {"Format(System.String,System.Object,System.Object,System.Object)", Tuple.Create("SkillTipThree", 2)}}},
            {"MiningOutpost.Orbital.Services.OpportunityRunner:DrawResolvePanel", new Dictionary<string, Tuple<string, int>> {
                {"Format(System.String,System.Object[])", Tuple.Create("Resolve", 2)}}},
            {"MiningOutpost.Orbital.Services.OpportunityRunner:DrawOptions", new Dictionary<string, Tuple<string, int>> {
                {"Concat(System.String,System.String)", Tuple.Create("Refusal", 2)}}},
            {"MiningOutpost.Orbital.Services.OpportunityRunner:DrawStandaloneRoll", new Dictionary<string, Tuple<string, int>> {
                {"Concat(System.String,System.String)", Tuple.Create("Refusal", 1)}}},
            {"MiningOutpost.Orbital.Services.OpportunityRunner:CheckRequirements", new Dictionary<string, Tuple<string, int>> {
                {"Concat(System.String,System.String)", Tuple.Create("Refusal", 1)}, {"Concat(System.String,System.String,System.String)", Tuple.Create("Required", 1)}}}
        };

        public static void Install(Harmony harmony)
        {
            foreach (string method in Bindings.Keys) harmony.Patch(AccessTools.Method(method), transpiler: new HarmonyMethod(typeof(WorksiteCheckPatch), nameof(Rewrite)));
            harmony.Patch(AccessTools.Method("MiningOutpost.Orbital.Services.SquadSheetUI:DrawSheet"), transpiler: new HarmonyMethod(typeof(WorksiteCheckPatch), nameof(SheetLabels)));
            foreach (string name in new[] { "CheckName", "StatName" }) harmony.Patch(AccessTools.Method("MiningOutpost.Orbital.Services.OpportunityRunner:" + name), postfix: new HarmonyMethod(typeof(WorksiteCheckPatch), name));
            harmony.Patch(AccessTools.Method("MiningOutpost.Orbital.Services.SquadSheetUI:StatFull"), postfix: new HarmonyMethod(typeof(WorksiteCheckPatch), nameof(StatFull)));
        }

        public static IEnumerable<CodeInstruction> Rewrite(IEnumerable<CodeInstruction> input, MethodBase __originalMethod)
        {
            string key = __originalMethod.DeclaringType.FullName + ":" + __originalMethod.Name;
            var expected = Bindings[key];
            var seen = new Dictionary<string, int>();
            int captions = 0, firefight = 0;
            var output = new List<CodeInstruction>();
            foreach (var instruction in input)
            {
                var code = new CodeInstruction(instruction);
                if (code.operand is MethodInfo method)
                {
                    if (method.DeclaringType == typeof(string))
                    {
                        string sig = method.Name + "(" + string.Join(",", method.GetParameters().Select(p => p.ParameterType.FullName)) + ")";
                        if (expected.TryGetValue(sig, out var spec))
                        {
                            code.operand = AccessTools.Method(typeof(WorksiteCheckPatch), spec.Item1);
                            seen[sig] = seen.TryGetValue(sig, out int count) ? count + 1 : 1;
                        }
                    }
                    else if (__originalMethod.Name == "BuildCheckTip" && method.Name == "ColoredPossessive" && method.DeclaringType == __originalMethod.DeclaringType)
                    {
                        code.operand = AccessTools.Method(typeof(WorksiteCheckPatch), nameof(PawnCaption));
                        captions++;
                    }
                }
                output.Add(code);
                if (__originalMethod.Name == "FullLine" && code.opcode == OpCodes.Ldstr && Equals(code.operand, "1d20 + REF + Combat"))
                {
                    output.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(WorksiteCheckPatch), nameof(FirefightFormula))));
                    firefight++;
                }
            }
            if (expected.Any(p => !seen.TryGetValue(p.Key, out int n) || n != p.Value.Item2) || (__originalMethod.Name == "BuildCheckTip" && captions != 1) || (__originalMethod.Name == "FullLine" && firefight != 1))
                throw new InvalidOperationException("Worksites check formatter source changed: " + key);
            return output;
        }

        public static string FirefightFormula(string original) => Russian ? "1d20 + РЕА + Бой" : original;

        public static IEnumerable<CodeInstruction> SheetLabels(IEnumerable<CodeInstruction> input)
        {
            var source = input.ToList();
            var output = new List<CodeInstruction>();
            int stats = 0, checks = 0;
            for (int i = 0; i < source.Count; i++)
            {
                var code = new CodeInstruction(source[i]);
                if (i > 0 && code.Calls(AccessTools.Method(typeof(Widgets), nameof(Widgets.Label), new[] { typeof(UnityEngine.Rect), typeof(string) })))
                {
                    string render = null;
                    if (source[i - 1].opcode == OpCodes.Ldelem_Ref) { render = nameof(ShortStat); stats++; }
                    else if (source[i - 1].opcode == OpCodes.Ldfld && source[i - 1].operand is FieldInfo field && field.Name == "name" && field.DeclaringType.FullName == "MiningOutpost.Orbital.Services.SquadSheetUI+Check") { render = nameof(DisplayName); checks++; }
                    if (render != null)
                    {
                        var format = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(WorksiteCheckPatch), render));
                        format.MoveLabelsFrom(code); format.MoveBlocksFrom(code);
                        output.Add(format);
                    }
                }
                output.Add(code);
            }
            if (stats != 1 || checks != 1) throw new InvalidOperationException("Worksites squad sheet label source changed");
            return output;
        }
    }
}
