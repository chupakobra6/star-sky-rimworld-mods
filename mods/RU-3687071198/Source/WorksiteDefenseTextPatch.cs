using System;using System.Linq;using System.Collections.Generic;using System.Reflection;using HarmonyLib;using Verse;
namespace Igor.WorksitesRussian{public static class WorksiteDefenseTextPatch{
private static bool Russian=>LanguageDatabase.activeLanguage?.folderName.StartsWith("Russian",StringComparison.OrdinalIgnoreCase)==true;
private static readonly Dictionary<string,Dictionary<string,int>> Counts=new Dictionary<string,Dictionary<string,int>>{
{"MiningOutpost.WorkSiteDefenseTracker:FireDefenseRaid",new Dictionary<string,int>{{"Array",1},{"Two",1}}},
{"MiningOutpost.WorkSiteDefenseTracker:OnDefenseSuccess",new Dictionary<string,int>{{"One",1},{"Array",1}}},
{"MiningOutpost.WorkSiteDefenseTracker:OnDefenseOverrun",new Dictionary<string,int>{{"Three",1},{"Array",1}}},
{"MiningOutpost.WorkSiteDefenseTracker:TrySwitchRaidersToSacking",new Dictionary<string,int>{{"Three",3}}},
{"MiningOutpost.WorkSiteDefenseTracker:Resolve",new Dictionary<string,int>{{"Four",1},{"One",1},{"Array",1}}},
{"MiningOutpost.WorkSiteDefenseTracker:CheckIdleConspiracy",new Dictionary<string,int>{{"Array",1},{"Four",1}}},
{"MiningOutpost.WorkSiteDefenseTracker:SiteDesc",new Dictionary<string,int>{{"Two",1},{"Four",1}}},
{"MiningOutpost.WorkSiteDefenseTracker:NotifyFriendlyFire",new Dictionary<string,int>{{"Array",1},{"Two",1}}},
{"MiningOutpost.WorkSiteDefenseTracker:NotifyWorksiteKilled",new Dictionary<string,int>{{"Array",1},{"Two",1}}},
{"MiningOutpost.WorkSiteDefenseTracker:NotifyTheft",new Dictionary<string,int>{{"Three",3},{"Two",1}}},
{"MiningOutpost.Patch_WorksiteDefense_UnpoweredTurretNotice:Postfix",new Dictionary<string,int>{{"Array",1}}}};
public static void Install(Harmony h){foreach(var key in Counts.Keys)h.Patch(AccessTools.Method(key),transpiler:new HarmonyMethod(typeof(WorksiteDefenseTextPatch),nameof(Rewrite)));}
public static string Two(string a,string b)=>Russian?WorksiteDefenseText.Two(a,b):string.Concat(a,b);
public static string Three(string a,string b,string c)=>Russian?WorksiteDefenseText.Three(a,b,c):string.Concat(a,b,c);
public static string Four(string a,string b,string c,string d)=>Russian?WorksiteDefenseText.Four(a,b,c,d):string.Concat(a,b,c,d);
public static string Array(string[] p)=>Russian?WorksiteDefenseText.Array(p):string.Concat(p);
public static string One(string f,object x)=>Russian?WorksiteDefenseText.One(f,x):string.Format(f,x);
public static IEnumerable<CodeInstruction> Rewrite(IEnumerable<CodeInstruction> input,MethodBase __originalMethod){
var expected=Counts[__originalMethod.DeclaringType.FullName+":"+__originalMethod.Name];var seen=new Dictionary<string,int>();var output=new List<CodeInstruction>();
foreach(var i in input){var c=new CodeInstruction(i);if(c.operand is MethodInfo m&&m.DeclaringType==typeof(string)){
var types=m.GetParameters().Select(p=>p.ParameterType).ToArray();string name=null;
if(m.Name=="Concat"&&types.Length>=2&&types.Length<=4&&types.All(t=>t==typeof(string)))name=types.Length==2?"Two":types.Length==3?"Three":"Four";
else if(m.Name=="Concat"&&types.SequenceEqual(new[]{typeof(string[])}))name="Array";
else if(m.Name=="Format"&&types.SequenceEqual(new[]{typeof(string),typeof(object)}))name="One";
if(name!=null){c.operand=AccessTools.Method(typeof(WorksiteDefenseTextPatch),name);seen[name]=seen.TryGetValue(name,out int count)?count+1:1;}}
output.Add(c);}
if(seen.Count!=expected.Count||expected.Any(p=>!seen.TryGetValue(p.Key,out int n)||n!=p.Value))throw new InvalidOperationException("Defense text formatter contract changed: "+__originalMethod);
return output;
}
}}
