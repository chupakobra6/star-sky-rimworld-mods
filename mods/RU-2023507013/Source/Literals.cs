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
            { "KCSG.DebugActions:Quickspawn", new Dictionary<string,string> {
                { "Settement (rect)", "Поселение (прямоугольник)" },
                { "Symbol...", "Символ..." },
                { "Temp structure...", "Временная постройка..." },
                { "Temp symbol...", "Временный символ..." },
                { "Tiled structure...", "Постройка из повторяющихся секций..." }
            } },
            { "KCSG.Designator_ExportToXml:.ctor", new Dictionary<string,string> {
                { "Export", "Экспорт" },
                { "Export a building to xml", "Экспортировать постройку в XML" }
            } },
            { "KCSG.Designator_ExportToXmlFromArea:.ctor", new Dictionary<string,string> {
                { "Export a building to xml from area", "Экспортировать постройку из выбранной зоны в XML" },
                { "Export from area", "Экспорт из зоны" }
            } },
            { "KCSG.Dialog_ExportWindow:DoWindowContents", new Dictionary<string,string> {
                { "Add tag", "Добавить тег" },
                { "Alway generate exported roof", "Всегда создавать экспортированную крышу" },
                { "Auto-conduit:", "Автоматические кабели:" },
                { "Cannot add empty tag.", "Нельзя добавить пустой тег." },
                { "Export filth:", "Экспортировать грязь:" },
                { "Export natural terrain:", "Экспортировать природную поверхность:" },
                { "Export plants:", "Экспортировать растения:" },
                { "Force generate roofs:", "Обязательно создавать крыши:" },
                { "Generate random items inside when used with SettlementLayoutDef", "Создавать внутри случайные предметы при использовании с SettlementLayoutDef" },
                { "Need roof clearance:", "Требуется место без крыши:" },
                { "Need to be placed in a rect free of roofs", "Размещать только на участке, полностью свободном от крыш" },
                { "Nothing to remove", "Нечего удалять" },
                { "Random rotation:", "Случайный поворот:" },
                { "Randomize rotation at generation", "Выбирать случайный поворот при генерации" },
                { "Randomize wall stuff at generation", "Выбирать случайный материал стен при генерации" },
                { "Randomize wall stuff:", "Случайный материал стен:" },
                { "Remove tag...", "Удалить тег..." },
                { "Save fuel values from pipenet and vanilla comprefuelable", "Сохранять запас ресурсов трубопровода и топлива стандартного CompRefuelable" },
                { "Save fuel:", "Сохранять топливо:" },
                { "Save power values from batteries", "Сохранять заряд аккумуляторов" },
                { "Save power:", "Сохранять заряд:" },
                { "Spawn conduits under impassables and doors automatically", "Автоматически создавать кабели под непроходимыми объектами и дверями" },
                { "Stockpile:", "Склад:" },
                { "Structure defName:", "Идентификатор постройки (defName):" },
                { "Structure export menu", "Экспорт постройки" },
                { "Structure tags:", "Теги постройки:" },
                { "Tags are used with SettlementLayoutDef", "Теги используются вместе с SettlementLayoutDef" },
                { "{0} tag(s) used", "Используется тегов: {0}" }
            } },
            { "KCSG.Dialog_ExportWindow:DrawFooter", new Dictionary<string,string> {
                { "Cannot use empty defName.", "Идентификатор defName не может быть пустым." },
                { "Close", "Закрыть" },
                { "Copied to clipboard.", "Скопировано в буфер обмена." },
                { "Copied {0} symbols to clipboard.", "В буфер обмена скопировано символов: {0}." },
                { "Copy structure", "Скопировать постройку" },
                { "Copy symbols", "Скопировать символы" },
                { "No new symbols needed.", "Новые символы не нужны." }
            } },
            { "KCSG.FallingStructureStrategy:SpawnThreats", new Dictionary<string,string> {
                { " (incoming)", " (приближается)" }
            } },
            { "KCSG.ScenPart_AddStartingStructure:DoEditInterface", new Dictionary<string,string> {
                { "No structure available", "Нет доступных построек" },
                { "Nothing to remove", "Нечего удалять" }
            } },
            { "MVCF.Reloading.Comps.CommandPart_Reloadable:PostInit", new Dictionary<string,string> {
                { "ammo", "боеприпасы" }
            } },
            { "MVCF.Utilities.PawnVerbGizmoUtility:GetMainAttackGizmoForPawn", new Dictionary<string,string> {
                { "Attack", "Атаковать" }
            } },
            { "ModSettingsFramework.Dialog_ModSettings_Patch:Postfix", new Dictionary<string,string> {
                { "Some changes might require you to restart the game to take effect!", "Для применения некоторых изменений может потребоваться перезапуск игры!" }
            } },
            { "Outposts.Outpost+<GetGizmos>d__72:MoveNext", new Dictionary<string,string> {
                { "Dev: All pawns 0% food", "Отладка: обнулить сытость всех персонажей" },
                { "Dev: Pack now", "Отладка: свернуть аванпост" },
                { "Dev: Produce now", "Отладка: произвести сейчас" },
                { "Dev: Random pawn takes 10 damage", "Отладка: нанести случайному персонажу 10 урона" },
                { "Reduce ticksTillPacked to 1", "Сократить время сворачивания до 1 тика" },
                { "Reduce ticksTillProduction to 10", "Сократить время до производства до 10 тиков" }
            } },
            { "PipeSystem.Alert_NoStorage:GetExplanation", new Dictionary<string,string> {
                { " network", " — сеть" }
            } },
            { "PipeSystem.CompAdvancedResourceProcessor+<CompGetGizmosExtra>d__76:MoveNext", new Dictionary<string,string> {
                { "Advance progress 1 day", "Добавить день прогресса" },
                { "Empty wastepack(s)", "Выгрузить пакеты отходов" },
                { "Finish in 10 ticks", "Завершить через 10 тиков" }
            } },
            { "PipeSystem.CompExplosiveContent+<CompGetGizmosExtra>d__32:MoveNext", new Dictionary<string,string> {
                { "DEV: Trigger countdown", "Отладка: начать обратный отсчёт" }
            } },
            { "PipeSystem.CompProperties_ResourceTrader+<SpecialDisplayStats>d__17:MoveNext", new Dictionary<string,string> {
                { "{0:##0} {1}/d", "{0:##0} {1}/д." }
            } },
            { "PipeSystem.CompResourceStorage+<CompGetGizmosExtra>d__42:MoveNext", new Dictionary<string,string> {
                { "DEBUG: Add 5", "Отладка: добавить 5" },
                { "DEBUG: Empty", "Отладка: опустошить" },
                { "DEBUG: Fill", "Отладка: наполнить" }
            } },
            { "PipeSystem.CompResourceTrader:CompInspectStringExtra", new Dictionary<string,string> {
                { "{0} {1:##0} {2}/d", "{0} {1:##0} {2}/д." }
            } },
            { "PipeSystem.CompSpawnerOrNet+<CompGetGizmosExtra>d__9:MoveNext", new Dictionary<string,string> {
                { "Spawn now", "Создать сейчас" }
            } },
            { "VEF.Abilities.AbilityDef:.ctor", new Dictionary<string,string> {
                { "Using ability: {0}", "Применяет способность: {0}" }
            } },
            { "VEF.AnimalBehaviours.AnimalBehaviours_Mod:SettingsCategory", new Dictionary<string,string> {
                { "Animal Behaviours", "Поведение животных" }
            } },
            { "VEF.AnimalBehaviours.CompAnimalProduct+<CompGetGizmosExtra>d__15:MoveNext", new Dictionary<string,string> {
                { "DEV: Set to produce now", "Отладка: подготовить продукцию" },
                { "Sets animal products to be ready to be gathered now", "Сделать продукцию животного готовой к сбору" }
            } },
            { "VEF.AnimalBehaviours.CompAsexualReproduction+<CompGetGizmosExtra>d__15:MoveNext", new Dictionary<string,string> {
                { "DEV: Reproduce now", "Отладка: размножиться" },
                { "Set asexual reproduction to trigger now", "Немедленно запустить бесполое размножение" }
            } },
            { "VEF.AnimalBehaviours.CompAsexualReproduction:CompInspectStringExtra", new Dictionary<string,string> {
                { " days)", " д.)" }
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
            { "VEF.AnimalBehaviours.VanillaAnimalsExpanded_Mod:SettingsCategory", new Dictionary<string,string> {
                { "Animal Toggles", "Включение видов животных" }
            } },
            { "VEF.AnimalGenes.CompAnimalGenes+<CompGetGizmosExtra>d__16:MoveNext", new Dictionary<string,string> {
                { "DEV: Do birth", "Отладка: вызвать роды" }
            } },
            { "VEF.Apparels.CompShieldBubble+<CompGetWornGizmosExtra>d__55:MoveNext", new Dictionary<string,string> {
                { "DEV: Break", "Отладка: пробить щит" },
                { "DEV: Clear reset", "Отладка: восстановить щит" }
            } },
            { "VEF.Apparels.CompShieldField+<GetGizmos>d__98:MoveNext", new Dictionary<string,string> {
                { "DEV: 0 energy", "Отладка: обнулить энергию" },
                { "DEV: 1 energy", "Отладка: установить энергию 1" },
                { "DEV: Flash interception cells", "Отладка: подсветить клетки перехвата" },
                { "DEV: Max energy", "Отладка: максимум энергии" }
            } },
            { "VEF.Apparels.CompSwitchApparel:CompGetWornGizmosExtra", new Dictionary<string,string> {
                { "Switch", "Переключить" },
                { "Switch to ", "Переключить на: " }
            } },
            { "VEF.Buildings.CompConfigurableSpawner+<CompGetGizmosExtra>d__20:MoveNext", new Dictionary<string,string> {
                { "DEBUG: Spawn product", "Отладка: создать продукцию" }
            } },
            { "VEF.Buildings.CompRockSpawner+<CompGetGizmosExtra>d__20:MoveNext", new Dictionary<string,string> {
                { "DEBUG: Spawn rock", "Отладка: создать обломок" }
            } },
            { "VEF.Buildings.Furniture_Settings:DoWindowContents", new Dictionary<string,string> {
                { "VFE_RandomOrSequentially", "chupakobra6_2023507013_VFE_RandomOrSequentially" }
            } },
            { "VEF.Buildings.SwappableBuilding+<GetGizmos>d__5:MoveNext", new Dictionary<string,string> {
                { "Activate", "Активировать" }
            } },
            { "VEF.DebugActions+<>c__DisplayClass0_1:<SpawnWorldObjectLayered>b__0", new Dictionary<string,string> {
                { "Invalid", "Недопустимо" }
            } },
            { "VEF.DebugActions+<>c__DisplayClass0_2:<SpawnWorldObjectLayered>b__1", new Dictionary<string,string> {
                { "Invalid", "Недопустимо" }
            } },
            { "VEF.DebugActions:ChangeThingStylePlayerCrafted", new Dictionary<string,string> {
                { "Random", "Случайный" },
                { "Standard", "Стандартный" }
            } },
            { "VEF.Genes.CompHumanHatcher+<CompGetGizmosExtra>d__23:MoveNext", new Dictionary<string,string> {
                { "DEV: Finish hatching", "Отладка: завершить вылупление" }
            } },
            { "VEF.Maps.CompPowerTraderFloor:CompInspectStringExtra", new Dictionary<string,string> {
                { "FloorWire_InspectStringPart", "chupakobra6_2023507013_FloorWire_InspectStringPart" }
            } },
            { "VEF.Maps.TerrainComp_TempControl:TransformLabel", new Dictionary<string,string> {
                { "HeatedFloor_HighPower", "chupakobra6_2023507013_HeatedFloor_HighPower" },
                { "HeatedFloor_LowPower", "chupakobra6_2023507013_HeatedFloor_LowPower" }
            } },
            { "VEF.Pawns.CompPawnDependsOn:CompGetGizmosExtra", new Dictionary<string,string> {
                { "Dev: Spawn pawn", "Отладка: создать персонажа" },
                { "Spawn this building's pawn if none currently exists", "Создать персонажа этой постройки, если его ещё нет" }
            } },
            { "VEF.Planet.MovingBase_PathFollower:CostToMove", new Dictionary<string,string> {
                { "EncumberedMerchantGuildTilesPerDayTip", "chupakobra6_2023507013_EncumberedMerchantGuildTilesPerDayTip" },
                { "FinalMerchantGuildMovementSpeed", "chupakobra6_2023507013_FinalMerchantGuildMovementSpeed" }
            } },
            { "VEF.Plants.MinifiedFlower+<GetGizmos>d__15:MoveNext", new Dictionary<string,string> {
                { "DEV: Destroy", "Отладка: уничтожить" },
                { "DEV: Die in 1 day", "Отладка: погибнуть через день" },
                { "DEV: Die in 1 hour", "Отладка: погибнуть через час" }
            } },
            { "VEF.Plants.Plant_Blooming+<GetGizmos>d__37:MoveNext", new Dictionary<string,string> {
                { "Cause weeds", "Добавить сорняки" },
                { "Increase age 1 year", "Добавить год возраста" }
            } },
            { "VEF.Plants.Plant_Blooming+<SpecialDisplayStats>d__39:MoveNext", new Dictionary<string,string> {
                { " to ", " — " }
            } },
            { "VEF.Sounds.EditWindow_SoundTest:DoWindowContents", new Dictionary<string,string> {
                { "Change sound (currently testing: ", "Сменить звук (сейчас: " },
                { "Copy settings of subsound {0}", "Скопировать настройки подзвука {0}" },
                { "Play sound", "Воспроизвести звук" },
                { "Search:", "Поиск:" },
                { "Subsound {0} <pitchRange>", "Подзвук {0}: диапазон высоты тона <pitchRange>" },
                { "Subsound {0} <volumeRange>", "Подзвук {0}: диапазон громкости <volumeRange>" }
            } },
            { "VEF.Sounds.VEDebug:AddVEOptions", new Dictionary<string,string> {
                { "More options..", "Другие действия..." },
                { "Restart", "Перезапуск" },
                { "Sound test", "Проверка звука" }
            } },
            { "VEF.Storyteller.PrefabExporter+Dialog_NameMassExport:DoWindowContents", new Dictionary<string,string> {
                { "Accept", "Принять" },
                { "Include roof", "Включить крыши" },
                { "Include space terrain", "Включить космическую поверхность" },
                { "Prefix:", "Префикс:" }
            } },
            { "VEF.Storyteller.PrefabExporter+Dialog_NameMassExport:MassExport", new Dictionary<string,string> {
                { "Copied {0} prefabs to clipboard.", "В буфер обмена скопировано заготовок: {0}." }
            } },
            { "VEF.Storyteller.PrefabExporter:MassExportPrefabs", new Dictionary<string,string> {
                { "Export prefab(s)", "Экспортировать заготовки построек" }
            } },
            { "VEF.Storyteller.QuestChainsDevWindow:DoWindowContents", new Dictionary<string,string> {
                { "Future Quests (", "Будущие задания (" },
                { "Quest Chains component not found.", "Компонент цепочек заданий не найден." },
                { "Quests (", "Задания (" }
            } },
            { "VEF.Storyteller.QuestChainsDevWindow:DrawFutureQuestInfo", new Dictionary<string,string> {
                { "  - Fires in: ", "  — До запуска: " },
                { "  - MTB: ", "  — Средний интервал: " },
                { " (Chain: ", " (цепочка: " },
                { " (at ", " (в " },
                { " days", " д." },
                { "Fire Now", "Запустить сейчас" },
                { "None", "Нет" }
            } },
            { "VEF.Storyteller.QuestChainsDevWindow:DrawQuestInfo", new Dictionary<string,string> {
                { "  - Accepted: ", "  — Принято: " },
                { "  - Completed: ", "  — Завершено: " },
                { "  - Expired: ", "  — Истекло: " },
                { "  - Outcome: ", "  — Исход: " },
                { "  - Quest is null", "  — Задание отсутствует" },
                { "  - State: ", "  — Состояние: " },
                { " (Chain: ", " (цепочка: " },
                { "Force Fail", "Завершить провалом" },
                { "Force Success", "Завершить успешно" },
                { "None", "Нет" }
            } },
            { "VEF.VFEGlobal:GeneralSettings", new Dictionary<string,string> {
                { "Warning: Enabling this might cause performance issues.", "Внимание: включение этой настройки может снизить производительность." }
            } },
            { "VEF.Weapons.AbilityWithChargesDetails:.ctor", new Dictionary<string,string> {
                { "charge", "заряд" },
                { "on cooldown", "перезарядка" }
            } },
            { "VEF.Weapons.Building_LaserGun:GetInspectString", new Dictionary<string,string> {
                { "LaserTurretNotCharged", "chupakobra6_2023507013_LaserTurretNotCharged" }
            } },
            { "VEF.Weapons.HeavyWeaponsSettings:DoSettingsWindowContents", new Dictionary<string,string> {
                { "Adjust HP deduction per shot for ", "Потеря прочности за выстрел: " },
                { "Enable HP deduction per shot for ", "Износ за выстрел: " }
            } }
            };
        private static readonly Dictionary<string,string[]> Parameters = new Dictionary<string,string[]> {
            { "VEF.Planet.MovingBase_PathFollower:CostToMove", new string[] { "System.Int32", "RimWorld.Planet.PlanetTile", "RimWorld.Planet.PlanetTile", "System.Nullable`1[System.Int32]", "System.Boolean", "System.Text.StringBuilder", "System.String", "System.Boolean" } }
        };
        public LiteralTranslations(ModContentPack content) : base(content) { }
        public static void Apply()
        {
            var harmony = new Harmony("igor.ru.2023507013.literals");
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
