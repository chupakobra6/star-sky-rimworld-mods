using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace Igor.RU3506573327
{
    [StaticConstructorOnStartup]
    internal static class PresetLabels
    {
        private static readonly FieldInfo Name=AccessTools.Field("UsefulMarks.Preset:name");
        private static readonly FieldInfo IsDefault=AccessTools.Field("UsefulMarks.Preset:defaultPreset");
        static PresetLabels()
        {
            var harmony=new Harmony("igor.ru.3506573327.labels");
            foreach(string method in new[]{"DrawPreset","LoadMarkers"})
                harmony.Patch(AccessTools.Method("UsefulMarks.Dialog_PresetManager:"+method),
                    transpiler:new HarmonyMethod(typeof(PresetLabels),nameof(RewriteDisplayNames)));
            harmony.Patch(AccessTools.Method("UsefulMarks.Dialog_EditMarkers:OpenPasteButtonMenu"),
                transpiler:new HarmonyMethod(typeof(PresetLabels),nameof(RepairPasteLabel)));
        }
        public static string DisplayName(object preset)
        {
            string name=(string)Name.GetValue(preset);
            if(!(bool)IsDefault.GetValue(preset))return name;
            switch(name)
            {
                case "Empty": return "chupakobra6_3506573327_PresetEmpty".Translate();
                case "Default": return "chupakobra6_3506573327_PresetDefault".Translate();
                case "Combat": return "chupakobra6_3506573327_PresetCombat".Translate();
                default: return name;
            }
        }
        public static IEnumerable<CodeInstruction> RewriteDisplayNames(IEnumerable<CodeInstruction> input)
        {
            var instructions=input.ToList();
            for(int i=0;i<instructions.Count;i++)
            {
                var copy=new CodeInstruction(instructions[i]);
                var next=i+1<instructions.Count?instructions[i+1].operand as MethodInfo:null;
                // Preserve the separate name comparison used to link a saved
                // preset. Only its visible label/message receives a new value.
                if(copy.opcode==OpCodes.Ldfld&&Equals(copy.operand,Name)&&next!=null&&
                    ((next.DeclaringType==typeof(Widgets)&&next.Name=="Label")||
                     (next.DeclaringType==typeof(string)&&next.Name=="Format")))
                {copy.opcode=OpCodes.Call;copy.operand=AccessTools.Method(typeof(PresetLabels),nameof(DisplayName));}
                yield return copy;
            }
        }
        public static IEnumerable<CodeInstruction> RepairPasteLabel(IEnumerable<CodeInstruction> input)
        {
            foreach(var instruction in input)
            {
                var copy=new CodeInstruction(instruction);
                if(copy.opcode==OpCodes.Ldstr&&Equals(copy.operand,"PasteMarkers"))copy.operand="chupakobra6_3506573327_PasteMarkers";
                yield return copy;
            }
        }
    }
}
