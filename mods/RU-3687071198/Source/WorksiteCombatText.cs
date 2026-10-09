using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace Igor.WorksitesRussian
{
    public static class WorksiteCombatText
    {
        public static string Location(string part)
        {
            switch(part)
            {
                case "head": part="голова";break;
                case "torso": part="туловище";break;
                case "arm": part="рука";break;
                case "leg": part="нога";break;
                case "a graze": return "Попадание по касательной.";
            }
            return "Место попадания: "+part+".";
        }
        public static string Rounds(object number)
        {
            int n=Math.Abs(Convert.ToInt32(number))%100;
            return number+" "+(n>=11&&n<=14?"раундов":n%10==1?"раунд":n%10>=2&&n%10<=4?"раунда":"раундов");
        }
        public static string UiOne(string format,object value)
        {
            if(format=="Banking aim ({0} held).")return "Улучшает прицеливание (накоплено: "+value+").";
            if(format=="Reload to {0}.")return "Перезаряжает оружие до полного магазина ("+value+" патр.).";
            if(format=="Out for {0} round(s), tends a wound.")return "Перевязывает рану и пропускает "+Rounds(value)+".";
            if(format=="Tending wounds. Back in {0}.")return "Перевязывает раны. Вернётся через "+Rounds(value)+".";
            return string.Format(format,value);
        }
        public static string Wound(string format,object a,object b)
        {
            switch(format)
            {
                case "is hit in the {0} for {1} and stops moving.": return "получает "+b+" урона и перестаёт двигаться. "+Location(a as string);
                case "is hit in the {0} for {1}, and does not get up.": return "получает "+b+" урона и больше не встаёт. "+Location(a as string);
                case "takes {0} to the {1} and goes limp.": return "получает "+a+" урона и безвольно обмякает. "+Location(b as string);
                case "takes {0} to the {1} and drops, out of the fight.": return "получает "+a+" урона, падает и выбывает из боя. "+Location(b as string);
                case "is hit in the {0} for {1}.": return "получает "+b+" урона. "+Location(a as string);
                default: return string.Format(format,a,b);
            }
        }
    }
    public static class WorksiteCombatPatch
    {
        public static string Wound(string format,object a,object b)=>LanguageDatabase.activeLanguage?.folderName.StartsWith("Russian",StringComparison.OrdinalIgnoreCase)==true
            ?WorksiteCombatText.Wound(format,a,b):string.Format(format,a,b);
        public static void Install(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method("MiningOutpost.Orbital.Services.FirefightSim:ApplyWound"),transpiler:new HarmonyMethod(typeof(WorksiteCombatPatch),nameof(Rewrite)));
            foreach(string method in new[]{"OrderHint","DrawCrewCard"})harmony.Patch(AccessTools.Method("MiningOutpost.Orbital.Services.Dialog_Firefight:"+method),transpiler:new HarmonyMethod(typeof(WorksiteCombatPatch),nameof(UiRewrite)));
        }
        public static string UiOne(string format,object value)=>LanguageDatabase.activeLanguage?.folderName.StartsWith("Russian",StringComparison.OrdinalIgnoreCase)==true
            ?WorksiteCombatText.UiOne(format,value):string.Format(format,value);
        public static IEnumerable<CodeInstruction> UiRewrite(IEnumerable<CodeInstruction> input,MethodBase __originalMethod)
        {
            var format=typeof(string).GetMethod("Format",new[]{typeof(string),typeof(object)});int count=0;
            foreach(var instruction in input)
            {
                var code=new CodeInstruction(instruction);
                if(code.Calls(format)){code.operand=AccessTools.Method(typeof(WorksiteCombatPatch),nameof(UiOne));count++;}
                yield return code;
            }
            int expected=__originalMethod.Name=="OrderHint"?3:1;
            if(count!=expected)throw new InvalidOperationException("Worksites combat hint formats changed: "+__originalMethod+" "+count);
        }
        public static IEnumerable<CodeInstruction> Rewrite(IEnumerable<CodeInstruction> input)
        {
            var format=typeof(string).GetMethod("Format",new[]{typeof(string),typeof(object),typeof(object)});int count=0;
            foreach(var instruction in input)
            {
                var code=new CodeInstruction(instruction);
                if(code.Calls(format)){code.operand=AccessTools.Method(typeof(WorksiteCombatPatch),nameof(Wound));count++;}
                yield return code;
            }
            if(count!=5)throw new InvalidOperationException("Worksites wound log formatter changed: "+count);
        }
    }
}
