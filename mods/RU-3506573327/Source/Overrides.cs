using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using HarmonyLib;
using RimWorld.IO;
using Verse;

namespace Igor.RU_3506573327
{
    // Replace only explicitly reviewed Russian resources before the parser sees them.
    // Full replacement also removes obsolete fields and mixed index/handle paths.
    public sealed class ReviewedOverrides : Mod
    {
        private static readonly bool FullReplacement = true;
        private static readonly bool ReviewedDefinitions = false;
        private static readonly bool ReplaceDirectories = false;
        private static readonly Dictionary<string,HashSet<string>> Keys = new Dictionary<string,HashSet<string>>();
        private static readonly Dictionary<string,HashSet<string>> Lists = new Dictionary<string,HashSet<string>>();
        private static readonly List<string> SourceRoots = new List<string>();
        private static readonly Dictionary<string,Dictionary<string,string[]>> WordTables = new Dictionary<string,Dictionary<string,string[]>>();
        private static readonly Dictionary<string,Gender> WordGenders = new Dictionary<string,Gender>();
        public ReviewedOverrides(ModContentPack content) : base(content)
        {
            var manifest = new XmlDocument();
            manifest.Load(Path.Combine(content.RootDir,"OverrideSources.xml"));
            var sources = new HashSet<string>(manifest.SelectNodes("/sources/li").Cast<XmlNode>().Select(n=>n.InnerText),StringComparer.OrdinalIgnoreCase);
            SourceRoots.AddRange(LoadedModManager.RunningModsListForReading.Where(m=>sources.Contains(m.PackageId))
                .Select(m=>m.RootDir.TrimEnd('/','\\')+"/"));
            var harmony = new Harmony("igor.ru.3506573327.overrides");
            LoadWordInfo(Path.Combine(content.RootDir,"Languages","Russian","WordInfo"),harmony);
            if(sources.Count == 0)return;
            if(ReplaceDirectories)
            {
                harmony.Patch(AccessTools.PropertyGetter(typeof(LoadedLanguage),"AllDirectories"),
                    postfix:new HarmonyMethod(typeof(ReviewedOverrides),nameof(FilterDirectories)));
                return;
            }
            var directory = Path.Combine(content.RootDir,"Languages","Russian");
            foreach(var file in Directory.GetFiles(directory,"*.xml",SearchOption.AllDirectories))
            {
                var relative = file.Substring(directory.Length+1).Replace('\\','/').Split('/');
                var kind = relative[0] == "Keyed" ? "Keyed" : relative[0] == "DefInjected" ? relative[1] : null;
                if(kind == null) continue;
                if(!Keys.ContainsKey(kind)) { Keys[kind]=new HashSet<string>(); Lists[kind]=new HashSet<string>(); }
                var xml = new XmlDocument(); xml.Load(file);
                foreach(XmlNode entry in xml.DocumentElement.ChildNodes)
                {
                    if(entry.NodeType != XmlNodeType.Element) continue;
                    Keys[kind].Add(entry.Name);
                    if(entry.SelectSingleNode("li") != null) Lists[kind].Add(entry.Name);
                }
            }
            harmony.Patch(AccessTools.Method(typeof(LoadedLanguage),"LoadFromFile_DefInject"),
                prefix: new HarmonyMethod(typeof(ReviewedOverrides),nameof(FilterDefs)));
            harmony.Patch(AccessTools.Method(typeof(LoadedLanguage),"LoadFromFile_Keyed"),
                prefix: new HarmonyMethod(typeof(ReviewedOverrides),nameof(FilterKeyed)));
        }
        private static void LoadWordInfo(string directory,Harmony harmony)
        {
            foreach(var name in new[]{"Case","Plural"})
            {
                var file=Path.Combine(directory,name+".txt");
                if(!File.Exists(file))continue;
                var table=new Dictionary<string,string[]>();
                foreach(var line in File.ReadAllLines(file))
                {
                    string[] values;
                    if(string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("//"))continue;
                    if(GenText.TryGetSeparatedValues(line,';',out values))table[values[0].ToLowerInvariant()]=values;
                }
                WordTables[name.ToLowerInvariant()]=table;
            }
            foreach(var gender in new[]{Gender.Male,Gender.Female,Gender.None})
            {
                var file=Path.Combine(directory,"Gender",(gender==Gender.None ? "Neuter" : gender.ToString())+".txt");
                if(!File.Exists(file))continue;
                foreach(var line in File.ReadAllLines(file))
                {
                    var word=line.Trim();
                    if(word.Length>0 && !word.StartsWith("//"))WordGenders[word.ToLowerInvariant()]=gender;
                }
            }
            if(WordTables.Count>0)harmony.Patch(AccessTools.Method(typeof(LanguageWordInfo),"RegisterLut"),
                postfix:new HarmonyMethod(typeof(ReviewedOverrides),nameof(OverrideWordTable)));
            if(WordGenders.Count>0)harmony.Patch(AccessTools.Method(typeof(LanguageWordInfo),"LoadFrom"),
                postfix:new HarmonyMethod(typeof(ReviewedOverrides),nameof(OverrideWordGenders)));
        }
        private static void OverrideWordTable(string __0,Dictionary<string,Dictionary<string,string[]>> ___lookupTables)
        {
            var language=LanguageDatabase.activeLanguage;
            if(language==null || !language.folderName.StartsWith("Russian",StringComparison.OrdinalIgnoreCase))return;
            Dictionary<string,string[]> reviewed,loaded;
            var name=__0.ToLowerInvariant();
            if(!WordTables.TryGetValue(name,out reviewed) || !___lookupTables.TryGetValue(name,out loaded))return;
            // The installed loader keeps the first lemma. Override only our
            // reviewed entries; a retained multi-mod bundle still supplies others.
            foreach(var pair in reviewed)loaded[pair.Key]=pair.Value;
        }
        private static void OverrideWordGenders(Tuple<VirtualDirectory,ModContentPack,string> __0,LoadedLanguage __1,Dictionary<string,Gender> ___genders)
        {
            if(!__1.folderName.StartsWith("Russian",StringComparison.OrdinalIgnoreCase))return;
            if(__0.Item2==null || !string.Equals(__0.Item2.PackageId,"starsky.rimworld.ru.3506573327",StringComparison.OrdinalIgnoreCase))return;
            // Once our own directory has loaded, later first-wins readers cannot
            // replace these entries. No need to revisit every other mod folder.
            foreach(var pair in WordGenders)___genders[pair.Key]=pair.Value;
        }
        private static void FilterDirectories(LoadedLanguage __instance,
            ref IEnumerable<Tuple<VirtualDirectory,ModContentPack,string>> __result)
        {
            // A reviewed complete replacement must precede Def type resolution:
            // an obsolete Def-type directory fails before LoadFromFile_DefInject.
            if(!__instance.folderName.StartsWith("Russian",StringComparison.OrdinalIgnoreCase))return;
            __result=__result.Where(entry=>!SourceRoots.Any(root=>
                entry.Item1.FullPath.Replace('\\','/').StartsWith(root,StringComparison.Ordinal)));
        }
        public static string RemoveSuperseded(string original, HashSet<string> keys, HashSet<string> lists)
        {
            var xml=new XmlDocument();xml.PreserveWhitespace=true;xml.LoadXml(original);
            bool changed=false;
            foreach(var entry in xml.DocumentElement.ChildNodes.Cast<XmlNode>().ToArray())
            {
                if(entry.NodeType!=XmlNodeType.Element)continue;
                if(keys.Contains(entry.Name) || lists.Any(key=>entry.Name.StartsWith(key+".",StringComparison.Ordinal)))
                { entry.ParentNode.RemoveChild(entry);changed=true; }
            }
            return changed ? xml.OuterXml : original;
        }
        public static string RemoveReviewedDefinitions(string original, HashSet<string> keys)
        {
            // A full translation can coexist with a multi-mod RU bundle. Replace
            // only its reviewed definitions, including obsolete numeric paths;
            // retain every other definition from that bundle.
            var definitions = new HashSet<string>(keys.Select(key => key.Split('.')[0]));
            var xml = new XmlDocument(); xml.PreserveWhitespace = true; xml.LoadXml(original);
            bool changed = false;
            foreach (var entry in xml.DocumentElement.ChildNodes.Cast<XmlNode>().ToArray())
            {
                if (entry.NodeType == XmlNodeType.Element && definitions.Contains(entry.Name.Split('.')[0]))
                { entry.ParentNode.RemoveChild(entry); changed = true; }
            }
            return changed ? xml.OuterXml : original;
        }
        private static void Filter(LoadedLanguage language,VirtualFile file,string kind,ref string contents)
        {
            if(!language.folderName.StartsWith("Russian",StringComparison.OrdinalIgnoreCase)
                || !SourceRoots.Any(root=>file.FullPath.Replace('\\','/').StartsWith(root,StringComparison.Ordinal)))return;
            if(FullReplacement) { contents="<LanguageData />";return; }
            if(!Keys.ContainsKey(kind))return;
            if(ReviewedDefinitions && kind != "Keyed")
            { contents=RemoveReviewedDefinitions(contents,Keys[kind]);return; }
            contents=RemoveSuperseded(contents,Keys[kind],Lists[kind]);
        }
        private static void FilterDefs(LoadedLanguage __instance,VirtualFile file,Type defType,ref string preloadedFileContents)
        {
            // A source may inject an inherited field through its concrete Def
            // subclass while our translation targets the owning base Def type.
            for(var type=defType;type!=null && typeof(Def).IsAssignableFrom(type);type=type.BaseType)
                Filter(__instance,file,type.Name,ref preloadedFileContents);
        }
        private static void FilterKeyed(LoadedLanguage __instance,VirtualFile file,ref string preloadedFileContents)
            => Filter(__instance,file,"Keyed",ref preloadedFileContents);
    }
}
