using System;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace IsekaiQuickQuests
{
    [HarmonyPatch(typeof(Letter), nameof(Letter.DrawButtonAt))]
    public static class QuickQuestLetterPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(Letter __instance, float topY)
        {
            if (!QuickQuestsMod.Settings.EnableMiddleClick)
                return true;

            Event current = Event.current;
            if (current == null || current.type != EventType.MouseDown || current.button != 2)
                return true;

            Rect buttonRect = new Rect(UI.screenWidth - Letter.DrawWidth - 12f, topY, Letter.DrawWidth, Letter.DrawHeight);
            if (!Mouse.IsOver(buttonRect) || !(__instance is ChoiceLetter choiceLetter) || choiceLetter.quest == null)
                return true;

            Quest quest = choiceLetter.quest;
            try
            {
                QuickAcceptAction action = Decide(quest);
                if (action == QuickAcceptAction.Ignore)
                    return true;

                if (action == QuickAcceptAction.OpenQuest)
                {
                    OpenQuest(choiceLetter, quest);
                }
                else
                {
                    AcceptQuest(choiceLetter, quest);
                }

                current.Use();
                return false;
            }
            catch (Exception exception)
            {
                Log.Warning($"[ISEKAI Quick Quests] Quick acceptance of '{quest.name}' cancelled: {exception}");
                OpenQuest(choiceLetter, quest);
                current.Use();
                return false;
            }
        }

        private static QuickAcceptAction Decide(Quest quest)
        {
            // Other mods' requirement/property getters can have side effects.
            // Do not evaluate any acceptance logic for quests we do not own.
            if (!IsIsekaiQuest(quest))
                return QuickAcceptAction.Ignore;

            AcceptanceReport report = QuestUtility.CanAcceptQuest(quest);
            QuestPart_Choice[] choiceParts = quest.PartsListForReading.OfType<QuestPart_Choice>().ToArray();
            int choiceCount = choiceParts.Length == 1 && choiceParts[0].choices != null
                ? choiceParts[0].choices.Count
                : -1;
            bool preventsAutoAccept = quest.PartsListForReading.Any(part => part.PreventsAutoAccept);

            return QuickAcceptDecision.Decide(
                true,
                quest.State == QuestState.NotYetAccepted,
                report.Accepted,
                quest.RequiresAccepter,
                preventsAutoAccept,
                choiceParts.Length,
                choiceCount);
        }

        private static bool IsIsekaiQuest(Quest quest)
        {
            string packageId = quest.root?.modContentPack?.PackageIdPlayerFacing;
            if (string.Equals(packageId, "JellyCreative.IsekaiLeveling", StringComparison.OrdinalIgnoreCase)
                || string.Equals(packageId, "Poupun.GuildFactionAddon", StringComparison.OrdinalIgnoreCase)
                || string.Equals(packageId, "Poupun.IsekaiCreaturesAddon", StringComparison.OrdinalIgnoreCase))
                return true;

            foreach (QuestPart part in quest.PartsListForReading)
            {
                string ns = part?.GetType().Namespace;
                if (IsNamespace(ns, "IsekaiLeveling")
                    || IsNamespace(ns, "IsekaiCreatures")
                    || IsNamespace(ns, "GuildFactionAddon"))
                    return true;
            }

            return false;
        }

        private static bool IsNamespace(string actual, string expected)
        {
            return actual != null && (actual == expected || actual.StartsWith(expected + ".", StringComparison.Ordinal));
        }

        private static void AcceptQuest(ChoiceLetter letter, Quest quest)
        {
            QuestPart_Choice choicePart = quest.PartsListForReading.OfType<QuestPart_Choice>().SingleOrDefault();
            if (choicePart != null)
                choicePart.Choose(choicePart.choices[0]);

            SoundDefOf.Quest_Accepted.PlayOneShotOnCamera();
            quest.Accept(null);
            Find.LetterStack.RemoveLetter(letter);
            Messages.Message(
                "chupakobra6_isekaiquickquests_QuestAccepted".Translate(quest.name),
                MessageTypeDefOf.TaskCompletion,
                historical: false);
        }

        private static void OpenQuest(ChoiceLetter letter, Quest quest)
        {
            Find.MainTabsRoot.SetCurrentTab(MainButtonDefOf.Quests);
            if (MainButtonDefOf.Quests.TabWindow is MainTabWindow_Quests questWindow)
                questWindow.Select(quest);
            else
                letter.OpenLetter();

            Find.LetterStack.RemoveLetter(letter);
        }
    }
}
