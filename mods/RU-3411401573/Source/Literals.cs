using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace Igor.RU_3411401573
{
    public sealed class LiteralTranslations : Mod
    {
        private static readonly Dictionary<string,Dictionary<string,string>> Methods =
            new Dictionary<string,Dictionary<string,string>> {
            { "VanillaQuestsExpandedTheGenerator.Building_Genetron+<GetGizmos>d__16:MoveNext", new Dictionary<string,string> {
                { "Set Running Time to 100 days", "Установить время работы: 100 дней" },
                { "Set fuel burned to 10000", "Установить расход топлива: 10 000" },
                { "Set geothermal studied to TRUE", "Отметить геотермальную АРК как изученную" },
                { "Set nuclear studied to TRUE", "Отметить ядерную АРК как изученную" }
            } },
            { "VanillaQuestsExpandedTheGenerator.Building_GenetronNuclear+<GetGizmos>d__9:MoveNext", new Dictionary<string,string> {
                { "Fake a restart", "Имитировать перезапуск" },
                { "Set uranium used to 10000", "Установить расход урана: 10 000" }
            } },
            { "VanillaQuestsExpandedTheGenerator.Building_GenetronOverdrive+<GetGizmos>d__32:MoveNext", new Dictionary<string,string> {
                { "Nuclear meltdown", "Расплавить активную зону реактора" },
                { "Reset overdrive cooldown", "Сбросить перерыв между форсажами" },
                { "Set overdrive successful for 5 days", "Засчитать 5 дней успешной работы на форсаже" },
                { "Stop overdrive", "Остановить форсаж" }
            } },
            { "VanillaQuestsExpandedTheGenerator.Building_GenetronTuning+<GetGizmos>d__0:MoveNext", new Dictionary<string,string> {
                { "Set time on 200% to 100 days", "Установить время работы на 200% мощности: 100 дней" }
            } },
            { "VanillaQuestsExpandedTheGenerator.Building_GenetronWithHazardModes+<GetGizmos>d__5:MoveNext", new Dictionary<string,string> {
                { "Set time in hazard mode to 100 days", "Установить время работы в опасном режиме: 100 дней" }
            } },
            { "VanillaQuestsExpandedTheGenerator.Building_GenetronWithMaintenance+<GetGizmos>d__11:MoveNext", new Dictionary<string,string> {
                { "Set maintenance to 10%", "Установить состояние АРК: 10%" }
            } },
            { "VanillaQuestsExpandedTheGenerator.Building_GenetronWithPowerSurge+<GetGizmos>d__9:MoveNext", new Dictionary<string,string> {
                { "Reset power surge cooldown", "Сбросить перерыв между энергетическими импульсами" },
                { "Set power surge usage to 10", "Установить число энергетических импульсов: 10" }
            } },
            { "VanillaQuestsExpandedTheGenerator.Building_GenetronWithSteamBoost+<GetGizmos>d__12:MoveNext", new Dictionary<string,string> {
                { "Reset steam boost cooldown", "Сбросить перерыв между паровыми форсажами" },
                { "Set steam boost usage to 10", "Установить число паровых форсажей: 10" }
            } },
            { "VanillaQuestsExpandedTheGenerator.HediffComp_Traitor+<CompGetGizmos>d__12:MoveNext", new Dictionary<string,string> {
                { "DEBUG: Traitor appears", "ОТЛ: раскрыть предателя" }
            } },
            { "VanillaQuestsExpandedTheGenerator.VanillaQuestsExpandedTheGenerator_Mod:SettingsCategory", new Dictionary<string,string> {
                { "VQE - The Generator", "VQE — Генератор" }
            } }
            };
        private static readonly Dictionary<string,string[]> Parameters = new Dictionary<string,string[]> {

        };
        public LiteralTranslations(ModContentPack content) : base(content)
        {
            var harmony = new Harmony("igor.ru.3411401573.literals");
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
}
