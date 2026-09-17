using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace IsekaiBalancedStatsQuickQuests
{
    public sealed class IsekaiFixesModSettings : ModSettings
    {
        public bool EnablePawnAutoDistribution = true;
        public AutoStatMode DefaultPawnMode = AutoStatMode.Balanced;
        public bool EnablePetAutoDistribution = true;
        public bool PetAutoEnabledByDefault = true;
        public PetAutoStatMode DefaultPetMode = PetAutoStatMode.PhysicalWeighted;
        public bool ShowModeChangeMessages = true;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref EnablePawnAutoDistribution, "enablePawnAutoDistribution", true);
            Scribe_Values.Look(ref DefaultPawnMode, "defaultPawnMode", AutoStatMode.Balanced);
            Scribe_Values.Look(ref EnablePetAutoDistribution, "enablePetAutoDistribution", true);
            Scribe_Values.Look(ref PetAutoEnabledByDefault, "petAutoEnabledByDefault", true);
            Scribe_Values.Look(ref DefaultPetMode, "defaultPetMode", PetAutoStatMode.PhysicalWeighted);
            Scribe_Values.Look(ref ShowModeChangeMessages, "showModeChangeMessages", true);
        }

        public void ResetToDefaults()
        {
            EnablePawnAutoDistribution = true;
            DefaultPawnMode = AutoStatMode.Balanced;
            EnablePetAutoDistribution = true;
            PetAutoEnabledByDefault = true;
            DefaultPetMode = PetAutoStatMode.PhysicalWeighted;
            ShowModeChangeMessages = true;
        }
    }

    public sealed class IsekaiBalancedStatsQuickQuestsMod : Mod
    {
        private static IsekaiFixesModSettings currentSettings = new IsekaiFixesModSettings();

        public IsekaiBalancedStatsQuickQuestsMod(ModContentPack content) : base(content)
        {
            currentSettings = GetSettings<IsekaiFixesModSettings>();
        }

        internal static IsekaiFixesModSettings Settings => currentSettings;

        public override string SettingsCategory()
        {
            return Content.Name;
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);
            listing.Label("IsekaiFixes_SettingsIntro".Translate());

            listing.GapLine();
            listing.Label("IsekaiFixes_SettingsPawnSection".Translate());
            listing.CheckboxLabeled(
                "IsekaiFixes_SettingsEnablePawnAuto".Translate(),
                ref currentSettings.EnablePawnAutoDistribution,
                "IsekaiFixes_SettingsEnablePawnAutoDesc".Translate());
            DrawPawnDefaultMode(listing);

            listing.GapLine();
            listing.Label("IsekaiFixes_SettingsPetSection".Translate());
            listing.CheckboxLabeled(
                "IsekaiFixes_SettingsEnablePetAuto".Translate(),
                ref currentSettings.EnablePetAutoDistribution,
                "IsekaiFixes_SettingsEnablePetAutoDesc".Translate());
            listing.CheckboxLabeled(
                "IsekaiFixes_SettingsPetEnabledDefault".Translate(),
                ref currentSettings.PetAutoEnabledByDefault,
                "IsekaiFixes_SettingsPetEnabledDefaultDesc".Translate());
            DrawPetDefaultMode(listing);

            listing.GapLine();
            listing.Label("IsekaiFixes_SettingsInterfaceSection".Translate());
            listing.CheckboxLabeled(
                "IsekaiFixes_SettingsShowModeMessages".Translate(),
                ref currentSettings.ShowModeChangeMessages,
                "IsekaiFixes_SettingsShowModeMessagesDesc".Translate());

            listing.Gap();
            if (listing.ButtonText("IsekaiFixes_SettingsResetDefaults".Translate()))
            {
                currentSettings.ResetToDefaults();
                WriteSettings();
            }

            listing.End();
        }

        private static void DrawPawnDefaultMode(Listing_Standard listing)
        {
            Rect row = listing.GetRect(30f);
            DrawModeRow(
                row,
                "IsekaiFixes_SettingsPawnDefaultMode".Translate(),
                "IsekaiFixes_SettingsPawnDefaultModeDesc".Translate(),
                currentSettings.DefaultPawnMode == AutoStatMode.ClassWeighted
                    ? "IsekaiFixes_AutoModeClassShort".Translate()
                    : "IsekaiFixes_AutoModeBalancedShort".Translate(),
                new List<FloatMenuOption>
                {
                    new FloatMenuOption(
                        "IsekaiFixes_AutoModeBalanced".Translate(),
                        () => currentSettings.DefaultPawnMode = AutoStatMode.Balanced),
                    new FloatMenuOption(
                        "IsekaiFixes_AutoModeClass".Translate(),
                        () => currentSettings.DefaultPawnMode = AutoStatMode.ClassWeighted)
                });
        }

        private static void DrawPetDefaultMode(Listing_Standard listing)
        {
            Rect row = listing.GetRect(30f);
            DrawModeRow(
                row,
                "IsekaiFixes_SettingsPetDefaultMode".Translate(),
                "IsekaiFixes_SettingsPetDefaultModeDesc".Translate(),
                currentSettings.DefaultPetMode == PetAutoStatMode.Balanced
                    ? "IsekaiFixes_PetAutoModeBalancedShort".Translate()
                    : "IsekaiFixes_PetAutoModePhysicalShort".Translate(),
                new List<FloatMenuOption>
                {
                    new FloatMenuOption(
                        "IsekaiFixes_PetAutoModePhysical".Translate(),
                        () => currentSettings.DefaultPetMode = PetAutoStatMode.PhysicalWeighted),
                    new FloatMenuOption(
                        "IsekaiFixes_PetAutoModeBalanced".Translate(),
                        () => currentSettings.DefaultPetMode = PetAutoStatMode.Balanced)
                });
        }

        private static void DrawModeRow(
            Rect row,
            string label,
            string tooltip,
            string currentValue,
            List<FloatMenuOption> options)
        {
            Rect labelRect = new Rect(row.x, row.y, row.width * 0.62f, row.height);
            Rect buttonRect = new Rect(labelRect.xMax + 8f, row.y, row.xMax - labelRect.xMax - 8f, row.height);

            TextAnchor oldAnchor = Text.Anchor;
            try
            {
                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label(labelRect, label);
            }
            finally
            {
                Text.Anchor = oldAnchor;
            }

            if (Widgets.ButtonText(buttonRect, currentValue))
                Find.WindowStack.Add(new FloatMenu(options));

            TooltipHandler.TipRegion(row, tooltip);
        }
    }
}
