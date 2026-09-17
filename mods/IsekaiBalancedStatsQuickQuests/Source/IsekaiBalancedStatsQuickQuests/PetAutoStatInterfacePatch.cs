using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using IsekaiLeveling;
using IsekaiLeveling.MobRanking;
using IsekaiLeveling.UI;
using RimWorld;
using UnityEngine;
using Verse;

namespace IsekaiBalancedStatsQuickQuests
{
    internal static class PetAutoStatControls
    {
        private const float ModeButtonGap = 5f;

        public static void Draw(
            Rect rect,
            Pawn pawn,
            MobRankComponent component,
            bool useIsekaiStyle,
            float alpha = 1f)
        {
            if (!IsekaiBalancedStatsQuickQuestsMod.Settings.EnablePetAutoDistribution)
                return;

            if (pawn?.Faction == null || !pawn.Faction.IsPlayer || pawn.RaceProps.Humanlike
                || component?.stats == null)
                return;

            AutoStatModeGameComponent gameComponent = AutoStatModeGameComponent.Get();
            bool enabled = gameComponent?.GetPetAutoEnabled(pawn) ?? true;
            PetAutoStatMode mode = gameComponent?.GetPetMode(pawn)
                                   ?? PetAutoStatMode.PhysicalWeighted;

            float toggleWidth = Mathf.Clamp(rect.width * 0.52f, 130f, 172f);
            Rect toggleRect = new Rect(rect.x, rect.y, toggleWidth, rect.height);
            Rect modeRect = new Rect(
                toggleRect.xMax + ModeButtonGap,
                rect.y,
                Mathf.Max(90f, rect.width - toggleWidth - ModeButtonGap),
                rect.height);

            GameFont oldFont = Text.Font;
            TextAnchor oldAnchor = Text.Anchor;
            Color oldColor = GUI.color;
            try
            {
                Text.Font = GameFont.Tiny;
                Text.Anchor = TextAnchor.MiddleLeft;
                GUI.color = new Color(1f, 1f, 1f, alpha);

                bool previous = enabled;
                IsekaiControlStyle.Checkbox(
                    toggleRect,
                    "IsekaiFixes_PetAutoToggle".Translate(),
                    ref enabled,
                    useIsekaiStyle,
                    alpha);
                TooltipHandler.TipRegion(toggleRect, "IsekaiFixes_PetAutoToggleDesc".Translate());

                if (enabled != previous)
                {
                    gameComponent?.SetPetAutoEnabled(pawn, enabled);
                    if (enabled && component.stats.availableStatPoints > 0)
                        PetAutoStatDistributor.Distribute(component);
                }

                if (IsekaiControlStyle.Button(
                        modeRect,
                        "IsekaiFixes_PetAutoModeButton".Translate(GetModeShortLabel(mode)),
                        useIsekaiStyle,
                        alpha))
                {
                    OpenModeMenu(pawn, component);
                }

                TooltipHandler.TipRegion(modeRect, "IsekaiFixes_PetAutoModeButtonDesc".Translate());
            }
            finally
            {
                Text.Font = oldFont;
                Text.Anchor = oldAnchor;
                GUI.color = oldColor;
            }
        }

        private static void OpenModeMenu(Pawn pawn, MobRankComponent component)
        {
            List<FloatMenuOption> options = new List<FloatMenuOption>
            {
                MakeModeOption(
                    pawn,
                    component,
                    PetAutoStatMode.PhysicalWeighted,
                    "IsekaiFixes_PetAutoModePhysical",
                    "IsekaiFixes_PetAutoModePhysicalDesc"),
                MakeModeOption(
                    pawn,
                    component,
                    PetAutoStatMode.Balanced,
                    "IsekaiFixes_PetAutoModeBalanced",
                    "IsekaiFixes_PetAutoModeBalancedDesc")
            };

            Find.WindowStack.Add(new FloatMenu(options));
        }

        private static FloatMenuOption MakeModeOption(
            Pawn pawn,
            MobRankComponent component,
            PetAutoStatMode mode,
            string labelKey,
            string descriptionKey)
        {
            string label = labelKey.Translate();
            return new FloatMenuOption(
                label + " — " + descriptionKey.Translate(),
                () =>
                {
                    AutoStatModeGameComponent gameComponent = AutoStatModeGameComponent.Get();
                    gameComponent?.SetPetMode(pawn, mode);
                    if (IsekaiBalancedStatsQuickQuestsMod.Settings.ShowModeChangeMessages)
                    {
                        Messages.Message(
                            "IsekaiFixes_PetAutoModeChanged".Translate(pawn.LabelShortCap, label),
                            pawn,
                            MessageTypeDefOf.TaskCompletion,
                            historical: false);
                    }

                    if ((gameComponent?.GetPetAutoEnabled(pawn) ?? true)
                        && component.stats.availableStatPoints > 0)
                    {
                        PetAutoStatDistributor.Distribute(component);
                    }
                });
        }

        private static string GetModeShortLabel(PetAutoStatMode mode)
        {
            return mode == PetAutoStatMode.Balanced
                ? "IsekaiFixes_PetAutoModeBalancedShort".Translate()
                : "IsekaiFixes_PetAutoModePhysicalShort".Translate();
        }
    }

    /// <summary>
    /// Draws the pet auto-allocation row in the free lower strip of ISEKAI's
    /// custom stat panel. The separate creature-stat window stays manual-only.
    /// </summary>
    [HarmonyPatch]
    public static class PetAutoStatCustomTabPatch
    {
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method(
                typeof(ITab_CreatureStats),
                "DrawStatPanel",
                new[]
                {
                    typeof(Rect),
                    typeof(MobRankComponent),
                    typeof(float),
                    typeof(float)
                });
        }

        [HarmonyPostfix]
        public static void Postfix(Rect fullRect, MobRankComponent rankComp, float curY, float alpha)
        {
            Pawn pawn = rankComp?.Pawn;
            if (pawn == null)
                return;

            const float panelWidth = 326f;
            float panelX = (fullRect.width - panelWidth) / 2f;
            Rect controlsRect = new Rect(panelX + 14f, curY + 146f, panelWidth - 28f, 22f);
            PetAutoStatControls.Draw(
                controlsRect,
                pawn,
                rankComp,
                useIsekaiStyle: true,
                alpha: alpha);
        }
    }

    /// <summary>
    /// Adds the same controls to ISEKAI's optional vanilla-style creature tab.
    /// </summary>
    [HarmonyPatch]
    public static class PetAutoStatVanillaTabPatch
    {
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(ITab_CreatureStats), "FillTabVanilla");
        }

        [HarmonyPostfix]
        public static void Postfix()
        {
            Pawn pawn = Find.Selector?.SingleSelectedThing as Pawn;
            MobRankComponent rankComp = pawn?.TryGetComp<MobRankComponent>();
            if (rankComp == null)
                return;

            // FillTabVanilla uses a fixed 350px tab contracted by 10px. Its
            // standard Stats button ends at y=314, leaving this row below it.
            PetAutoStatControls.Draw(
                new Rect(10f, 320f, 330f, 24f),
                pawn,
                rankComp,
                useIsekaiStyle: false);
        }
    }
}
