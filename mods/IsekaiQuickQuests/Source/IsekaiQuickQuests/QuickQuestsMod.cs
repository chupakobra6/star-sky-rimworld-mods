using UnityEngine;
using Verse;

namespace IsekaiQuickQuests
{
    public sealed class QuickQuestSettings : ModSettings
    {
        public bool EnableMiddleClick = true;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref EnableMiddleClick, "enableMiddleClick", true);
        }
    }

    public sealed class QuickQuestsMod : Mod
    {
        private static QuickQuestSettings currentSettings = new QuickQuestSettings();

        public QuickQuestsMod(ModContentPack content) : base(content)
        {
            currentSettings = GetSettings<QuickQuestSettings>();
        }

        internal static QuickQuestSettings Settings => currentSettings;

        public override string SettingsCategory()
        {
            return Content.Name;
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);
            listing.CheckboxLabeled(
                "chupakobra6_isekaiquickquests_SettingsEnableMiddleClick".Translate(),
                ref currentSettings.EnableMiddleClick,
                "chupakobra6_isekaiquickquests_SettingsEnableMiddleClickDesc".Translate());
            listing.Gap();
            if (listing.ButtonText("chupakobra6_isekaiquickquests_SettingsResetDefaults".Translate()))
            {
                currentSettings.EnableMiddleClick = true;
                WriteSettings();
            }
            listing.End();
        }
    }
}
