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
            } },
            { "ColonyManagerRedux.Managers.ManagerJob_Mining+<>c:<DesignationLabel>b__90_0", new Dictionary<string,string> {
                { "{0}x {1}", "{0} × {1}" }
            } }
            };
        private static readonly Dictionary<string,string[]> Parameters = new Dictionary<string,string[]> {
            { "ColonyManagerRedux.I18n:YieldOne", new string[] { "System.Single", "Verse.ThingDef" } },
            { "ColonyManagerRedux.Managers.ManagerJob_Power:.ctor", new string[] { "ColonyManagerRedux.Manager" } },
            { "ColonyManagerRedux.Managers.ManagerJob_Mining+<>c:<DesignationLabel>b__90_0", new string[] { "Verse.ThingDefCountClass" } }
        };
        public LiteralTranslations(ModContentPack content) : base(content) { }
        public static void Apply()
        {
            var harmony = new Harmony("igor.ru.3310027356.literals");
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
            string message = "[Star Sky RU 3310027356] " + target + ": " + detail;
            Log.WarningOnce(message, message.GetHashCode());
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
            { "ColonyManagerRedux.I18n:YieldOne", new Variant17("ColonyManagerRedux.I18n:YieldOne", new string[] { "System.Single", "Verse.ThingDef" }, false, true, new Dictionary<string,string> {  }, new Dictionary<string,string> { { " x", " ×" } }, null) },
            { "ColonyManagerRedux.Managers.ManagerJob_Mining+<>c:<DesignationLabel>b__90_0", new Variant17("ColonyManagerRedux.Managers.ManagerJob_Mining+<>c:<DesignationLabel>b__90_0", new string[] { "Verse.ThingDefCountClass" }, false, true, new Dictionary<string,string> {  }, new Dictionary<string,string> { { "x ", " × " } }, null) },
            { "ColonyManagerRedux.Managers.ManagerTab_Mining:GetMineralTooltip", new Variant17("ColonyManagerRedux.Managers.ManagerTab_Mining:GetMineralTooltip", null, false, true, new Dictionary<string,string> {  }, new Dictionary<string,string> { { " x", " ×" } }, null) },
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
