using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using LudeonTK;
using Verse;

namespace Igor.RU_3524716849
{
    // Attribute text is metadata, not an ldstr. Only the reviewed method/name
    // pairs are changed, and only a UI copy of the attribute is modified.
    public sealed class DebugActionNames : Mod
    {
        private static readonly Dictionary<string, Dictionary<string, string>> Names =
            new Dictionary<string, Dictionary<string, string>> {
            { "KeyzAllowUtilities.DebugActions:ClearItemsArea", new Dictionary<string, string> {
                { "Clear items in area (rect)", "Удалить предметы в области (прямоугольник)" }
            } },
            { "KeyzAllowUtilities.DebugActions:ClearPlantsArea", new Dictionary<string, string> {
                { "Clear plants in area (rect)", "Удалить растения в области (прямоугольник)" }
            } },
            { "KeyzAllowUtilities.DebugActions:GrowPlantsOfType", new Dictionary<string, string> {
                { "Grow plants of type (rect)", "Вырастить растения выбранного вида (прямоугольник)" }
            } },
            { "KeyzAllowUtilities.DebugActions:GrowPlantsToMaturity", new Dictionary<string, string> {
                { "Grow plants to maturity in area (rect)", "Дорастить растения в области до зрелости (прямоугольник)" }
            } },
            { "KeyzAllowUtilities.DebugActions:ReplantArea", new Dictionary<string, string> {
                { "Replant area (rect)", "Заново засадить область (прямоугольник)" }
            } }
            };
        public DebugActionNames(ModContentPack content) : base(content)
        {
            new Harmony("igor.ru.3524716849.debugnames").Patch(
                AccessTools.Method(typeof(DebugTabMenu_Actions), "GenerateCacheForMethod"),
                prefix: new HarmonyMethod(typeof(DebugActionNames), nameof(Translate)));
        }
        public static void Translate(MethodInfo __0, ref DebugActionAttribute __1)
        {
            if (__0?.DeclaringType == null || __1 == null
                || LanguageDatabase.activeLanguage?.folderName.StartsWith("Russian", StringComparison.OrdinalIgnoreCase) != true
                || !Names.TryGetValue(__0.DeclaringType.FullName + ":" + __0.Name, out var pairs)
                || __1.name == null || !pairs.TryGetValue(__1.name, out var russian)) return;
            __1 = (DebugActionAttribute)AccessTools.Method(typeof(object), "MemberwiseClone").Invoke(__1, null);
            __1.name = russian;
        }
    }
}
