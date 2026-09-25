using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace Igor.RU_2023507013
{
    public sealed class LiteralTranslations : Mod
    {
        private static readonly Dictionary<string,Dictionary<string,string>> Methods =
            new Dictionary<string,Dictionary<string,string>> {
            { "VEF.DebugActions:ChangeThingStylePlayerCrafted", new Dictionary<string,string> {
                { "Standard", "Стандартный" },
                { "Random", "Случайный" }
            } },
            { "VEF.DebugActions+<>c__DisplayClass0_1:<SpawnWorldObjectLayered>b__0", new Dictionary<string,string> {
                { "Invalid", "Недопустимо" }
            } },
            { "VEF.DebugActions+<>c__DisplayClass0_2:<SpawnWorldObjectLayered>b__1", new Dictionary<string,string> {
                { "Invalid", "Недопустимо" }
            } },
            { "VEF.VFEGlobal:GeneralSettings", new Dictionary<string,string> {
                { "Warning: Enabling this might cause performance issues.", "Внимание: включение этой настройки может снизить производительность." }
            } },
            { "VEF.Sounds.VEDebug:AddVEOptions", new Dictionary<string,string> {
                { "More options..", "Другие действия..." },
                { "Sound test", "Проверка звука" },
                { "Restart", "Перезапуск" }
            } },
            { "VEF.Sounds.EditWindow_SoundTest:DoWindowContents", new Dictionary<string,string> {
                { "Search:", "Поиск:" },
                { "Change sound (currently testing: ", "Сменить звук (сейчас: " },
                { "Subsound {0} <volumeRange>", "Подзвук {0}: диапазон громкости <volumeRange>" },
                { "Subsound {0} <pitchRange>", "Подзвук {0}: диапазон высоты тона <pitchRange>" },
                { "Copy settings of subsound {0}", "Скопировать настройки подзвука {0}" },
                { "Play sound", "Воспроизвести звук" }
            } },
            { "VEF.Pawns.CompPawnDependsOn:CompGetGizmosExtra", new Dictionary<string,string> {
                { "Dev: Spawn pawn", "Отладка: создать персонажа" },
                { "Spawn this building's pawn if none currently exists", "Создать персонажа этой постройки, если его ещё нет" }
            } },
            { "VEF.Weapons.AbilityWithChargesDetails:.ctor", new Dictionary<string,string> {
                { "charge", "заряд" },
                { "on cooldown", "перезарядка" }
            } },
            { "VEF.Weapons.HeavyWeaponsSettings:DoSettingsWindowContents", new Dictionary<string,string> {
                { "Enable HP deduction per shot for ", "Износ за выстрел: " },
                { "Adjust HP deduction per shot for ", "Потеря прочности за выстрел: " }
            } },
            { "VEF.Storyteller.PrefabExporter:MassExportPrefabs", new Dictionary<string,string> {
                { "Export prefab(s)", "Экспортировать заготовки построек" }
            } },
            { "VEF.Storyteller.PrefabExporter+Dialog_NameMassExport:DoWindowContents", new Dictionary<string,string> {
                { "Prefix:", "Префикс:" },
                { "Include roof", "Включить крыши" },
                { "Include space terrain", "Включить космическую поверхность" },
                { "Accept", "Принять" }
            } },
            { "VEF.Storyteller.PrefabExporter+Dialog_NameMassExport:MassExport", new Dictionary<string,string> {
                { "Copied {0} prefabs to clipboard.", "В буфер обмена скопировано заготовок: {0}." }
            } },
            { "VEF.Storyteller.QuestChainsDevWindow:DoWindowContents", new Dictionary<string,string> {
                { "Quest Chains component not found.", "Компонент цепочек заданий не найден." },
                { "Quests (", "Задания (" },
                { "Future Quests (", "Будущие задания (" }
            } },
            { "VEF.Storyteller.QuestChainsDevWindow:DrawQuestInfo", new Dictionary<string,string> {
                { " (Chain: ", " (цепочка: " },
                { "Force Success", "Завершить успешно" },
                { "Force Fail", "Завершить провалом" },
                { "  - State: ", "  — Состояние: " },
                { "  - Outcome: ", "  — Исход: " },
                { "  - Quest is null", "  — Задание отсутствует" },
                { "  - Accepted: ", "  — Принято: " },
                { "  - Completed: ", "  — Завершено: " },
                { "  - Expired: ", "  — Истекло: " },
                { "None", "Нет" }
            } },
            { "VEF.Storyteller.QuestChainsDevWindow:DrawFutureQuestInfo", new Dictionary<string,string> {
                { " (Chain: ", " (цепочка: " },
                { "Fire Now", "Запустить сейчас" },
                { "  - Fires in: ", "  — До запуска: " },
                { " (at ", " (в " },
                { "  - MTB: ", "  — Средний интервал: " },
                { " days", " д." },
                { "None", "Нет" }
            } },
            { "VEF.Plants.Plant_Blooming+<GetGizmos>d__37:MoveNext", new Dictionary<string,string> {
                { "Increase age 1 year", "Добавить год возраста" },
                { "Cause weeds", "Добавить сорняки" }
            } },
            { "VEF.Plants.Plant_Blooming+<SpecialDisplayStats>d__39:MoveNext", new Dictionary<string,string> {
                { " to ", " — " }
            } },
            { "VEF.Plants.MinifiedFlower+<GetGizmos>d__15:MoveNext", new Dictionary<string,string> {
                { "DEV: Destroy", "Отладка: уничтожить" },
                { "DEV: Die in 1 hour", "Отладка: погибнуть через час" },
                { "DEV: Die in 1 day", "Отладка: погибнуть через день" }
            } },
            { "VEF.Genes.CompHumanHatcher+<CompGetGizmosExtra>d__23:MoveNext", new Dictionary<string,string> {
                { "DEV: Finish hatching", "Отладка: завершить вылупление" }
            } },
            { "VEF.Buildings.CompConfigurableSpawner+<CompGetGizmosExtra>d__20:MoveNext", new Dictionary<string,string> {
                { "DEBUG: Spawn product", "Отладка: создать продукцию" }
            } },
            { "VEF.Buildings.CompRockSpawner+<CompGetGizmosExtra>d__20:MoveNext", new Dictionary<string,string> {
                { "DEBUG: Spawn rock", "Отладка: создать обломок" }
            } },
            { "VEF.Apparels.CompSwitchApparel:CompGetWornGizmosExtra", new Dictionary<string,string> {
                { "Switch", "Переключить" },
                { "Switch to ", "Переключить на: " }
            } },
            { "VEF.Apparels.CompShieldField+<GetGizmos>d__98:MoveNext", new Dictionary<string,string> {
                { "DEV: 0 energy", "Отладка: обнулить энергию" },
                { "DEV: 1 energy", "Отладка: установить энергию 1" },
                { "DEV: Max energy", "Отладка: максимум энергии" },
                { "DEV: Flash interception cells", "Отладка: подсветить клетки перехвата" }
            } },
            { "VEF.Apparels.CompShieldBubble+<CompGetWornGizmosExtra>d__55:MoveNext", new Dictionary<string,string> {
                { "DEV: Break", "Отладка: пробить щит" },
                { "DEV: Clear reset", "Отладка: восстановить щит" }
            } },
            { "VEF.AnimalGenes.CompAnimalGenes+<CompGetGizmosExtra>d__15:MoveNext", new Dictionary<string,string> {
                { "DEV: Do birth", "Отладка: вызвать роды" }
            } },
            { "VEF.AnimalBehaviours.CompAnimalProduct+<CompGetGizmosExtra>d__15:MoveNext", new Dictionary<string,string> {
                { "DEV: Set to produce now", "Отладка: подготовить продукцию" },
                { "Sets animal products to be ready to be gathered now", "Сделать продукцию животного готовой к сбору" }
            } },
            { "VEF.AnimalBehaviours.CompAsexualReproduction:CompInspectStringExtra", new Dictionary<string,string> {
                { " days)", " д.)" }
            } },
            { "VEF.AnimalBehaviours.CompAsexualReproduction+<CompGetGizmosExtra>d__15:MoveNext", new Dictionary<string,string> {
                { "DEV: Reproduce now", "Отладка: размножиться" },
                { "Set asexual reproduction to trigger now", "Немедленно запустить бесполое размножение" }
            } },
            { "VEF.AnimalBehaviours.CompDiseasesAfterPeriod+<GetGizmos>d__5:MoveNext", new Dictionary<string,string> {
                { "DEBUG: Give age related diseases", "Отладка: добавить возрастные болезни" }
            } },
            { "VEF.AnimalBehaviours.CompProperties_AutoNutrition:.ctor", new Dictionary<string,string> {
                { "Eating food", "Ест пищу" }
            } },
            { "VEF.AnimalBehaviours.HediffComp_AsexualReproduction:GetLabel", new Dictionary<string,string> {
                { " days)", " д.)" }
            } },
            { "VEF.AnimalBehaviours.HediffComp_Resurrect:get_CompLabelInBracketsExtra", new Dictionary<string,string> {
                { " lives", " жизн." }
            } },
            { "VEF.AnimalBehaviours.AnimalBehaviours_Mod:SettingsCategory", new Dictionary<string,string> {
                { "Animal Behaviours", "Поведение животных" }
            } },
            { "VEF.AnimalBehaviours.VanillaAnimalsExpanded_Mod:SettingsCategory", new Dictionary<string,string> {
                { "Animal Toggles", "Включение видов животных" }
            } },
            { "VEF.Abilities.AbilityDef:.ctor", new Dictionary<string,string> {
                { "Using ability: {0}", "Применяет способность: {0}" }
            } },
            { "Outposts.Outpost+<GetGizmos>d__72:MoveNext", new Dictionary<string,string> {
                { "Dev: Produce now", "Отладка: произвести сейчас" },
                { "Reduce ticksTillProduction to 10", "Сократить время до производства до 10 тиков" },
                { "Dev: Random pawn takes 10 damage", "Отладка: нанести случайному персонажу 10 урона" },
                { "Dev: All pawns 0% food", "Отладка: обнулить сытость всех персонажей" },
                { "Dev: Pack now", "Отладка: свернуть аванпост" },
                { "Reduce ticksTillPacked to 1", "Сократить время сворачивания до 1 тика" }
            } },
            { "KCSG.FallingStructureStrategy:SpawnThreats", new Dictionary<string,string> {
                { " (incoming)", " (приближается)" }
            } },
            { "KCSG.ScenPart_AddStartingStructure:DoEditInterface", new Dictionary<string,string> {
                { "No structure available", "Нет доступных построек" },
                { "Nothing to remove", "Нечего удалять" }
            } },
            { "KCSG.Designator_ExportToXml:.ctor", new Dictionary<string,string> {
                { "Export", "Экспорт" },
                { "Export a building to xml", "Экспортировать постройку в XML" }
            } },
            { "KCSG.Designator_ExportToXmlFromArea:.ctor", new Dictionary<string,string> {
                { "Export from area", "Экспорт из зоны" },
                { "Export a building to xml from area", "Экспортировать постройку из выбранной зоны в XML" }
            } },
            { "KCSG.Dialog_ExportWindow:DoWindowContents", new Dictionary<string,string> {
                { "Structure export menu", "Экспорт постройки" },
                { "Structure defName:", "Идентификатор постройки (defName):" },
                { "Auto-conduit:", "Автоматические кабели:" },
                { "Spawn conduits under impassables and doors automatically", "Автоматически создавать кабели под непроходимыми объектами и дверями" },
                { "Export filth:", "Экспортировать грязь:" },
                { "Export plants:", "Экспортировать растения:" },
                { "Export natural terrain:", "Экспортировать природную поверхность:" },
                { "Stockpile:", "Склад:" },
                { "Generate random items inside when used with SettlementLayoutDef", "Создавать внутри случайные предметы при использовании с SettlementLayoutDef" },
                { "Force generate roofs:", "Обязательно создавать крыши:" },
                { "Alway generate exported roof", "Всегда создавать экспортированную крышу" },
                { "Need roof clearance:", "Требуется место без крыши:" },
                { "Need to be placed in a rect free of roofs", "Размещать только на участке, полностью свободном от крыш" },
                { "Randomize wall stuff:", "Случайный материал стен:" },
                { "Randomize wall stuff at generation", "Выбирать случайный материал стен при генерации" },
                { "Random rotation:", "Случайный поворот:" },
                { "Randomize rotation at generation", "Выбирать случайный поворот при генерации" },
                { "Save fuel:", "Сохранять топливо:" },
                { "Save fuel values from pipenet and vanilla comprefuelable", "Сохранять запас ресурсов трубопровода и топлива стандартного CompRefuelable" },
                { "Save power:", "Сохранять заряд:" },
                { "Save power values from batteries", "Сохранять заряд аккумуляторов" },
                { "Structure tags:", "Теги постройки:" },
                { "Tags are used with SettlementLayoutDef", "Теги используются вместе с SettlementLayoutDef" },
                { "{0} tag(s) used", "Используется тегов: {0}" },
                { "Add tag", "Добавить тег" },
                { "Cannot add empty tag.", "Нельзя добавить пустой тег." },
                { "Remove tag...", "Удалить тег..." },
                { "Nothing to remove", "Нечего удалять" }
            } },
            { "KCSG.Dialog_ExportWindow:DrawFooter", new Dictionary<string,string> {
                { "Copy structure", "Скопировать постройку" },
                { "Copied to clipboard.", "Скопировано в буфер обмена." },
                { "Cannot use empty defName.", "Идентификатор defName не может быть пустым." },
                { "Copy symbols", "Скопировать символы" },
                { "Copied {0} symbols to clipboard.", "В буфер обмена скопировано символов: {0}." },
                { "No new symbols needed.", "Новые символы не нужны." },
                { "Close", "Закрыть" }
            } },
            { "KCSG.DebugActions:Quickspawn", new Dictionary<string,string> {
                { "Temp structure...", "Временная постройка..." },
                { "Symbol...", "Символ..." },
                { "Temp symbol...", "Временный символ..." },
                { "Settement (rect)", "Поселение (прямоугольник)" },
                { "Tiled structure...", "Постройка из повторяющихся секций..." }
            } },
            { "MVCF.Reloading.Comps.CommandPart_Reloadable:PostInit", new Dictionary<string,string> {
                { "ammo", "боеприпасы" }
            } },
            { "ModSettingsFramework.Dialog_ModSettings_Patch:Postfix", new Dictionary<string,string> {
                { "Some changes might require you to restart the game to take effect!", "Для применения некоторых изменений может потребоваться перезапуск игры!" }
            } },
            { "PipeSystem.Alert_NoStorage:GetExplanation", new Dictionary<string,string> {
                { " network", " — сеть" }
            } },
            { "PipeSystem.CompAdvancedResourceProcessor+<CompGetGizmosExtra>d__76:MoveNext", new Dictionary<string,string> {
                { "Finish in 10 ticks", "Завершить через 10 тиков" },
                { "Advance progress 1 day", "Добавить день прогресса" },
                { "Empty wastepack(s)", "Выгрузить пакеты отходов" }
            } },
            { "PipeSystem.CompExplosiveContent+<CompGetGizmosExtra>d__32:MoveNext", new Dictionary<string,string> {
                { "DEV: Trigger countdown", "Отладка: начать обратный отсчёт" }
            } },
            { "PipeSystem.CompProperties_ResourceTrader+<SpecialDisplayStats>d__17:MoveNext", new Dictionary<string,string> {
                { "{0:##0} {1}/d", "{0:##0} {1}/д." }
            } },
            { "PipeSystem.CompResourceStorage+<CompGetGizmosExtra>d__42:MoveNext", new Dictionary<string,string> {
                { "DEBUG: Fill", "Отладка: наполнить" },
                { "DEBUG: Add 5", "Отладка: добавить 5" },
                { "DEBUG: Empty", "Отладка: опустошить" }
            } },
            { "PipeSystem.CompResourceTrader:CompInspectStringExtra", new Dictionary<string,string> {
                { "{0} {1:##0} {2}/d", "{0} {1:##0} {2}/д." }
            } },
            { "PipeSystem.CompSpawnerOrNet+<CompGetGizmosExtra>d__9:MoveNext", new Dictionary<string,string> {
                { "Spawn now", "Создать сейчас" }
            } },
            { "VEF.Weapons.Building_LaserGun:GetInspectString", new Dictionary<string,string> {
                { "LaserTurretNotCharged", "chupakobra6_2023507013_LaserTurretNotCharged" }
            } },
            { "VEF.Maps.TerrainComp_TempControl:TransformLabel", new Dictionary<string,string> {
                { "HeatedFloor_LowPower", "chupakobra6_2023507013_HeatedFloor_LowPower" },
                { "HeatedFloor_HighPower", "chupakobra6_2023507013_HeatedFloor_HighPower" }
            } },
            { "VEF.Maps.CompPowerTraderFloor:CompInspectStringExtra", new Dictionary<string,string> {
                { "FloorWire_InspectStringPart", "chupakobra6_2023507013_FloorWire_InspectStringPart" }
            } },
            { "VEF.Buildings.Furniture_Settings:DoWindowContents", new Dictionary<string,string> {
                { "VFE_RandomOrSequentially", "chupakobra6_2023507013_VFE_RandomOrSequentially" }
            } },
            { "VEF.Planet.MovingBase_PathFollower:CostToMove", new Dictionary<string,string> {
                { "EncumberedMerchantGuildTilesPerDayTip", "chupakobra6_2023507013_EncumberedMerchantGuildTilesPerDayTip" },
                { "FinalMerchantGuildMovementSpeed", "chupakobra6_2023507013_FinalMerchantGuildMovementSpeed" }
            } },
            { "VEF.Buildings.SwappableBuilding+<GetGizmos>d__5:MoveNext", new Dictionary<string,string> {
                { "Activate", "Активировать" }
            } },
            { "MVCF.Utilities.PawnVerbGizmoUtility:GetMainAttackGizmoForPawn", new Dictionary<string,string> {
                { "Attack", "Атаковать" }
            } }
            };
        private static readonly Dictionary<string,string[]> Parameters = new Dictionary<string,string[]> {
            { "VEF.Planet.MovingBase_PathFollower:CostToMove", new string[] { "System.Int32", "RimWorld.Planet.PlanetTile", "RimWorld.Planet.PlanetTile", "System.Nullable`1[System.Int32]", "System.Boolean", "System.Text.StringBuilder", "System.String", "System.Boolean" } }
        };
        public LiteralTranslations(ModContentPack content) : base(content) { }
        public static void Apply()
        {
            var harmony = new Harmony("igor.ru.2023507013.literals");
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
