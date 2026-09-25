using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace Igor.CharacterEditorRussian
{
    [StaticConstructorOnStartup]
    internal static class EditorBlockLabels
    {
        static EditorBlockLabels() { LongEventHandler.ExecuteWhenFinished(Install); }

        static void Install()
        {
            var type=AccessTools.TypeByName("CharacterEditor.Listing_X");
            // Mono shares native generic bodies between reference types. Clone the
            // capacity-only call chain; never detour a shared generic original.
            Copy(AccessTools.Method(type,"BlockLabel").MakeGenericMethod(typeof(PawnCapacityDef)),nameof(DrawLabel),nameof(Translate));
            Copy(AccessTools.Method(type,"ListViewParam").MakeGenericMethod(typeof(PawnCapacityDef)),nameof(DrawRow),nameof(RouteLabel));
            Copy(AccessTools.Method(type,"FullListViewParam").MakeGenericMethod(typeof(PawnCapacityDef),typeof(PawnCapacityModifier)),nameof(DrawList),nameof(RouteRow));
            new Harmony("igor.ru.1874644848.capacity-labels").Patch(AccessTools.Method("CharacterEditor.DialogGenery:DrawCapacities"),
                transpiler:new HarmonyMethod(typeof(EditorBlockLabels),nameof(RouteList)));
        }

        static void Copy(MethodInfo source,string destination,string transpiler)
        {
            Harmony.ReversePatch(source,new HarmonyMethod(typeof(EditorBlockLabels),destination),
                AccessTools.Method(typeof(EditorBlockLabels),transpiler));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Rect DrawLabel(object listing,int padding,float width,PawnCapacityDef def,float value,float secValue)
            => throw new NotImplementedException("Character Editor capacity label copy was not installed");

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void DrawRow(object listing,float width,PawnCapacityDef def,ref PawnCapacityDef selected,
            ref float value,ref float secValue,float min,float max,bool isInt,bool remove,Action<PawnCapacityDef> removeAction)
            => throw new NotImplementedException("Character Editor capacity row copy was not installed");

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void DrawList(object listing,List<PawnCapacityModifier> items,ref PawnCapacityDef selected,
            Func<PawnCapacityModifier,PawnCapacityDef> getDef,Func<PawnCapacityModifier,float> getValue,
            Func<PawnCapacityModifier,float> getSecValue,Func<PawnCapacityModifier,float> getMin,
            Func<PawnCapacityModifier,float> getMax,bool isInt,bool remove,
            Action<PawnCapacityModifier,float> setValue,Action<PawnCapacityModifier,float> setSecValue,
            Action<PawnCapacityDef> removeAction)
            => throw new NotImplementedException("Character Editor capacity list copy was not installed");

        static IEnumerable<CodeInstruction> RouteLabel(IEnumerable<CodeInstruction> input) => Route(input,"BlockLabel",nameof(DrawLabel));
        static IEnumerable<CodeInstruction> RouteRow(IEnumerable<CodeInstruction> input) => Route(input,"ListViewParam",nameof(DrawRow));
        static IEnumerable<CodeInstruction> RouteList(IEnumerable<CodeInstruction> input) => Route(input,"FullListViewParam",nameof(DrawList));

        static IEnumerable<CodeInstruction> Route(IEnumerable<CodeInstruction> input,string source,string target)
        {
            int changed=0;
            foreach(var original in input)
            {
                var instruction=new CodeInstruction(original);
                var method=instruction.operand as MethodInfo;
                if(method!=null&&method.DeclaringType.FullName=="CharacterEditor.Listing_X"&&method.Name==source)
                {
                    instruction.opcode=OpCodes.Call;instruction.operand=AccessTools.Method(typeof(EditorBlockLabels),target);changed++;
                }
                yield return instruction;
            }
            if(changed!=1)throw new InvalidOperationException("Character Editor capacity call chain changed: "+source+" / "+changed);
        }

        static IEnumerable<CodeInstruction> Translate(IEnumerable<CodeInstruction> input)
        {
            int changed=0;
            foreach(var original in input)
            {
                var instruction=new CodeInstruction(original);
                if(instruction.opcode==OpCodes.Ldstr)
                {
                    string value=(string)instruction.operand;
                    string key=value==" offset: " ? "CapacityOffset" : value=="      factor: " ? "CapacityFactor" : null;
                    if(key!=null)
                    {
                        if(EditorText.Russian)instruction.operand=EditorText.Value(key);
                        changed++;
                    }
                }
                yield return instruction;
            }
            if(changed!=2)throw new InvalidOperationException("Character Editor capacity label literals changed: "+changed);
        }
    }
}
