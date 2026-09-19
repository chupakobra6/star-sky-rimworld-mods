using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using UnityEngine;
using Verse;

namespace Chupakobra6.CollectionPresets
{
    public sealed class PresetsMod : Mod
    {
        private const string Prefix = "chupakobra6_chupakobra6_collection_presets_";
        private readonly List<Preset> presets = new List<Preset>();
        private readonly HashSet<string> selected = new HashSet<string>();
        private bool loaded;
        private Vector2 scroll;
        private string status = "";
        private static string UndoFile { get { return Path.Combine(GenFilePaths.ConfigFolderPath, "chupakobra6-presets-undo.xml"); } }
        public PresetsMod(ModContentPack content) : base(content) { }
        private static string T(string name) { return (Prefix + name).Translate(); }
        public override string SettingsCategory() { return T("Category"); }
        private void Load()
        {
            if (loaded) return;
            loaded = true;
            var ids = new HashSet<string>();
            foreach (var mod in LoadedModManager.RunningModsListForReading)
            {
                var folder = Path.Combine(mod.RootDir, "Presets");
                if (!Directory.Exists(folder)) continue;
                foreach (var file in Directory.GetFiles(folder, "*.xml").OrderBy(f => f, StringComparer.Ordinal))
                {
                    try
                    {
                        var preset = Preset.Read(file);
                        if (!ids.Add(preset.Id)) throw new InvalidDataException("Duplicate preset: " + preset.Id);
                        presets.Add(preset);
                    }
                    catch (Exception e) { status = T("Failed") + " " + e.Message; Log.Warning("Collection Presets: " + file + ": " + e.Message); }
                }
            }
        }
        private static SettingsTarget Resolve(Preset p)
        {
            var mod = LoadedModManager.ModHandles.SingleOrDefault(m => m.Content.PackageId.Equals(p.Reader, StringComparison.OrdinalIgnoreCase)
                && m.GetType().Name == p.ModClass);
            if (mod == null) throw new InvalidOperationException(p.Reader + ": " + T("Unavailable"));
            var settings = typeof(Mod).GetField("modSettings", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(mod);
            if (settings == null)
            {
                // Some mods defer reading settings until their own UI or first use.
                var type = mod.GetType().Assembly.GetType(p.SettingsClass, true);
                settings = typeof(Mod).GetMethod("GetSettings").MakeGenericMethod(type).Invoke(mod, null);
            }
            var filename = typeof(LoadedModManager).GetMethod("GetSettingsFilename", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            var file = (string)filename.Invoke(null, new object[] { mod.Content.FolderName, p.ModClass });
            return new SettingsTarget { Instance = settings, File = file, Save = mod.WriteSettings };
        }
        // SimpleBabyCarry keeps a separate global backup when its settings window is opened.
        // Restore that global object before taking our snapshot; never touch a loaded colony.
        private static void PrepareGlobalPreferences(IEnumerable<Preset> items)
        {
            foreach (var p in items.Where(p => p.Reader == "b4ttl3m3ds.simplebabycarry"))
            {
                var settings = Resolve(p).Instance;
                var method = settings.GetType().GetMethod("RestoreGlobalPreferenceForWrite", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (method == null || method.GetParameters().Length != 0 || method.ReturnType != typeof(void))
                    throw new InvalidOperationException("SimpleBabyCarry settings API changed");
                method.Invoke(settings, null);
            }
        }
        private void Run(Action action)
        {
            if (Current.Game != null) { status = T("MainMenuOnly"); return; }
            try { action(); status = T("Applied"); }
            catch (Exception e) { status = T("Failed") + " " + e.GetBaseException().Message; Log.Warning(status); }
        }
        public override void DoSettingsWindowContents(Rect rect)
        {
            Load();
            Widgets.Label(new Rect(rect.x, rect.y, rect.width, 62f), T("Intro"));
            float top = rect.y + 66f;
            if (Widgets.ButtonText(new Rect(rect.x, top, rect.width / 2f - 4f, 32f), T("SelectAll")))
            {
                selected.Clear();
                foreach (var p in presets)
                    try { PresetEngine.Check(p, Resolve); selected.Add(p.Id); } catch { }
            }
            if (Widgets.ButtonText(new Rect(rect.x + rect.width / 2f + 4f, top, rect.width / 2f - 4f, 32f), T("ApplySelected")))
                Run(() => {
                    var chosen = presets.Where(p => selected.Contains(p.Id)).ToArray();
                    foreach (var p in chosen) PresetEngine.Check(p, Resolve);
                    PrepareGlobalPreferences(chosen);
                    PresetEngine.Apply(chosen, Resolve, UndoFile);
                });
            var viewport = new Rect(rect.x, top + 40f, rect.width, Math.Max(40f, rect.height - 218f));
            var content = new Rect(0f, 0f, Math.Max(0f, viewport.width - 18f), Math.Max(viewport.height, presets.Count * 96f));
            Widgets.BeginScrollView(viewport, ref scroll, content);
            try
            {
                float y = 0f;
                foreach (var p in presets)
                {
                    bool enabled = selected.Contains(p.Id);
                    Widgets.CheckboxLabeled(new Rect(0f, y, content.width, 28f), p.Title, ref enabled);
                    if (enabled) selected.Add(p.Id); else selected.Remove(p.Id);
                    Widgets.Label(new Rect(0f, y + 30f, content.width, 62f), p.Description);
                    y += 96f;
                }
            }
            finally { Widgets.EndScrollView(); }
            float bottom = viewport.yMax + 8f;
            if (File.Exists(UndoFile) && Widgets.ButtonText(new Rect(rect.x, bottom, rect.width, 32f), T("Undo")))
                Run(() => {
                    var owners = Documents.Read(UndoFile).SelectNodes("/PreviousSettings/Preset").Cast<XmlElement>()
                        .Select(e => new Preset { Reader = e.GetAttribute("reader"), ModClass = e.GetAttribute("modClass"), SettingsClass = e.GetAttribute("settingsClass") });
                    PrepareGlobalPreferences(owners);
                    PresetEngine.Undo(Resolve, UndoFile);
                });
            Widgets.Label(new Rect(rect.x, bottom + 38f, rect.width, 72f), status.Length == 0 ? T("MainMenuOnly") : status);
        }
    }
}
