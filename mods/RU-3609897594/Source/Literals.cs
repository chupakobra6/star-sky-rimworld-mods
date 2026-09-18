using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace Igor.RU_3609897594
{
    public sealed class LiteralTranslations : Mod
    {
        private static readonly Dictionary<string,Dictionary<string,string>> Methods =
            new Dictionary<string,Dictionary<string,string>> {
            { "NiceInventoryTab.ApparelSlotUtility:OpenFloatMenu", new Dictionary<string,string> {
                { "DEV create", "ОТЛ: создать" }
            } },
            { "NiceInventoryTab.Dialog_OptimizeEquipment:DoWindowContents", new Dictionary<string,string> {
                { "EquipmentMaster", "chupakobra6_3609897594_EquipmentMaster" },
                { " WORK IN PROGRESS", " — В РАЗРАБОТКЕ" }
            } },
            { "NiceInventoryTab.Dialog_OptimizeEquipment:StepTwo", new Dictionary<string,string> {
                { "Back", "Назад" },
                { "Wear", "Надеть" }
            } },
            { "NiceInventoryTab.Dialog_OptimizeEquipment:DrawFinalApparelList", new Dictionary<string,string> {
                { "Final apparel", "Подобранная одежда" }
            } },
            { "NiceInventoryTab.Dialog_OptimizeEquipment:StepOne", new Dictionary<string,string> {
                { "Back", "Назад" },
                { "Next", "Далее" },
                { "Can take off apparel", "Разрешить снимать надетую одежду" }
            } },
            { "NiceInventoryTab.Dialog_OptimizeEquipment:DrawStatList", new Dictionary<string,string> {
                { "Stat to maximize", "Улучшаемые показатели" },
                { "Stats", "Показатели" }
            } },
            { "NiceInventoryTab.Dialog_OptimizeEquipment:DrawSlotsList", new Dictionary<string,string> {
                { "Apparel layers", "Слои одежды" }
            } },
            { "NiceInventoryTab.Dialog_OptimizeEquipment:DrawSlot", new Dictionary<string,string> {
                { "Apparel for layer:", "Одежда для слоя:" }
            } },
            { "NiceInventoryTab.EquippedItem:Draw", new Dictionary<string,string> {
                { " layers: ", " слои: " },
                { "  |  groups: ", "  |  части тела: " }
            } },
            { "NiceInventoryTab.Settings:DrawQualityColorsSettings", new Dictionary<string,string> {
                { "Glow ", "Свечение: " }
            } },
            { "NiceInventoryTab.NiceInventoryTabMod:SettingsCategory", new Dictionary<string,string> {
                { "Nice Inventory Tab", "Nice Inventory Tab — снаряжение" }
            } },
            { "MoreWidgets.Patch_ShowCoords:AddSetting", new Dictionary<string,string> {
                { "View", "Отображение" }
            } },
            { "MoreWidgets.Patch_ShowCoords:InitActions_Post", new Dictionary<string,string> {
                { "Map coords in UI", "Координаты карты в интерфейсе" },
                { "Map coords at pointer", "Координаты карты у курсора" }
            } }
            };
        private static readonly Dictionary<string,string[]> Parameters = new Dictionary<string,string[]> {
            { "MoreWidgets.Patch_ShowCoords:InitActions_Post", new string[] { "LudeonTK.DebugActionNode" } },
            { "NiceInventoryTab.ApparelSlotUtility:OpenFloatMenu", new string[] { "System.Collections.Generic.List`1[[NiceInventoryTab.ApparelSlotUtility+PotentialSlot, NiceInventoryTab]], mscorlib", "System.Collections.Generic.List`1[[Verse.ApparelLayerDef, Assembly-CSharp]], mscorlib", "System.Boolean", "System.Collections.Generic.List`1[[Verse.FloatMenuOption, Assembly-CSharp]], mscorlib" } }
        };
        public LiteralTranslations(ModContentPack content) : base(content) { }
        public static void Apply()
        {
            var harmony = new Harmony("igor.ru.3609897594.literals");
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
