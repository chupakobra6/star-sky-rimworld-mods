using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace Igor.RU_927155256
{
    [StaticConstructorOnStartup]
    public static class MissingTooltip
    {
        static MissingTooltip() { LongEventHandler.ExecuteWhenFinished(Apply); }
        private static void Apply()
        {
            var target=AccessTools.TypeByName("SimpleSidearms.rimworld.Gizmo_SidearmsList")
                .GetConstructors(BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance)
                .Single(c=>c.GetParameters().Length==4);
            new Harmony("igor.ru.927155256.tooltip").Patch(target,
                transpiler:new HarmonyMethod(typeof(MissingTooltip),nameof(Transpile)));
        }
        private static string Key(string original)
        {
            return LanguageDatabase.activeLanguage?.folderName.StartsWith("Russian",StringComparison.OrdinalIgnoreCase)==true
                ? "chupakobra6_927155256_GizmoTooltip" : original;
        }
        private static IEnumerable<CodeInstruction> Transpile(IEnumerable<CodeInstruction> input)
        {
            int found=0;
            foreach(var instruction in input)
            {
                yield return instruction;
                if(instruction.opcode==OpCodes.Ldstr && (string)instruction.operand=="DrawSidearm_gizmoTooltip")
                {
                    found++;
                    yield return new CodeInstruction(OpCodes.Call,AccessTools.Method(typeof(MissingTooltip),nameof(Key)));
                }
            }
            if(found!=1)throw new InvalidOperationException("Simple Sidearms tooltip constructor changed: "+found);
        }
    }
}
