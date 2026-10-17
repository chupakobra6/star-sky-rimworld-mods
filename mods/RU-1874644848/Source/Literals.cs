using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace Igor.RU_1874644848
{
    public sealed class LiteralTranslations : Mod
    {
        private static readonly Dictionary<string,Dictionary<string,string>> Methods =
            new Dictionary<string,Dictionary<string,string>> {
            { "CharacterEditor.ApparelTool:AskToWearIncompatibleApparel", new Dictionary<string,string> {
                { "core or royalty apparel that is not compatible to the current body type found - try to wear it anyway?\n\nif you do, you will get texture error messages on savegame load. but they should be harmless.", "Найдена одежда из основной игры или Royalty, несовместимая с текущим телосложением. Всё равно надеть её?\n\nПри загрузке сохранения появятся ошибки текстур; они, как правило, не мешают игре." }
            } },
            { "CharacterEditor.CEditor+EditorUI+BlockBio:AChangeIdeo", new Dictionary<string,string> {
                { "Do not click on back button or you will need to restart your game. This is not a bug from the Editor, but because of the fluid ideo.", "Не нажимайте кнопку «Назад»: иначе потребуется перезапустить игру. Это ограничение связано с изменяемой идеологией, а не с редактором." }
            } },
            { "CharacterEditor.CEditor+EditorUI+BlockHealth:DrawHediffListing", new Dictionary<string,string> {
                { "health conditions are malformed -> consider to use 'full heal'", "Данные состояний здоровья повреждены. Попробуйте команду «Полное исцеление»." }
            } },
            { "CharacterEditor.CEditor+EditorUI+BlockNeeds:LabelHelper", new Dictionary<string,string> {
                { "\nBaseMood: ", "\nБазовое настроение: " },
                { " BaseOpinion: ", " Базовое мнение: " },
                { "Stage ", "Стадия " }
            } },
            { "CharacterEditor.CEditor+EditorUI+BlockPawnList:.ctor", new Dictionary<string,string> {
                { "n.a.", "н/д" }
            } },
            { "CharacterEditor.CEditor+EditorUI+BlockPawnList:DrawListCount", new Dictionary<string,string> {
                { "max", "макс." }
            } },
            { "CharacterEditor.CEditor+EditorUI+BlockPerson+<>c__DisplayClass65_0:<ASavePawn>b__0", new Dictionary<string,string> {
                { "Data: ", "Данные: " }
            } },
            { "CharacterEditor.CEditor+ModOptions+DialogConfigurate:AExportSlots", new Dictionary<string,string> {
                { "export successful to ", "Экспорт завершён: " }
            } },
            { "CharacterEditor.CEditor+ModOptions+DialogConfigurate:AImportSlots", new Dictionary<string,string> {
                { "import successful from ", "Импорт завершён: " }
            } },
            { "CharacterEditor.DialogAddHediff:CheckIsReady", new Dictionary<string,string> {
                { "(Genitor)", "(родитель)" }
            } },
            { "CharacterEditor.DialogAddHediff:DoWindowContents", new Dictionary<string,string> {
                { "toggle value overriding. white=inactive, green=active\nclick on severity value to input overrides", "Включить или выключить замену значений: белый — выключена, зелёный — включена\nНажмите на значение тяжести состояния, чтобы задать замену" }
            } },
            { "CharacterEditor.DialogAddThought:DrawSlider", new Dictionary<string,string> {
                { "mood", "Настроение" },
                { "mood [", "Настроение [" },
                { "opinion", "Мнение" },
                { "opinion [", "Мнение [" }
            } },
            { "CharacterEditor.DialogAddThought:GetTooltipForThought", new Dictionary<string,string> {
                { "\nStage ", "\nСтадия " },
                { "\nrequires hediff: ", "\nТребуется состояние здоровья: " },
                { "\nrequires love relation", "\nТребуются романтические отношения" },
                { "\nrequires trait: ", "\nТребуется черта характера: " },
                { "\nrequires weapon", "\nТребуется оружие" },
                { " BaseMood: ", " Базовое настроение: " },
                { " BaseOpinion: ", " Базовое мнение: " }
            } },
            { "CharacterEditor.DialogCapsuleUI:AMoveToInterstellar", new Dictionary<string,string> {
                { "try to rescan. could not find ", "Повторите сканирование. Не найдено: " }
            } },
            { "CharacterEditor.DialogCapsuleUI:DrawUIObjects", new Dictionary<string,string> {
                { "Scan Map", "Сканировать карту" }
            } },
            { "CharacterEditor.DialogChangeMutant:DrawLowerButtons", new Dictionary<string,string> {
                { "Turn to human", "Превратить в человека" },
                { "Turn to mutant", "Превратить в мутанта" }
            } },
            { "CharacterEditor.DialogChangeMutant:DrawParameters", new Dictionary<string,string> {
                { "MutantDef: ", "Тип мутанта: " },
                { "RotStage: ", "Стадия разложения: " }
            } },
            { "CharacterEditor.DialogChangeMutant:Mutantlabel", new Dictionary<string,string> {
                { "Mutations: ", "Мутации: " }
            } },
            { "CharacterEditor.DialogColorPicker:DrawGeneColors", new Dictionary<string,string> {
                { "add as new gene", "Добавить как новый ген" },
                { "select color from gene\n[CTRL]remove gene", "Выбрать цвет из гена\n[CTRL] — удалить ген" }
            } },
            { "CharacterEditor.DialogGenery:DrawBools", new Dictionary<string,string> {
                { "showGizmoOnMultiSelect", "Показывать команду при выборе нескольких пешек" },
                { "showGizmoOnWorldView", "Показывать команду на карте мира" },
                { "showGizmoWhenDrafted", "Показывать команду в режиме призыва" }
            } },
            { "CharacterEditor.DialogObjects:DrawLowerButtons", new Dictionary<string,string> {
                { "rotation", "Поворот: " }
            } },
            { "CharacterEditor.DialogObjects:OnAccept", new Dictionary<string,string> {
                { "not available in starting screen", "Недоступно на экране подготовки к игре" }
            } },
            { "CharacterEditor.DialogPsychology:DoWindowContents", new Dictionary<string,string> {
                { "changes will be applied after some passed time", "Изменения вступят в силу через некоторое время" }
            } },
            { "CharacterEditor.DialogXenoType:ALoadCustomXenotype", new Dictionary<string,string> {
                { "loading custom xenotype ", "Загрузка пользовательского ксенотипа: " }
            } },
            { "CharacterEditor.DialogXenoType:ALoadXenotypeDef", new Dictionary<string,string> {
                { "loading xenotypeDef ", "Загрузка стандартного ксенотипа: " }
            } },
            { "CharacterEditor.DialogXenoType:AcceptInner", new Dictionary<string,string> {
                { "please choose a xenotype name!", "Укажите название ксенотипа!" }
            } },
            { "CharacterEditor.DialogXenoType:OnGenesChanged", new Dictionary<string,string> {
                { "predefined unloaded", "Выбранный стандартный ксенотип сброшен" }
            } },
            { "CharacterEditor.FLabel:GetFormattedValue", new Dictionary<string,string> {
                { " cells]", " кл.]" },
                { " cps]", " кл/с]" },
                { " rpm]", " выстр./мин]" },
                { " s]", " с]" },
                { " ticks]", " тик.]" }
            } },
            { "CharacterEditor.Listing_X:GetFormattedValue", new Dictionary<string,string> {
                { " cells]", " кл.]" },
                { " cps]", " кл/с]" },
                { " rpm]", " выстр./мин]" },
                { " s]", " с]" },
                { " ticks]", " тик.]" }
            } },
            { "CharacterEditor.Listing_X:TextEntryLabeledWithDefaultAndCopy", new Dictionary<string,string> {
                { " saved entities", " (сохранённые записи)" },
                { " saved entity", " (сохранённая запись)" }
            } },
            { "CharacterEditor.PawnxTool:CheckIfSpaceZombie", new Dictionary<string,string> {
                { "can't create zombies in space!", "В космосе нельзя создавать зомби!" }
            } },
            { "CharacterEditor.PawnxTool:Delete", new Dictionary<string,string> {
                { "can't delete below the minimum number", "Нельзя уменьшить число пешек ниже минимума" }
            } },
            { "CharacterEditor.PlacingTool:DropPawnWithPod", new Dictionary<string,string> {
                { " droppod", " в транспортной капсуле" }
            } },
            { "CharacterEditor.PlacingTool:PlaceInCustomPosition", new Dictionary<string,string> {
                { "place object", "Разместить объект" }
            } },
            { "CharacterEditor.PresetPawn:MissingMods", new Dictionary<string,string> {
                { "Missing Mods: ", "Отсутствуют моды: " }
            } },
            { "CharacterEditor.PresetPawn:ShowPawnMods", new Dictionary<string,string> {
                { "\nMissing Mods:\n", "\nОтсутствующие моды:\n" },
                { "Pawn Mods:\n", "Моды пешки:\n" }
            } },
            { "CharacterEditor.RelationTool:GetPawnRowTooltip", new Dictionary<string,string> {
                { "Compatibility: ", "Совместимость: " },
                { "RomanceChanceFactor: ", "Множитель вероятности романтических отношений: " }
            } },
            { "CharacterEditor.SZWidgets:GetFormattedValue", new Dictionary<string,string> {
                { " cells]", " кл.]" },
                { " cps]", " кл/с]" },
                { " rpm]", " выстр./мин]" },
                { " s]", " с]" },
                { " ticks]", " тик.]" }
            } },
            { "CharacterEditor.TraitTool:GetTooltipForSkillpoints", new Dictionary<string,string> {
                { " Genetic", " От генов" },
                { "Base lvl ", "Базовый уровень: " }
            } }
            };
        private static readonly Dictionary<string,string[]> Parameters = new Dictionary<string,string[]> {
            { "CharacterEditor.PlacingTool:PlaceInCustomPosition", new string[] { "CharacterEditor.Selected", "System.Action`1[[CharacterEditor.Selected, CharacterEditor]]" } }
        };
        public LiteralTranslations(ModContentPack content) : base(content) { }
        public static void Apply()
        {
            var harmony = new Harmony("igor.ru.1874644848.literals");
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
