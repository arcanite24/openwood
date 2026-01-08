using UnityEngine;

namespace OpenWood.Core.UI
{
    /// <summary>
    /// Color palette for the UI system, matching the game's style.
    /// These colors are designed to match Littlewood's warm, tan-colored UI.
    /// </summary>
    public static class UIColors
    {
        #region Panel Colors

        /// <summary>
        /// Default panel background color (warm tan).
        /// </summary>
        public static readonly Color PanelBackground = new Color(0.95f, 0.89f, 0.78f, 1f);

        /// <summary>
        /// Darker panel background for contrast.
        /// </summary>
        public static readonly Color PanelBackgroundDark = new Color(0.85f, 0.79f, 0.68f, 1f);

        /// <summary>
        /// Lighter panel background.
        /// </summary>
        public static readonly Color PanelBackgroundLight = new Color(1f, 0.95f, 0.88f, 1f);

        /// <summary>
        /// Semi-transparent panel background.
        /// </summary>
        public static readonly Color PanelBackgroundTransparent = new Color(0.95f, 0.89f, 0.78f, 0.9f);

        #endregion

        #region Window Colors

        /// <summary>
        /// Window title bar color.
        /// </summary>
        public static readonly Color TitleBar = new Color(0.88f, 0.75f, 0.58f, 1f);

        /// <summary>
        /// Window title text color.
        /// </summary>
        public static readonly Color TitleText = new Color(0.35f, 0.25f, 0.18f, 1f);

        /// <summary>
        /// Close button color.
        /// </summary>
        public static readonly Color CloseButton = new Color(0.85f, 0.35f, 0.35f, 1f);

        /// <summary>
        /// Close button hover color.
        /// </summary>
        public static readonly Color CloseButtonHover = new Color(0.95f, 0.45f, 0.45f, 1f);

        #endregion

        #region Button Colors

        /// <summary>
        /// Button normal state color.
        /// </summary>
        public static readonly Color ButtonNormal = new Color(0.9f, 0.82f, 0.7f, 1f);

        /// <summary>
        /// Button highlighted/hover state color.
        /// </summary>
        public static readonly Color ButtonHighlighted = new Color(0.95f, 0.88f, 0.78f, 1f);

        /// <summary>
        /// Button pressed state color.
        /// </summary>
        public static readonly Color ButtonPressed = new Color(0.78f, 0.7f, 0.58f, 1f);

        /// <summary>
        /// Button selected state color.
        /// </summary>
        public static readonly Color ButtonSelected = new Color(0.85f, 0.78f, 0.68f, 1f);

        /// <summary>
        /// Button disabled state color.
        /// </summary>
        public static readonly Color ButtonDisabled = new Color(0.75f, 0.72f, 0.68f, 0.5f);

        /// <summary>
        /// Button text color.
        /// </summary>
        public static readonly Color ButtonText = new Color(0.3f, 0.22f, 0.15f, 1f);

        #endregion

        #region Text Colors

        /// <summary>
        /// Standard label text color.
        /// </summary>
        public static readonly Color LabelText = new Color(0.3f, 0.22f, 0.15f, 1f);

        /// <summary>
        /// Header text color (slightly darker/bolder).
        /// </summary>
        public static readonly Color HeaderText = new Color(0.25f, 0.18f, 0.1f, 1f);

        /// <summary>
        /// Value text color (golden/highlighted).
        /// </summary>
        public static readonly Color ValueText = new Color(0.6f, 0.45f, 0.2f, 1f);

        /// <summary>
        /// Muted/secondary text color.
        /// </summary>
        public static readonly Color MutedText = new Color(0.5f, 0.45f, 0.4f, 1f);

        /// <summary>
        /// Error/negative text color.
        /// </summary>
        public static readonly Color ErrorText = new Color(0.75f, 0.25f, 0.25f, 1f);

        /// <summary>
        /// Success/positive text color.
        /// </summary>
        public static readonly Color SuccessText = new Color(0.25f, 0.6f, 0.35f, 1f);

        #endregion

        #region Toggle/Slider Colors

        /// <summary>
        /// Toggle background when off.
        /// </summary>
        public static readonly Color ToggleOff = new Color(0.75f, 0.7f, 0.62f, 1f);

        /// <summary>
        /// Toggle background when on.
        /// </summary>
        public static readonly Color ToggleOn = new Color(0.55f, 0.75f, 0.55f, 1f);

        /// <summary>
        /// Toggle checkmark color.
        /// </summary>
        public static readonly Color ToggleCheckmark = new Color(0.25f, 0.5f, 0.25f, 1f);

        /// <summary>
        /// Slider background color.
        /// </summary>
        public static readonly Color SliderBackground = new Color(0.75f, 0.7f, 0.62f, 1f);

        /// <summary>
        /// Slider fill color.
        /// </summary>
        public static readonly Color SliderFill = new Color(0.65f, 0.55f, 0.4f, 1f);

        /// <summary>
        /// Slider handle color.
        /// </summary>
        public static readonly Color SliderHandle = new Color(0.9f, 0.82f, 0.7f, 1f);

        #endregion

        #region Item Slot Colors

        /// <summary>
        /// Item slot background color.
        /// </summary>
        public static readonly Color ItemSlotBackground = new Color(0.85f, 0.79f, 0.68f, 1f);

        /// <summary>
        /// Item slot border color.
        /// </summary>
        public static readonly Color ItemSlotBorder = new Color(0.7f, 0.62f, 0.5f, 1f);

        /// <summary>
        /// Item slot highlighted color.
        /// </summary>
        public static readonly Color ItemSlotHighlighted = new Color(0.95f, 0.9f, 0.8f, 1f);

        /// <summary>
        /// Item count text color.
        /// </summary>
        public static readonly Color ItemCountText = new Color(1f, 1f, 1f, 1f);

        #endregion

        #region ScrollView Colors

        /// <summary>
        /// Scrollbar track color.
        /// </summary>
        public static readonly Color ScrollbarTrack = new Color(0.8f, 0.74f, 0.64f, 1f);

        /// <summary>
        /// Scrollbar handle color.
        /// </summary>
        public static readonly Color ScrollbarHandle = new Color(0.7f, 0.62f, 0.5f, 1f);

        #endregion

        #region Utility

        /// <summary>
        /// Fully transparent color.
        /// </summary>
        public static readonly Color Transparent = new Color(0, 0, 0, 0);

        /// <summary>
        /// Creates a color with modified alpha.
        /// </summary>
        public static Color WithAlpha(Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }

        /// <summary>
        /// Lightens a color by the given amount.
        /// </summary>
        public static Color Lighten(Color color, float amount)
        {
            return new Color(
                Mathf.Min(1f, color.r + amount),
                Mathf.Min(1f, color.g + amount),
                Mathf.Min(1f, color.b + amount),
                color.a
            );
        }

        /// <summary>
        /// Darkens a color by the given amount.
        /// </summary>
        public static Color Darken(Color color, float amount)
        {
            return new Color(
                Mathf.Max(0f, color.r - amount),
                Mathf.Max(0f, color.g - amount),
                Mathf.Max(0f, color.b - amount),
                color.a
            );
        }

        #endregion
    }
}
