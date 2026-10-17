using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using LudeonTK;
using Verse;

namespace Igor.RU_2891845502
{
    // Attribute text is metadata, not an ldstr. Only the reviewed method/name
    // pairs are changed, and only a UI copy of the attribute is modified.
    public sealed class DebugActionNames : Mod
    {
        private static readonly Dictionary<string, Dictionary<string, string>> Names =
            new Dictionary<string, Dictionary<string, string>> {
            { "AlphaGenes.GenerateBiotechLabQuest:GenerateLabQuest", new Dictionary<string, string> {
                { "Generate Biotech lab quest", "Создать задание с биотехнологической лабораторией" }
            } }
            };
        public DebugActionNames(ModContentPack content) : base(content)
        {
            new Harmony("igor.ru.2891845502.debugnames").Patch(
                AccessTools.Method(typeof(DebugTabMenu_Actions), "GenerateCacheForMethod"),
                prefix: new HarmonyMethod(typeof(DebugActionNames), nameof(Translate)));
        }
        public static void Translate(MethodInfo __0, ref DebugActionAttribute __1)
        {
            if (__0?.DeclaringType == null || __1 == null
                || LanguageDatabase.activeLanguage?.folderName.StartsWith("Russian", StringComparison.OrdinalIgnoreCase) != true
                || !Names.TryGetValue(__0.DeclaringType.FullName + ":" + __0.Name, out var pairs)
                || __1.name == null || !pairs.TryGetValue(__1.name, out var russian)) return;
            __1 = (DebugActionAttribute)AccessTools.Method(typeof(object), "MemberwiseClone").Invoke(__1, null);
            __1.name = russian;
        }
    }
}
