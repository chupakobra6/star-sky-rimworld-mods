using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace Igor.RU_3692756679
{
    public sealed class LiteralTranslations : Mod
    {
        private static readonly Dictionary<string,Dictionary<string,string>> Methods =
            new Dictionary<string,Dictionary<string,string>> {
            { "b4ttl3m3ds.simplebabycarry.DataModels.Building_SpacerBabyPod+<GetGizmos>d__61:MoveNext", new Dictionary<string,string> {
                { "Body", "Тело" },
                { "DEV: Disable pod baby pos", "Отладка: скрыть настройку положения" },
                { "DEV: Enable pod baby pos", "Отладка: настроить положение младенца в капсуле" },
                { "DEV: Log values", "Отладка: записать значения в журнал" },
                { "DEV: Reset", "Отладка: сбросить" },
                { "Head", "Голова" },
                { "Rot", "Поворот" },
                { "Scale", "Масштаб" }
            } },
            { "b4ttl3m3ds.simplebabycarry.DataModels.CompPawnCarryBaby:CompGetGizmosExtra", new Dictionary<string,string> {
                { "DEV: Baby X +", "Отладка: Младенец X +" },
                { "DEV: Baby X -", "Отладка: Младенец X −" },
                { "DEV: Baby Y + (Depth)", "Отладка: Младенец Y + (глубина)" },
                { "DEV: Baby Y - (Depth)", "Отладка: Младенец Y − (глубина)" },
                { "DEV: Baby Z +", "Отладка: Младенец Z +" },
                { "DEV: Baby Z -", "Отладка: Младенец Z −" },
                { "DEV: Body Rot +", "Отладка: Поворот тела +" },
                { "DEV: Body Rot -", "Отладка: Поворот тела −" },
                { "DEV: Carry Apparel Reset ?", "Отладка: Сбросить параметры переноски?" },
                { "DEV: Carry Apparel SCALE +", "Отладка: Масштаб переноски +" },
                { "DEV: Carry Apparel SCALE -", "Отладка: Масштаб переноски −" },
                { "DEV: Carry Apparel X +0.01", "Отладка: Переноска X +0.01" },
                { "DEV: Carry Apparel X -0.01", "Отладка: Переноска X −0.01" },
                { "DEV: Carry Apparel Y +0.01", "Отладка: Переноска Y +0.01" },
                { "DEV: Carry Apparel Y -0.01", "Отладка: Переноска Y −0.01" },
                { "DEV: Carry Apparel Z +0.01", "Отладка: Переноска Z +0.01" },
                { "DEV: Carry Apparel Z -0.01", "Отладка: Переноска Z −0.01" },
                { "DEV: Disable baby gizmos", "Отладка: Скрыть команды настройки младенца" },
                { "DEV: Ear Reset", "Отладка: Сбросить параметры ушей" },
                { "DEV: Ear X +0.01", "Отладка: Уши X +0.01" },
                { "DEV: Ear X -0.01", "Отладка: Уши X −0.01" },
                { "DEV: Ear Y +0.01", "Отладка: Уши Y +0.01" },
                { "DEV: Ear Y -0.01", "Отладка: Уши Y −0.01" },
                { "DEV: Ear Z +0.01", "Отладка: Уши Z +0.01" },
                { "DEV: Ear Z -0.01", "Отладка: Уши Z −0.01" },
                { "DEV: Enable baby gizmos", "Отладка: Показать команды настройки младенца" },
                { "DEV: Eye Reset", "Отладка: Сбросить параметры глаз" },
                { "DEV: Eye Scale +", "Отладка: Масштаб глаз +" },
                { "DEV: Eye Scale -", "Отладка: Масштаб глаз −" },
                { "DEV: Eye X +0.01", "Отладка: Глаза X +0.01" },
                { "DEV: Eye X -0.01", "Отладка: Глаза X −0.01" },
                { "DEV: Eye Y +0.01", "Отладка: Глаза Y +0.01" },
                { "DEV: Eye Y -0.01", "Отладка: Глаза Y −0.01" },
                { "DEV: Eye Z +0.01", "Отладка: Глаза Z +0.01" },
                { "DEV: Eye Z -0.01", "Отладка: Глаза Z −0.01" },
                { "DEV: Head Tilt +", "Отладка: Наклон головы +" },
                { "DEV: Head Tilt -", "Отладка: Наклон головы −" },
                { "DEV: Head X +", "Отладка: Голова X +" },
                { "DEV: Head X -", "Отладка: Голова X −" },
                { "DEV: Head Y + (Depth)", "Отладка: Голова Y + (глубина)" },
                { "DEV: Head Y - (Depth)", "Отладка: Голова Y − (глубина)" },
                { "DEV: Head Z +", "Отладка: Голова Z +" },
                { "DEV: Head Z -", "Отладка: Голова Z −" },
                { "DEV: Hide Swaddle (OFF)", "Отладка: Скрывать пелёнки (ВЫКЛ)" },
                { "DEV: Hide Swaddle (ON)", "Отладка: Скрывать пелёнки (ВКЛ)" },
                { "DEV: Mirror override OFF", "Отладка: Принудительное отражение ВЫКЛ" },
                { "DEV: Mirror override ON", "Отладка: Принудительное отражение ВКЛ" },
                { "DEV: Nose Reset", "Отладка: Сбросить параметры носа" },
                { "DEV: Nose X +0.01", "Отладка: Нос X +0.01" },
                { "DEV: Nose X -0.01", "Отладка: Нос X −0.01" },
                { "DEV: Nose Y +0.01", "Отладка: Нос Y +0.01" },
                { "DEV: Nose Y -0.01", "Отладка: Нос Y −0.01" },
                { "DEV: Nose Z +0.01", "Отладка: Нос Z +0.01" },
                { "DEV: Nose Z -0.01", "Отладка: Нос Z −0.01" },
                { "DEV: Print debug", "Отладка: Записать отладочные данные в журнал" },
                { "DEV: Reset deltas", "Отладка: Сбросить смещения" },
                { "DEV: Reset preset", "Отладка: Сбросить шаблон" },
                { "DEV: Scale +", "Отладка: Масштаб +" },
                { "DEV: Scale -", "Отладка: Масштаб −" },
                { "DEV: Sling shows BODY (OFF)", "Отладка: Показывать тело в слинге (ВЫКЛ)" },
                { "DEV: Sling shows BODY (ON)", "Отладка: Показывать тело в слинге (ВКЛ)" },
                { "DEV: Tail Mirror (OFF)", "Отладка: Отражение хвоста (ВЫКЛ)" },
                { "DEV: Tail Mirror (ON)", "Отладка: Отражение хвоста (ВКЛ)" },
                { "DEV: Tail Reset", "Отладка: Сбросить параметры хвоста" },
                { "DEV: Tail Scale +", "Отладка: Масштаб хвоста +" },
                { "DEV: Tail Scale -", "Отладка: Масштаб хвоста −" },
                { "DEV: Tail Tilt +", "Отладка: Наклон хвоста +" },
                { "DEV: Tail Tilt -", "Отладка: Наклон хвоста −" },
                { "DEV: Tail X +0.01", "Отладка: Хвост X +0.01" },
                { "DEV: Tail X -0.01", "Отладка: Хвост X −0.01" },
                { "DEV: Tail Y +0.01", "Отладка: Хвост Y +0.01" },
                { "DEV: Tail Y -0.01", "Отладка: Хвост Y −0.01" },
                { "DEV: Tail Z +0.01", "Отладка: Хвост Z +0.01" },
                { "DEV: Tail Z -0.01", "Отладка: Хвост Z −0.01" }
            } },
            { "b4ttl3m3ds.simplebabycarry.DataModels.CompPawnCarryBaby:GetForceDrawCarryApparelLabel", new Dictionary<string,string> {
                { "DEV: Draw baby carrier (OFF)", "Отладка: показывать переноску (ВЫКЛ)" },
                { "DEV: Draw baby carrier (ON)", "Отладка: показывать переноску (ВКЛ)" }
            } },
            { "b4ttl3m3ds.simplebabycarry.DebugUtils.SCB_DevPositionGizmos+<FloatButtons>d__2:MoveNext", new Dictionary<string,string> {
                { "DEV: ", "Отладка: " }
            } },
            { "b4ttl3m3ds.simplebabycarry.DebugUtils.SCB_DevPositionGizmos+<Vector3Buttons>d__1:MoveNext", new Dictionary<string,string> {
                { " Y (depth) +", " Y (глубина) +" },
                { " Y (depth) -", " Y (глубина) -" },
                { "DEV: ", "Отладка: " }
            } },
            { "b4ttl3m3ds.simplebabycarry.Helper.BabyCarryHelpers:GetCellUnsafeReason", new Dictionary<string,string> {
                { "baby", "младенец" }
            } },
            { "b4ttl3m3ds.simplebabycarry.Main_HarmonyInitializer:SettingsCategory", new Dictionary<string,string> {
                { "Simple Baby Carry", "Simple Baby Carry — ношение младенцев" }
            } },
            { "b4ttl3m3ds.simplebabycarry.Patches.Patch_AddCustomCarryBabyOption:FloatMenuAction_PutSomewhereSafe", new Dictionary<string,string> {
                { "Cannot find a safe, reachable location for {0}.", "Не удаётся найти безопасное и доступное место для младенца «{0}»." }
            } },
            { "b4ttl3m3ds.simplebabycarry.Patches.Patch_AddCustomCarryBabyOption:Postfix", new Dictionary<string,string> {
                { "ReservedBy", "chupakobra6_3692756679_ReservedBy" }
            } }
            };
        private static readonly Dictionary<string,string[]> Parameters = new Dictionary<string,string[]> {

        };
        public LiteralTranslations(ModContentPack content) : base(content) { }
        public static void Apply()
        {
            var harmony = new Harmony("igor.ru.3692756679.literals");
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
