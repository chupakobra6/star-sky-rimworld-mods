using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;

namespace Chupakobra6.CollectionPresets
{
    // Pure settings contract: runtime supplies the actual settings object and its writer.
    public sealed class Preset
    {
        public string Id, Title, Description, Reader, ModClass, SettingsClass;
        public readonly List<SettingValue> Values = new List<SettingValue>();
        public static Preset Read(string path)
        {
            var doc = Documents.Read(path);
            var root = doc.DocumentElement;
            if (root == null || root.Name != "SettingsPreset") throw new InvalidDataException(path);
            var config = root.SelectSingleNode("Config") as XmlElement;
            if (config == null || root.SelectNodes("Config").Count != 1) throw new InvalidDataException("One settings owner is required");
            var p = new Preset { Id = Required(root, "id"), Title = Required(root, "title"),
                Description = Required(root, "description"), Reader = Required(config, "reader"),
                ModClass = Required(config, "modClass"), SettingsClass = Required(config, "settingsClass") };
            var names = new HashSet<string>();
            foreach (XmlElement value in config.SelectNodes("Value"))
            {
                var member = Required(value, "member");
                var xmlPath = Required(value, "path");
                if (!Regex.IsMatch(member, @"\A[A-Za-z_][A-Za-z0-9_]*\z")
                    || !Regex.IsMatch(xmlPath, @"\AModSettings/[A-Za-z_][A-Za-z0-9_]*\z")
                    || !names.Add(member)) throw new InvalidDataException("Invalid or duplicate settings field");
                p.Values.Add(new SettingValue { Member = member, Path = xmlPath, Type = Required(value, "type"),
                    Default = value.HasAttribute("default") ? value.GetAttribute("default") : null, Value = value.InnerText });
            }
            if (p.Values.Count == 0) throw new InvalidDataException("Empty settings preset");
            return p;
        }
        private static string Required(XmlElement element, string name)
        {
            var value = element.GetAttribute(name);
            if (string.IsNullOrWhiteSpace(value)) throw new InvalidDataException("Missing " + name);
            return value;
        }
    }
    public sealed class SettingValue { public string Member, Path, Type, Default, Value; }
    public sealed class SettingsTarget
    {
        public object Instance;
        public string File;
        public Action Save;
    }
    internal sealed class Change
    {
        internal Preset Preset;
        internal SettingValue Value;
        internal SettingsTarget Target;
        internal FieldInfo Field;
        internal object Before, After;
    }
    public static class Documents
    {
        public static XmlDocument Read(string file)
        {
            var doc = new XmlDocument { XmlResolver = null };
            using (var reader = XmlReader.Create(file, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null }))
                doc.Load(reader);
            return doc;
        }
        public static void Write(XmlDocument doc, string file)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(file));
            var temp = file + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                doc.Save(temp);
                if (File.Exists(file)) File.Replace(temp, file, null);
                else File.Move(temp, file);
            }
            finally { if (File.Exists(temp)) File.Delete(temp); }
        }
    }
    public static class PresetEngine
    {
        public static object Parse(Type type, string value)
        {
            if (type == typeof(bool)) return bool.Parse(value);
            if (type == typeof(string)) return value;
            if (type == typeof(int)) return int.Parse(value, CultureInfo.InvariantCulture);
            if (type == typeof(float))
            {
                var number = float.Parse(value, CultureInfo.InvariantCulture);
                if (float.IsNaN(number) || float.IsInfinity(number)) throw new InvalidDataException("Non-finite setting");
                return number;
            }
            throw new InvalidDataException("Unsupported setting type: " + type);
        }
        private static string Format(object value)
        {
            if (value is float) return ((float)value).ToString("R", CultureInfo.InvariantCulture);
            return value is bool ? ((bool)value ? "true" : "false") : Convert.ToString(value, CultureInfo.InvariantCulture);
        }
        private static List<Change> Prepare(IEnumerable<Preset> presets, Func<Preset, SettingsTarget> resolve)
        {
            var changes = new List<Change>();
            foreach (var preset in presets)
            {
                var target = resolve(preset);
                if (target == null || target.Instance == null || target.Instance.GetType().FullName != preset.SettingsClass)
                    throw new InvalidDataException("Settings owner changed: " + preset.Title);
                foreach (var value in preset.Values)
                {
                    if (!Regex.IsMatch(value.Path ?? "", @"\AModSettings/[A-Za-z_][A-Za-z0-9_]*\z"))
                        throw new InvalidDataException("Invalid settings path");
                    var field = target.Instance.GetType().GetField(value.Member, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (field == null || field.IsInitOnly || field.IsLiteral) throw new InvalidDataException("Settings field changed: " + value.Member);
                    if (value.Type != null && value.Type != field.FieldType.FullName) throw new InvalidDataException("Settings field type changed: " + value.Member);
                    var after = Parse(field.FieldType, value.Value);
                    if (changes.Any(c => c.Target.File == target.File && c.Value.Path == value.Path && c.Value.Member != value.Member))
                        throw new InvalidDataException("Two settings fields target the same XML path: " + value.Path);
                    var other = changes.FirstOrDefault(c => c.Target.File == target.File && c.Value.Member == value.Member);
                    if (other != null)
                    {
                        if (!Equals(other.After, after) || other.Value.Path != value.Path) throw new InvalidDataException("Conflicting presets: " + value.Member);
                        continue;
                    }
                    changes.Add(new Change { Preset = preset, Value = value, Target = target, Field = field,
                        Before = field.GetValue(target.Instance), After = after });
                }
            }
            if (changes.Count == 0) throw new InvalidDataException("No settings selected");
            return changes;
        }
        public static void Check(Preset preset, Func<Preset, SettingsTarget> resolve) { Prepare(new[] { preset }, resolve); }
        public static void Apply(IEnumerable<Preset> presets, Func<Preset, SettingsTarget> resolve, string undoFile)
        {
            var changes = Prepare(presets, resolve);
            if (changes.All(c => Equals(c.Before, c.After))) return; // Preserve the useful undo record on a repeated click.
            // Record only selected fields: no paths, account data or unrelated settings in the undo record.
            var undo = new XmlDocument();
            var root = undo.CreateElement("PreviousSettings"); undo.AppendChild(root);
            foreach (var group in changes.GroupBy(c => c.Preset))
            {
                var p = group.Key; var item = undo.CreateElement("Preset"); root.AppendChild(item);
                item.SetAttribute("reader", p.Reader); item.SetAttribute("modClass", p.ModClass); item.SetAttribute("settingsClass", p.SettingsClass);
                foreach (var change in group)
                {
                    var value = undo.CreateElement("Value"); item.AppendChild(value);
                    value.SetAttribute("member", change.Value.Member); value.SetAttribute("path", change.Value.Path);
                    value.SetAttribute("type", change.Field.FieldType.FullName);
                    if (change.Value.Default != null) value.SetAttribute("default", change.Value.Default);
                    value.SetAttribute("expected", Format(change.After)); value.InnerText = Format(change.Before);
                }
            }
            var oldUndo = File.Exists(undoFile) ? File.ReadAllBytes(undoFile) : null;
            Documents.Write(undo, undoFile);
            try { Commit(changes); }
            catch { RestoreFile(undoFile, oldUndo); throw; }
        }
        public static void Undo(Func<Preset, SettingsTarget> resolve, string undoFile)
        {
            var doc = Documents.Read(undoFile);
            if (doc.DocumentElement.Name != "PreviousSettings") throw new InvalidDataException("Invalid undo record");
            var changes = new List<Change>();
            foreach (XmlElement item in doc.DocumentElement.SelectNodes("Preset"))
            {
                var p = new Preset { Id = "undo", Title = item.GetAttribute("reader"), Reader = item.GetAttribute("reader"),
                    ModClass = item.GetAttribute("modClass"), SettingsClass = item.GetAttribute("settingsClass") };
                foreach (XmlElement value in item.SelectNodes("Value"))
                    p.Values.Add(new SettingValue { Member = value.GetAttribute("member"), Path = value.GetAttribute("path"), Type = value.GetAttribute("type"),
                        Default = value.HasAttribute("default") ? value.GetAttribute("default") : null, Value = value.InnerText });
                var prepared = Prepare(new[] { p }, resolve);
                foreach (var change in prepared)
                {
                    var value = item.SelectNodes("Value").Cast<XmlElement>().Single(v => v.GetAttribute("member") == change.Value.Member);
                    var expected = Parse(change.Field.FieldType, value.GetAttribute("expected"));
                    // Do not erase manual changes made after applying a preset.
                    if (!Equals(change.Before, expected) && !Equals(change.Before, change.After))
                        throw new InvalidOperationException("Setting was edited since applying the preset: " + change.Value.Member);
                    var disk = LoadSettings(change.Target.File, change.Preset.SettingsClass).DocumentElement.SelectSingleNode(change.Value.Path);
                    string diskValue = disk == null ? change.Value.Default : disk.InnerText;
                    if (diskValue == null || !Equals(Parse(change.Field.FieldType, diskValue), expected))
                        throw new InvalidOperationException("Settings file was edited since applying the preset: " + change.Value.Member);
                }
                changes.AddRange(prepared);
            }
            Commit(changes);
            File.Delete(undoFile);
        }
        private static void Commit(List<Change> changes)
        {
            var groups = changes.GroupBy(c => c.Target.File).ToArray();
            var originals = groups.ToDictionary(g => g.Key, g => File.Exists(g.Key) ? File.ReadAllBytes(g.Key) : null);
            // Validate every file before changing any in-memory settings.
            foreach (var group in groups) LoadSettings(group.Key, group.First().Preset.SettingsClass);
            try
            {
                foreach (var change in changes) change.Field.SetValue(change.Target.Instance, change.After);
                foreach (var group in groups)
                {
                    // Keep unrelated current disk fields even if the mod's writer drops unknown data.
                    bool existed = File.Exists(group.Key);
                    var doc = LoadSettings(group.Key, group.First().Preset.SettingsClass);
                    group.First().Target.Save();
                    if (!existed) doc = LoadSettings(group.Key, group.First().Preset.SettingsClass);
                    foreach (var change in group)
                    {
                        if (!Equals(change.Field.GetValue(change.Target.Instance), change.After))
                            throw new InvalidOperationException("Settings writer changed the requested value: " + change.Value.Member);
                        var settings = (XmlElement)doc.DocumentElement.SelectSingleNode("ModSettings");
                        var name = change.Value.Path.Substring("ModSettings/".Length);
                        XmlConvert.VerifyNCName(name);
                        var node = settings.SelectSingleNode(name) as XmlElement;
                        if (node == null) { node = doc.CreateElement(name); settings.AppendChild(node); }
                        node.InnerText = Format(change.After);
                    }
                    Documents.Write(doc, group.Key);
                }
            }
            catch
            {
                foreach (var change in changes) change.Field.SetValue(change.Target.Instance, change.Before);
                foreach (var original in originals) RestoreFile(original.Key, original.Value);
                throw;
            }
        }
        private static XmlDocument LoadSettings(string file, string type)
        {
            XmlDocument doc;
            if (File.Exists(file)) doc = Documents.Read(file);
            else { doc = new XmlDocument(); doc.LoadXml("<SettingsBlock><ModSettings/></SettingsBlock>"); }
            if (doc.DocumentElement.Name != "SettingsBlock") throw new InvalidDataException("Unexpected settings root: " + file);
            var node = doc.DocumentElement.SelectSingleNode("ModSettings") as XmlElement;
            if (node == null || (node.HasAttribute("Class") && node.GetAttribute("Class") != type))
                throw new InvalidDataException("Unexpected settings class: " + file);
            node.SetAttribute("Class", type);
            return doc;
        }
        private static void RestoreFile(string file, byte[] content)
        {
            if (content == null) { if (File.Exists(file)) File.Delete(file); }
            else File.WriteAllBytes(file, content);
        }
    }
}
