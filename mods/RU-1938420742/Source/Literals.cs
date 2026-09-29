using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace Igor.RU_1938420742
{
    public sealed class LiteralTranslations : Mod
    {
        private static readonly Dictionary<string,Dictionary<string,string>> Methods =
            new Dictionary<string,Dictionary<string,string>> {
            { "VEE.GameCondition_MultiStage:get_TooltipString", new Dictionary<string,string> {
                { "DEV: Time to next stage: ", "ОТЛАДКА: до следующей стадии: " }
            } },
            { "VEE.HediffComp_Traitor:CompDebugString", new Dictionary<string,string> {
                { "ticksToDisappear: ", "Тиков до раскрытия: " }
            } },
            { "VEE.HediffComp_Traitor+<CompGetGizmos>d__10:MoveNext", new Dictionary<string,string> {
                { "DEBUG: Now", "ОТЛАДКА: раскрыть предателя" }
            } },
            { "VEE.Settings.VEEMod:SettingsCategory", new Dictionary<string,string> {
                { "Vanilla Events Expanded", "Vanilla Events Expanded — события" }
            } },
            { "VEE.Settings.VEESettings:DoSettingsWindowContents", new Dictionary<string,string> {
                { "Base chance: {0}", "Базовая вероятность: {0}" }
            } },
            { "VEE.Settings.VEESettings:ResetWorldCondButton", new Dictionary<string,string> {
                { "Nothing to end", "Нет действующих событий" }
            } },
            { "VEE.Settings.VEESettings:ResetMapCondButton", new Dictionary<string,string> {
                { "Nothing to end", "Нет действующих событий" }
            } }
            };
        private static readonly Dictionary<string,string[]> Parameters = new Dictionary<string,string[]> {

        };
        public LiteralTranslations(ModContentPack content) : base(content)
        {
            var harmony = new Harmony("igor.ru.1938420742.literals");
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
}
