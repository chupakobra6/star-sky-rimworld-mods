using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace Igor.RU_3781978940
{
    public sealed class LiteralTranslations : Mod
    {
        private static readonly Dictionary<string,Dictionary<string,string>> Methods =
            new Dictionary<string,Dictionary<string,string>> {
            { "BioTabEnhanced.BioTabEnhancedMod:DrawGeneralSettings", new Dictionary<string,string> {
                { "Gradient hue color", "Цвет градиента" }
            } },
            { "BioTabEnhanced.EnhancedCharacterCard+RimTalkCompat:Tooltip", new Dictionary<string,string> {
                { "RimTalk Persona", "Личность RimTalk" },
                { "RimTalk Persona\n\n", "Личность RimTalk\n\n" }
            } },
            { "BioTabEnhanced.EnhancedCharacterCard:DrawHeader", new Dictionary<string,string> {
                { "BIO", "БИО" }
            } },
            { "BioTabEnhanced.EnhancedCharacterCard:EverybodyDifferentHeightLabel", new Dictionary<string,string> {
                { " cm", " см" }
            } },
            { "BioTabEnhanced.EnhancedCharacterCard:EverybodyDifferentWeightLabel", new Dictionary<string,string> {
                { " kg", " кг" },
                { " lb", " фунт." }
            } },
            { "BioTabEnhanced.EnhancedCharacterCard:GetExtraFactionTooltip", new Dictionary<string,string> {
                { "</color> originally belongs to.\n\n<color=#999999>Click to view factions.</color>", "</color>.\n\n<color=#999999>Нажмите, чтобы открыть список фракций.</color>" },
                { "<color=#E5E53A>Home Faction</color>\n\nThe faction <color=#D89B56>", "<color=#E5E53A>Исходная фракция</color>\n\nПервоначально к этой фракции относится <color=#D89B56>" }
            } },
            { "BioTabEnhanced.EnhancedCharacterCard:GetFactionTooltip", new Dictionary<string,string> {
                { "</color> currently belongs to.\n\n<color=#999999>Click to view factions.</color>", "</color>.\n\n<color=#999999>Нажмите, чтобы открыть список фракций.</color>" },
                { "<color=#E5E53A>Faction</color>\n\nThe faction <color=#D89B56>", "<color=#E5E53A>Фракция</color>\n\nК этой фракции сейчас относится <color=#D89B56>" }
            } },
            { "BioTabEnhanced.EnhancedCharacterCard:GetGenderChipTooltip", new Dictionary<string,string> {
                { "Other", "Другой" }
            } },
            { "BioTabEnhanced.EnhancedCharacterCard:GetIdeologyTooltip", new Dictionary<string,string> {
                { "\n\n<color=#999999>Click for more info.</color>", "\n\n<color=#999999>Нажмите, чтобы узнать подробности.</color>" },
                { "</color>\nCertainty: ", "</color>\nУверенность: " }
            } },
            { "BioTabEnhanced.EnhancedCharacterCard:GetIncapableTooltip", new Dictionary<string,string> {
                { "<color=#E5E53A>Disabled work types:</color>\n", "<color=#E5E53A>Недоступные виды работ:</color>\n" },
                { "Caused by backstory: ", "Из-за предыстории: " }
            } },
            { "BioTabEnhanced.EnhancedCharacterCard:GetPersonalitySectionTitle", new Dictionary<string,string> {
                { "'s Personality", ": личность" }
            } },
            { "BioTabEnhanced.EnhancedCharacterCard:GetQuestLodgerInfo", new Dictionary<string,string> {
                { "Quest guest\n\n", "Гость по заданию\n\n" }
            } },
            { "BioTabEnhanced.EnhancedCharacterCard:GetTags", new Dictionary<string,string> {
                { "Age ", "Возраст: " }
            } },
            { "BioTabEnhanced.EnhancedCharacterCard:GetXenotypeTooltip", new Dictionary<string,string> {
                { "\n\n<color=#999999>Click to view ", "\n\n<color=#999999>Нажмите, чтобы посмотреть гены: " },
                { "'s genes.</color>", ".</color>" },
                { "<color=#E5E53A>Xenotype: ", "<color=#E5E53A>Ксенотип: " }
            } },
            { "BioTabEnhanced.EnhancedCharacterCard:NormalizeEducationKnowledgeLabel", new Dictionary<string,string> {
                { "Automobile", "Транспорт" },
                { "Firearm", "Огнестрельное оружие" }
            } },
            { "BioTabEnhanced.EnhancedCharacterCard:PopulateEducationProgress", new Dictionary<string,string> {
                { "\n\nProgress to ", "\n\nПрогресс — " }
            } }
            };
        private static readonly Dictionary<string,string[]> Parameters = new Dictionary<string,string[]> {

        };
        public LiteralTranslations(ModContentPack content) : base(content) { }
        public static void Apply()
        {
            var harmony = new Harmony("igor.ru.3781978940.literals");
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
