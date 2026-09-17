using IsekaiLeveling.UI;
using RimWorld;
using UnityEngine;
using Verse;

namespace IsekaiBalancedStatsQuickQuests
{
    /// <summary>
    /// Compact controls matching IsekaiListing. The original large navigation
    /// textures are not suitable for the 20-24px allocation rows, so this uses
    /// the same card, hover, copper-border and text palette as its small UI.
    /// </summary>
    internal static class IsekaiControlStyle
    {
        public static bool Button(
            Rect rect,
            string label,
            bool useIsekaiStyle,
            float alpha = 1f)
        {
            if (!useIsekaiStyle)
                return Widgets.ButtonText(rect, label);

            GameFont oldFont = Text.Font;
            TextAnchor oldAnchor = Text.Anchor;
            Color oldColor = GUI.color;
            try
            {
                bool hovered = Mouse.IsOver(rect);
                bool pressed = hovered
                               && Event.current.type == EventType.MouseDown
                               && Event.current.button == 0;

                Color background = pressed
                    ? IsekaiListing.BgPressed
                    : Color.Lerp(IsekaiListing.BgCard, IsekaiListing.BgHover, hovered ? 0.75f : 0f);
                GUI.color = new Color(background.r, background.g, background.b, alpha);
                GUI.DrawTexture(rect, BaseContent.WhiteTex);

                Color border = hovered ? IsekaiListing.AccentGold : IsekaiListing.AccentCopper;
                GUI.color = new Color(border.r, border.g, border.b, (hovered ? 0.8f : 0.5f) * alpha);
                GUI.DrawTexture(new Rect(rect.x, rect.yMax - 1f, rect.width, 1f), BaseContent.WhiteTex);

                Color text = Color.Lerp(
                    IsekaiListing.TextSecondary,
                    IsekaiListing.TextPrimary,
                    hovered ? 1f : 0f);
                GUI.color = new Color(text.r, text.g, text.b, alpha);
                Text.Font = GameFont.Tiny;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(rect.ContractedBy(3f, 0f), label);

                return Widgets.ButtonInvisible(rect);
            }
            finally
            {
                Text.Font = oldFont;
                Text.Anchor = oldAnchor;
                GUI.color = oldColor;
            }
        }

        public static void Checkbox(
            Rect rect,
            string label,
            ref bool value,
            bool useIsekaiStyle,
            float alpha = 1f)
        {
            if (!useIsekaiStyle)
            {
                Widgets.CheckboxLabeled(rect, label, ref value);
                return;
            }

            GameFont oldFont = Text.Font;
            Color oldColor = GUI.color;
            try
            {
                Color text = Mouse.IsOver(rect)
                    ? IsekaiListing.TextPrimary
                    : IsekaiListing.TextSecondary;
                GUI.color = new Color(text.r, text.g, text.b, alpha);
                Text.Font = GameFont.Tiny;
                Widgets.CheckboxLabeled(rect, label, ref value);
            }
            finally
            {
                Text.Font = oldFont;
                GUI.color = oldColor;
            }
        }
    }
}
