using UnityEngine;
using Verse;

namespace IsekaiNoAutoCameraJump
{
    public sealed class CameraJumpModSettings : ModSettings
    {
        public bool SuppressIsekaiHunts = true;
        public bool SuppressCreatureHunts = true;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref SuppressIsekaiHunts, "suppressIsekaiHunts", true);
            Scribe_Values.Look(ref SuppressCreatureHunts, "suppressCreatureHunts", true);
        }

        public void ResetToDefaults()
        {
            SuppressIsekaiHunts = true;
            SuppressCreatureHunts = true;
        }
    }

    public sealed class IsekaiNoAutoCameraJumpMod : Mod
    {
        private static CameraJumpModSettings currentSettings = new CameraJumpModSettings();

        public IsekaiNoAutoCameraJumpMod(ModContentPack content) : base(content)
        {
            currentSettings = GetSettings<CameraJumpModSettings>();
        }

        internal static CameraJumpModSettings Settings => currentSettings;

        public override string SettingsCategory()
        {
            return Content.Name;
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);
            listing.Label("IsekaiNoCameraJump_SettingsIntro".Translate());
            listing.GapLine();
            listing.CheckboxLabeled(
                "IsekaiNoCameraJump_SuppressIsekaiHunts".Translate(),
                ref currentSettings.SuppressIsekaiHunts,
                "IsekaiNoCameraJump_SuppressIsekaiHuntsDesc".Translate());
            listing.CheckboxLabeled(
                "IsekaiNoCameraJump_SuppressCreatureHunts".Translate(),
                ref currentSettings.SuppressCreatureHunts,
                "IsekaiNoCameraJump_SuppressCreatureHuntsDesc".Translate());
            listing.Gap();

            if (listing.ButtonText("IsekaiNoCameraJump_ResetDefaults".Translate()))
            {
                currentSettings.ResetToDefaults();
                WriteSettings();
            }

            listing.End();
        }
    }
}
