using System;
using UnityEngine;
using UnityEngine.UI;

namespace OpenWood.Core.UI
{
    /// <summary>
    /// A toggle/checkbox styled like the game's native toggles.
    /// </summary>
    public class UIToggle : UIElement
    {
        #region Private Fields

        private readonly Image _backgroundImage;
        private readonly Image _checkmarkImage;
        private readonly Toggle _toggle;
        private readonly TMPro.TextMeshProUGUI _label;
        private Action<bool> _onValueChanged;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the toggle component.
        /// </summary>
        public Toggle Toggle => _toggle;

        /// <summary>
        /// Gets or sets the toggle value.
        /// </summary>
        public bool IsOn
        {
            get => _toggle?.isOn ?? false;
            set { if (_toggle != null) _toggle.isOn = value; }
        }

        /// <summary>
        /// Gets or sets the label text.
        /// </summary>
        public string Label
        {
            get => _label?.text ?? "";
            set { if (_label != null) _label.text = value; }
        }

        #endregion

        #region Factory Method

        /// <summary>
        /// Creates a new toggle.
        /// </summary>
        public static UIToggle Create(Transform parent, string label, bool initialValue = false, Action<bool> onValueChanged = null)
        {
            return new UIToggle(parent, label, initialValue, onValueChanged);
        }

        #endregion

        #region Constructor

        private UIToggle(Transform parent, string label, bool initialValue, Action<bool> onValueChanged)
        {
            _onValueChanged = onValueChanged;

            GameObject = new GameObject("UIToggle");
            GameObject.transform.SetParent(parent, false);

            RectTransform = GameObject.AddComponent<RectTransform>();
            RectTransform.sizeDelta = new Vector2(200, 30);

            // Add layout element
            var layoutElement = GameObject.AddComponent<LayoutElement>();
            layoutElement.minWidth = 100;
            layoutElement.minHeight = 25;
            layoutElement.preferredHeight = 30;
            layoutElement.flexibleWidth = 1;

            // Horizontal layout for checkbox + label
            var horizontalLayout = GameObject.AddComponent<HorizontalLayoutGroup>();
            horizontalLayout.spacing = 10;
            horizontalLayout.childAlignment = TextAnchor.MiddleLeft;
            horizontalLayout.childControlWidth = false;
            horizontalLayout.childControlHeight = true;
            horizontalLayout.childForceExpandWidth = false;
            horizontalLayout.childForceExpandHeight = false;

            // Checkbox background
            var checkboxObj = new GameObject("Checkbox");
            checkboxObj.transform.SetParent(GameObject.transform, false);

            var checkboxRect = checkboxObj.AddComponent<RectTransform>();
            checkboxRect.sizeDelta = new Vector2(24, 24);

            var checkboxLayout = checkboxObj.AddComponent<LayoutElement>();
            checkboxLayout.minWidth = 24;
            checkboxLayout.minHeight = 24;
            checkboxLayout.preferredWidth = 24;
            checkboxLayout.preferredHeight = 24;

            _backgroundImage = checkboxObj.AddComponent<Image>();
            _backgroundImage.sprite = UISprites.GetButtonSprite();
            _backgroundImage.type = Image.Type.Sliced;
            _backgroundImage.color = UIColors.ToggleOff;

            // Checkmark
            var checkmarkObj = new GameObject("Checkmark");
            checkmarkObj.transform.SetParent(checkboxObj.transform, false);

            var checkmarkRect = checkmarkObj.AddComponent<RectTransform>();
            checkmarkRect.anchorMin = new Vector2(0.15f, 0.15f);
            checkmarkRect.anchorMax = new Vector2(0.85f, 0.85f);
            checkmarkRect.offsetMin = Vector2.zero;
            checkmarkRect.offsetMax = Vector2.zero;

            _checkmarkImage = checkmarkObj.AddComponent<Image>();
            _checkmarkImage.sprite = UISprites.GetCheckmarkSprite();
            _checkmarkImage.color = UIColors.ToggleCheckmark;

            // Label
            var labelObj = new GameObject("Label");
            labelObj.transform.SetParent(GameObject.transform, false);

            var labelRect = labelObj.AddComponent<RectTransform>();
            
            var labelLayout = labelObj.AddComponent<LayoutElement>();
            labelLayout.flexibleWidth = 1;

            _label = labelObj.AddComponent<TMPro.TextMeshProUGUI>();
            _label.text = label;
            _label.font = UISprites.GetGameFont();
            _label.fontSize = 14;
            _label.color = UIColors.LabelText;
            _label.alignment = TMPro.TextAlignmentOptions.Left;

            // Toggle component
            _toggle = GameObject.AddComponent<Toggle>();
            _toggle.targetGraphic = _backgroundImage;
            _toggle.graphic = _checkmarkImage;
            _toggle.isOn = initialValue;

            _toggle.onValueChanged.AddListener(OnToggleChanged);

            // Update visual state
            UpdateVisualState(initialValue);
        }

        #endregion

        #region Private Methods

        private void OnToggleChanged(bool value)
        {
            UpdateVisualState(value);
            _onValueChanged?.Invoke(value);
        }

        private void UpdateVisualState(bool isOn)
        {
            _backgroundImage.color = isOn ? UIColors.ToggleOn : UIColors.ToggleOff;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the value changed callback.
        /// </summary>
        public UIToggle OnValueChanged(Action<bool> callback)
        {
            _onValueChanged = callback;
            return this;
        }

        /// <summary>
        /// Sets the checkbox color when off.
        /// </summary>
        public UIToggle SetOffColor(Color color)
        {
            if (!_toggle.isOn)
            {
                _backgroundImage.color = color;
            }
            return this;
        }

        /// <summary>
        /// Sets the checkbox color when on.
        /// </summary>
        public UIToggle SetOnColor(Color color)
        {
            if (_toggle.isOn)
            {
                _backgroundImage.color = color;
            }
            return this;
        }

        /// <summary>
        /// Sets the checkmark color.
        /// </summary>
        public UIToggle SetCheckmarkColor(Color color)
        {
            _checkmarkImage.color = color;
            return this;
        }

        /// <summary>
        /// Sets the label color.
        /// </summary>
        public UIToggle SetLabelColor(Color color)
        {
            _label.color = color;
            return this;
        }

        /// <summary>
        /// Sets the font size.
        /// </summary>
        public UIToggle SetFontSize(float size)
        {
            _label.fontSize = size;
            return this;
        }

        #endregion
    }
}
