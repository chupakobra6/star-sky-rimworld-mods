namespace IsekaiQuickQuests
{
    internal enum QuickAcceptAction
    {
        Ignore,
        OpenQuest,
        Accept
    }

    internal static class QuickAcceptDecision
    {
        public static QuickAcceptAction Decide(
            bool isIsekaiQuest,
            bool isNotYetAccepted,
            bool canAccept,
            bool requiresAccepter,
            bool preventsAutoAccept,
            int choicePartCount,
            int choiceCount)
        {
            if (!isIsekaiQuest)
                return QuickAcceptAction.Ignore;

            if (!isNotYetAccepted || !canAccept || requiresAccepter || preventsAutoAccept)
                return QuickAcceptAction.OpenQuest;

            if (choicePartCount == 0)
                return QuickAcceptAction.Accept;

            if (choicePartCount == 1 && choiceCount == 1)
                return QuickAcceptAction.Accept;

            return QuickAcceptAction.OpenQuest;
        }
    }
}
