using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace Igor.RU_2800857642
{
    public sealed class LiteralTranslations : Mod
    {
        private static readonly Dictionary<string,Dictionary<string,string>> Methods =
            new Dictionary<string,Dictionary<string,string>> {
            { "MapPreview.MapPreviewWidget:MakeTooltip", new Dictionary<string,string> {
                { "None", "Нет" }
            } },
            { "MapPreview.MapPreviewMod:SettingsCategory", new Dictionary<string,string> {
                { "Map Preview", "Map Preview — предпросмотр карты" }
            } },
            { "MapPreview.MapPreviewToolbar+ButtonOpenSettings:BiomeWorkerScoresDebugGUI", new Dictionary<string,string> {
                { "Biome worker scores for selected tile", "Оценки пригодности биомов для выбранного участка" }
            } },
            { "MapPreview.MapPreviewWidgetWithPreloader:HandleError", new Dictionary<string,string> {
                { "Map preview generation failed because one of your mods is broken. Please validate your mod files and make sure that your load order is correct. Most importantly, do not put any mods above 'Core' unless they specifically say so in their description!", "Не удалось создать предпросмотр карты из-за неисправности одного из модов. Проверьте целостность файлов модов и порядок загрузки. Не размещайте моды выше Core, если этого прямо не требует их описание!" }
            } },
            { "MapPreview.MapSeedRerollWindow:DoWindowContents", new Dictionary<string,string> {
                { "Original", "Исходное" }
            } },
            { "LunarFramework.Utility.LunarModSettings:DoSettingsWindowContents", new Dictionary<string,string> {
                { "An error occured while loading this mod. Check the log file for more information.", "При загрузке этого мода произошла ошибка. Подробности — в журнале игры." }
            } },
            { "LunarFramework.Bootstrap.Entrypoint:OnError", new Dictionary<string,string> {
                { "Failed to load mod '", "Не удалось загрузить мод «" },
                { "', ", "», " },
                { "' because its files are damaged or incomplete. Redownload the mod to fix this problem.", "»: его файлы повреждены или загружены не полностью. Скачайте мод заново." }
            } },
            { "LunarFramework.Bootstrap.Entrypoint:OnConflict", new Dictionary<string,string> {
                { "Failed to load mod '", "Не удалось загрузить мод «" },
                { "' because it is incompatible with '", "» из-за несовместимости с модом «" },
                { "'.", "»." },
                { "' because it is incompatible with old versions of '", "» из-за несовместимости со старыми версиями мода «" },
                { "'. Update '", "». Обновите «" },
                { "' to version ", "» до версии " },
                { " or newer to fix this problem.", " или новее." }
            } },
            { "LunarFramework.Bootstrap.Entrypoint:OnGameOutdated", new Dictionary<string,string> {
                { "it requires RimWorld version ", "требуется RimWorld версии " },
                { " or later.", " или новее." },
                { "Failed to load mod '", "Не удалось загрузить мод «" },
                { "' because it ", "»: " },
                { "requires RimWorld version {0} or newer. You currently have RimWorld ", "требуется RimWorld версии {0} или новее. У вас установлена версия " },
                { "version ", "" },
                { ", please update the game. \n\nIf you are using Steam, make sure that there is no old beta channel selected. \n[ RimWorld -> Properties -> Betas -> Beta Participation -> set to None ]", ". Обновите игру.\n\nЕсли вы используете Steam, убедитесь, что не выбрана старая бета-версия.\n[ RimWorld → Свойства → Бета-версии → Участие в бета-тестировании → Нет ]" }
            } },
            { "LunarFramework.Bootstrap.Entrypoint:ShowMessageAfterStartup", new Dictionary<string,string> {
                { "You can download the latest version from the project's GitHub Releases page.", "Последнюю версию можно скачать в разделе Releases на странице проекта в GitHub." },
                { "If you are using Steam, simply unsubscribe from '", "Если вы используете Steam, отпишитесь от мода «" },
                { "', then restart Steam and resubscribe. This will force Steam to redownload the mod files and update them to the latest version.", "», перезапустите Steam и подпишитесь заново. Steam повторно скачает файлы мода и обновит его до последней версии." },
                { "Open GitHub page", "Открыть страницу GitHub" },
                { "Open Workshop page", "Открыть страницу Мастерской" }
            } },
            { "HugsLogPublisher.HugsLibUtility+<>c__DisplayClass8_0:<AwaitUnityWebResponse>g__PollingAction|0", new Dictionary<string,string> {
                { "timed out", "время ожидания истекло" },
                { "{0} replied with {1}: {2}", "{0}: ответ {1}: {2}" },
                { "UnityWebRequest failed", "Ошибка UnityWebRequest" }
            } },
            { "HugsLogPublisher.LogPublisher:AbortUpload", new Dictionary<string,string> {
                { "Aborted by user", "Отправка прервана пользователем" }
            } },
            { "HugsLogPublisher.LogPublisher:BeginUpload", new Dictionary<string,string> {
                { "Failed to collect data", "Не удалось собрать данные" }
            } },
            { "HugsLogPublisher.LogPublisher:OnUploadComplete", new Dictionary<string,string> {
                { "Failed to parse response", "Не удалось обработать ответ" }
            } },
            { "HugsLogPublisher.Dialog_PublishLogs:DoWindowContents", new Dictionary<string,string> {
                { "HugsLogPublisher.errorTryAlternativePlatformHint", "chupakobra6_2800857642_errorTryAlternativePlatformHint" }
            } },
            { "HugsLogPublisher.Dialog_PublishLogsOptions:DoWindowContents", new Dictionary<string,string> {
                { "HugsLogPublisher.usePreviousLog", "chupakobra6_2800857642_usePreviousLog" },
                { "HugsLogPublisher.usePreviousLog_tip", "chupakobra6_2800857642_usePreviousLog_tip" },
                { "HugsLogPublisher.alternativePlatform", "chupakobra6_2800857642_alternativePlatform" },
                { "HugsLogPublisher.alternativePlatform_tip", "chupakobra6_2800857642_alternativePlatform_tip" },
                { "HugsLogPublisher.githubToken", "chupakobra6_2800857642_githubToken" },
                { "HugsLogPublisher.githubToken_tip", "chupakobra6_2800857642_githubToken_tip" }
            } },
            { "HugsLogPublisher.Patches.Patch_LudeonTK_EditWindow_Log:DoWindowContents_Postfix", new Dictionary<string,string> {
                { "HugsLogPublisher.shareBtnDescr", "chupakobra6_2800857642_shareBtnDescr" }
            } }
            };
        private static readonly Dictionary<string,string[]> Parameters = new Dictionary<string,string[]> {
            { "LunarFramework.Bootstrap.Entrypoint:OnError", new string[] { "LunarFramework.Bootstrap.LunarMod", "System.String", "System.Boolean", "System.Exception" } }
        };
        public LiteralTranslations(ModContentPack content) : base(content) { }
        public static void Apply()
        {
            var harmony = new Harmony("igor.ru.2800857642.literals");
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
            string message = "[Star Sky RU 2800857642] " + target + ": " + detail;
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
            { "HugsLogPublisher.HugsLibUtility+<>c__DisplayClass8_0:<AwaitUnityWebResponse>g__PollingAction|0", new Variant17("HugsLogPublisher.HugsLibUtility+<>c__DisplayClass8_0:<AwaitUnityWebResponse>g__PollingAction|0", null, false, true, new Dictionary<string,string> { { "UnityWebRequest failed", "Ошибка UnityWebRequest" }, { "timed out", "время ожидания истекло" } }, new Dictionary<string,string> { { " replied with ", ": ответ " } }, null) },
            { "LunarFramework.Bootstrap.Entrypoint:OnGameOutdated", new Variant17("LunarFramework.Bootstrap.Entrypoint:OnGameOutdated", null, false, true, new Dictionary<string,string> { { " or later.", " или новее." }, { "' because it ", "»: " }, { "Failed to load mod '", "Не удалось загрузить мод «" }, { "If you are using Steam, make sure that there is no old beta channel selected. \n[ RimWorld -> Properties -> Betas -> Beta Participation -> set to None ]", "Если вы используете Steam, убедитесь, что не выбрана старая бета-версия.\n[ RimWorld → Свойства → Бета-версии → Участие в бета-тестировании → Нет ]" }, { "it requires RimWorld version ", "требуется RimWorld версии " }, { "version ", "" } }, new Dictionary<string,string> { { " or newer. You currently have RimWorld ", " или новее. У вас установлена версия " }, { ", please update the game. \n\n", ". Обновите игру.\n\n" }, { "requires RimWorld version ", "требуется RimWorld версии " } }, new string[] { "it requires RimWorld version ", " or later.", "Failed to load mod '", "' because it ", "requires RimWorld version ", " or newer. You currently have RimWorld ", "version ", ", please update the game. \n\n", "If you are using Steam, make sure that there is no old beta channel selected. \n[ RimWorld -> Properties -> Betas -> Beta Participation -> set to None ]" }) },
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
