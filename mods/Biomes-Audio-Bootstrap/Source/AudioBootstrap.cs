using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using System.Xml;
using HarmonyLib;
using Verse;

namespace Igor.BiomesAudio
{
    public sealed class AudioBootstrap : Mod
    {
        public AudioBootstrap(ModContentPack content) : base(content)
        {
            Initialize(LoadedModManager.RunningModsListForReading.ToDictionary(
                mod => mod.PackageId, mod => mod.RootDir, StringComparer.OrdinalIgnoreCase), Log.Warning, GenFilePaths.SaveDataFolderPath);
            Log.Message("[Biomes Audio Bootstrap] Prepared " + Mapped + " verified PCM WAV replacements before early content loading.");
        }
        static Dictionary<string,string> replacements = new Dictionary<string,string>(StringComparer.Ordinal);
        public static int Mapped { get { return Volatile.Read(ref replacements).Count; } }
        internal static void Initialize(IDictionary<string,string> active, Action<string> warn, string profile=null)
        {
            var next = new Dictionary<string,string>(StringComparer.Ordinal);
            foreach(var pairIDs in new[]{
                new[]{"biomesteam.biomescore", "starsky.rimworld.biomescore.fixes"},
                new[]{"biomesteam.biomescaverns", "starsky.rimworld.biomescaverns.fixes"},
                new[]{"biomesteam.biomesprehistoric", "starsky.rimworld.biomesprehistoric.fixes"},
                new[]{"jellycreative.isekaileveling", "starsky.rimworld.isekai.audiofix"},
                new[]{"dankpyon.medieval.overhaul", "starsky.rimworld.medievaloverhaul.audiofix"},
                new[]{"syrchalis.processor.framework", "starsky.rimworld.processorframework.audiofix"}})
            {
                string source, fix;
                if(!active.TryGetValue(pairIDs[0],out source)
                    || !active.TryGetValue(pairIDs[1],out fix)) continue;
                try
                {
                    // A pair becomes visible only when its entire manifest is verified.
                    // A broken pair cannot hide or block independent healthy pairs.
                    var pair = ReadPair(source,fix);
                    if(pair.Keys.Any(next.ContainsKey)) throw new InvalidDataException("Audio source belongs to more than one pair");
                    foreach(var entry in pair) next.Add(entry.Key,entry.Value);
                }
                catch(Exception error)
                {
                    warn("[Biomes Audio Bootstrap] " + pairIDs[0] + " uses original audio: " + error.Message);
                }
            }
            string profileManifest=profile==null ? null : Path.Combine(profile,"Config","validation-audio-cache.xml");
            if(active.ContainsKey("igor.preset.validation") && profileManifest!=null && File.Exists(profileManifest))
            {
                var xml=new XmlDocument();xml.Load(profileManifest);
                if(xml.DocumentElement.Name!="validationAudioCache")throw new InvalidDataException("Invalid validation audio cache manifest");
                foreach(XmlElement entry in xml.SelectNodes("/validationAudioCache/pair"))
                {
                    string package=entry.GetAttribute("package"),source;
                    if(!active.TryGetValue(package,out source))throw new InvalidDataException("Validation audio source is inactive: "+package);
                    var pair=ReadPair(source,Under(profile,entry.GetAttribute("cache")));
                    if(pair.Keys.Any(next.ContainsKey))throw new InvalidDataException("Validation audio source has another active cache: "+package);
                    foreach(var item in pair)next.Add(item.Key,item.Value);
                }
            }
            Volatile.Write(ref replacements,next);
            var target = AccessTools.Method(typeof(GenFilePaths),"SafeURIForUnityWWWFromPath",new[]{typeof(string)});
            if(target==null || target.IsGenericMethod || target.DeclaringType.IsGenericType)
                throw new MissingMethodException("Non-generic Unity audio URI helper is unavailable");
            new Harmony("igor.biomes.audio.bootstrap").Patch(target,
                postfix:new HarmonyMethod(typeof(AudioBootstrap),nameof(Redirect)));
        }
        static Dictionary<string,string> ReadPair(string source, string fix)
        {
            var result = new Dictionary<string,string>(StringComparer.Ordinal);
            foreach(string line in File.ReadLines(Path.Combine(fix,"audio-manifest.tsv")))
            {
                var fields=line.Split('\t');
                if(fields.Length!=3) throw new InvalidDataException("Malformed audio manifest");
                string original=Under(source,fields[0]);
                string replacement=Under(Path.Combine(fix,"AudioCache"),fields[1]);
                if(!File.Exists(original) || !File.Exists(replacement))
                    throw new InvalidDataException("Missing audio asset: "+fields[0]);
                using(var sha=SHA256.Create()) using(var stream=File.OpenRead(original))
                    if(BitConverter.ToString(sha.ComputeHash(stream)).Replace("-","").ToLowerInvariant()!=fields[2])
                        throw new InvalidDataException("Workshop audio changed; rebuild local audio fix: "+fields[0]);
                result.Add(original,new Uri(replacement).AbsoluteUri);
            }
            return result;
        }
        static string Under(string root, string relative)
        {
            root=Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar)+Path.DirectorySeparatorChar;
            string path=Path.GetFullPath(Path.Combine(root,relative));
            if(Path.IsPathRooted(relative) || !path.StartsWith(root,StringComparison.Ordinal))
                throw new InvalidDataException("Audio manifest path escapes its owner");
            return path;
        }
        static void Redirect(string rawPath, ref string __result)
        {
            string replacement;
            if(Volatile.Read(ref replacements).TryGetValue(rawPath,out replacement)) __result=replacement;
        }
    }
}
