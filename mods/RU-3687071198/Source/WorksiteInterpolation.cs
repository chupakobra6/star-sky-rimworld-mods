using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;
using Verse;

namespace Igor.WorksitesRussian
{
    // Waylith builds use interpolated-string handlers in places where the Unity
    // build used Format or Concat. Adapt only the reviewed source expressions.
    internal static class WorksiteInterpolation
    {
        private const string Handler = "System.Runtime.CompilerServices.DefaultInterpolatedStringHandler";
        private const string ParleyDialog = "MiningOutpost.Parley.Dialog_Parley:";
        private const string ParleyCalculator = "MiningOutpost.Parley.ParleySuccessCalculator:";
        private const string Defense = "MiningOutpost.WorkSiteDefenseTracker:";

        private enum Shape { Format, StringArray, DefenseVictory, DefenseResolve }

        private static readonly Dictionary<string, Dictionary<string, Shape>> Bindings =
            new Dictionary<string, Dictionary<string, Shape>>
            {
                [ParleyDialog + "CacheHeaderInfo"] = Arrays(
                    "{0} greets {1} {2} and asks what business you want to handle."),
                [ParleyCalculator + "CalculateWithSpecifics"] = Formats(
                    "{0} {1} in the caravan.", "All {0} colonists are unarmed.",
                    "{0} {1} unarmed.", "All {0} colonists are unarmored.", "{0} {1} unarmored.",
                    "{0} war {1} in the caravan.", "{0} additional {1} in the caravan."),
                [ParleyDialog + "BuildRewardDescription"] = Formats(
                    "They will give you {0} {1}.", "They can offer between {0} and {1} {2}.",
                    "They can offer between {0} and {1} silver.", "{0}'s goodwill toward you will increase by {1}.",
                    "All colonists receive +{0} mood for {1} days.", "{0} gains {1}-{2} royal favor."),
                [ParleyCalculator + "GetSpecificityLabel"] = Arrays("Demanding specific {0} ({1})."),
                ["MiningOutpost.Parley.AtonementGenerator:GenerateDemand"] = new Dictionary<string, Shape>
                {
                    ["{0}, {1} is requesting a fine of {2} silver."] = Shape.Format,
                    ["{0}. {1} demands you hand over your prisoner, {2}."] = Shape.StringArray,
                    ["{0}. {1} demands you hand over your colonist, {2}."] = Shape.StringArray,
                    ["{0}, {1} demands that you give him {2}'s {3} as tribute."] = Shape.StringArray,
                },
                [Defense + "FireDefenseRaid"] = Arrays("{0} is attacking {1}. Fight alongside the defenders to drive them away!{2}"),
                [Defense + "OnDefenseSuccess"] = new Dictionary<string, Shape>
                {
                    ["You helped {0} defeat the {1}. They are grateful that {2} came to their aid.\n\nYou will not be able to attack this outpost for {3} days. They are willing to parley and trade with you more favorably in the future."] = Shape.DefenseVictory,
                },
                [Defense + "Resolve"] = new Dictionary<string, Shape>
                {
                    ["{0} defeated {1}. Their ranks were bolstered, their armories were replenished, and they will be holding the site for another {2} days."] = Shape.DefenseResolve,
                },
                [Defense + "OnDefenseOverrun"] = Arrays("Everyone at {0} was killed by {1}. The outpost was destroyed."),
                [Defense + "CheckIdleConspiracy"] = Arrays("Since you are not participating in the defense, {0} suspects you of conspiring with the {1}!"),
                [Defense + "NotifyFriendlyFire"] = Arrays("{0} attacked the worksite {1} {2}."),
                [Defense + "NotifyWorksiteKilled"] = Arrays("{0} killed the worksite {1} {2}."),
                ["MiningOutpost.Patch_WorksiteDefense_UnpoweredTurretNotice:Postfix"] = Arrays("The {0} built by {1} is unpowered."),
                ["MiningOutpost.Orbital.Services.FirefightSim:ApplyWound"] = Formats(
                    "is hit in the {0} for {1} and stops moving.", "is hit in the {0} for {1}, and does not get up.",
                    "takes {0} to the {1} and goes limp.", "takes {0} to the {1} and drops, out of the fight.",
                    "is hit in the {0} for {1}."),
                ["MiningOutpost.Orbital.Services.Dialog_Firefight:OrderHint"] = Formats(
                    "Banking aim ({0} held).", "Reload to {0}.", "Out for {0} round(s), tends a wound."),
                ["MiningOutpost.Orbital.Services.Dialog_Firefight:DrawCrewCard"] = Formats("Tending wounds. Back in {0}."),
                ["MiningOutpost.Orbital.Services.OpportunityRunner:CheckSub"] = Formats("Firefight (DC {0})", "{0} ({1}) - {2}"),
                ["MiningOutpost.Orbital.Services.OpportunityRunner:CheckMath"] = Formats("{0}   vs DC {1} ({2})"),
                ["MiningOutpost.Orbital.Services.OpportunityRunner:BuildCheckTip"] = Formats(
                    "{0} {1} ({2})", "There is a <color={0}>{1}%</color> chance to succeed."),
                ["MiningOutpost.Orbital.Services.OpportunityRunner:DrawResolvePanel"] = Formats(
                    "{0} {1}  +  {2} {3}  +  1d20", "{0} + {1} + {2} = {3}   vs DC {4}"),
                ["MiningOutpost.Orbital.Services.StatLab:SkillFactorLabel"] = Formats("{0}{1} ({2})"),
                ["MiningOutpost.Orbital.Services.StatLab:Explain"] = Formats("{0} {1}÷2"),
                ["MiningOutpost.Orbital.Services.SquadSheetUI:SkillTip"] = Formats(
                    "{0} ({1})", "{0} {1}", "Cooking {0} + Crafting {1}, ÷4 = +{2}", "{0} {1} ÷2 = +{2}"),
            };

