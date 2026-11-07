using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace Igor.RU_3112549163
{
    public sealed class LiteralTranslations : Mod
    {
        private static readonly Dictionary<string,Dictionary<string,string>> Methods =
            new Dictionary<string,Dictionary<string,string>> {
            { "EBSGFramework.Building_SleepCasket+<GetGizmos>d__27:MoveNext", new Dictionary<string,string> {
                { "DEV: Empty nutrition", "ОТЛ: опустошить запас пищи" },
                { "DEV: Fill nutrition", "ОТЛ: заполнить запас пищи" }
            } },
            { "EBSGFramework.Command_AbilityAutocastToggle:GizmoOnGUI", new Dictionary<string,string> {
                { "DEV: ", "ОТЛ: " }
            } },
            { "EBSGFramework.CompAbilityEffect_PushTarget:CanMoveTarget", new Dictionary<string,string> {
                { "CannotSkipTargetTooLarge", "chupakobra6_3112549163_CannotSkipTargetTooLarge" }
            } },
            { "EBSGFramework.CompAbilityEffect_PushTarget:Valid", new Dictionary<string,string> {
                { "CannotSkipTarget", "chupakobra6_3112549163_CannotSkipTarget" }
            } },
            { "EBSGFramework.CompProperties_AbilityLimitedCharges:.ctor", new Dictionary<string,string> {
                { "charge", "заряд" },
                { "on cooldown", "перезаряжается" }
            } },
            { "EBSGFramework.CompProperties_SpawnBaby:.ctor", new Dictionary<string,string> {
                { "born", "рождение" }
            } },
            { "EBSGFramework.CompUseEffect_OffsetComaCapacity+<SpecialDisplayStats>d__4:MoveNext", new Dictionary<string,string> {
                { "EBSG_ComaRestCapacityDesc", "chupakobra6_3112549163_ComaCapacityDescription" }
            } },
            { "EBSGFramework.ConditionalStatAffecter_WorldLayer:GetLabel", new Dictionary<string,string> {
                { "EBSG_CorrectLayer", "chupakobra6_3112549163_EBSG_CorrectLayer" }
            } },
            { "EBSGFramework.EquipRestrictExtension:CanEquipTraitCheck", new Dictionary<string,string> {
                { "EBSG_HediffRestrictedEquipment_None", "chupakobra6_3112549163_TraitRestriction" }
            } },
            { "EBSGFramework.GeneGizmo_ComaRestCapacity:GizmoOnGUI", new Dictionary<string,string> {
                { "EBSG_ComaRestCapacityDesc", "chupakobra6_3112549163_ComaCapacityDescription" }
            } },
            { "EBSGFramework.GeneGizmo_ResourceGene:GetTooltip", new Dictionary<string,string> {
                { "NeverConsumeResource", "chupakobra6_3112549163_NeverConsumeResource" }
            } },
            { "EBSGFramework.Gene_Coma+<GetGizmos>d__66:MoveNext", new Dictionary<string,string> {
                { "DEV: Set capacity", "ОТЛ: задать лимит подключений" },
                { "DEV: Wake and apply bonuses", "ОТЛ: пробудить и применить бонусы" }
            } },
            { "EBSGFramework.HediffComp_Modular:RecacheGizmo", new Dictionary<string,string> {
                { "No ejectable modules", "Нет модулей для извлечения" }
            } },
            { "EBSGFramework.HediffComp_RemoveWhenDamaged:CompDebugString", new Dictionary<string,string> {
                { "Remaining: {0}", "Осталось: {0}" }
            } },
            { "EBSGFramework.HediffComp_TieredRegeneration:CompDebugString", new Dictionary<string,string> {
                { "Not currently healing", "Сейчас лечение не идёт" },
                { "heal : {0} in {1}", "лечение: {0} через {1}" },
                { "regrowTicksRemaining : {0}", "до отрастания (тики): {0}" }
            } },
            { "EBSGFramework.IngestionOutcomeDoer_OffsetResource+<SpecialDisplayStats>d__3:MoveNext", new Dictionary<string,string> {
                { "Resource", "chupakobra6_3112549163_Resource" },
                { "ResourceDesc", "chupakobra6_3112549163_ResourceDesc" }
            } },
            { "EBSGFramework.ModuleSlot:.ctor", new Dictionary<string,string> {
                { "Undefined", "Без названия" }
            } },
            { "EBSGFramework.StatPart_FertilityByGenes:ExplanationPart", new Dictionary<string,string> {
                { "\nAt least one gene stops fertility from going above a certain value.", "\nОдин или несколько генов не позволяют плодовитости подняться выше определённого значения." },
                { "\nAt least one gene stops fertility from going below a certain value.", "\nОдин или несколько генов не позволяют плодовитости опуститься ниже определённого значения." },
                { "\nThis pawn has fertility limiting genes that are not active due to sterilization.", "\nГены, ограничивающие плодовитость, не действуют из-за стерилизации." }
            } },
            { "EBSGFramework.VanishingGene:TickInterval", new Dictionary<string,string> {
                { " has been successfully removed!", " — ген удалён." }
            } }
            };
        private static readonly Dictionary<string,string[]> Parameters = new Dictionary<string,string[]> {

        };
        public LiteralTranslations(ModContentPack content) : base(content) { }
        public static void Apply()
        {
            var harmony = new Harmony("igor.ru.3112549163.literals");
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
            string message = "[Star Sky RU 3112549163] " + target + ": " + detail;
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
            { "EBSGFramework.HediffComp_RemoveWhenDamaged:CompDebugString", new Variant17("EBSGFramework.HediffComp_RemoveWhenDamaged:CompDebugString", null, false, true, new Dictionary<string,string> {  }, new Dictionary<string,string> { { "Remaining: ", "Осталось: " } }, null) },
            { "EBSGFramework.HediffComp_TieredRegeneration:CompDebugString", new Variant17("EBSGFramework.HediffComp_TieredRegeneration:CompDebugString", null, false, true, new Dictionary<string,string> { { "Not currently healing", "Сейчас лечение не идёт" } }, new Dictionary<string,string> { { " in ", " через " }, { "heal : ", "лечение: " }, { "regrowTicksRemaining : ", "до отрастания (тики): " } }, new string[] { "Not currently healing", "heal : ", " in ", "regrowTicksRemaining : " }) },
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
