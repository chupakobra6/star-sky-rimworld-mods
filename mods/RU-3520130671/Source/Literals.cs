using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace Igor.RU_3520130671
{
    public sealed class LiteralTranslations : Mod
    {
        private static readonly Dictionary<string,Dictionary<string,string>> Methods =
            new Dictionary<string,Dictionary<string,string>> {
            { "NiceBillTab.BillValidator:CanExecuteBill", new Dictionary<string,string> {
                { "Invalid bill or pawn", "Некорректные данные задания или персонажа" },
                { "Cannot work while drafted", "В режиме призыва работать нельзя" },
                { "No work type defined for recipe", "Для рецепта не указан вид работы" },
                { "Work type ", "Недоступный вид работы: " },
                { " is disabled", "" },
                { "Not assigned to work type ", "Не назначена работа: " },
                { "Required skill ", "Недоступный навык: " },
                { "Work tags disabled", "Персонаж не способен выполнять требуемую работу" },
                { "No work table available", "Нет доступного рабочего стола" },
                { "Work table ", "Запрещено использовать: " },
                { " is forbidden", "" },
                { "Cannot reach work table", "Нет пути к рабочему столу" },
                { "Work table is not usable", "Рабочим столом нельзя пользоваться" },
                { "Insufficient {0} skill (requires {1})", "Недостаточный навык «{0}» (нужен уровень {1})" },
                { "Missing ingredients: ", "Не хватает ингредиентов: " },
                { "Bill is suspended", "Задание приостановлено" },
                { "Bill repeat count is zero", "Число повторений задания равно нулю" },
                { "Target count already reached", "Нужное количество уже изготовлено" }
            } },
            { "NiceBillTab.ITab_Bills_FillTab_Patch:Prefix", new Dictionary<string,string> {
                { "DEV", "ОТЛ" },
                { "Bill[{0}] {1} : {2} {3}", "Задание[{0}] {1} : {2} {3}" },
                { "Recipe[{0}] {1} : {2} {3} : {4} -> {5}", "Рецепт[{0}] {1} : {2} {3} : {4} -> {5}" }
            } },
            { "NiceBillTab.NiceBillTabMod:SettingsCategory", new Dictionary<string,string> {
                { "Nice Bill Tab", "Nice Bill Tab — задания" }
            } },
            { "NiceBillTab.Settings:DoWindowContents", new Dictionary<string,string> {
                { "Small value", "Малое значение" },
                { "Medium value", "Среднее значение" },
                { "Big value", "Большое значение" }
            } },
            { "NiceBillTab.StatCache:CacheForInternal", new Dictionary<string,string> {
                { "kg", "кг" }
            } },
            { "NiceBillTab.StatRecordAccuracy:Draw", new Dictionary<string,string> {
                { "0..3 m", "0–3 м" },
                { "4..12 m", "4–12 м" },
                { "13..25 m", "13–25 м" },
                { "25+ m", "25+ м" }
            } }
            };
        private static readonly Dictionary<string,string[]> Parameters = new Dictionary<string,string[]> {

        };
        public LiteralTranslations(ModContentPack content) : base(content) { }
        public static void Apply()
        {
            var harmony = new Harmony("igor.ru.3520130671.literals");
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
