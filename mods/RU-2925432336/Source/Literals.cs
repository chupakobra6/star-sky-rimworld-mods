using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace Igor.RU_2925432336
{
    public sealed class LiteralTranslations : Mod
    {
        private static readonly Dictionary<string,Dictionary<string,string>> Methods =
            new Dictionary<string,Dictionary<string,string>> {
            { "BigAndSmall.PrerequisiteValidator:ValidationDescription", new Dictionary<string,string> {
                { "ERROR", "ОШИБКА" }
            } },
            { "BigAndSmall.CompProperties_UseConditionQuantity:.ctor", new Dictionary<string,string> {
                { "Needs at least 1 to use.", "Для использования требуется хотя бы 1 шт." }
            } },
            { "BigAndSmall.Dialog_PickGenes:DoWindowContents", new Dictionary<string,string> {
                { "Available to Incorporate:", "Доступны для усвоения:" },
                { "'s genes:", ": гены" },
                { "Current Metabolic Efficiency: {0}", "Текущая эффективность метаболизма: {0}" },
                { " -> <color={0}>{1}</color> (Min: {2})", " → <color={0}>{1}</color> (минимум: {2})" },
                { " (Min: {0})", " (минимум: {0})" },
                { "Warning: Metabolism will be too low. Random genes will be removed!", "Метаболизм станет слишком низким. Случайные гены будут удалены!" },
                { "Incorporate Gene", "Усвоить ген" }
            } },
            { "BigAndSmall.ProductionGeneSettings:.ctor", new Dictionary<string,string> {
                { "NameMissing", "Название не задано" }
            } },
            { "BigAndSmall.CompAbilityEffect_SlimeCost:GizmoDisabled", new Dictionary<string,string> {
                { "Ability Disabled: Missing Required Power Gene", "Способность недоступна: нет необходимого гена, создающего запас слизи." },
                { "Ability Disabled: Not enough Power", "Способность недоступна: недостаточно слизи." }
            } },
            { "BigAndSmall.EngulfHediff:get_LabelInBrackets", new Dictionary<string,string> {
                { "FULLNESS CALCULATION FAILED", "ОШИБКА РАСЧЁТА НАПОЛНЕННОСТИ" }
            } },
            { "BigAndSmall.SoulCollector:get_LabelInBrackets", new Dictionary<string,string> {
                { "SPIRIT POWER CALCULATION FAILED", "ОШИБКА РАСЧЁТА СИЛЫ ДУШИ" }
            } },
            { "BigAndSmall.GreaterDeathless:PostAdd", new Dictionary<string,string> {
                { "Death refusal", "Отказ от смерти" }
            } },
            { "BigAndSmall.ReturningSoul:Notify_PawnDied", new Dictionary<string,string> {
                { "Returning soul", "Возвращение души" }
            } },
            { "BigAndSmall.DraculVampirism:PostTick", new Dictionary<string,string> {
                { "Half-Vampire", "Полувампир" }
            } },
            { "BigAndSmall.ProductionHediffSettings:.ctor", new Dictionary<string,string> {
                { "NameMissing", "Название не задано" }
            } },
            { "BigAndSmall.ProductionHediffSettings+ProductionSettings:ProductTooltip", new Dictionary<string,string> {
                { "ProductLabelMissing", "Название продукта не задано" }
            } },
            { "BigAndSmall.ShrinkRayHediff:KillTarget", new Dictionary<string,string> {
                { "{0} popped out of existance", "Пуф! {0} исчезает без следа." }
            } },
            { "BigAndSmall.JobDriver_Reanimate:ReanimatePawn", new Dictionary<string,string> {
                { "Returned ", "Восставший: " }
            } },
            { "BigAndSmall.TryExecuteWorker:Postfix", new Dictionary<string,string> {
                { "Skadi", "Скади" },
                { "Huntress", "Охотница" },
                { "Angrboda ", "Ангрбода " },
                { "Angra ", "Ангра " },
                { "Jarnvid", "Ярнвид" },
                { "Gerd ", "Герд " },
                { "Evergreen", "Вечнозелёная" },
                { "Angrboda", "Ангрбода" },
                { "Gerd", "Герд" }
            } },
            { "BigAndSmall.LockedNeedClass:GetLabel", new Dictionary<string,string> {
                { " Min", " (минимум)" }
            } },
            { "BigAndSmall.Herculean_CanEquip_Postfix_Patch:Postfix", new Dictionary<string,string> {
                { "Probably a mod conflict :|", "Вероятен конфликт модов :|" }
            } },
            { "BigAndSmall.EditPawnWindow:<MakeStandardSection>g__GetStyleName|34_2", new Dictionary<string,string> {
                { "{0}, Style {1}", "{0}, вариант {1}" }
            } },
            { "BigAndSmall.RacialFeature:.ctor", new Dictionary<string,string> {
                { "Unnamed", "Без названия" },
                { "No description available.", "Описание отсутствует." }
            } },
            { "BigAndSmall.RomanceTags:GetDescriptions", new Dictionary<string,string> {
                { ": N/A", ": недоступно" }
            } },
            { "BigAndSmall.Debugging.BigAndSmallDebugActions:TransformToRace", new Dictionary<string,string> {
                { "[FORCE]: {0,0}\t ({1})", "[ПРИНУДИТЕЛЬНО]: {0,0}\t ({1})" }
            } },
            { "BigAndSmall.Debugging.BigAndSmallDebugActions:ClearJunk", new Dictionary<string,string> {
                { "Clear Junk", "Удалить неиспользуемые данные" }
            } },
            { "BigAndSmall.Debugging.BigAndSmallDebugActions:MiscDebug", new Dictionary<string,string> {
                { "Set Graphics Dirty", "Обновить графику" },
                { "Set Age to 0", "Обнулить возраст" },
                { "Remove Custom Graphics", "Сбросить изменения внешности" },
                { "Set Custom Color A", "Задать первый цвет" },
                { "Set Custom Color B", "Задать второй цвет" },
                { "Set Custom Color C", "Задать третий цвет" },
                { "Randomise Faction", "Случайная фракция" },
                { "Test Apparel Restrictions", "Проверить ограничения на одежду" }
            } },
            { "BigAndSmall.Debugging.BigAndSmallDebugActions:SetCustomColor", new Dictionary<string,string> {
                { "Random", "Случайный цвет" }
            } },
            { "BigAndSmall.Debugging.BigAndSmallDebugActions+<>c:<MiscDebug>b__16_6", new Dictionary<string,string> {
                { "Randomize Faction", "Случайная фракция" }
            } },
            { "BigAndSmall.Debugging.DebugUIPatches:DoGeneDebugButton", new Dictionary<string,string> {
                { "Set to Race...", "Сменить расу…" },
                { "Apply/Append RaceDef", "Применить или добавить расу" },
                { "Set exact xenotype + race", "Задать ксенотип вместе с расой" },
                { "Apply xenotype", "Применить ксенотип" },
                { "Spawn Xenogerm", "Создать ксеносемя" },
                { "Reapply Genes", "Повторно применить гены" },
                { "Remove overriden genes", "Удалить подавленные гены" },
                { "Remove all Endogenes", "Удалить все эндогены" },
                { "Remove all Xenogenes", "Удалить все ксеногены" },
                { "Discombobulate", "Перемешать гены" },
                { "Set to random xenotype", "Задать случайный ксенотип" },
                { "Set to Baseline Human [Force]", "Сделать базовым человеком [принудительно]" }
            } },
            { "BigAndSmall.Debugging.DebugUIPatches+<>c__DisplayClass1_0:<DoGeneDebugButton>b__3", new Dictionary<string,string> {
                { " (force)", " (принудительно)" }
            } },
            { "RedHealth.HealthManager:GetTooltip", new Dictionary<string,string> {
                { "DEBUG VARS", "ДАННЫЕ ОТЛАДКИ" },
                { "PercentOfLife Input: {0:f1}%", "Доля прожитой жизни на входе: {0:f1}%" },
                { "Main Health Curve: (", "Основная кривая здоровья: (" }
            } },
            { "BigAndSmall.ConditionalStatAffecter_InVacuum:get_Label", new Dictionary<string,string> {
                { "BS_InVacuum", "chupakobra6_2925432336_BS_InVacuum" }
            } },
            { "BigAndSmall.GeneDef_GetDescriptionFull:Postfix", new Dictionary<string,string> {
                { "BP_GenePrerequisitesUnknownType", "chupakobra6_2925432336_BP_GenePrerequisitesUnknownType" }
            } },
            { "BigAndSmall.PawnExtensionExtension:TryGetDescription", new Dictionary<string,string> {
                { "BS_PreventDisfigurement", "chupakobra6_2925432336_BS_PreventDisfigurement" }
            } }
            };
        private static readonly Dictionary<string,string[]> Parameters = new Dictionary<string,string[]> {

        };
        public LiteralTranslations(ModContentPack content) : base(content)
        {
            var harmony = new Harmony("igor.ru.2925432336.literals");
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
}