        private sealed class Expression
        {
            public int End;
            public string Format;
            public Shape Shape;
            public readonly List<(int Index, Type Type)> Values = new List<(int, Type)>();
            public readonly List<object> Parts = new List<object>();
        }

        private static Dictionary<string, Shape> Formats(params string[] values) =>
            values.ToDictionary(value => value, _ => Shape.Format);

        private static Dictionary<string, Shape> Arrays(params string[] values) =>
            values.ToDictionary(value => value, _ => Shape.StringArray);

        private static bool Russian => LanguageDatabase.activeLanguage?.folderName.StartsWith("Russian", StringComparison.OrdinalIgnoreCase) == true;

        private static readonly MethodInfo RussianGetter = AccessTools.PropertyGetter(typeof(WorksiteInterpolation), nameof(Russian));
        private static readonly MethodInfo ArrayConcat = AccessTools.Method(typeof(string), nameof(string.Concat), new[] { typeof(string[]) });
        private static readonly MethodInfo ObjectConcat = AccessTools.Method(typeof(string), nameof(string.Concat), new[] { typeof(object) });

        private static string Key(MethodBase method) => method == null ? "unknown target" : method.DeclaringType.FullName + ":" + method.Name;

        internal static HarmonyMethod BeforeLiterals(Type owner, string name) =>
            new HarmonyMethod(owner, name) { before = new[] { "igor.ru.3687071198.literals" } };

