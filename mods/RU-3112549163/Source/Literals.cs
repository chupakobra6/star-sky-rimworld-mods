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
