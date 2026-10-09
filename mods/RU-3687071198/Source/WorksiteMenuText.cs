using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace Igor.WorksitesRussian
{
    public static class WorksiteMenuText
    {
        private static bool Russian=>LanguageDatabase.activeLanguage?.folderName.StartsWith("Russian",StringComparison.OrdinalIgnoreCase)==true;
        public static void Install(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method("MiningOutpost.OrbitalPlatformRegistry:GetDockAtLabel"),postfix:new HarmonyMethod(typeof(WorksiteMenuText),nameof(DockLabel)));
            harmony.Patch(AccessTools.Method("MiningOutpost.Orbital.Dialog_OrbitalServices:DrawHeader"),transpiler:new HarmonyMethod(typeof(WorksiteMenuText),nameof(OrbitalAlignment)));
            harmony.Patch(AccessTools.Method("MiningOutpost.Parley.Dialog_Parley:BuildSpecificOptions"),postfix:new HarmonyMethod(typeof(WorksiteMenuText),nameof(RoleOptions)));
        }
        public static void DockLabel(ref string __result)
        {
            if(Russian&&__result=="Dock at orbital platform")__result="Пристыковаться к орбитальной станции";
        }
        public static string Alignment(string source)=>Russian?WorksiteParleyPatch.AlignmentLabel(source):source;
        public static void RoleOptions(object __instance,object __0)
        {
            if(!Russian||Convert.ToInt32(__0)!=1)return;
            var options=(IList)AccessTools.Field(__instance.GetType(),"specificOptions").GetValue(__instance);
            for(int i=0;i<options.Count;i++)
            {
                var option=options[i];var type=option.GetType();
                var specifics=AccessTools.Field(type,"specifics").GetValue(option);
                if(specifics==null)continue;
                string role=(string)AccessTools.Field(specifics.GetType(),"specificRole").GetValue(specifics);
                if(role==null)continue;
                AccessTools.Field(type,"label").SetValue(option,WorksiteDemandText.Role(role));
                options[i]=option;
            }
        }
        public static IEnumerable<CodeInstruction> OrbitalAlignment(IEnumerable<CodeInstruction> input)
        {
            var field=AccessTools.Field("MiningOutpost.Orbital.Dialog_OrbitalServices:alignmentLabel");int count=0;
            foreach(var instruction in input)
            {
                yield return instruction;
                if(instruction.opcode==OpCodes.Ldfld&&Equals(instruction.operand,field))
                {yield return new CodeInstruction(OpCodes.Call,AccessTools.Method(typeof(WorksiteMenuText),nameof(Alignment)));count++;}
            }
            if(count!=1)throw new InvalidOperationException("Worksites orbital alignment display changed: "+count);
        }
    }
}