        internal static List<CodeInstruction> Normalize(List<CodeInstruction> source, MethodBase method, ILGenerator generator)
        {
            if (generator == null || !Bindings.TryGetValue(Key(method), out var bindings)) return source;

            var before = new Dictionary<int, CodeInstruction[]>();
            var after = new Dictionary<int, List<CodeInstruction>>();
            foreach (var expression in FindExpressions(source, bindings))
            {
                var locals = expression.Values.Select(value => generator.DeclareLocal(value.Type)).ToArray();
                for (int i = 0; i < locals.Length; i++)
                {
                    before.Add(expression.Values[i].Index, new[]
                    {
                        new CodeInstruction(OpCodes.Dup),
                        new CodeInstruction(OpCodes.Stloc, locals[i]),
                    });
                }

                // Keep the exact upstream result for all other languages.
                var done = generator.DefineLabel();
                var output = new List<CodeInstruction>
                {
                    new CodeInstruction(OpCodes.Call, RussianGetter),
                    new CodeInstruction(OpCodes.Brfalse, done),
                    new CodeInstruction(OpCodes.Pop),
                };
                Emit(output, expression, locals);
                output.Add(new CodeInstruction(OpCodes.Nop).WithLabels(done));
                after.Add(expression.End, output);
            }

            if (after.Count == 0) return source;
            var result = new List<CodeInstruction>();
            for (int i = 0; i < source.Count; i++)
            {
                var code = new CodeInstruction(source[i]);
                if (before.TryGetValue(i, out var capture))
                {
                    capture[0].MoveLabelsFrom(code);
                    result.AddRange(capture);
                }
                result.Add(code);
                if (after.TryGetValue(i, out var rewrite)) result.AddRange(rewrite);
            }
            return result;
        }

        private static IEnumerable<Expression> FindExpressions(List<CodeInstruction> source, Dictionary<string, Shape> bindings)
        {
            Expression expression = null;
            var format = new StringBuilder();
            object receiver = null;
            int lastHandlerCall = -1;
            for (int i = 0; i < source.Count; i++)
            {
                if (source[i].opcode != OpCodes.Call || !(source[i].operand is MethodBase method)
                    || method.DeclaringType.FullName != Handler) continue;

                int nextPart = lastHandlerCall + 1;
                lastHandlerCall = i;
                if (method.IsConstructor)
                {
                    expression = i >= 3 && IsLocalAddress(source[i - 3])
                        && source[i - 2].LoadsConstant() && source[i - 1].LoadsConstant()
                        && method.GetParameters().Select(p => p.ParameterType).SequenceEqual(new[] { typeof(int), typeof(int) })
                        ? new Expression() : null;
                    receiver = expression == null ? null : source[i - 3].operand;
                    format.Clear();
                }
                else if (expression != null && method.Name == "AppendLiteral" && i >= 2
                    && IsLocalAddress(source[i - 2]) && Equals(receiver, source[i - 2].operand)
                    && source[i - 1].opcode == OpCodes.Ldstr && source[i - 1].operand is string literal)
                {
                    format.Append(literal);
                    expression.Parts.Add(literal);
                }
                else if (expression != null && method.Name == "AppendFormatted"
                    && nextPart < i && IsLocalAddress(source[nextPart]) && Equals(receiver, source[nextPart].operand)
                    && method.GetParameters().Length == 1 && source[i].blocks.Count == 0)
                {
                    var type = method.GetParameters()[0].ParameterType;
                    if (type.IsGenericParameter && method is MethodInfo generic && generic.IsGenericMethod)
                        type = generic.GetGenericArguments()[0];
                    if (type != typeof(string) && type != typeof(int) && type != typeof(float) && type != typeof(double))
                    {
                        expression = null;
                        continue;
                    }
                    int index = expression.Values.Count;
                    format.Append('{').Append(index).Append('}');
                    expression.Values.Add((i, type));
                    expression.Parts.Add(index);
                }
                else if (expression != null && method.Name == "ToStringAndClear" && i > 0
                    && IsLocalAddress(source[i - 1]) && Equals(receiver, source[i - 1].operand)
                    && source[i].blocks.Count == 0 && bindings.TryGetValue(format.ToString(), out var shape))
                {
                    expression.End = i;
                    expression.Format = format.ToString();
                    expression.Shape = shape;
                    yield return expression;
                    expression = null;
                }
                else expression = null;
            }
        }

