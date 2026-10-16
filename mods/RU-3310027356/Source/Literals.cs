using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace Igor.RU_3310027356
{
    public sealed class LiteralTranslations : Mod
    {
        private static readonly Dictionary<string,Dictionary<string,string>> Methods =
            new Dictionary<string,Dictionary<string,string>> {
            { "ColonyManagerRedux.CompManagerStation+<CompGetGizmosExtra>d__4:MoveNext", new Dictionary<string,string> {
                { "DEV: Run manager job", "Разработчикам: выполнить поручение" },
                { "DEV: Running manager job...", "Разработчикам: поручение выполняется…" }
            } },
            { "ColonyManagerRedux.CompManagerStation:<CompGetGizmosExtra>b__4_1", new Dictionary<string,string> {
                { "No manager jobs currently need to run.", "Сейчас нет поручений, требующих выполнения." }
            } },
            { "ColonyManagerRedux.DetailedLegendRenderer:DrawDetailedLegend", new Dictionary<string,string> {
                { " (error:\n", " (ошибка:\n" }
            } },
            { "ColonyManagerRedux.I18n:YieldOne", new Dictionary<string,string> {
                { "{0} x{1:F0}", "{0} ×{1:F0}" }
            } },
            { "ColonyManagerRedux.JobTracker:JobStatuses", new Dictionary<string,string> {
                { "All jobs: ", "Все поручения: " },
                { "Has no jobs: ", "Поручений нет: " },
                { "Job count: ", "Количество поручений: " },
                { "Next job: ", "Следующее поручение: " },
                { "Next jobs in order of priority: ", "Следующие поручения в порядке приоритета: " }
            } },
            { "ColonyManagerRedux.ManagerJob:ToString", new Dictionary<string,string> {
                { "Active: ", "Активно: " },
                { "HasBeenUpdated: ", "Обновлялось: " },
                { "Interval: ", "Интервал: " },
                { "IsManaged: ", "Под управлением: " },
                { "IsSuspended: ", "Приостановлено: " },
                { "JobCreatedTick: ", "Тик создания: " },
                { "JobState: ", "Состояние поручения: " },
                { "LastActionTick: ", "Тик последнего действия: " },
                { "Load ID:", "Идентификатор загрузки:" },
                { "Priority: ", "Приоритет: " },
                { "ShouldDoNow: ", "Выполнить сейчас: " },
                { "ShouldUpdate: ", "Нуждается в обновлении: " },
                { "TicksSinceLastUpdate: ", "Тиков с обновления: " },
                { "TicksSinceShouldUpdate: ", "Тиков после срока обновления: " }
            } },
            { "ColonyManagerRedux.ManagerTab:DoTabContents", new Dictionary<string,string> {
                { "Copy to clipboard", "Копировать в буфер обмена" },
                { "Exception copied to clipboard.", "Сведения об ошибке скопированы в буфер обмена." }
            } },
            { "ColonyManagerRedux.Managers.Dialog_MiningDebugOptions:DoListingItems", new Dictionary<string,string> {
                { "AllowedBuilding", "Разрешённая постройка" },
                { "AllowedMineral", "Разрешённое ископаемое" },
                { "DrawPath", "Показать путь" },
                { "DrawSupportGrid", "Показать сетку опор" },
                { "Faction", "Фракция" },
                { "GetBaseCenter", "Центр базы" },
                { "IsInAllowedArea", "В разрешённой зоне" },
                { "IsReachable", "Доступна" },
                { "IsRelevantDeconstructionTarget", "Подходящая цель разборки" },
                { "IsRelevantMiningTarget", "Подходящая цель добычи" },
                { "IsRoofSupport: advanced", "Опора крыши: точная проверка" },
                { "IsRoofSupport: basic", "Опора крыши: простая проверка" },
                { "IsRoomDivider", "Стена между помещениями" },
                { "IsValidDeconstructionTarget", "Допустимая цель разборки" },
                { "IsValidMiningTarget", "Допустимая цель добычи" }
            } },
            { "ColonyManagerRedux.Managers.ManagerJob_Mining+<>c:<DesignationLabel>b__84_0", new Dictionary<string,string> {
                { "{0}x {1}", "{0} × {1}" }
            } },
            { "ColonyManagerRedux.Managers.ManagerJob_Power:.ctor", new Dictionary<string,string> {
                { "W", "Вт" }
            } },
            { "ColonyManagerRedux.Managers.ManagerTab_ImportExport:get_CurrentSaveNameBase", new Dictionary<string,string> {
                { "ManagerJobs_", "Поручения_" },
                { "Template_", "Шаблон_" }
            } },
            { "ColonyManagerRedux.Managers.ManagerTab_Livestock:DoMainContent", new Dictionary<string,string> {
                { "ColonyManagerRedux.Thresholds.DeleteBillTooltip", "chupakobra6_3310027356_DeleteJob" },
                { "ColonyManagerRedux.Thresholds.ManageBillTooltip", "chupakobra6_3310027356_ManageJob" }
            } },
            { "ColonyManagerRedux.Managers.ManagerTab_Livestock:GetTrainerLabel", new Dictionary<string,string> {
                { "BUG: INVALID", "ОШИБКА: НЕДОПУСТИМОЕ ЗНАЧЕНИЕ" }
            } },
            { "ColonyManagerRedux.Managers.ManagerTab_Mining:DoMainContent", new Dictionary<string,string> {
                { "DEV: Debug Options", "Разработчикам: параметры отладки" }
            } },
            { "ColonyManagerRedux.Managers.ManagerTab_Mining:GetMineralTooltip", new Dictionary<string,string> {
                { "{0} x{1}", "{0} ×{1}" }
            } },
            { "ColonyManagerRedux.Managers.ManagerTab_Power:GetSubLabel", new Dictionary<string,string> {
                { "{0} producers, {1} consumers, {2} batteries", "Генераторов: {0}, потребителей: {1}, аккумуляторов: {2}" }
            } },
            { "ColonyManagerRedux.Managers.ManagerTab_Power:get_DisabledReason", new Dictionary<string,string> {
                { "Not sure. It should be enabled? Send a bug report.", "Неизвестная причина: раздел должен быть доступен. Сообщите об ошибке." }
            } },
            { "ColonyManagerRedux.Managers.Trigger_PawnKind:GetTooltip", new Dictionary<string,string> {
                { "This job is in an error state because the game could not find the PawnKindDef it expected for this job. The likeliest causes for this is that the PawnKindDef was renamed by a mod, removed from a mod, or that the mod that added it itself was disabled or removed.", "Поручение не работает, потому что игра не нашла ожидаемый тип существа (PawnKindDef). Вероятнее всего, определение переименовано или удалено из мода либо сам добавлявший его мод отключён или удалён." }
            } },
            { "ColonyManagerRedux.Managers.Trigger_PawnKind:get_ExpectedPawnKindName", new Dictionary<string,string> {
                { "']", "»]" },
                { "[PawnKindDef was saved as '", "[Тип существа сохранён как «" }
            } },
            { "ColonyManagerRedux.WindowTriggerThresholdDetails:DoWindowContents", new Dictionary<string,string> {
                { "Unknown operator", "Неизвестный оператор" }
            } }
            };
        private static readonly Dictionary<string,string[]> Parameters = new Dictionary<string,string[]> {
            { "ColonyManagerRedux.I18n:YieldOne", new string[] { "System.Single", "Verse.ThingDef" } },
            { "ColonyManagerRedux.Managers.ManagerJob_Power:.ctor", new string[] { "ColonyManagerRedux.Manager" } }
        };
        public LiteralTranslations(ModContentPack content) : base(content) { }
        public static void Apply()
        {
            var harmony = new Harmony("igor.ru.3310027356.literals");
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
