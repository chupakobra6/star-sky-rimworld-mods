using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace Igor.WorksitesRussian
{
    public static class WorksiteCardPatch
    {
        private static bool Russian=>LanguageDatabase.activeLanguage?.folderName.StartsWith("Russian",StringComparison.OrdinalIgnoreCase)==true;
        private static readonly FieldInfo CategoryField=AccessTools.Field("MiningOutpost.Orbital.Services.HandEval:CategoryNames");
        public static void Install(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method("MiningOutpost.Orbital.Services.Card:RankString"),postfix:new HarmonyMethod(typeof(WorksiteCardPatch),nameof(FacePostfix)));
            harmony.Patch(AccessTools.Method("MiningOutpost.Orbital.Services.HandEval:RankWord"),postfix:new HarmonyMethod(typeof(WorksiteCardPatch),nameof(RankPostfix)));
            foreach(string target in new[]{"MiningOutpost.Orbital.Services.HandEval:ShowdownHandText","MiningOutpost.Orbital.Services.PokerSim:PlayerHandReadable","MiningOutpost.Orbital.Services.PokerSim:HandCategoryReadable"})
                harmony.Patch(AccessTools.Method(target),transpiler:new HarmonyMethod(typeof(WorksiteCardPatch),nameof(RewriteCategories)));
            harmony.Patch(AccessTools.Method("MiningOutpost.Orbital.Services.OrbitalGamblingTable:DrawActionBadge"),
                transpiler:new HarmonyMethod(typeof(WorksiteCardPatch),nameof(RewriteBadge)));
        }
        public static void FacePostfix(byte __0,ref string __result){if(Russian)__result=WorksiteCardText.Face(__0);}
        public static void RankPostfix(int __0,bool __1,ref string __result){if(Russian)__result=WorksiteCardText.RankWord(__0,__1);}
        public static string[] Categories()=>Russian?WorksiteCardText.Categories:(string[])CategoryField.GetValue(null);
        public static string Badge(string original)=>Russian?WorksiteCardText.Badge(original):original;
        public static IEnumerable<CodeInstruction> RewriteCategories(IEnumerable<CodeInstruction> input,MethodBase __originalMethod)
        {
            int count=0;
            foreach(var original in input)
            {
                var instruction=new CodeInstruction(original);
                if(instruction.opcode==OpCodes.Ldsfld&&Equals(instruction.operand,CategoryField))
                {instruction.opcode=OpCodes.Call;instruction.operand=AccessTools.Method(typeof(WorksiteCardPatch),nameof(Categories));count++;}
                yield return instruction;
            }
            int expected=__originalMethod.Name=="PlayerHandReadable"?1:2;
            if(count!=expected)throw new InvalidOperationException("Worksites card category reads changed: "+__originalMethod+" expected "+expected+", got "+count);
        }
        public static IEnumerable<CodeInstruction> RewriteBadge(IEnumerable<CodeInstruction> input)
        {
            int reads=0,translated=0;
            foreach(var original in input)
            {
                var instruction=new CodeInstruction(original);yield return instruction;
                if(instruction.opcode!=OpCodes.Ldarg_1)continue;
                reads++;
                // First three reads perform empty/sentinel checks and English
                // color classification. Only CalcSize and Label get Russian.
                if(reads>3){yield return new CodeInstruction(OpCodes.Call,AccessTools.Method(typeof(WorksiteCardPatch),nameof(Badge)));translated++;}
            }
            if(reads!=5||translated!=2)throw new InvalidOperationException("Worksites action badge data flow changed: "+reads+" reads, "+translated+" displays");
        }
    }
}