        private static bool IsLocalAddress(CodeInstruction code) => code.opcode == OpCodes.Ldloca || code.opcode == OpCodes.Ldloca_S;

        private static void Emit(List<CodeInstruction> output, Expression expression, LocalBuilder[] locals)
        {
            switch (expression.Shape)
            {
                case Shape.Format:
                    EmitFormat(output, expression.Format, locals);
                    break;
                case Shape.StringArray:
                    EmitStringArray(output, expression.Parts, locals);
                    break;
                case Shape.DefenseVictory:
                    EmitStringArray(output, new object[]
                    {
                        "You helped ", 0, " defeat the ", 1, ". They are grateful that ", 2, " came to their aid.\n\n",
                        "You will not be able to attack this outpost for {0} days. They are willing to parley and trade with you more favorably in the future.",
                    }, locals, locals[3]);
                    break;
                case Shape.DefenseResolve:
                    EmitStringArray(output, new object[]
                    {
                        0, " defeated ", 1, ". Their ranks were bolstered, their armories were replenished, ",
                        "and they will be holding the site for another {0} days.",
                    }, locals, locals[2]);
                    break;
            }
        }

        private static void EmitFormat(List<CodeInstruction> output, string format, LocalBuilder[] locals)
        {
            output.Add(new CodeInstruction(OpCodes.Ldstr, format));
            Type[] signature;
            if (locals.Length <= 3)
            {
                foreach (var local in locals) EmitBoxed(output, local);
                signature = new[] { typeof(string) }.Concat(locals.Select(_ => typeof(object))).ToArray();
            }
            else
            {
                EmitArray(output, typeof(object), locals.Length, i => EmitBoxed(output, locals[i]));
                signature = new[] { typeof(string), typeof(object[]) };
            }
            output.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(string), nameof(string.Format), signature)));
        }

        private static void EmitStringArray(List<CodeInstruction> output, IList<object> parts, LocalBuilder[] locals, LocalBuilder nestedFormatValue = null)
        {
            EmitArray(output, typeof(string), parts.Count, i =>
            {
                if (parts[i] is int index)
                {
                    EmitBoxed(output, locals[index]);
                    if (locals[index].LocalType != typeof(string))
                        output.Add(new CodeInstruction(OpCodes.Call, ObjectConcat));
                }
                else if (nestedFormatValue != null && i == parts.Count - 1)
                    EmitFormat(output, (string)parts[i], new[] { nestedFormatValue });
                else output.Add(new CodeInstruction(OpCodes.Ldstr, (string)parts[i]));
            });
            output.Add(new CodeInstruction(OpCodes.Call, ArrayConcat));
        }

        private static void EmitArray(List<CodeInstruction> output, Type elementType, int length, Action<int> emitElement)
        {
            output.Add(new CodeInstruction(OpCodes.Ldc_I4, length));
            output.Add(new CodeInstruction(OpCodes.Newarr, elementType));
            for (int i = 0; i < length; i++)
            {
                output.Add(new CodeInstruction(OpCodes.Dup));
                output.Add(new CodeInstruction(OpCodes.Ldc_I4, i));
                emitElement(i);
                output.Add(new CodeInstruction(OpCodes.Stelem_Ref));
            }
        }

        private static void EmitBoxed(List<CodeInstruction> output, LocalBuilder local)
        {
            output.Add(new CodeInstruction(OpCodes.Ldloc, local));
            if (local.LocalType.IsValueType)
                output.Add(new CodeInstruction(OpCodes.Box, local.LocalType));
        }

        internal static List<CodeInstruction> Unchanged(List<CodeInstruction> source, MethodBase method)
        {
            Log.Warning("[Igor Worksites RU] Unrecognized formatting in " + Key(method));
            return source;
        }
    }
}
