using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace Igor.RU_3771580900
{
    internal static class DawnLiteralVariants
    {
        // The 1.7 assembly uses AppendLiteral instead of the 1.6 composite strings.
        private static readonly Dictionary<string, (string[] Literals, Dictionary<string, string> Translations)> Variants =
            new Dictionary<string, (string[], Dictionary<string, string>)>
            {
                ["DawnNewDay.DawnSettings:ShowAppearanceSection"] = (
                    new[] { ": ", "x", " X (", " px)", " Y (", " px)" },
                    new Dictionary<string, string> { [": "] = ": ×", ["x"] = "", [" px)"] = " пикс.)" }),
                ["DawnNewDay.DawnSettings:<ShowAppearanceSection>b__51_0"] = (
                    new[] { "x" }, new Dictionary<string, string> { ["x"] = "×" }),
                ["DawnNewDay.DawnSettings:ShowExtraSection"] = (
                    new[] { " (", "h)" }, new Dictionary<string, string> { ["h)"] = ":00)" }),
                ["DawnNewDay.DawnSeparator:DoContents"] = (
                    new[] { " (", "%)", " (", " px)", " (", " px)" },
                    new Dictionary<string, string> { [" px)"] = " пикс.)" }),
                ["DawnNewDay.FormatContext:TimeRemaining"] = (
                    new[] { "h" }, new Dictionary<string, string> { ["h"] = " ч" }),
                ["DawnNewDay.DawnFormatterRegex+<>c:<.cctor>b__5_21"] = (
                    new[] { "m" }, new Dictionary<string, string> { ["m"] = " м" }),
                ["DawnNewDay.DawnFormatterRegex+<>c:<.cctor>b__5_22"] = (
                    new[] { "km" }, new Dictionary<string, string> { ["km"] = " км" }),
            };

        internal static Dictionary<string, string> Match(string target, IList<CodeInstruction> instructions)
        {
            if (!Variants.TryGetValue(target, out var variant)) return null;
            var literals = new List<string>();
            for (int i = 0; i < instructions.Count; i++)
            {
                var instruction = instructions[i];
                if (instruction.opcode != OpCodes.Ldstr || !(instruction.operand is string text)) continue;
                bool appendLiteral = i + 1 < instructions.Count && instructions[i + 1].opcode == OpCodes.Call
                    && instructions[i + 1].operand is MethodInfo method && method.Name == "AppendLiteral"
                    && method.DeclaringType.FullName == "System.Runtime.CompilerServices.DefaultInterpolatedStringHandler";
                if (appendLiteral) literals.Add(text);
                else if (variant.Translations.ContainsKey(text)) return null;
            }
            return literals.SequenceEqual(variant.Literals, StringComparer.Ordinal) ? variant.Translations : null;
        }
    }
}
