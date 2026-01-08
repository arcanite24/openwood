using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OpenWood.Core.UI
{
    /// <summary>
    /// A text label styled like the game's native text.
    /// </summary>
    public class UILabel : UIElement
    {
        #region Private Fields

        private readonly TextMeshProUGUI _text;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the TextMeshPro component.
        /// </summary>
        public TextMeshProUGUI TMPText => _text;

        /// <summary>
        /// Gets or sets the label text.
        /// </summary>
        public string Text
        {
            get => _text?.text ?? "";
            set { if (_text != null) _text.text = value; }
        }

        /// <summary>
        /// Gets or sets the text color.
        /// </summary>
        public Color Color
        {
            get => _text?.color ?? Color.white;
            set { if (_text != null) _text.color = value; }
        }

        /// <summary>
        /// Gets or sets the font size.
        /// </summary>
        public float FontSize
        {
            get => _text?.fontSize ?? 16;
            set { if (_text != null) _text.fontSize = value; }
        }

        #endregion

        #region Factory Method

        /// <summary>
        /// Creates a new label.
        /// </summary>
        public static UILabel Create(Transform parent, string text)
        {
            return new UILabel(parent, text);
        }

        #endregion

        #region Constructor

        private UILabel(Transform parent, string text)
        {
            GameObject = new GameObject("UILabel");
            GameObject.transform.SetParent(parent, false);

            RectTransform = GameObject.AddComponent<RectTransform>();
            RectTransform.sizeDelta = new Vector2(200, 30);

            // Add layout element for use in layouts
            var layoutElement = GameObject.AddComponent<LayoutElement>();
            layoutElement.minWidth = 50;
            layoutElement.minHeight = 20;
            layoutElement.flexibleWidth = 1;

            _text = GameObject.AddComponent<TextMeshProUGUI>();
            _text.text = text;
            _text.font = UISprites.GetGameFont();
            _text.fontSize = 16;
            _text.color = UIColors.LabelText;
            _text.alignment = TextAlignmentOptions.Left;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the text alignment.
        /// </summary>
        public UILabel SetAlignment(TextAlignmentOptions alignment)
        {
            _text.alignment = alignment;
            return this;
        }

        /// <summary>
        /// Centers the text.
        /// </summary>
        public UILabel Centered()
        {
            _text.alignment = TextAlignmentOptions.Center;
            return this;
        }

        /// <summary>
        /// Aligns text to the right.
        /// </summary>
        public UILabel RightAligned()
        {
            _text.alignment = TextAlignmentOptions.Right;
            return this;
        }

        /// <summary>
        /// Sets the label as a header style.
        /// </summary>
        public UILabel AsHeader()
        {
            _text.fontSize = 22;
            _text.fontStyle = FontStyles.Bold;
            _text.color = UIColors.HeaderText;
            return this;
        }

        /// <summary>
        /// Sets the label as a subheader style.
        /// </summary>
        public UILabel AsSubheader()
        {
            _text.fontSize = 18;
            _text.fontStyle = FontStyles.Bold;
            return this;
        }

        /// <summary>
        /// Sets the label as a value/highlighted style.
        /// </summary>
        public UILabel AsValue()
        {
            _text.fontStyle = FontStyles.Bold;
            _text.color = UIColors.ValueText;
            return this;
        }

        /// <summary>
        /// Makes the text bold.
        /// </summary>
        public UILabel Bold()
        {
            _text.fontStyle = FontStyles.Bold;
            return this;
        }

        /// <summary>
        /// Makes the text italic.
        /// </summary>
        public UILabel Italic()
        {
            _text.fontStyle = FontStyles.Italic;
            return this;
        }

        /// <summary>
        /// Sets text wrapping.
        /// </summary>
        public UILabel SetWrapping(bool enabled)
        {
            _text.enableWordWrapping = enabled;
            return this;
        }

        /// <summary>
        /// Sets text overflow mode.
        /// </summary>
        public UILabel SetOverflow(TextOverflowModes mode)
        {
            _text.overflowMode = mode;
            return this;
        }

        /// <summary>
        /// Sets a custom color.
        /// </summary>
        public UILabel SetColor(Color color)
        {
            _text.color = color;
            return this;
        }

        /// <summary>
        /// Sets the font size.
        /// </summary>
        public UILabel SetFontSize(float size)
        {
            _text.fontSize = size;
            return this;
        }

        #endregion
    }
}
