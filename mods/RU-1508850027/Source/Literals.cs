using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace Igor.RU_1508850027
{
    public sealed class LiteralTranslations : Mod
    {
        private static readonly Dictionary<string,Dictionary<string,string>> Methods =
            new Dictionary<string,Dictionary<string,string>> {
            { "RimHUD.Interface.ListingPlus:LinkLabel", new Dictionary<string,string> {
                { "Click to visit URL:\n", "Нажмите, чтобы открыть ссылку:\n" }
            } },
            { "RimHUD.Interface.Hud.HudTimings+Timing:Finish", new Dictionary<string,string> {
                { "[[Max={0}, Now={1} ms]", "[[Макс={0}, сейчас={1} мс]" },
                { "[M={0}, N={1}]", "[М={0}, С={1}]" }
            } },
            { "RimHUD.Interface.Hud.Layout.LayoutElement:GetLabel", new Dictionary<string,string> {
                { "[Missing Def]", "[Определение отсутствует]" }
            } },
            { "RimHUD.Interface.Dialog.Dialog_Config:DrawContent", new Dictionary<string,string> {
                { "Version 1.17.5", "Версия 1.17.5" },
                { " [DEV MODE - Shift+Click to disable]", " [РЕЖИМ РАЗРАБОТЧИКА — Shift + щелчок для отключения]" }
            } },
            { "RimHUD.Interface.Dialog.Dialog_Error:.ctor", new Dictionary<string,string> {
                { "RimHUD Auto-deactivation reason:\n", "Причина автоматического отключения RimHUD:\n" }
            } },
            { "RimHUD.Interface.Dialog.Dialog_Error:DoWindowContents", new Dictionary<string,string> {
                { "RimHUD v1.17.5 has automatically deactivated due to the following error(s):", "RimHUD 1.17.5 автоматически отключён из-за следующих ошибок:" },
                { "Copy to clipboard", "Копировать в буфер обмена" },
                { "RimHUD Auto-deactivation details copied to clipboard", "Сведения об автоматическом отключении RimHUD скопированы в буфер обмена" },
                { "Trace:", "Стек вызовов:" },
                { "Reactivate", "Включить снова" },
                { "Close", "Закрыть" }
            } },
            { "RimHUD.Engine.Credits:Draw", new Dictionary<string,string> {
                { "This mod is kindly supported by:", "Этот мод поддерживают:" },
                { "A special thanks to all above and to countless others who have helped along the way.", "Особая благодарность всем перечисленным и многим другим, кто помогал в работе над модом." },
                { "For more information, check out the mod description", "Подробности — в описании мода" },
                { "(None)", "(Пока никого)" }
            } },
            { "RimHUD.Engine.Credits+Entry:FromXml", new Dictionary<string,string> {
                { " (Click to see link)", " (нажмите, чтобы открыть ссылку)" }
            } },
            { "RimHUD.Engine.Report+ErrorInfo:.ctor", new Dictionary<string,string> {
                { "(No stacktrace)", "(Стек вызовов отсутствует)" },
                { "This is a duplicate error and no longer contains useful information to report.", "Эта ошибка повторяется; в ней больше нет новых сведений для отчёта." },
                { "Repeated deactivations, check full log", "Повторные отключения — проверьте полный журнал" },
                { "Clicking Reactivate will reset your config in order to avoid this error.", "При нажатии «Включить снова» настройки будут сброшены, чтобы предотвратить повторение этой ошибки." },
                { "This error appears to have been triggered by:\n\n", "По-видимому, ошибку вызвали:\n\n" },
                { "\n\nPlease consider reporting this issue to the relevant mod author(s).", "\n\nСообщите об этой проблеме авторам соответствующих модов." },
                { "The error appears to have triggered outside of RimHUD", "По-видимому, ошибка возникла за пределами RimHUD" }
            } },
            { "RimHUD.Engine.Report+ErrorInfo:CopyToClipboard", new Dictionary<string,string> {
                { "[[RimHUD v1.17.5 Auto-deactivation report]]\n", "[[Отчёт об автоматическом отключении RimHUD 1.17.5]]\n" },
                { "(This error appears to have been triggered by '", "(По-видимому, ошибку вызвал мод «" },
                { "\n\nStacktrace:\n", "\n\nСтек вызовов:\n" },
                { "')\n\n", "»)\n\n" }
            } },
            { "RimHUD.Interface.Dialog.Tabs.Tab_ConfigCredits:get_Label", new Dictionary<string,string> {
                { "Credits", "Благодарности" }
            } },
            { "RimHUD.Interface.Hud.Models.Values.GenderRaceAndAgeValue:GetValue", new Dictionary<string,string> {
                { "{0} {1}", "{0} г. {1}" }
            } }
            };
        private static readonly Dictionary<string,string[]> Parameters = new Dictionary<string,string[]> {
            { "RimHUD.Interface.Dialog.Dialog_Error:.ctor", new string[] { "RimHUD.Engine.Report+ErrorInfo" } },
            { "RimHUD.Engine.Report+ErrorInfo:.ctor", new string[] { "System.Exception" } }
        };
        public LiteralTranslations(ModContentPack content) : base(content) { }
        public static void Apply()
        {
            var harmony = new Harmony("igor.ru.1508850027.literals");
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
