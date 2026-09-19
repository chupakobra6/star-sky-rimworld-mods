using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace Igor.RU_3694533072
{
    public sealed class LiteralTranslations : Mod
    {
        private static readonly Dictionary<string,Dictionary<string,string>> Methods =
            new Dictionary<string,Dictionary<string,string>> {
            { "SeamlessDoors.SeamlessDoorsMod:DoSettingsWindowContents", new Dictionary<string,string> {
                { "Automatic floor proxy (recommended)", "Автоматическое скрытое покрытие (рекомендуется)" },
                { "Odyssey fix (Needed)", "Исправление для Odyssey (необходимо)" },
                { "On: an invisible proxy floor is placed automatically under every door, so adjacent floor tiles always render clean edges and the seamless blend is always active.\n\nOff: classic mode — seamless blending only activates when you manually place a floor tile under a door.", "Вкл.: под каждой дверью автоматически размещается невидимое служебное покрытие. Соседние клетки пола получают ровные края, а переход между ними всегда остаётся бесшовным.\n\nВыкл.: классический режим — бесшовный переход работает только там, где вы вручную уложили пол под дверью." },
                { "Corrects visual behaviour for Odyssey-specific terrain.\n\nOn: the proxy floor is skipped on Odyssey terrain (vacstone, orbital platforms, etc.) that uses its own rendering pipeline. Gravship substructure is handled separately and is unaffected.\n\nOff: the proxy is placed on all Odyssey terrain. Disabling this will likely cause broken visuals on Odyssey maps and is not recommended.", "Исправляет отображение покрытий из Odyssey.\n\nВкл.: служебное покрытие не размещается на поверхностях Odyssey с особой отрисовкой: вакуумном камне, орбитальных платформах и т. п. Основание гравикорабля обрабатывается отдельно; этот параметр на него не влияет.\n\nВыкл.: служебное покрытие размещается на всех поверхностях Odyssey. Это может нарушить их отображение на картах Odyssey, поэтому отключать параметр не рекомендуется." }
            } }
            };
        private static readonly Dictionary<string,string[]> Parameters = new Dictionary<string,string[]> {
            { "SeamlessDoors.SeamlessDoorsMod:DoSettingsWindowContents", new string[] { "UnityEngine.Rect" } }
        };
        public LiteralTranslations(ModContentPack content) : base(content)
        {
            var harmony = new Harmony("igor.ru.3694533072.literals");
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
}
