using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using HarmonyLib;
using RimWorld.IO;
using Verse;

namespace Igor.RU_2891845502
{
    // AGI has an optional XML patch. Capture its startup setting, so a later
    // settings-window edit cannot change translation before the required restart.
    public sealed class IntegratedVariants : Mod
    {
        private static bool integrated;
        private static string root;
        private static string variants;
        public IntegratedVariants(ModContentPack content) : base(content)
        {
            root = content.RootDir.TrimEnd('/', '\\') + "/Languages/Russian/";
            variants = File.ReadAllText(Path.Combine(content.RootDir, "Variants/WithoutIntegrated.xml"));
            if (ModsConfig.IsActive("kupa.alphagenesintegrated"))
            {
                var field = AccessTools.Field("AlphaGenesIntegrated.AlphaGenesIntegratedSettings:AGI_Settings_ApplyGenepackPatch");
                if (field == null) throw new InvalidOperationException("AGI genepack setting changed upstream.");
                integrated = (bool)field.GetValue(null);
            }
            new Harmony("igor.ru.2891845502.variants").Patch(
                AccessTools.Method(typeof(LoadedLanguage), "LoadFromFile_DefInject"),
                prefix: new HarmonyMethod(typeof(IntegratedVariants), nameof(Filter)));
        }
        public static string SelectVariants(string original, string alternative, bool useIntegrated)
        {
            if (useIntegrated) return original;
            var xml = new XmlDocument(); xml.LoadXml(original);
            var choices = new XmlDocument(); choices.LoadXml(alternative);
            var replacements = choices.DocumentElement.ChildNodes.Cast<XmlNode>()
                .Where(n => n.NodeType == XmlNodeType.Element).ToDictionary(n => n.Name);
            bool changed = false;
            foreach (XmlNode entry in xml.DocumentElement.ChildNodes)
            {
                if (replacements.TryGetValue(entry.Name, out var replacement))
                {
                    entry.InnerXml = replacement.InnerXml;
                    changed = true;
                }
            }
            return changed ? xml.OuterXml : original;
        }
        private static void Filter(LoadedLanguage __instance, VirtualFile file, ref string preloadedFileContents)
        {
            if (!__instance.folderName.StartsWith("Russian", StringComparison.OrdinalIgnoreCase)
                || !file.FullPath.Replace('\\', '/').StartsWith(root, StringComparison.Ordinal)) return;
            preloadedFileContents = SelectVariants(preloadedFileContents, variants, integrated);
        }
    }
}
