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
