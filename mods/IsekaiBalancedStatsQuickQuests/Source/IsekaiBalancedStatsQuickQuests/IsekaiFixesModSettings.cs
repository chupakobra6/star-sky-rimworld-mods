using System.Collections.Generic;
using RimWorld;
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
        public EnemyStatMode EnemyHumanMode = EnemyStatMode.CombatRole;
        public EnemyStatMode EnemyCreatureMode = EnemyStatMode.CombatRole;
        public bool EnableForgeBalance = true;
        public float ForgeCombatMultiplier = 0.5f;
        public float ForgeMoveSpeedMultiplier = 0.5f;
        public float ForgeMoveSpeedCap = 0.2f;
        public float ForgeCostMultiplier = 2f;
        public float LevelMoveSpeedMultiplier = 0.5f;
        public bool EnablePawnMoveSpeedCap = true;
        public float PawnMoveSpeedCap = 12f;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref EnablePawnAutoDistribution, "enablePawnAutoDistribution", true);
            Scribe_Values.Look(ref DefaultPawnMode, "defaultPawnMode", AutoStatMode.Balanced);
            Scribe_Values.Look(ref EnablePetAutoDistribution, "enablePetAutoDistribution", true);
            Scribe_Values.Look(ref PetAutoEnabledByDefault, "petAutoEnabledByDefault", true);
            Scribe_Values.Look(ref DefaultPetMode, "defaultPetMode", PetAutoStatMode.PhysicalWeighted);
            Scribe_Values.Look(ref ShowModeChangeMessages, "showModeChangeMessages", true);
            Scribe_Values.Look(ref EnemyHumanMode, "enemyHumanMode", EnemyStatMode.CombatRole);
            Scribe_Values.Look(ref EnemyCreatureMode, "enemyCreatureMode", EnemyStatMode.CombatRole);
            Scribe_Values.Look(ref EnableForgeBalance, "enableForgeBalance", true);
            Scribe_Values.Look(ref ForgeCombatMultiplier, "forgeCombatMultiplier", 0.5f);
            Scribe_Values.Look(ref ForgeMoveSpeedMultiplier, "forgeMoveSpeedMultiplier", 0.5f);
            Scribe_Values.Look(ref ForgeMoveSpeedCap, "forgeMoveSpeedCap", 0.2f);
            Scribe_Values.Look(ref ForgeCostMultiplier, "forgeCostMultiplier", 2f);
            Scribe_Values.Look(ref LevelMoveSpeedMultiplier, "levelMoveSpeedMultiplier", 0.5f);
            Scribe_Values.Look(ref EnablePawnMoveSpeedCap, "enablePawnMoveSpeedCap", true);
            Scribe_Values.Look(ref PawnMoveSpeedCap, "pawnMoveSpeedCap", 12f);
            if (Scribe.mode == LoadSaveMode.PostLoadInit) NormalizeBalanceSettings();
        }

        public void NormalizeBalanceSettings()
        {
            if (!System.Enum.IsDefined(typeof(EnemyStatMode), EnemyHumanMode)) EnemyHumanMode = EnemyStatMode.Original;
            if (!System.Enum.IsDefined(typeof(EnemyStatMode), EnemyCreatureMode)) EnemyCreatureMode = EnemyStatMode.Original;
            ForgeCombatMultiplier = FiniteClamp(ForgeCombatMultiplier, 0f, 1f, 0.5f);
            ForgeMoveSpeedMultiplier = FiniteClamp(ForgeMoveSpeedMultiplier, 0f, 1f, 0.5f);
            ForgeMoveSpeedCap = FiniteClamp(ForgeMoveSpeedCap, 0f, 1f, 0.2f);
            ForgeCostMultiplier = FiniteClamp(ForgeCostMultiplier, 1f, 5f, 2f);
            LevelMoveSpeedMultiplier = FiniteClamp(LevelMoveSpeedMultiplier, 0f, 1f, 0.5f);
            PawnMoveSpeedCap = FiniteClamp(PawnMoveSpeedCap, 5f, 30f, 12f);
        }

        private static float FiniteClamp(float value, float min, float max, float fallback)
        {
            return float.IsNaN(value) || float.IsInfinity(value) ? fallback : Mathf.Clamp(value, min, max);
        }

        // Recommended balance settings. Existing colonist/pet choices,
        // allocation history and item upgrade levels are never reset here.
        public void ApplyModerateBalance()
        {
            EnemyHumanMode = EnemyStatMode.CombatRole;
            EnemyCreatureMode = EnemyStatMode.CombatRole;
            EnableForgeBalance = true;
            ForgeCombatMultiplier = 0.5f;
            ForgeMoveSpeedMultiplier = 0.5f;
            ForgeMoveSpeedCap = 0.2f;
            ForgeCostMultiplier = 2f;
            LevelMoveSpeedMultiplier = 0.5f;
            EnablePawnMoveSpeedCap = true;
            PawnMoveSpeedCap = 12f;
        }

        public void RestoreUpstreamBalance()
        {
            EnemyHumanMode = EnemyStatMode.Original;
            EnemyCreatureMode = EnemyStatMode.Original;
            EnableForgeBalance = false;
            ForgeCombatMultiplier = 1f;
            ForgeMoveSpeedMultiplier = 1f;
            ForgeMoveSpeedCap = 0f;
            ForgeCostMultiplier = 1f;
            LevelMoveSpeedMultiplier = 1f;
            EnablePawnMoveSpeedCap = false;
            PawnMoveSpeedCap = 12f;
        }

        public void ResetToDefaults()
        {
            EnablePawnAutoDistribution = true;
            DefaultPawnMode = AutoStatMode.Balanced;
            EnablePetAutoDistribution = true;
            PetAutoEnabledByDefault = true;
            DefaultPetMode = PetAutoStatMode.PhysicalWeighted;
            ShowModeChangeMessages = true;
            ApplyModerateBalance();
        }
    }

    public sealed class IsekaiBalancedStatsQuickQuestsMod : Mod
    {
        private static IsekaiFixesModSettings currentSettings = new IsekaiFixesModSettings();
        private Vector2 settingsScroll;
        private float settingsContentHeight = 1400f;

        public IsekaiBalancedStatsQuickQuestsMod(ModContentPack content) : base(content)
        {
            currentSettings = GetSettings<IsekaiFixesModSettings>();
        }

        internal static IsekaiFixesModSettings Settings => currentSettings;

        public override string SettingsCategory()
        {
            return Content.Name;
        }

        public override void WriteSettings()
        {
            currentSettings.NormalizeBalanceSettings();
            base.WriteSettings();
            StatDefOf.MoveSpeed?.Worker?.DeleteStatCache();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            Rect content = new Rect(0f, 0f, Mathf.Max(200f, inRect.width - 20f), settingsContentHeight);
            Widgets.BeginScrollView(inRect, ref settingsScroll, content);
            listing.Begin(content);
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
            listing.Label("IsekaiFixes_EnemySection".Translate());
            listing.Label("IsekaiFixes_EnemyScope".Translate());
            DrawEnemyMode(listing, false);
            DrawEnemyMode(listing, true);

            listing.GapLine();
            listing.Label("IsekaiFixes_PawnSpeedSection".Translate());
            listing.Label("IsekaiFixes_PawnSpeedScope".Translate());
            DrawBalanceSlider(listing, "IsekaiFixes_LevelSpeedMultiplier", ref currentSettings.LevelMoveSpeedMultiplier, 0f, 1f, true);
            listing.CheckboxLabeled("IsekaiFixes_PawnSpeedCapEnable".Translate(), ref currentSettings.EnablePawnMoveSpeedCap);
            if (currentSettings.EnablePawnMoveSpeedCap)
                DrawSpeedCapSlider(listing);

            listing.GapLine();
            listing.Label("IsekaiFixes_ForgeSection".Translate());
            listing.CheckboxLabeled("IsekaiFixes_ForgeEnable".Translate(), ref currentSettings.EnableForgeBalance);
            if (currentSettings.EnableForgeBalance)
            {
                listing.Label("IsekaiFixes_ForgeScope".Translate());
                DrawBalanceSlider(listing, "IsekaiFixes_ForgeCombat", ref currentSettings.ForgeCombatMultiplier, 0f, 1f, true);
                DrawBalanceSlider(listing, "IsekaiFixes_ForgeMovement", ref currentSettings.ForgeMoveSpeedMultiplier, 0f, 1f, true);
                DrawBalanceSlider(listing, "IsekaiFixes_ForgeMovementCap", ref currentSettings.ForgeMoveSpeedCap, 0f, 1f, true);
                listing.Label("IsekaiFixes_ForgeCapHint".Translate());
                DrawBalanceSlider(listing, "IsekaiFixes_ForgeCost", ref currentSettings.ForgeCostMultiplier, 1f, 5f, false);
            }
            listing.Gap();
            if (listing.ButtonText("IsekaiFixes_BalancePreset".Translate()))
            {
                currentSettings.ApplyModerateBalance();
                WriteSettings();
            }
            listing.Label("IsekaiFixes_BalancePresetDesc".Translate());
            if (listing.ButtonText("IsekaiFixes_BalanceUpstream".Translate()))
            {
                currentSettings.RestoreUpstreamBalance();
                WriteSettings();
            }
            listing.Label("IsekaiFixes_BalanceUpstreamDesc".Translate());

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

            settingsContentHeight = listing.CurHeight + 20f;
            listing.End();
            Widgets.EndScrollView();
        }

        private static void DrawBalanceSlider(Listing_Standard listing, string key, ref float value, float min, float max, bool percent)
        {
            listing.Label(key.Translate() + ": " + (percent ? (value * 100f).ToString("F0") + "%" : value.ToString("F2") + "×"));
            value = Mathf.Round(listing.Slider(value, min, max) * 100f) / 100f;
        }

        private static void DrawSpeedCapSlider(Listing_Standard listing)
        {
            listing.Label("IsekaiFixes_PawnSpeedCap".Translate() + ": "
                + currentSettings.PawnMoveSpeedCap.ToString("F1") + " " + "IsekaiFixes_SpeedUnit".Translate());
            currentSettings.PawnMoveSpeedCap = Mathf.Round(listing.Slider(currentSettings.PawnMoveSpeedCap, 5f, 30f) * 2f) / 2f;
        }

        private static void DrawEnemyMode(Listing_Standard listing, bool creature)
        {
            EnemyStatMode value = creature ? currentSettings.EnemyCreatureMode : currentSettings.EnemyHumanMode;
            var options = new List<FloatMenuOption>();
            foreach (EnemyStatMode mode in new[] { EnemyStatMode.Original, EnemyStatMode.Balanced, EnemyStatMode.CombatRole })
            {
                EnemyStatMode selected = mode;
                options.Add(new FloatMenuOption(("IsekaiFixes_EnemyMode" + mode).Translate(), () =>
                {
                    if (creature) currentSettings.EnemyCreatureMode = selected;
                    else currentSettings.EnemyHumanMode = selected;
                }));
            }
            DrawModeRow(listing.GetRect(48f), (creature ? "IsekaiFixes_EnemyCreatures" : "IsekaiFixes_EnemyHumans").Translate(),
                "IsekaiFixes_EnemyScope".Translate(), ("IsekaiFixes_EnemyMode" + value).Translate(), options);
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
