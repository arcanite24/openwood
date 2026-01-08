using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OpenWood.Core.UI
{
    /// <summary>
    /// A button styled like the game's native buttons.
    /// </summary>
    public class UIButton : UIElement
    {
        #region Private Fields

        private readonly Image _backgroundImage;
        private readonly TextMeshProUGUI _text;
        private readonly Button _button;
        private Action _onClick;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the button component.
        /// </summary>
        public Button Button => _button;

        /// <summary>
        /// Gets or sets the button text.
        /// </summary>
        public string Text
        {
            get => _text?.text ?? "";
            set { if (_text != null) _text.text = value; }
        }

        /// <summary>
        /// Gets or sets whether the button is interactable.
        /// </summary>
        public bool Interactable
        {
            get => _button?.interactable ?? false;
            set { if (_button != null) _button.interactable = value; }
        }

        #endregion

        #region Factory Method

        /// <summary>
        /// Creates a new button.
        /// </summary>
        public static UIButton Create(Transform parent, string text, Action onClick = null)
        {
            return new UIButton(parent, text, onClick);
        }

        #endregion

        #region Constructor

        private UIButton(Transform parent, string text, Action onClick)
        {
            _onClick = onClick;

            GameObject = new GameObject("UIButton");
            GameObject.transform.SetParent(parent, false);

            RectTransform = GameObject.AddComponent<RectTransform>();
            RectTransform.sizeDelta = new Vector2(120, 40);

            // Add layout element for use in layouts
            var layoutElement = GameObject.AddComponent<LayoutElement>();
            layoutElement.minWidth = 80;
            layoutElement.minHeight = 35;
            layoutElement.preferredWidth = 120;
            layoutElement.preferredHeight = 40;

            _backgroundImage = GameObject.AddComponent<Image>();
            _backgroundImage.sprite = UISprites.GetButtonSprite();
            _backgroundImage.type = Image.Type.Sliced;
            _backgroundImage.color = UIColors.ButtonNormal;

            _button = GameObject.AddComponent<Button>();
            _button.targetGraphic = _backgroundImage;
            
            // Set up button colors for hover/press states
            var colors = _button.colors;
            colors.normalColor = UIColors.ButtonNormal;
            colors.highlightedColor = UIColors.ButtonHighlighted;
            colors.pressedColor = UIColors.ButtonPressed;
            colors.selectedColor = UIColors.ButtonSelected;
            colors.disabledColor = UIColors.ButtonDisabled;
            _button.colors = colors;

            _button.onClick.AddListener(() => _onClick?.Invoke());

            // Text
            var textObj = new GameObject("Text");
            textObj.transform.SetParent(GameObject.transform, false);

            var textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(5, 2);
            textRect.offsetMax = new Vector2(-5, -2);

            _text = textObj.AddComponent<TextMeshProUGUI>();
            _text.text = text;
            _text.font = UISprites.GetGameFont();
            _text.fontSize = 16;
            _text.color = UIColors.ButtonText;
            _text.alignment = TextAlignmentOptions.Center;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the click callback.
        /// </summary>
        public UIButton OnClick(Action callback)
        {
            _onClick = callback;
            return this;
        }

        /// <summary>
        /// Sets the button's normal color.
        /// </summary>
        public UIButton SetColor(Color color)
        {
            var colors = _button.colors;
            colors.normalColor = color;
            _button.colors = colors;
            return this;
        }

        /// <summary>
        /// Sets the button text color.
        /// </summary>
        public UIButton SetTextColor(Color color)
        {
            _text.color = color;
            return this;
        }

        /// <summary>
        /// Sets the font size.
        /// </summary>
        public UIButton SetFontSize(float size)
        {
            _text.fontSize = size;
            return this;
        }

        /// <summary>
        /// Sets a custom sprite for the button.
        /// </summary>
        public UIButton SetSprite(Sprite sprite)
        {
            _backgroundImage.sprite = sprite;
            return this;
        }

        /// <summary>
        /// Sets the button as a small/compact style.
        /// </summary>
        public UIButton AsSmall()
        {
            RectTransform.sizeDelta = new Vector2(80, 30);
            _text.fontSize = 12;
            return this;
        }

        /// <summary>
        /// Sets the button as a large style.
        /// </summary>
        public UIButton AsLarge()
        {
            RectTransform.sizeDelta = new Vector2(200, 50);
            _text.fontSize = 20;
            return this;
        }

        #endregion
    }
}
