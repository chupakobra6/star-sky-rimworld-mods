using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using IsekaiLeveling;
using IsekaiLeveling.UI;
using RimWorld;
using UnityEngine;
using Verse;

namespace IsekaiBalancedStatsQuickQuests
{
    /// <summary>
    /// Adds the distribution-mode selector directly to ISEKAI's existing
    /// auto-distribute row. All three ISEKAI stats layouts ultimately call
    /// this exact Widgets.CheckboxLabeled overload.
    /// </summary>
    [HarmonyPatch]
    public static class AutoStatModeInterfacePatch
    {
        private const float ModeButtonGap = 4f;

        public static MethodBase TargetMethod()
        {
            return AccessTools.Method(
                typeof(Widgets),
                nameof(Widgets.CheckboxLabeled),
                new[]
                {
                    typeof(Rect),
                    typeof(string),
                    typeof(bool).MakeByRefType(),
                    typeof(bool),
                    typeof(Texture2D),
                    typeof(Texture2D),
                    typeof(bool),
                    typeof(bool)
                });
        }

        [HarmonyPrefix]
        public static void Prefix(ref Rect rect, ref string label, out PatchState __state)
        {
            __state = null;
            if (!IsekaiBalancedStatsQuickQuestsMod.Settings.EnablePawnAutoDistribution)
                return;

            if (!string.Equals(label, "Isekai_AutoDistribute".Translate().ToString(), StringComparison.Ordinal))
                return;

            Pawn pawn = Find.Selector?.SingleSelectedThing as Pawn;
            IsekaiComponent component = pawn?.GetComp<IsekaiComponent>();
            if (component == null)
                return;

            float modeButtonWidth = Mathf.Clamp(rect.width * 0.43f, 118f, 145f);
            Rect modeRect = new Rect(
                rect.xMax - modeButtonWidth,
                rect.y,
                modeButtonWidth,
                rect.height);

            __state = new PatchState(pawn, component, modeRect);

            // Reserve the right-hand part of ISEKAI's existing row for our
            // selector while letting the original checkbox draw normally.
            rect.width = Mathf.Max(120f, rect.width - modeButtonWidth - ModeButtonGap);
            label = "IsekaiFixes_AutoToggleShort".Translate();
        }

        [HarmonyPostfix]
        public static void Postfix(PatchState __state)
        {
            if (__state == null)
                return;

            AutoStatMode mode = AutoStatModeGameComponent.Get()?.GetMode(__state.Pawn)
                                ?? AutoStatMode.Balanced;
            string modeLabel = ModeLabel(mode);

            GameFont oldFont = Text.Font;
            Color oldColor = GUI.color;
            try
            {
                Text.Font = GameFont.Tiny;
                GUI.color = Color.white;

                if (IsekaiControlStyle.Button(
                        __state.ModeRect,
                        "IsekaiFixes_AutoModeButton".Translate(modeLabel),
                        IsekaiLevelingSettings.UseIsekaiUI))
                {
                    OpenModeMenu(__state.Pawn, __state.Component);
                }

                TooltipHandler.TipRegion(
                    __state.ModeRect,
                    "IsekaiFixes_AutoModeButtonDesc".Translate());
            }
            finally
            {
                Text.Font = oldFont;
                GUI.color = oldColor;
            }
        }

        private static void OpenModeMenu(Pawn pawn, IsekaiComponent component)
        {
            List<FloatMenuOption> options = new List<FloatMenuOption>
            {
                MakeOption(
                    pawn,
                    component,
                    AutoStatMode.Balanced,
                    "IsekaiFixes_AutoModeBalanced",
                    "IsekaiFixes_AutoModeBalancedDesc")
            };

            bool hasClass = !string.IsNullOrEmpty(component.passiveTree?.assignedTree);
            if (hasClass)
            {
                options.Add(MakeOption(
                    pawn,
                    component,
                    AutoStatMode.ClassWeighted,
                    "IsekaiFixes_AutoModeClass",
                    "IsekaiFixes_AutoModeClassDesc"));
            }
            else
            {
                FloatMenuOption disabledClassOption = new FloatMenuOption(
                    "IsekaiFixes_AutoModeClassUnavailable".Translate(),
                    null);
                disabledClassOption.tooltip = "IsekaiFixes_AutoModeRequiresClass".Translate();
                options.Add(disabledClassOption);
            }

            Find.WindowStack.Add(new FloatMenu(options));
        }

        private static FloatMenuOption MakeOption(
            Pawn pawn,
            IsekaiComponent component,
            AutoStatMode mode,
            string labelKey,
            string descriptionKey)
        {
            string label = labelKey.Translate();
            return new FloatMenuOption(
                label + " — " + descriptionKey.Translate(),
                () =>
                {
                    AutoStatModeGameComponent.Get()?.SetMode(pawn, mode);
                    if (IsekaiBalancedStatsQuickQuestsMod.Settings.ShowModeChangeMessages)
                    {
                        Messages.Message(
                            "IsekaiFixes_AutoModeChanged".Translate(pawn.LabelShortCap, label),
                            pawn,
                            MessageTypeDefOf.TaskCompletion,
                            historical: false);
                    }

                    if (component.autoDistributeStats && component.stats.availableStatPoints > 0)
                        component.AutoDistributeByClass();
                });
        }

        private static string ModeLabel(AutoStatMode mode)
        {
            return mode == AutoStatMode.ClassWeighted
                ? "IsekaiFixes_AutoModeClassShort".Translate()
                : "IsekaiFixes_AutoModeBalancedShort".Translate();
        }

        public sealed class PatchState
        {
            public PatchState(Pawn pawn, IsekaiComponent component, Rect modeRect)
            {
                Pawn = pawn;
                Component = component;
                ModeRect = modeRect;
            }

            public Pawn Pawn { get; }
            public IsekaiComponent Component { get; }
            public Rect ModeRect { get; }
        }
    }
}
