using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace Igor.PawnGenerationFixes
{
    public sealed class ModEntry : Mod
    {
        public ModEntry(ModContentPack content) : base(content)
        {
            var harmony=new Harmony("igor.pawngeneration.fixes");
            harmony.Patch(
                AccessTools.Method(typeof(PawnGenerator), "TryGenerateNewPawnInternal"),
                transpiler: new HarmonyMethod(typeof(RejectedPawn), nameof(RejectedPawn.Transpiler)));
            if(ModsConfig.IsActive("erdelf.humanoidalienraces")) SolidBioWorkRequirements.Install(harmony);
        }
    }
    public static class RejectedPawn
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var code = instructions.ToList();
            var discard = AccessTools.Method(typeof(PawnGenerator), "DiscardGeneratedPawn");
            var matches = code.Select((c,i)=>new {c,i}).Where(x=>x.c.opcode==OpCodes.Ldstr
                && (string)x.c.operand=="Generated pawn incapable of required skill: ").ToArray();
            if(matches.Length!=1) throw new InvalidOperationException("Pawn Generation Fixes: required-skill failure branch changed.");
            var at=matches[0].i-1;
            var previous=code.FindLastIndex(at, i=>i.Calls(discard));
            if(at<0 || code[at].opcode!=OpCodes.Ldarg_1 || previous<2
                || code[previous-1].opcode!=OpCodes.Ldfld
                || !((FieldInfo)code[previous-1].operand).FieldType.Equals(typeof(Pawn)))
                throw new InvalidOperationException("Pawn Generation Fixes: rejected pawn load pattern changed.");
            // Other rejection branches already call the engine's disposal routine.
            // This branch returned null without removing generated relationships.
            var first=new CodeInstruction(code[previous-2].opcode,code[previous-2].operand);
            first.labels.AddRange(code[at].labels); code[at].labels.Clear();
            first.blocks.AddRange(code[at].blocks); code[at].blocks.Clear();
            code.InsertRange(at,new[] {first,
                new CodeInstruction(code[previous-1].opcode,code[previous-1].operand),
                new CodeInstruction(OpCodes.Call,discard)});
            return code;
        }
    }
}
