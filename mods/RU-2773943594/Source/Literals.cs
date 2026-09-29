using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace Igor.RU_2773943594
{
    public sealed class LiteralTranslations : Mod
    {
        private static readonly Dictionary<string,Dictionary<string,string>> Methods =
            new Dictionary<string,Dictionary<string,string>> {
            { "GeologicalLandforms.ExtensionUtils:ContentSourceLabel", new Dictionary<string,string> {
                { "unknown", "неизвестный источник" },
                { "vanilla", "основная игра" },
                { " DLC", " — дополнение" }
            } },
            { "GeologicalLandforms.GraphEditor.EditorMockTileInfo:DoEditorGUI", new Dictionary<string,string> {
                { "Simulated tile properties", "Параметры условного участка" },
                { "Biome", "Биом" },
                { "Hilliness", "Рельеф" },
                { "Direction", "Направление" }
            } },
            { "GeologicalLandforms.GraphEditor.LandformGraphEditor:DoWindowContents", new Dictionary<string,string> {
                { "Node Editor Initiation failed! Check console for more information!", "Не удалось запустить редактор узлов. Подробности — в журнале игры." }
            } },
            { "GeologicalLandforms.GraphEditor.LandformGraphInterface:DrawToolbarGUI", new Dictionary<string,string> {
                { "World tile {0} ({1})", "Участок мира {0} ({1})" },
                { "Simulated tile ({0})", "Условный участок ({0})" },
                { "Selected world tile ({0})", "Выбранный участок мира ({0})" },
                { "World tile of map {0} ({1})", "Участок карты {0} ({1})" },
                { "Simulated tile (custom)", "Условный участок (свои параметры)" }
            } },
            { "GeologicalLandforms.GraphEditor.NodeTerrainGridPreview:MakeTooltip", new Dictionary<string,string> {
                { "Natural rock", "Горная порода" }
            } },
            { "GeologicalLandforms.GraphEditor.NodeUIMapIncidents:DoWindowContents", new Dictionary<string,string> {
                { "Add incident entry", "Добавить происшествие" },
                { "Add game condition entry", "Добавить состояние игры" },
                { "Add raid strategy entry", "Добавить стратегию налёта" },
                { "Add arrival mode entry", "Добавить способ прибытия" }
            } },
            { "GeologicalLandforms.GraphEditor.NodeUIWorldTileReq+BiomeCondition+<>c__DisplayClass5_0:<EditorGUI>b__0", new Dictionary<string,string> {
                { "Any unspecified biome", "Любой неуказанный биом" },
                { "Add entry", "Добавить запись" }
            } },
            { "GeologicalLandforms.GraphEditor.NodeUIWorldTileReq+WorldObjectNearbyCondition+<>c__DisplayClass6_0:<EditorGUI>b__0", new Dictionary<string,string> {
                { "Add entry", "Добавить запись" }
            } },
            { "GeologicalLandforms.GraphEditor.Landform:get_TranslatedName", new Dictionary<string,string> {
                { "Unknown", "Неизвестная форма" }
            } },
            { "NodeEditorFramework.Standard.NodeEditorInterface:DrawToolbarGUI", new Dictionary<string,string> {
                { "Save Type: ", "Способ хранения: " },
                { "\nSave Path: ", "\nПуть сохранения: " },
                { "Canvas Type: ", "Тип схемы: " }
            } },
            { "NodeEditorFramework.IO.ImportExportFormat:ImportLocationArgsGUI", new Dictionary<string,string> {
                { "Import canvas from ", "Импорт схемы из " }
            } },
            { "NodeEditorFramework.IO.ImportExportFormat:ExportLocationArgsGUI", new Dictionary<string,string> {
                { "Export canvas to ", "Экспорт схемы в " }
            } },
            { "NodeEditorFramework.IO.ImportExportManager:FillImportFormatMenu", new Dictionary<string,string> {
                { "No IO Formats found", "Форматы ввода и вывода не найдены" }
            } },
            { "NodeEditorFramework.IO.ImportExportManager:FillExportFormatMenu", new Dictionary<string,string> {
                { "No IO Formats found", "Форматы ввода и вывода не найдены" }
            } },
            { "GeologicalLandforms.DebugActions:BiomeEntryDetailsGUI", new Dictionary<string,string> {
                { " (P)", " (загрязнение)" }
            } },
            { "GeologicalLandforms.GeologicalLandformsMod:SettingsCategory", new Dictionary<string,string> {
                { "Geological Landforms", "Geological Landforms — формы рельефа" }
            } },
            { "GeologicalLandforms.TerrainTabUI:FindLandform", new Dictionary<string,string> {
                { "matching", "подходящая под условия поиска" }
            } }
            };
        private static readonly Dictionary<string,string[]> Parameters = new Dictionary<string,string[]> {

        };
        public LiteralTranslations(ModContentPack content) : base(content) { }
        public static void Apply()
        {
            var harmony = new Harmony("igor.ru.2773943594.literals");
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
