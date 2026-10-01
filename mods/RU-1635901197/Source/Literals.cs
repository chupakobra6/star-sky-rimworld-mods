using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace Igor.RU_1635901197
{
    public sealed class LiteralTranslations : Mod
    {
        private static readonly Dictionary<string,Dictionary<string,string>> Methods =
            new Dictionary<string,Dictionary<string,string>> {
            { "FacialAnimation.FacialAnimationModSettings:DrawDev", new Dictionary<string,string> {
                { "Debug Info", "Отладочная информация" },
                { "RenTex:{0},Mat:{1},Graphic:{2}", "Текстуры: {0}, материалы: {1}, графика: {2}" },
                { "Output Tex Count", "Записывать число текстур в журнал" },
                { "Outout Logs", "Записать данные в журнал" },
                { "Re-draw RenTex", "Перерисовать текстуры" }
            } },
            { "FacialAnimation.FacialAnimationModSettings:DoWindowContents", new Dictionary<string,string> {
                { "Reset", "Сбросить" }
            } },
            { "FacialAnimation.NL_SelectPartWindow:DrawAnimationTempParam", new Dictionary<string,string> {
                { "Mood:{0:0.00}", "Настроение: {0:0.00}" },
                { "Pain:{0:0.00}", "Боль: {0:0.00}" }
            } },
            { "FacialAnimation.NL_SelectPartWindow:DrawDevModeOption", new Dictionary<string,string> {
                { "Debug Tools for Illustrators", "Инструменты для художников" },
                { "Grid: {0:0}", "Сетка: {0:0}" },
                { "Hot-reload PNGs", "Обновлять PNG без перезапуска" },
                { "e.g.,\\Mods\\InDevMod\\Textures", "Пример: \\Mods\\InDevMod\\Textures" },
                { "Enable hot-reload PNGs.", "Включить автоматическое обновление PNG." },
                { "Try to update PNGs.", "Попытаться обновить PNG." }
            } },
            { "FacialAnimation.NL_SelectPartWindow:DrawTemporaryAnimationList", new Dictionary<string,string> {
                { "Debug Tool for Animators", "Инструмент для аниматоров" },
                { "Hot-reload Defs", "Обновлять определения без перезапуска" },
                { "e.g.,\\Mods\\InDevMod\\Defs", "Пример: \\Mods\\InDevMod\\Defs" },
                { "Enable hot-reload Defs.", "Включить автоматическое обновление определений." },
                { "Try to update Defs.", "Попытаться обновить определения." },
                { "Play Repeat", "Повторять" },
                { "Play Temporary Animation", "Воспроизвести временную анимацию" }
            } }
            };
        private static readonly Dictionary<string,string[]> Parameters = new Dictionary<string,string[]> {

        };
        public LiteralTranslations(ModContentPack content) : base(content) { }
        public static void Apply()
        {
            var harmony = new Harmony("igor.ru.1635901197.literals");
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
