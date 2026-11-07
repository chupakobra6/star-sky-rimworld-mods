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
            { "VEF.Planet.MovingBase_PathFollower:CostToMove", new string[] { "System.Int32", "RimWorld.Planet.PlanetTile", "RimWorld.Planet.PlanetTile", "System.Nullable`1<System.Int32>", "System.Boolean", "System.Text.StringBuilder", "System.String", "System.Boolean" } }
        };
        public LiteralTranslations(ModContentPack content) : base(content) { }
        public static void Apply()
        {
            var harmony = new Harmony("igor.ru.2023507013.literals");
            foreach (var target in BuildTargets(UseVersion17))
            {
                if (UseVersion17 && MissingOptional17Owner(target)) continue;
                MethodBase method;
                try { method = ResolveTarget(target); }
                catch (MissingMethodException exception)
                {
                    Warn(target, exception.Message);
                    continue;
                }
                harmony.Patch(method,
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
            if (target == AbilityReportSource)
                return AccessTools.DeclaredPropertyGetter(AccessTools.TypeByName("VEF.Abilities.AbilityDef"), "JobReportString")
                    ?? throw new MissingMethodException("Exact ability report getter was not found");
            int split = target.LastIndexOf(':');
            var type = AccessTools.TypeByName(target.Substring(0, split));
            string name = target.Substring(split + 1);
            if (type == null) throw new MissingMethodException("Exact translation type was not found: " + target);
            const BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.Instance | BindingFlags.Static;
            IEnumerable<MethodBase> candidates = name == ".ctor"
                ? type.GetConstructors(flags).Where(m => m.Name == name).Cast<MethodBase>()
                : type.GetMethods(flags).Where(m => m.Name == name).Cast<MethodBase>();
            if (TryGetParameters(target, UseVersion17, out var parameters))
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
            return TranspileTarget(input, __originalMethod, UseVersion17);
        }
        public static IEnumerable<CodeInstruction> TranspileTarget(IEnumerable<CodeInstruction> input, MethodBase __originalMethod, bool useVersion17)
        {
            if (__originalMethod.DeclaringType.FullName == "VEF.Abilities.AbilityDef" && __originalMethod.Name == "get_JobReportString")
                return TranspileAbilityReport(input, __originalMethod);
            string target = __originalMethod.DeclaringType.FullName + ":" + __originalMethod.Name;
            if (useVersion17 && Version17ByTarget.TryGetValue(target, out var variant))
                return Transpile17(input, target, variant);
            var translations = Methods[target];
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
                Warn(target, "Source strings changed: " + string.Join(", ", translations.Keys.Where(k => !found.Contains(k))));
            return output;
        }
        private static void Warn(string target, string detail)
        {
            string message = "[Star Sky RU 2023507013] " + target + ": " + detail;
            Log.WarningOnce(message, message.GetHashCode());
        }

        // The constructor supplies the source literal, but Defs already exist when the patch runs.
        private const string AbilityReportSource = "VEF.Abilities.AbilityDef:.ctor";

        private static IEnumerable<CodeInstruction> TranspileAbilityReport(IEnumerable<CodeInstruction> input, MethodBase method)
        {
            var translation = Methods[AbilityReportSource].Single();
            var field = AccessTools.DeclaredField(method.DeclaringType, "jobReportString");
            var output = input.ToList();
            int read = field?.FieldType == typeof(string) && !field.IsStatic
                ? output.FindIndex(instruction => instruction.LoadsField(field)) : -1;
            if (read < 0 || output.Skip(read + 1).Any(instruction => instruction.LoadsField(field)))
            {
                Warn(AbilityReportSource, "Expected one ability report template read");
                return output;
            }
            output.InsertRange(read + 1, new[] {
                new CodeInstruction(OpCodes.Ldstr, translation.Key),
                new CodeInstruction(OpCodes.Ldstr, translation.Value),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(LiteralTranslations), nameof(SelectAbilityReport)))
            });
            return output;
        }

        private static string SelectAbilityReport(string template, string original, string russian)
        {
            return string.Equals(template, original, StringComparison.Ordinal) ? Select(template, russian) : template;
        }

        private sealed class Variant17
        {
            internal readonly string Target;
            internal readonly string[] Parameters, Shape;
            internal readonly bool Optional, Enabled;
            internal readonly Dictionary<string,string> Exact, Append;

            internal Variant17(string target, string[] parameters, bool optional, bool enabled,
                Dictionary<string,string> exact, Dictionary<string,string> append, string[] shape)
            {
                Target = target;
                Parameters = parameters;
                Optional = optional;
                Enabled = enabled;
                Exact = exact;
                Append = append;
                Shape = shape;
            }
        }

        private static readonly Dictionary<string,Variant17> Version17 = new Dictionary<string,Variant17> {
            { "KCSG.Dialog_ExportWindow:DoWindowContents", new Variant17("KCSG.Dialog_ExportWindow:DoWindowContents", null, false, true, new Dictionary<string,string> { { "Add tag", "Добавить тег" }, { "Alway generate exported roof", "Всегда создавать экспортированную крышу" }, { "Auto-conduit:", "Автоматические кабели:" }, { "Cannot add empty tag.", "Нельзя добавить пустой тег." }, { "Export filth:", "Экспортировать грязь:" }, { "Export natural terrain:", "Экспортировать природную поверхность:" }, { "Export plants:", "Экспортировать растения:" }, { "Force generate roofs:", "Обязательно создавать крыши:" }, { "Generate random items inside when used with SettlementLayoutDef", "Создавать внутри случайные предметы при использовании с SettlementLayoutDef" }, { "Need roof clearance:", "Требуется место без крыши:" }, { "Need to be placed in a rect free of roofs", "Размещать только на участке, полностью свободном от крыш" }, { "Nothing to remove", "Нечего удалять" }, { "Random rotation:", "Случайный поворот:" }, { "Randomize rotation at generation", "Выбирать случайный поворот при генерации" }, { "Randomize wall stuff at generation", "Выбирать случайный материал стен при генерации" }, { "Randomize wall stuff:", "Случайный материал стен:" }, { "Remove tag...", "Удалить тег..." }, { "Save fuel values from pipenet and vanilla comprefuelable", "Сохранять запас ресурсов трубопровода и топлива стандартного CompRefuelable" }, { "Save fuel:", "Сохранять топливо:" }, { "Save power values from batteries", "Сохранять заряд аккумуляторов" }, { "Save power:", "Сохранять заряд:" }, { "Spawn conduits under impassables and doors automatically", "Автоматически создавать кабели под непроходимыми объектами и дверями" }, { "Stockpile:", "Склад:" }, { "Structure defName:", "Идентификатор постройки (defName):" }, { "Structure export menu", "Экспорт постройки" }, { "Structure tags:", "Теги постройки:" }, { "Tags are used with SettlementLayoutDef", "Теги используются вместе с SettlementLayoutDef" } }, new Dictionary<string,string> { { " tag(s) used", " – число используемых тегов" } }, null) },
            { "KCSG.Dialog_ExportWindow:DrawFooter", new Variant17("KCSG.Dialog_ExportWindow:DrawFooter", null, false, true, new Dictionary<string,string> { { "Cannot use empty defName.", "Идентификатор defName не может быть пустым." }, { "Close", "Закрыть" }, { "Copied to clipboard.", "Скопировано в буфер обмена." }, { "Copy structure", "Скопировать постройку" }, { "Copy symbols", "Скопировать символы" }, { "No new symbols needed.", "Новые символы не нужны." } }, new Dictionary<string,string> { { " symbols to clipboard.", "." }, { "Copied ", "В буфер обмена скопировано символов: " } }, new string[] { "Copy structure", "Copied to clipboard.", "Cannot use empty defName.", "Copy symbols", "_North", "_South", "_East", "_West", "", "", "\n\n", "Copied ", " symbols to clipboard.", "No new symbols needed.", "Close" }) },
            { "PipeSystem.CompAdvancedResourceProcessor+<CompGetGizmosExtra>d__76:MoveNext", new Variant17("PipeSystem.CompAdvancedResourceProcessor+<CompGetGizmosExtra>d__77:MoveNext", new string[] {  }, false, true, new Dictionary<string,string> { { "Advance progress 1 day", "Добавить день прогресса" }, { "Empty wastepack(s)", "Выгрузить пакеты отходов" }, { "Finish in 10 ticks", "Завершить через 10 тиков" } }, null, null) },
            { "PipeSystem.CompProperties_ResourceTrader+<SpecialDisplayStats>d__17:MoveNext", new Variant17("PipeSystem.CompProperties_ResourceTrader+<SpecialDisplayStats>d__17:MoveNext", null, false, true, new Dictionary<string,string> { { "/d", "/д." } }, null, new string[] { "PipeSystem_Consumption", " ", "/d", "PipeSystem_ConsumptionExplained", "PipeSystem_IdleConsumption", " ", "/d", "PipeSystem_IdleConsumptionExplained", "PipeSystem_Production", " ", "/d", "PipeSystem_ProductionExplained" }) },
            { "PipeSystem.CompResourceStorage+<CompGetGizmosExtra>d__42:MoveNext", new Variant17("PipeSystem.CompResourceStorage+<CompGetGizmosExtra>d__44:MoveNext", new string[] {  }, false, true, new Dictionary<string,string> { { "DEBUG: Add 5", "Отладка: добавить 5" }, { "DEBUG: Configure gauge", "Отладка: настроить шкалу" }, { "DEBUG: Empty", "Отладка: опустошить" }, { "DEBUG: Fill", "Отладка: наполнить" } }, null, null) },
            { "PipeSystem.CompResourceTrader:CompInspectStringExtra", new Variant17("PipeSystem.CompResourceTrader:CompInspectStringExtra", null, false, true, new Dictionary<string,string> {  }, new Dictionary<string,string> { { "/d", "/д." } }, null) },
            { "VEF.AnimalGenes.CompAnimalGenes+<CompGetGizmosExtra>d__16:MoveNext", new Variant17("VEF.AnimalGenes.CompAnimalGenes+<CompGetGizmosExtra>d__18:MoveNext", new string[] {  }, false, true, new Dictionary<string,string> { { "DEV: Do birth", "Отладка: вызвать роды" } }, null, null) },
            { "VEF.Sounds.EditWindow_SoundTest:DoWindowContents", new Variant17("VEF.Sounds.EditWindow_SoundTest:DoWindowContents", null, false, true, new Dictionary<string,string> { { "Change sound (currently testing: ", "Сменить звук (сейчас: " }, { "Play sound", "Воспроизвести звук" }, { "Search:", "Поиск:" } }, new Dictionary<string,string> { { " <pitchRange>", ": диапазон высоты тона <pitchRange>" }, { " <volumeRange>", ": диапазон громкости <volumeRange>" }, { "Copy settings of subsound ", "Скопировать настройки подзвука " }, { "Subsound ", "Подзвук " } }, null) },
            { "VEF.Storyteller.PrefabExporter+Dialog_NameMassExport:MassExport", new Variant17("VEF.Storyteller.PrefabExporter+Dialog_NameMassExport:MassExport", null, false, true, new Dictionary<string,string> {  }, new Dictionary<string,string> { { " prefabs to clipboard.", "." }, { "Copied ", "В буфер обмена скопировано заготовок: " } }, new string[] { "VEF_PrefabBorder", "VEF_PrefabBorder", "_", "Copied ", " prefabs to clipboard." }) },
        };
        private static readonly Dictionary<string,Variant17> Version17ByTarget =
            Version17.Values.Where(variant => variant.Enabled).ToDictionary(variant => variant.Target);
        private static bool UseVersion17
        {
            get
            {
                var version = RimWorld.VersionControl.CurrentVersion;
                return version.Major == 1 && version.Minor == 7;
            }
        }

        public static IEnumerable<string> BuildTargets(bool useVersion17)
        {
            foreach (var target in Methods.Keys)
            {
                if (!IsApplicable(target)) continue;
                if (!useVersion17 || !Version17.TryGetValue(target, out var variant))
                    yield return target;
                else if (variant.Enabled)
                    yield return variant.Target;
            }
        }

        private static bool TryGetParameters(string target, bool useVersion17, out string[] parameters)
        {
            if (useVersion17 && Version17ByTarget.TryGetValue(target, out var variant))
            {
                parameters = variant.Parameters;
                return parameters != null;
            }
            return Parameters.TryGetValue(target, out parameters);
        }

        private static bool MissingOptional17Owner(string target)
        {
            return Version17ByTarget.TryGetValue(target, out var variant) && variant.Optional
                && AccessTools.TypeByName(target.Substring(0, target.LastIndexOf(':'))) == null;
        }

        private static bool IsAppendLiteral17(CodeInstruction instruction)
        {
            if ((instruction.opcode != OpCodes.Call && instruction.opcode != OpCodes.Callvirt)
                || !(instruction.operand is MethodInfo method)
                || method.Name != "AppendLiteral") return false;
            string owner = method.DeclaringType.FullName;
            return owner == "System.Runtime.CompilerServices.DefaultInterpolatedStringHandler"
                || owner == "System.Text.StringBuilder+AppendInterpolatedStringHandler";
        }

        private static IEnumerable<CodeInstruction> Transpile17(IEnumerable<CodeInstruction> input, string target, Variant17 variant)
        {
            var instructions = input as IList<CodeInstruction> ?? input.ToList();
            if (variant.Shape != null && !instructions.Where(i => i.opcode == OpCodes.Ldstr)
                .Select(i => (string)i.operand).SequenceEqual(variant.Shape, StringComparer.Ordinal))
            {
                Warn(target, "Source literal shape changed on RimWorld 1.7");
                return instructions;
            }

            var output = new List<CodeInstruction>();
            var exactFound = new HashSet<string>();
            var appendFound = new HashSet<string>();
            var select = AccessTools.Method(typeof(LiteralTranslations), nameof(Select));
            for (int i = 0; i < instructions.Count; i++)
            {
                var instruction = instructions[i];
                output.Add(instruction);
                if (instruction.opcode != OpCodes.Ldstr || !(instruction.operand is string source)) continue;
                string russian;
                if (variant.Append != null && i + 1 < instructions.Count && IsAppendLiteral17(instructions[i + 1])
                    && variant.Append.TryGetValue(source, out russian))
                    appendFound.Add(source);
                else if (variant.Exact != null && variant.Exact.TryGetValue(source, out russian))
                    exactFound.Add(source);
                else continue;
                output.Add(new CodeInstruction(OpCodes.Ldstr, russian));
                output.Add(new CodeInstruction(OpCodes.Call, select));
            }
            var missing = (variant.Exact?.Keys.Except(exactFound) ?? Enumerable.Empty<string>())
                .Concat(variant.Append?.Keys.Except(appendFound) ?? Enumerable.Empty<string>()).ToArray();
            if (missing.Length > 0)
                Warn(target, "Source strings changed on RimWorld 1.7: " + string.Join(", ", missing));
            return output;
        }
    }
    [StaticConstructorOnStartup]
    internal static class DeferredLiterals
    {
        static DeferredLiterals() { LongEventHandler.ExecuteWhenFinished(LiteralTranslations.Apply); }
    }
}
