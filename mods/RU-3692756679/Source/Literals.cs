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
            { "b4ttl3m3ds.simplebabycarry.DataModels.Building_SpacerBabyPod+<DevPositionGizmos>d__66:MoveNext", new Dictionary<string,string> {
                { "Body", "Тело" },
                { "DEV: Disable pod baby pos", "Отладка: скрыть настройку положения" },
                { "DEV: Enable pod baby pos", "Отладка: настроить положение младенца в капсуле" },
                { "DEV: Log values", "Отладка: записать значения в журнал" },
                { "DEV: Reset", "Отладка: сбросить" },
                { "Head", "Голова" },
                { "Rot", "Поворот" },
                { "Scale", "Масштаб" }
            } },
            { "b4ttl3m3ds.simplebabycarry.DataModels.CompPawnCarryBabyDevTools:AddBabyAndHeadDevGizmos", new Dictionary<string,string> {
                { "DEV: Baby X +", "Отладка: Младенец X +" },
                { "DEV: Baby X -", "Отладка: Младенец X −" },
                { "DEV: Baby Y + (Depth)", "Отладка: Младенец Y + (глубина)" },
                { "DEV: Baby Y - (Depth)", "Отладка: Младенец Y − (глубина)" },
                { "DEV: Baby Z +", "Отладка: Младенец Z +" },
                { "DEV: Baby Z -", "Отладка: Младенец Z −" },
                { "DEV: Body Rot +", "Отладка: Поворот тела +" },
                { "DEV: Body Rot -", "Отладка: Поворот тела −" },
                { "DEV: Head Tilt +", "Отладка: Наклон головы +" },
                { "DEV: Head Tilt -", "Отладка: Наклон головы −" },
                { "DEV: Head X +", "Отладка: Голова X +" },
                { "DEV: Head X -", "Отладка: Голова X −" },
                { "DEV: Head Y + (Depth)", "Отладка: Голова Y + (глубина)" },
                { "DEV: Head Y - (Depth)", "Отладка: Голова Y − (глубина)" },
                { "DEV: Head Z +", "Отладка: Голова Z +" },
                { "DEV: Head Z -", "Отладка: Голова Z −" },
                { "DEV: Scale +", "Отладка: Масштаб +" },
                { "DEV: Scale -", "Отладка: Масштаб −" }
            } },
            { "b4ttl3m3ds.simplebabycarry.DataModels.CompPawnCarryBabyDevTools:AddCarryApparelDevGizmos", new Dictionary<string,string> {
                { "DEV: Carry Apparel Reset ?", "Отладка: Сбросить параметры переноски?" },
                { "DEV: Carry Apparel SCALE +", "Отладка: Масштаб переноски +" },
                { "DEV: Carry Apparel SCALE -", "Отладка: Масштаб переноски −" },
                { "DEV: Carry Apparel X +0.01", "Отладка: Переноска X +0.01" },
                { "DEV: Carry Apparel X -0.01", "Отладка: Переноска X −0.01" },
                { "DEV: Carry Apparel Y +0.01", "Отладка: Переноска Y +0.01" },
                { "DEV: Carry Apparel Y -0.01", "Отладка: Переноска Y −0.01" },
                { "DEV: Carry Apparel Z +0.01", "Отладка: Переноска Z +0.01" },
                { "DEV: Carry Apparel Z -0.01", "Отладка: Переноска Z −0.01" },
                { "DEV: Hide Swaddle (OFF)", "Отладка: Скрывать пелёнки (ВЫКЛ)" },
                { "DEV: Hide Swaddle (ON)", "Отладка: Скрывать пелёнки (ВКЛ)" },
                { "DEV: Mirror override OFF", "Отладка: Принудительное отражение ВЫКЛ" },
                { "DEV: Mirror override ON", "Отладка: Принудительное отражение ВКЛ" },
                { "DEV: Reset deltas", "Отладка: Сбросить смещения" },
                { "DEV: Reset preset", "Отладка: Сбросить шаблон" },
                { "DEV: Sling shows BODY (OFF)", "Отладка: Показывать тело в слинге (ВЫКЛ)" },
                { "DEV: Sling shows BODY (ON)", "Отладка: Показывать тело в слинге (ВКЛ)" }
            } },
            { "b4ttl3m3ds.simplebabycarry.DataModels.CompPawnCarryBabyDevTools:AddDevGizmos", new Dictionary<string,string> {
                { "DEV: Disable baby gizmos", "Отладка: Скрыть команды настройки младенца" },
                { "DEV: Enable baby gizmos", "Отладка: Показать команды настройки младенца" },
                { "DEV: Print debug", "Отладка: Записать отладочные данные в журнал" }
            } },
            { "b4ttl3m3ds.simplebabycarry.DataModels.CompPawnCarryBabyDevTools:AddFaceOverlayDevGizmos", new Dictionary<string,string> {
                { "DEV: Ear Reset", "Отладка: Сбросить параметры ушей" },
                { "DEV: Ear X +0.01", "Отладка: Уши X +0.01" },
                { "DEV: Ear X -0.01", "Отладка: Уши X −0.01" },
                { "DEV: Ear Y +0.01", "Отладка: Уши Y +0.01" },
                { "DEV: Ear Y -0.01", "Отладка: Уши Y −0.01" },
                { "DEV: Ear Z +0.01", "Отладка: Уши Z +0.01" },
                { "DEV: Ear Z -0.01", "Отладка: Уши Z −0.01" },
                { "DEV: Eye Reset", "Отладка: Сбросить параметры глаз" },
                { "DEV: Eye Scale +", "Отладка: Масштаб глаз +" },
                { "DEV: Eye Scale -", "Отладка: Масштаб глаз −" },
                { "DEV: Eye X +0.01", "Отладка: Глаза X +0.01" },
                { "DEV: Eye X -0.01", "Отладка: Глаза X −0.01" },
                { "DEV: Eye Y +0.01", "Отладка: Глаза Y +0.01" },
                { "DEV: Eye Y -0.01", "Отладка: Глаза Y −0.01" },
                { "DEV: Eye Z +0.01", "Отладка: Глаза Z +0.01" },
                { "DEV: Eye Z -0.01", "Отладка: Глаза Z −0.01" },
                { "DEV: Nose Reset", "Отладка: Сбросить параметры носа" },
                { "DEV: Nose X +0.01", "Отладка: Нос X +0.01" },
                { "DEV: Nose X -0.01", "Отладка: Нос X −0.01" },
                { "DEV: Nose Y +0.01", "Отладка: Нос Y +0.01" },
                { "DEV: Nose Y -0.01", "Отладка: Нос Y −0.01" },
                { "DEV: Nose Z +0.01", "Отладка: Нос Z +0.01" },
                { "DEV: Nose Z -0.01", "Отладка: Нос Z −0.01" }
            } },
            { "b4ttl3m3ds.simplebabycarry.DataModels.CompPawnCarryBabyDevTools:AddTailDevGizmos", new Dictionary<string,string> {
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
            { "b4ttl3m3ds.simplebabycarry.DataModels.CompPawnCarryBabyDevTools:GetForceDrawCarryApparelLabel", new Dictionary<string,string> {
                { "DEV: Draw baby carrier (OFF)", "Отладка: показывать переноску (ВЫКЛ)" },
                { "DEV: Draw baby carrier (ON)", "Отладка: показывать переноску (ВКЛ)" }
            } },
            { "b4ttl3m3ds.simplebabycarry.DebugUtils.SCB_DevPositionGizmos+<FloatButtons>d__3:MoveNext", new Dictionary<string,string> {
                { "DEV: ", "Отладка: " }
            } },
            { "b4ttl3m3ds.simplebabycarry.DebugUtils.SCB_DevPositionGizmos+<Vector3Buttons>d__1:MoveNext", new Dictionary<string,string> {
                { " Y (depth) +", " Y (глубина) +" },
                { " Y (depth) -", " Y (глубина) -" },
                { "DEV: ", "Отладка: " }
            } },
            { "b4ttl3m3ds.simplebabycarry.Helper.BabyCellSafety:GetCellUnsafeReason", new Dictionary<string,string> {
                { "baby", "младенец" }
            } },
            { "b4ttl3m3ds.simplebabycarry.Main_HarmonyInitializer:SettingsCategory", new Dictionary<string,string> {
                { "Simple Baby Carry", "Simple Baby Carry — ношение младенцев" }
            } }
            };
        private static readonly Dictionary<string,string[]> Parameters = new Dictionary<string,string[]> {
            { "b4ttl3m3ds.simplebabycarry.DataModels.Building_SpacerBabyPod+<DevPositionGizmos>d__66:MoveNext", new string[] {  } },
            { "b4ttl3m3ds.simplebabycarry.DataModels.CompPawnCarryBabyDevTools:AddBabyAndHeadDevGizmos", new string[] { "b4ttl3m3ds.simplebabycarry.DataModels.CompPawnCarryBaby", "System.Collections.Generic.List`1<Verse.Gizmo>" } },
            { "b4ttl3m3ds.simplebabycarry.DataModels.CompPawnCarryBabyDevTools:AddCarryApparelDevGizmos", new string[] { "b4ttl3m3ds.simplebabycarry.DataModels.CompPawnCarryBaby", "System.Collections.Generic.List`1<Verse.Gizmo>" } },
            { "b4ttl3m3ds.simplebabycarry.DataModels.CompPawnCarryBabyDevTools:AddDevGizmos", new string[] { "b4ttl3m3ds.simplebabycarry.DataModels.CompPawnCarryBaby", "System.Collections.Generic.List`1<Verse.Gizmo>", "System.Collections.Generic.IEnumerable`1<Verse.Gizmo>" } },
            { "b4ttl3m3ds.simplebabycarry.DataModels.CompPawnCarryBabyDevTools:AddFaceOverlayDevGizmos", new string[] { "b4ttl3m3ds.simplebabycarry.DataModels.CompPawnCarryBaby", "System.Collections.Generic.List`1<Verse.Gizmo>" } },
            { "b4ttl3m3ds.simplebabycarry.DataModels.CompPawnCarryBabyDevTools:AddTailDevGizmos", new string[] { "b4ttl3m3ds.simplebabycarry.DataModels.CompPawnCarryBaby", "System.Collections.Generic.List`1<Verse.Gizmo>" } },
            { "b4ttl3m3ds.simplebabycarry.DataModels.CompPawnCarryBabyDevTools:GetForceDrawCarryApparelLabel", new string[] { "b4ttl3m3ds.simplebabycarry.DataModels.CompPawnCarryBaby" } },
            { "b4ttl3m3ds.simplebabycarry.DebugUtils.SCB_DevPositionGizmos+<FloatButtons>d__3:MoveNext", new string[] {  } }
        };
        public LiteralTranslations(ModContentPack content) : base(content) { }
        public static void Apply()
        {
            var harmony = new Harmony("igor.ru.3692756679.literals");
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
            IEnumerable<MethodBase> candidates = name == ".ctor"
                ? type.GetConstructors(flags).Where(m => m.Name == name).Cast<MethodBase>()
                : type.GetMethods(flags).Where(m => m.Name == name).Cast<MethodBase>();
            if (Parameters.TryGetValue(target, out var parameters))
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
