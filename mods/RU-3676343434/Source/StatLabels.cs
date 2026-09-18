using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Igor.RU_3676343434
{
    // The editor stores StatDef identifiers. Resolve their displayed names without
    // changing settings keys or requiring a global Keyed entry for every modded stat.
    public sealed class StatLabels : Mod
    {
        public StatLabels(ModContentPack content) : base(content)
        {
            new Harmony("igor.ru.3676343434.statlabels").Patch(
                AccessTools.Method("PerformanceEsmolas.PerformanceEsmolasMod:DoSettingsWindowContents"),
                transpiler:new HarmonyMethod(typeof(StatLabels),nameof(Transpile)));
        }
        public static TaggedString TranslateLabel(string key)
        {
            if(LanguageDatabase.activeLanguage?.folderName.StartsWith("Russian",StringComparison.OrdinalIgnoreCase)==true)
            {
                var stat=DefDatabase<StatDef>.GetNamedSilentFail(key);
                if(stat!=null)return stat.LabelCap;
            }
            return key.Translate();
        }
        private static IEnumerable<CodeInstruction> Transpile(IEnumerable<CodeInstruction> instructions)
        {
            var original=AccessTools.Method(typeof(Translator),nameof(Translator.Translate),new[]{typeof(string)});
            var replacement=AccessTools.Method(typeof(StatLabels),nameof(TranslateLabel));
            int matches=0;
            foreach(var instruction in instructions)
            {
                if(instruction.Calls(original)){instruction.opcode=OpCodes.Call;instruction.operand=replacement;matches++;}
                yield return instruction;
            }
            if(matches!=20)throw new InvalidOperationException("Esmolas settings translation calls changed: "+matches);
        }
    }
}
