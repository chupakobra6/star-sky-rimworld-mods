using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Igor.RU_3112549163
{
    // DefInjected addresses StatPart labels by list index. Other active mods
    // can insert parts before EBSG, so resolve the reviewed text by CLR type
    // after the final Def list exists and before language injection runs.
    internal static class SemanticParts
    {
        private sealed class Label
        {
            internal string Owner, DefName, PartType, Russian, Requires;
        }

        internal static readonly List<string> Applied = new List<string>();
		private static List<Label> Labels;

        internal static void Install(ModContentPack content)
        {
			if (Labels != null) return;
			Labels = ReadLabels(content.RootDir);
            new Harmony("igor.ru.3112549163.semantic-parts").Patch(
                AccessTools.Method(typeof(LoadedLanguage), "InjectIntoData_BeforeImpliedDefs"),
                prefix: new HarmonyMethod(typeof(SemanticParts), nameof(Inject)));
        }

        private static List<Label> ReadLabels(string root)
        {
            var xml = new XmlDocument();
            xml.Load(Path.Combine(root, "DefPartLabels.xml"));
            var result = new List<Label>();
            foreach (XmlElement node in xml.SelectNodes("/labels/li"))
                result.Add(new Label {
                    Owner = node.GetAttribute("owner"),
                    DefName = node.GetAttribute("defName"),
                    PartType = node.GetAttribute("partType"),
                    Russian = node.SelectSingleNode("russian").InnerText,
                    Requires = node.GetAttribute("requires")
                });
            return result;
        }

        private static void Inject(LoadedLanguage __instance)
        {
            if (!__instance.folderName.StartsWith("Russian", StringComparison.OrdinalIgnoreCase)) return;
            if (!(bool)AccessTools.Field(typeof(LoadedLanguage), "dataIsLoaded").GetValue(__instance))
                AccessTools.Method(typeof(LoadedLanguage), "LoadData").Invoke(__instance, null);
            var package = __instance.defInjections.FirstOrDefault(p => p.defType == typeof(StatDef));
            if (package == null)
            {
                package = new DefInjectionPackage(typeof(StatDef));
                __instance.defInjections.Add(package);
            }
            foreach (var label in Labels)
            {
                if (label.Requires.Length > 0 && !LoadedModManager.RunningModsListForReading.Any(m =>
                    m.PackageId.Equals(label.Requires, StringComparison.OrdinalIgnoreCase))) continue;
                var stat = DefDatabase<StatDef>.GetNamedSilentFail(label.DefName);
				if (stat != null && !string.Equals(stat.modContentPack?.PackageId, label.Owner, StringComparison.OrdinalIgnoreCase))
				{
					Log.Error("[RU EBSG] StatDef owner changed: " + label.DefName);
					continue;
				}
                var matches = stat?.parts?.Where(p => p.GetType().FullName == label.PartType).ToList();
                if (matches == null || matches.Count != 1)
                {
                    Log.Error("[RU EBSG] Expected one StatPart " + label.DefName + "/" + label.PartType);
                    continue;
                }
                var key = label.DefName + ".parts." + stat.parts.IndexOf(matches[0]) + ".label";
                package.injections[key] = new DefInjectionPackage.DefInjection {
                    path = key,
                    nonBackCompatiblePath = key,
                    injection = label.Russian,
                    fileSource = "DefPartLabels.xml"
                };
                Applied.Add(key + " -> " + label.PartType);
            }
        }
    }
}
