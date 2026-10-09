using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using LudeonTK;
using Verse;

namespace Igor.RU_3215569151
{
    // Attribute text is metadata, not an ldstr. Only the reviewed method/name
    // pairs are changed, and only a UI copy of the attribute is modified.
    public sealed class DebugActionNames : Mod
    {
        private static readonly Dictionary<string, Dictionary<string, string>> Names =
            new Dictionary<string, Dictionary<string, string>> {
            { "VSEWW.DebugOptions:AddModifier", new Dictionary<string, string> {
                { "Add modifier to wave", "Добавить модификатор волны" }
            } },
            { "VSEWW.DebugOptions:ResetToWaveOne", new Dictionary<string, string> {
                { "Reset to wave 1", "Вернуться к волне 1" }
            } },
            { "VSEWW.DebugOptions:ResetWave", new Dictionary<string, string> {
                { "Reset wave", "Сбросить волну" }
            } },
            { "VSEWW.DebugOptions:RewardTest", new Dictionary<string, string> {
                { "Rewards test", "Проверить награды" }
            } },
            { "VSEWW.DebugOptions:SendAllReward", new Dictionary<string, string> {
                { "Send all rewards", "Выдать все награды" }
            } },
            { "VSEWW.DebugOptions:SendWaveNow", new Dictionary<string, string> {
                { "Send wave now", "Отправить волну сейчас" }
            } },
            { "VSEWW.DebugOptions:SkipToWave", new Dictionary<string, string> {
                { "Skip to wave...", "Перейти к волне..." }
            } }
            };
        public DebugActionNames(ModContentPack content) : base(content)
        {
            new Harmony("igor.ru.3215569151.debugnames").Patch(
                AccessTools.Method(typeof(DebugTabMenu_Actions), "GenerateCacheForMethod"),
                prefix: new HarmonyMethod(typeof(DebugActionNames), nameof(Translate)));
        }
        public static void Translate(MethodInfo __0, ref DebugActionAttribute __1)
        {
            if (__0?.DeclaringType == null || __1 == null
                || LanguageDatabase.activeLanguage?.folderName.StartsWith("Russian", StringComparison.OrdinalIgnoreCase) != true
                || !Names.TryGetValue(__0.DeclaringType.FullName + ":" + __0.Name, out var pairs)
                || __1.name == null || !pairs.TryGetValue(__1.name, out var russian)) return;
            __1 = (DebugActionAttribute)AccessTools.Method(typeof(object), "MemberwiseClone").Invoke(__1, null);
            __1.name = russian;
        }
    }
}
