using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace Igor.RU_3524716849
{
    public sealed class LiteralTranslations : Mod
    {
        private static readonly Dictionary<string,Dictionary<string,string>> Methods =
            new Dictionary<string,Dictionary<string,string>> {
            { "KeyzAllowUtilities.DebugActions:ClearItemsArea", new Dictionary<string,string> {
                { "Clear Items", "Удалить предметы" }
            } },
            { "KeyzAllowUtilities.DebugActions:ClearPlantsArea", new Dictionary<string,string> {
                { "Clear Plants", "Удалить растения" }
            } },
            { "KeyzAllowUtilities.DebugActions:GrowPlantsToMaturity", new Dictionary<string,string> {
                { "Grow", "Дорастить до зрелости" }
            } },
            { "KeyzAllowUtilities.DebugActions:ReplantArea", new Dictionary<string,string> {
                { "Replant", "Заново засадить" }
            } },
            { "KeyzAllowUtilities.Designator_FinishOff:CanDesignateCell", new Dictionary<string,string> {
                { "No Selectables", "Нет целей для добивания" }
            } },
            { "KeyzAllowUtilities.Designator_SelectSimilar:CanDesignateCell", new Dictionary<string,string> {
                { "No Selectables", "Нет объектов для выделения" }
            } },
            { "KeyzAllowUtilities.Designator_StripMine:<DoExtraGuiControls>b__15_0", new Dictionary<string,string> {
                { "<size=10%>Horiz <color=blue>Offset</color></size>", "<size=10%>Гориз. <color=blue>сдвиг</color></size>" },
                { "<size=10%>Horiz <color=red>Spacing</color></size>", "<size=10%>Гориз. <color=red>зазор</color></size>" },
                { "<size=10%>Vert <color=blue>Offset</color></size>", "<size=10%>Верт. <color=blue>сдвиг</color></size>" },
                { "<size=10%>Vert <color=red>Spacing</color></size>", "<size=10%>Верт. <color=red>зазор</color></size>" }
            } },
            { "KeyzAllowUtilities.Settings:DoWindowContents", new Dictionary<string,string> {
                { "KAU_DisableSelectRotting", "chupakobra6_3524716849_DisableSelectRotting" },
                { "Version: ", "Версия: " }
            } },
            { "KeyzAllowUtilities.WorkGiver_FinishOff:JobOnThing", new Dictionary<string,string> {
                { "Melee too low", "Слишком низкий навык ближнего боя" }
            } }
            };
        private static readonly Dictionary<string,string[]> Parameters = new Dictionary<string,string[]> {
            { "KeyzAllowUtilities.DebugActions:ClearItemsArea", new string[] {  } },
            { "KeyzAllowUtilities.DebugActions:ClearPlantsArea", new string[] {  } },
            { "KeyzAllowUtilities.DebugActions:GrowPlantsToMaturity", new string[] {  } },
            { "KeyzAllowUtilities.DebugActions:ReplantArea", new string[] {  } },
            { "KeyzAllowUtilities.Designator_FinishOff:CanDesignateCell", new string[] { "Verse.IntVec3" } },
            { "KeyzAllowUtilities.Designator_SelectSimilar:CanDesignateCell", new string[] { "Verse.IntVec3" } },
            { "KeyzAllowUtilities.Designator_StripMine:<DoExtraGuiControls>b__15_0", new string[] {  } },
            { "KeyzAllowUtilities.Settings:DoWindowContents", new string[] { "UnityEngine.Rect" } },
            { "KeyzAllowUtilities.WorkGiver_FinishOff:JobOnThing", new string[] { "Verse.Pawn", "Verse.Thing", "System.Boolean" } }
        };
        public LiteralTranslations(ModContentPack content) : base(content) { }
        public static void Apply()
        {
            var harmony = new Harmony("igor.ru.3524716849.literals");
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
