using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using LudeonTK;
using Verse;

namespace Igor.RU_2023507013
{
    // Attribute text is metadata, not an ldstr. Only the reviewed method/name
    // pairs are changed, and only a UI copy of the attribute is modified.
    public sealed class DebugActionNames : Mod
    {
        private static readonly Dictionary<string, Dictionary<string, string>> Names =
            new Dictionary<string, Dictionary<string, string>> {
            { "KCSG.DebugActions:DestroyHostilePawns", new Dictionary<string, string> {
                { "Destroy hostile pawns", "Уничтожить враждебных персонажей" }
            } },
            { "KCSG.DebugActions:Quickspawn", new Dictionary<string, string> {
                { "Quickspawn...", "Быстро создать..." }
            } },
            { "KCSG.DebugActions:RemoveAllPawns", new Dictionary<string, string> {
                { "Destroy all pawns", "Уничтожить всех персонажей" }
            } },
            { "KCSG.DebugActions:ReportSymbolsNow", new Dictionary<string, string> {
                { "Report symbol amount now", "Сообщить текущее число символов" }
            } },
            { "KCSG.DebugActions:SpawnRocks", new Dictionary<string, string> {
                { "Spawn rocks (rect)", "Создать скалы (прямоугольник)" }
            } },
            { "KCSG.DebugActions:SpawnStructure", new Dictionary<string, string> {
                { "Quickspawn structure", "Быстро создать сооружение" }
            } },
            { "MVCF.DebugActionsMVCF:LogManagedVerbs", new Dictionary<string, string> {
                { "Log MVCF ManagedVerbs", "Записать MVCF ManagedVerbs в журнал" }
            } },
            { "MVCF.DebugActionsMVCF:LogVerbs", new Dictionary<string, string> {
                { "Log Verbs", "Записать действия Verbs в журнал" }
            } },
            { "PipeSystem.PipeSystemDebug:ToggleDebugLog", new Dictionary<string, string> {
                { "Toggle verbose loging", "Переключить подробный журнал" }
            } },
            { "PipeSystem.PipeSystemDebug:ToggleGridView", new Dictionary<string, string> {
                { "Toggle resources grids drawing", "Переключить отображение сетей ресурсов" }
            } },
            { "VEF.Abilities.Ability:GiveAbility", new Dictionary<string, string> {
                { "Give ability...", "Дать способность..." }
            } },
            { "VEF.Sounds.ForcedMusicManager:StopAll", new Dictionary<string, string> {
                { "End forced music", "Остановить принудительную музыку" }
            } },
            { "VEF.Storyteller.PrefabExporter:MassExportPrefabs", new Dictionary<string, string> {
                { "Export prefab(s)", "Экспортировать префабы" }
            } }
            };
        public DebugActionNames(ModContentPack content) : base(content)
        {
            new Harmony("igor.ru.2023507013.debugnames").Patch(
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
