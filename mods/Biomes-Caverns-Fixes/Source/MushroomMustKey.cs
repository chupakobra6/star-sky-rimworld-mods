using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;
namespace Igor.CavernsTextFix
{
    public sealed class ModEntry : Mod
    {
        public ModEntry(ModContentPack content) : base(content)
        {
            new Harmony("igor.biomescaverns.mushroommust").Patch(
                AccessTools.Method("WorkGiver_FillMushroomFermentingBarrel:ResetStaticData"),
                transpiler:new HarmonyMethod(typeof(ModEntry),nameof(Transpile)));
        }
        public static IEnumerable<CodeInstruction> Transpile(IEnumerable<CodeInstruction> input)
        {
            var output=new List<CodeInstruction>();int replaced=0;
            foreach(var instruction in input)
            {
                if(instruction.opcode==OpCodes.Ldstr && Equals(instruction.operand,"NoMushroomWineMust"))
                {instruction.operand="BMT_NoMushroomMust";replaced++;}
                output.Add(instruction);
            }
            if(replaced!=1)throw new InvalidOperationException("Expected one missing-mushroom-must key; found "+replaced);
            return output;
        }
    }
}
