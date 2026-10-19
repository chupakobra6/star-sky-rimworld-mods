using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;
using System.Security.Cryptography;
using HarmonyLib;
using Verse;

namespace Igor.RU_3756132017
{
    // HAR's styling type initializes textures. Install after content loading.
    [StaticConstructorOnStartup]
    internal static class HARDisplayLabelsStartup
    {
        static HARDisplayLabelsStartup() { HARDisplayLabels.Install(); }
    }

    // HAR uses these names as routing keys and also prints them directly.
    // Replace only the reviewed display reads, never the underlying fields.
    public static class HARDisplayLabels
    {
        private const string Channel = "AlienRace.AlienPartGenerator+ColorChannelGenerator";
        private const string Offset = "AlienRace.AlienPartGenerator+OffsetNamed";
        private const string BodyAddon = "AlienRace.AlienPartGenerator+BodyAddon";
        private static readonly Dictionary<string, string> Keys = new Dictionary<string, string> {
            { "skin", "chupakobra6_3756132017_HARDisplay_skin" }, { "hair", "chupakobra6_3756132017_HARDisplay_hair" },
            { "Moyo_tail", "chupakobra6_3756132017_HARDisplay_Moyo_tail" },
            { "Moyo_antennae", "chupakobra6_3756132017_HARDisplay_Moyo_antennae" },
            { "Moyo_Hair", "chupakobra6_3756132017_HARDisplay_Moyo_Hair" },
            { "Center", "chupakobra6_3756132017_HARDisplay_Center" }
        };

        public static void Install()
        {
            var harmony = new Harmony("igor.ru.3756132017.har-display-labels");
            foreach (var target in Targets())
                harmony.Patch(target, transpiler: new HarmonyMethod(typeof(HARDisplayLabels), nameof(Transpile)));
        }

        public static IEnumerable<MethodBase> Targets()
        {
            yield return AccessTools.Method(AccessTools.TypeByName("AlienRace.StylingStation"), "DoChannelList");
            yield return AccessTools.Method(AccessTools.TypeByName("AlienRace.TweakValues"), "TweakValuesInstanceBased");
            yield return AccessTools.Method(AccessTools.TypeByName("AlienRace.TweakValues+<>c__DisplayClass11_9"), "<TweakValuesInstanceBased>b__31");
            yield return AccessTools.Method(AccessTools.TypeByName("AlienRace.TweakValues+<>c__DisplayClass11_9"), "<TweakValuesInstanceBased>b__24");
        }

        private static void RequireReviewedAssembly(MethodBase method)
        {
            using (var stream = File.OpenRead(method.Module.Assembly.Location))
            using (var hash = SHA256.Create())
                if (BitConverter.ToString(hash.ComputeHash(stream)).Replace("-", "").ToLowerInvariant()
                    != "9b5dc05932e9dfaa963c2fa5253127e050b074686468e03c145831dcf2d7f626")
                    throw new InvalidOperationException("HAR display consumer assembly changed; review required: " + method);
        }

        private static object Read(object instance, string field)
            => instance == null ? null : AccessTools.Field(instance.GetType(), field)?.GetValue(instance);

        public static string Label(object item)
        {
            string type = item?.GetType().FullName;
            string original = Read(item, "name") as string;
            var field = type == Channel ? "colorChannels" : type == Offset ? "offsetDefaults" : null;
            return TranslateOwned(item, original, field);
        }

        public static string LabelBodyAddon(object item)
        {
            var field = AccessTools.Field(AccessTools.TypeByName(BodyAddon), "defaultOffset");
            string original = item != null && field?.DeclaringType.IsInstanceOfType(item) == true
                ? field.GetValue(item) as string : null;
            return TranslateOwned(item, original, "bodyAddons");
        }

        private static string TranslateOwned(object item, string original, string field)
        {
            var language = LanguageDatabase.activeLanguage;
            if (original == null || language == null || !language.folderName.StartsWith("Russian", StringComparison.OrdinalIgnoreCase)
                || !Keys.TryGetValue(original, out var key)) return original;
            var moyo = DefDatabase<ThingDef>.GetNamedSilentFail("Alien_Moyo");
            if (moyo == null || moyo.modContentPack?.PackageId != "nemonian.my2") return original;
            var generator = Read(Read(Read(moyo, "alienRace"), "generalSettings"), "alienPartGenerator");
            if (field == null || !(Read(generator, field) is IEnumerable owned)
                || !owned.Cast<object>().Any(candidate => ReferenceEquals(candidate, item))) return original;
            return key.CanTranslate() ? key.Translate().ToString() : original;
        }

        public static IEnumerable<CodeInstruction> Transpile(IEnumerable<CodeInstruction> instructions, MethodBase __originalMethod)
        {
            RequireReviewedAssembly(__originalMethod);
            var code = instructions.ToList();
            bool channel = __originalMethod.Name == "DoChannelList";
            bool main = __originalMethod.Name == "TweakValuesInstanceBased";
            bool addon = __originalMethod.Name == "<TweakValuesInstanceBased>b__24";
            string owner = channel ? Channel : addon ? BodyAddon : Offset;
            string fieldName = addon ? "defaultOffset" : "name";
            int count = 0;
            foreach (var instruction in code)
            {
                if (instruction.opcode != OpCodes.Ldfld || !(instruction.operand is FieldInfo field)
                    || field.Name != fieldName || field.DeclaringType.FullName != owner) continue;
                count++;
                // Current HAR: channel's third read feeds Widgets.Label;
                // tweak reads 1/2/5 feed Dropdown/Concat/Label, 3/4 compare
                // technical keys. Each menu/offset-label closure has one display read.
                if (channel ? count == 3 : main ? count == 1 || count == 2 || count == 5 : count == 1)
                {
                    instruction.opcode = OpCodes.Call;
                    instruction.operand = AccessTools.Method(typeof(HARDisplayLabels), addon ? nameof(LabelBodyAddon) : nameof(Label));
                }
            }
            int expected = channel ? 3 : main ? 5 : 1;
            if (count != expected) throw new InvalidOperationException("HAR display field reads changed in " + __originalMethod);
            return code;
        }
    }
}
