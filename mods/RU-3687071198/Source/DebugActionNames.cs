using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using LudeonTK;
using Verse;

namespace Igor.RU_3687071198
{
    // Attribute text is metadata, not an ldstr. Only the reviewed method/name
    // pairs are changed, and only a UI copy of the attribute is modified.
    public sealed class DebugActionNames : Mod
    {
        private static readonly Dictionary<string, Dictionary<string, string>> Names =
            new Dictionary<string, Dictionary<string, string>> {
            { "MiningOutpost.FactionCleanupUtility:CleanupOrphanedTemporaryFactions", new Dictionary<string, string> {
                { "Clean up orphaned temporary factions", "Удалить бесхозные временные фракции" }
            } },
            { "MiningOutpost.FactionCleanupUtility:ListTemporaryFactions", new Dictionary<string, string> {
                { "List all temporary factions", "Показать все временные фракции" }
            } },
            { "MiningOutpost.Orbital.OrbitalHellpodDebug:SendHellpodStrike", new Dictionary<string, string> {
                { "Orbital: send hellpod strike now", "Орбита: немедленно начать атаку из адских капсул" }
            } },
            { "MiningOutpost.WorkSiteDefenseTracker:DebugRandomDefenseMission", new Dictionary<string, string> {
                { "Random worksite defence mission", "Создать случайное задание по защите рабочего объекта" }
            } },
            { "MiningOutpost.WorkSiteRevengeTracker:DebugFireNextRaid", new Dictionary<string, string> {
                { "Fire next revenge raid NOW", "Запустить следующий ответный налёт СЕЙЧАС" }
            } },
            { "MiningOutpost.WorkSiteRevengeTracker:DebugForceStartRevenge", new Dictionary<string, string> {
                { "Force start revenge sequence", "Принудительно начать серию ответных налётов" }
            } },
            { "MiningOutpost.WorldComponent_WorkSiteQuestTracker+DebugActions:LogSaturationInfo", new Dictionary<string, string> {
                { "Log saturation info", "Записать сведения о насыщении в журнал" }
            } },
            { "MiningOutpost.WorldComponent_WorkSiteQuestTracker+DebugActions:SpawnSpecificWorkSiteQuest", new Dictionary<string, string> {
                { "Spawn specific work site quest...", "Создать задание для выбранного рабочего объекта..." }
            } },
            { "MiningOutpost.WorldComponent_WorkSiteQuestTracker+DebugActions:SpawnWorkSiteQuest", new Dictionary<string, string> {
                { "Spawn random work site quest", "Создать задание для случайного рабочего объекта" }
            } }
            };
        public DebugActionNames(ModContentPack content) : base(content)
        {
            new Harmony("igor.ru.3687071198.debugnames").Patch(
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
