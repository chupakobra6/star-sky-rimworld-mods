using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using LudeonTK;
using Verse;

namespace Igor.RU_2925432336
{
    public sealed class DisplayLabels : Mod
    {
        private static bool Russian => LanguageDatabase.activeLanguage?.folderName
            .StartsWith("Russian", StringComparison.OrdinalIgnoreCase) == true;
        private static readonly Dictionary<string, string> Romance = new Dictionary<string, string> {
            {"Humanlike", "Гуманоиды"}, {"Human", "Люди"}, {"Android", "Андроиды"},
            {"Mechanoid", "Механоиды"}, {"Mech", "Мехи"}, {"Centaur", "Кентавры"},
            {"Arachnid", "Паукообразные"}, {"Spider", "Пауки"}, {"Insect", "Насекомые"},
            {"Bear", "Медведи"}, {"Bird", "Птицы"}, {"Bovine", "Быки"}, {"Camelid", "Верблюдовые"},
            {"Canine", "Псовые"}, {"Caprine", "Козьи"}, {"Dragon", "Драконы"}, {"Elephant", "Слоны"},
            {"Equine", "Лошадиные"}, {"Feline", "Кошачьи"}, {"Leporid", "Зайцевые"},
            {"Reptile", "Пресмыкающиеся"}, {"Rodent", "Грызуны"}, {"Serpent", "Змеи"},
            {"Snake", "Змеи"}, {"Swine", "Свиньи"}
        };
        private static readonly Dictionary<string, string> DebugNames = new Dictionary<string, string> {
            {"View Mutations", "Просмотреть мутации"}, {"Transform To Race...", "Сменить расу..."},
            {"Colonist of Xenotype", "Поселенец заданного ксенотипа"}, {"Villager of Xenotype", "Житель заданного ксенотипа"},
            {"Random Human Pawnkind", "Случайный вид человека"}, {"Random Animal Pawnkind", "Случайный вид животного"},
            {"Random Sapient Animal", "Случайное разумное животное"}, {"Random Mechanoid Pawnkind", "Случайный вид механоида"},
            {"Random Sapient Mechanoid", "Случайный разумный механоид"}, {"Random of Xenotype", "Случайный персонаж заданного ксенотипа"},
            {"Colonist of Race", "Поселенец заданной расы"}, {"Force-refresh cache on pawn", "Принудительно обновить кэш персонажа"},
            {"Make animal Sapient", "Сделать животное разумным"}, {"Clear Cache for pawn then refresh", "Очистить и обновить кэш персонажа"},
            {"Clear Junk", "Удалить неиспользуемые данные"}, {"Edit Graphics", "Изменить внешность"}, {"Misc...", "Прочее..."}
        };
        public DisplayLabels(ModContentPack content) : base(content)
        {
            var harmony = new Harmony("igor.ru.2925432336.display");
            harmony.Patch(AccessTools.Method("BigAndSmall.EditPawnWindow:DrawTitle"),
                prefix: new HarmonyMethod(typeof(DisplayLabels), nameof(Category)));
            harmony.Patch(AccessTools.Method("BigAndSmall.RomanceTags:GetDescriptions"),
                postfix: new HarmonyMethod(typeof(DisplayLabels), nameof(RomanceLabels)));
            harmony.Patch(AccessTools.Method(typeof(DebugTabMenu_Actions), "GenerateCacheForMethod"),
                prefix: new HarmonyMethod(typeof(DisplayLabels), nameof(DebugLabel)));
        }
        public static void Category(ref string __0)
        {
            if (!Russian) return;
            if (__0 == "Hair") __0 = "Волосы";
            else if (__0 == "Skin") __0 = "Кожа";
        }
        public static void RomanceLabels(List<string> __result)
        {
            if (!Russian) return;
            for (int i = 0; i < __result.Count; i++)
            {
                var text = __result[i];
                int colon = text.IndexOf(':');
                if (colon > 0 && Romance.TryGetValue(text.Substring(0, colon), out var label))
                    __result[i] = label + text.Substring(colon);
            }
        }
        public static void DebugLabel(MethodInfo __0, ref DebugActionAttribute __1)
        {
            if (!Russian || __0.DeclaringType.Assembly != AccessTools.TypeByName("BigAndSmall.BigSmallMod").Assembly
                || !DebugNames.TryGetValue(__1.name, out var name)) return;
            // Preserve the cached attribute and every behavior flag; only this UI copy changes.
            __1 = (DebugActionAttribute)AccessTools.Method(typeof(object), "MemberwiseClone").Invoke(__1, null);
            __1.name = name;
            if (__1.category == "Big & Small - Spawn") __1.category = "Big & Small — создание";
        }
    }
}
