using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Xml;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace Igor.RU_2773943594
{
    [StaticConstructorOnStartup]
    public static partial class EditorDisplay
    {
        public static readonly Dictionary<string, string> Vocabulary = new Dictionary<string, string>();
        public static readonly List<MethodBase> PatchedRenderers = new List<MethodBase>();
        static readonly Regex Numbered = new Regex(@"^(Input|Output|Option|Branch|Width|Angle|Speed) (\d+)$");
        static readonly Regex Kernel = new Regex(@"^Kernel \((.+)\)$");
        static EditorDisplay() { LongEventHandler.ExecuteWhenFinished(Install); }
        public static bool Russian => LanguageDatabase.activeLanguage?.folderName.StartsWith("Russian", StringComparison.OrdinalIgnoreCase) == true;

        public static string Select(string text)
        {
            if (!Russian || text == null) return text;
            if (Vocabulary.TryGetValue(text, out var translated)) return translated;
            var numbered = Numbered.Match(text);
            if (numbered.Success && Vocabulary.TryGetValue(numbered.Groups[1].Value, out translated))
                return translated + " " + numbered.Groups[2].Value;
            var kernel = Kernel.Match(text);
            if (kernel.Success) return "Фильтр (" + Select(kernel.Groups[1].Value.Replace('_', ' ')) + ")";
            if (text.EndsWith(" [-]", StringComparison.Ordinal))
                return Select(text.Substring(0, text.Length - 4)) + " [-]";
            return text;
        }

        public static GUIContent Content(GUIContent original)
        {
            if (original == null || !Russian) return original;
            string text = Select(original.text), tooltip = Select(original.tooltip);
            if (text == original.text && tooltip == original.tooltip) return original;
            return new GUIContent(original) { text = text, tooltip = tooltip };
        }

        static void Install()
        {
            var pack = LoadedModManager.RunningModsListForReading.Single(m => m.PackageIdPlayerFacing == "starsky.rimworld.ru.2773943594");
            var xml = new XmlDocument();
            xml.Load(Path.Combine(pack.RootDir, "RenderVocabulary.xml"));
            foreach (XmlNode entry in xml.SelectNodes("/vocabulary/entry"))
                Vocabulary.Add(entry["english"].InnerText, entry["russian"].InnerText);

            var harmony = new Harmony("igor.ru.2773943594.editor-display");
            var assemblies = new[] { AccessTools.TypeByName("TerrainGraph.NodeBase").Assembly,
                AccessTools.TypeByName("GeologicalLandforms.GraphEditor.Landform").Assembly };
            var types = assemblies.SelectMany(a => a.GetTypes()).ToArray();
            var targets = new HashSet<MethodBase>();
            foreach (var pair in RenderTargets)
            {
                var definition = AccessTools.TypeByName(pair[0]);
                if (definition == null) throw new MissingMemberException(pair[0]);
                IEnumerable<Type> resolved;
                if (definition.IsGenericTypeDefinition)
                {
                    var closed = new HashSet<Type>();
                    foreach (var type in types.Where(t => !t.ContainsGenericParameters))
                        for (var ancestor = type; ancestor != null; ancestor = ancestor.BaseType)
                            if (ancestor.IsGenericType && ancestor.GetGenericTypeDefinition() == definition) closed.Add(ancestor);
                    resolved = closed;
                }
                else resolved = new[] { definition };
                int matched = 0;
                foreach (var type in resolved)
                    foreach (var method in type.GetMethods(AccessTools.allDeclared).Where(m => m.Name == pair[1] && !m.ContainsGenericParameters && m.GetMethodBody() != null))
                        if (PatchProcessor.GetOriginalInstructions(method).Any(IsDisplayCall)) { targets.Add(method); matched++; }
                if (matched == 0) throw new MissingMethodException("No renderer resolved: " + string.Join(":", pair));
            }
            foreach (var method in targets)
            {
                harmony.Patch(method, transpiler: new HarmonyMethod(typeof(EditorDisplay), nameof(Render)));
                PatchedRenderers.Add(method);
            }
            harmony.Patch(AccessTools.Method("GeologicalLandforms.GraphEditor.LandformGraphEditor+<>c:<InitialSetup>b__18_0"),
                prefix: new HarmonyMethod(typeof(EditorDisplay), nameof(Dropdown)));
            harmony.Patch(AccessTools.Method("GeologicalLandforms.GraphEditor.Landform:TranslatedNameWithDirection"),
                postfix: new HarmonyMethod(typeof(EditorDisplay), nameof(DirectionLabel)));
            harmony.Patch(AccessTools.Method("GeologicalLandforms.UserInterfaceUtils:LabelForTileMutator"),
                postfix: new HarmonyMethod(typeof(EditorDisplay), nameof(MutatorSuffix)));
        }

        public static void Dropdown(ref List<string> __0)
        {
            if (Russian) __0 = __0.Select(Select).ToList();
        }

        public static void DirectionLabel(object __instance, Rot4 __0, ref string __result)
        {
            if (!Russian) return;
            var type = __instance.GetType();
            if (!(bool)AccessTools.Property(type, "DisplayNameHasDirection").GetValue(__instance)) return;
            string name = (string)AccessTools.Property(type, "TranslatedName").GetValue(__instance);
            string direction = (string)AccessTools.Method(type, "TranslatedDirection").Invoke(__instance, new object[] { __0 });
            __result = name + " (" + direction + ")";
        }

        public static void MutatorSuffix(ref string __result)
        {
            if (!Russian || __result == null) return;
            foreach (var pair in new[] { new[] { "(tribal)", "(племя)" }, new[] { "(outlander)", "(чужеземцы)" },
                new[] { "(tropical)", "(тропики)" }, new[] { "(multiple)", "(несколько островов)" },
                new[] { "(crater)", "(кратер)" }, new[] { "(cove)", "(бухта)" } })
                if (__result.EndsWith(pair[0], StringComparison.Ordinal))
                    __result = __result.Substring(0, __result.Length - pair[0].Length) + pair[1];
        }

        public static bool IsDisplayCall(CodeInstruction instruction)
        {
            if (instruction.opcode != OpCodes.Call && instruction.opcode != OpCodes.Callvirt) return false;
            if (!(instruction.operand is MethodInfo method)) return false;
            string type = method.DeclaringType.FullName;
            bool draw = (type == "UnityEngine.GUI" || type == "UnityEngine.GUILayout")
                && new[] { "Label", "Box", "Button", "Toggle", "RepeatButton" }.Contains(method.Name);
            bool measure = type == "UnityEngine.GUIStyle" && (method.Name == "CalcSize" || method.Name == "CalcHeight");
            // This exact enum overload has a separate string label. Do not match
            // ToggleTableRow<string>, whose string key is persisted by settings.
            bool river = type == "LunarFramework.GUI.LunarGUI" && method.Name == "ToggleTableRow"
                && method.IsGenericMethod && method.GetGenericArguments().Single().FullName == "GeologicalLandforms.RiverType";
            return (draw || measure || river) && method.GetParameters().Any(p => p.ParameterType == typeof(string) || p.ParameterType == typeof(GUIContent));
        }

        public static IEnumerable<CodeInstruction> Render(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            foreach (var instruction in instructions)
            {
                if (!IsDisplayCall(instruction)) { yield return instruction; continue; }
                var parameters = ((MethodInfo)instruction.operand).GetParameters();
                var locals = parameters.Select(p => generator.DeclareLocal(p.ParameterType)).ToArray();
                // Translate only the arguments consumed by drawing/measurement. The
                // original node, port, menu path and editable text stay untouched.
                for (int i = locals.Length - 1; i >= 0; i--)
                {
                    var save = new CodeInstruction(OpCodes.Stloc, locals[i]);
                    if (i == locals.Length - 1) { save.MoveLabelsFrom(instruction); save.MoveBlocksFrom(instruction); }
                    yield return save;
                }
                for (int i = 0; i < locals.Length; i++)
                {
                    yield return new CodeInstruction(OpCodes.Ldloc, locals[i]);
                    if (parameters[i].ParameterType == typeof(string))
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EditorDisplay), nameof(Select)));
                    else if (parameters[i].ParameterType == typeof(GUIContent))
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EditorDisplay), nameof(Content)));
                }
                yield return instruction;
            }
        }
    }
}
