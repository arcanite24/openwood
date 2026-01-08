using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OpenWood.Core.UI
{
    /// <summary>
    /// A slider styled like the game's native sliders.
    /// </summary>
    public class UISlider : UIElement
    {
        #region Private Fields

        private readonly Slider _slider;
        private readonly Image _backgroundImage;
        private readonly Image _fillImage;
        private readonly Image _handleImage;
        private readonly TextMeshProUGUI _label;
        private readonly TextMeshProUGUI _valueText;
        private Action<float> _onValueChanged;
        private string _valueFormat = "{0:F1}";

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the slider component.
        /// </summary>
        public Slider Slider => _slider;

        /// <summary>
        /// Gets or sets the slider value.
        /// </summary>
        public float Value
        {
            get => _slider?.value ?? 0;
            set { if (_slider != null) _slider.value = value; }
        }

        /// <summary>
        /// Gets or sets the minimum value.
        /// </summary>
        public float MinValue
        {
            get => _slider?.minValue ?? 0;
            set { if (_slider != null) _slider.minValue = value; }
        }

        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        public float MaxValue
        {
            get => _slider?.maxValue ?? 1;
            set { if (_slider != null) _slider.maxValue = value; }
        }

        /// <summary>
        /// Gets or sets whether the slider uses whole numbers only.
        /// </summary>
        public bool WholeNumbers
        {
            get => _slider?.wholeNumbers ?? false;
            set { if (_slider != null) _slider.wholeNumbers = value; }
        }

        #endregion

        #region Factory Method

        /// <summary>
        /// Creates a new slider.
        /// </summary>
        public static UISlider Create(Transform parent, string label, float minValue = 0, float maxValue = 1, float initialValue = 0.5f)
        {
            return new UISlider(parent, label, minValue, maxValue, initialValue);
        }

        #endregion

        #region Constructor

        private UISlider(Transform parent, string label, float minValue, float maxValue, float initialValue)
        {
            GameObject = new GameObject("UISlider");
            GameObject.transform.SetParent(parent, false);

            RectTransform = GameObject.AddComponent<RectTransform>();
            RectTransform.sizeDelta = new Vector2(250, 45);

            // Add layout element
            var layoutElement = GameObject.AddComponent<LayoutElement>();
            layoutElement.minWidth = 150;
            layoutElement.minHeight = 40;
            layoutElement.preferredHeight = 45;
            layoutElement.flexibleWidth = 1;

            // Vertical layout for label row + slider row
            var verticalLayout = GameObject.AddComponent<VerticalLayoutGroup>();
            verticalLayout.spacing = 4;
            verticalLayout.childControlWidth = true;
            verticalLayout.childControlHeight = false;
            verticalLayout.childForceExpandWidth = true;
            verticalLayout.childForceExpandHeight = false;

            // Label row
            var labelRow = new GameObject("LabelRow");
            labelRow.transform.SetParent(GameObject.transform, false);

            var labelRowRect = labelRow.AddComponent<RectTransform>();
            labelRowRect.sizeDelta = new Vector2(0, 18);

            var labelRowLayout = labelRow.AddComponent<LayoutElement>();
            labelRowLayout.preferredHeight = 18;

            var labelRowHorizontal = labelRow.AddComponent<HorizontalLayoutGroup>();
            labelRowHorizontal.childControlWidth = true;
            labelRowHorizontal.childForceExpandWidth = true;

            // Label text
            var labelObj = new GameObject("Label");
            labelObj.transform.SetParent(labelRow.transform, false);

            _label = labelObj.AddComponent<TextMeshProUGUI>();
            _label.text = label;
            _label.font = UISprites.GetGameFont();
            _label.fontSize = 14;
            _label.color = UIColors.LabelText;
            _label.alignment = TextAlignmentOptions.Left;

            // Value text
            var valueObj = new GameObject("Value");
            valueObj.transform.SetParent(labelRow.transform, false);

            var valueLayout = valueObj.AddComponent<LayoutElement>();
            valueLayout.minWidth = 50;
            valueLayout.flexibleWidth = 0;

            _valueText = valueObj.AddComponent<TextMeshProUGUI>();
            _valueText.font = UISprites.GetGameFont();
            _valueText.fontSize = 14;
            _valueText.color = UIColors.ValueText;
            _valueText.alignment = TextAlignmentOptions.Right;

            // Slider row
            var sliderRow = new GameObject("SliderRow");
            sliderRow.transform.SetParent(GameObject.transform, false);

            var sliderRowRect = sliderRow.AddComponent<RectTransform>();
            sliderRowRect.sizeDelta = new Vector2(0, 20);

            var sliderRowLayout = sliderRow.AddComponent<LayoutElement>();
            sliderRowLayout.preferredHeight = 20;

            // Background
            var backgroundObj = new GameObject("Background");
            backgroundObj.transform.SetParent(sliderRow.transform, false);

            var backgroundRect = backgroundObj.AddComponent<RectTransform>();
            backgroundRect.anchorMin = new Vector2(0, 0.25f);
            backgroundRect.anchorMax = new Vector2(1, 0.75f);
            backgroundRect.offsetMin = Vector2.zero;
            backgroundRect.offsetMax = Vector2.zero;

            _backgroundImage = backgroundObj.AddComponent<Image>();
            _backgroundImage.sprite = UISprites.GetButtonSprite();
            _backgroundImage.type = Image.Type.Sliced;
            _backgroundImage.color = UIColors.SliderBackground;

            // Fill area
            var fillAreaObj = new GameObject("FillArea");
            fillAreaObj.transform.SetParent(sliderRow.transform, false);

            var fillAreaRect = fillAreaObj.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0, 0.25f);
            fillAreaRect.anchorMax = new Vector2(1, 0.75f);
            fillAreaRect.offsetMin = new Vector2(5, 0);
            fillAreaRect.offsetMax = new Vector2(-5, 0);

            // Fill
            var fillObj = new GameObject("Fill");
            fillObj.transform.SetParent(fillAreaObj.transform, false);

            var fillRect = fillObj.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;

            _fillImage = fillObj.AddComponent<Image>();
            _fillImage.sprite = UISprites.GetButtonSprite();
            _fillImage.type = Image.Type.Sliced;
            _fillImage.color = UIColors.SliderFill;

            // Handle area
            var handleAreaObj = new GameObject("HandleSlideArea");
            handleAreaObj.transform.SetParent(sliderRow.transform, false);

            var handleAreaRect = handleAreaObj.AddComponent<RectTransform>();
            handleAreaRect.anchorMin = new Vector2(0, 0);
            handleAreaRect.anchorMax = new Vector2(1, 1);
            handleAreaRect.offsetMin = new Vector2(10, 0);
            handleAreaRect.offsetMax = new Vector2(-10, 0);

            // Handle
            var handleObj = new GameObject("Handle");
            handleObj.transform.SetParent(handleAreaObj.transform, false);

            var handleRect = handleObj.AddComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(16, 20);

            _handleImage = handleObj.AddComponent<Image>();
            _handleImage.sprite = UISprites.GetButtonSprite();
            _handleImage.type = Image.Type.Sliced;
            _handleImage.color = UIColors.SliderHandle;

            // Slider component
            _slider = sliderRow.AddComponent<Slider>();
            _slider.fillRect = fillRect;
            _slider.handleRect = handleRect;
            _slider.targetGraphic = _handleImage;
            _slider.direction = Slider.Direction.LeftToRight;
            _slider.minValue = minValue;
            _slider.maxValue = maxValue;
            _slider.value = initialValue;

            _slider.onValueChanged.AddListener(OnSliderChanged);

            UpdateValueText(initialValue);
        }

        #endregion

        #region Private Methods

        private void OnSliderChanged(float value)
        {
            UpdateValueText(value);
            _onValueChanged?.Invoke(value);
        }

        private void UpdateValueText(float value)
        {
            _valueText.text = string.Format(_valueFormat, value);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the value changed callback.
        /// </summary>
        public UISlider OnValueChanged(Action<float> callback)
        {
            _onValueChanged = callback;
            return this;
        }

        /// <summary>
        /// Sets the value display format string.
        /// </summary>
        public UISlider SetValueFormat(string format)
        {
            _valueFormat = format;
            UpdateValueText(_slider.value);
            return this;
        }

        /// <summary>
        /// Configures the slider for integer values only.
        /// </summary>
        public UISlider AsWholeNumbers()
        {
            _slider.wholeNumbers = true;
            _valueFormat = "{0:F0}";
            UpdateValueText(_slider.value);
            return this;
        }

        /// <summary>
        /// Configures the slider for percentage display.
        /// </summary>
        public UISlider AsPercentage()
        {
            _valueFormat = "{0:P0}";
            UpdateValueText(_slider.value);
            return this;
        }

        /// <summary>
        /// Sets the fill color.
        /// </summary>
        public UISlider SetFillColor(Color color)
        {
            _fillImage.color = color;
            return this;
        }

        /// <summary>
        /// Sets the handle color.
        /// </summary>
        public UISlider SetHandleColor(Color color)
        {
            _handleImage.color = color;
            return this;
        }

        /// <summary>
        /// Hides the value text.
        /// </summary>
        public UISlider HideValue()
        {
            _valueText.enabled = false;
            return this;
        }

        /// <summary>
        /// Sets the label color.
        /// </summary>
        public UISlider SetLabelColor(Color color)
        {
            _label.color = color;
            return this;
        }

        #endregion
    }
}
