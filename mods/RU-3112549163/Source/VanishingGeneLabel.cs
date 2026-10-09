using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace Igor.RU_3112549163
{
    [StaticConstructorOnStartup]
    public static class VanishingGeneLabel
    {
        static VanishingGeneLabel()
        {
            new Harmony("igor.ru.3112549163.vanishinglabel").Patch(
                AccessTools.Method("EBSGFramework.VanishingGene:TickInterval"),
                transpiler:new HarmonyMethod(typeof(VanishingGeneLabel),nameof(ReadableLabel)));
        }
        static IEnumerable<CodeInstruction> ReadableLabel(IEnumerable<CodeInstruction> instructions)
        {
            int replaced=0;
            foreach(var instruction in instructions)
            {
                var method=instruction.operand as MethodInfo;
                if(instruction.opcode==OpCodes.Callvirt&&method!=null&&method.DeclaringType==typeof(object)&&method.Name=="ToString")
                {
                    instruction.opcode=OpCodes.Call;
                    instruction.operand=AccessTools.Method(typeof(VanishingGeneLabel),nameof(Label));
                    replaced++;
                }
                yield return instruction;
            }
            if(replaced!=1)throw new InvalidOperationException("EBSG vanishing-gene notification changed; expected one definition display call.");
        }
        static string Label(object value)
        {
            var def=value as Def;
            return def!=null&&LanguageDatabase.activeLanguage.folderName.StartsWith("Russian",StringComparison.OrdinalIgnoreCase)
                ?def.LabelCap.ToString():value.ToString();
        }
    }
}
