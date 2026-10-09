using System;using System.Collections.Generic;using System.Reflection;using System.Reflection.Emit;using HarmonyLib;using Verse;
namespace Igor.WorksitesRussian
{
 public static class WorksiteDescriptorPatch
 {
  private static bool Russian=>LanguageDatabase.activeLanguage?.folderName.StartsWith("Russian",StringComparison.OrdinalIgnoreCase)==true;
  private static readonly MethodInfo ArrayConcat=typeof(string).GetMethod("Concat",new[]{typeof(string[])});
  private static readonly MethodInfo SingleFormat=typeof(string).GetMethod("Format",new[]{typeof(string),typeof(object)});
  public static void Install(Harmony harmony){foreach(var target in new[]{"MiningOutpost.CorporateDescriptorGenerator:GenerateDescriptor","MiningOutpost.CorporateDescriptorGenerator:GenerateHomesteadDescriptor","MiningOutpost.OrbitalAlignment:GenerateDescriptor"})harmony.Patch(AccessTools.Method(target),transpiler:new HarmonyMethod(typeof(WorksiteDescriptorPatch),nameof(Rewrite)));}
  public static string Corporate(string[] parts)=>Russian?WorksiteDescriptorText.CorporateArray(parts):string.Concat(parts);
  public static string Natural(string[] parts)=>Russian?WorksiteDescriptorText.OrbitalNatural(parts):string.Concat(parts);
  public static string Contrast(string[] parts)=>Russian?WorksiteDescriptorText.OrbitalContrast(parts):string.Concat(parts);
  public static string Concession(string format,object value)=>Russian?WorksiteDescriptorText.Concession(format,value):string.Format(format,value);
  public static IEnumerable<CodeInstruction> Rewrite(IEnumerable<CodeInstruction> input,MethodBase __originalMethod){
   bool orbital=__originalMethod.DeclaringType.FullName=="MiningOutpost.OrbitalAlignment";int arrays=0,formats=0;
   foreach(var original in input){var instruction=new CodeInstruction(original);
    if(instruction.Calls(ArrayConcat)){instruction.operand=AccessTools.Method(typeof(WorksiteDescriptorPatch),orbital?(arrays==0?nameof(Natural):nameof(Contrast)):nameof(Corporate));arrays++;}
    else if(orbital&&instruction.Calls(SingleFormat)){instruction.operand=AccessTools.Method(typeof(WorksiteDescriptorPatch),nameof(Concession));formats++;}
    yield return instruction;
   }
   if(arrays!=(orbital?2:1)||formats!=(orbital?2:0))throw new InvalidOperationException("Worksites descriptor construction changed: "+__originalMethod+" arrays="+arrays+" formats="+formats);
  }
 }
}
