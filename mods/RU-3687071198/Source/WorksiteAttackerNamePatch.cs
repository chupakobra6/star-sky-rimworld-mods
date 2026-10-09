using System;using System.Collections.Generic;using System.Linq;using HarmonyLib;using Verse;
namespace Igor.WorksitesRussian{
public static class WorksiteAttackerNamePatch{
private static bool Russian=>LanguageDatabase.activeLanguage?.folderName.StartsWith("Russian",StringComparison.OrdinalIgnoreCase)==true;
public static void Install(Harmony h){h.Patch(AccessTools.Method("MiningOutpost.WorkSiteDefenseTracker:GenerateTemporaryAttackerName"),transpiler:new HarmonyMethod(typeof(WorksiteAttackerNamePatch),nameof(Rewrite)));h.Patch(AccessTools.Method("MiningOutpost.WorkSiteDefenseTracker:AttackerLabel"),postfix:new HarmonyMethod(typeof(WorksiteAttackerNamePatch),nameof(Label)));}
public static string Name(string a,string b,string c)=>Russian?WorksiteAttackerNames.Name(a,b,c):string.Concat(a,b,c);
public static void Label(ref string __result){if(Russian)__result=WorksiteAttackerNames.Label(__result);}
public static IEnumerable<CodeInstruction> Rewrite(IEnumerable<CodeInstruction> input){var target=AccessTools.Method(typeof(string),nameof(string.Concat),new[]{typeof(string),typeof(string),typeof(string)});var result=new List<CodeInstruction>();int count=0;foreach(var i in input){var c=new CodeInstruction(i);if(c.Calls(target)){c.operand=AccessTools.Method(typeof(WorksiteAttackerNamePatch),nameof(Name));count++;}result.Add(c);}if(count!=2)throw new InvalidOperationException("Attacker name concat contract changed: "+count);return result;}
}}
