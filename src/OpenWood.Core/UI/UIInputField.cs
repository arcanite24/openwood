using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OpenWood.Core.UI
{
    /// <summary>
    /// A text input field styled like the game's native inputs.
    /// </summary>
    public class UIInputField : UIElement
    {
        #region Private Fields

        private readonly TMP_InputField _inputField;
        private readonly Image _backgroundImage;
        private readonly TextMeshProUGUI _text;
        private readonly TextMeshProUGUI _placeholder;
        private Action<string> _onValueChanged;
        private Action<string> _onEndEdit;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the input field component.
        /// </summary>
        public TMP_InputField InputField => _inputField;

        /// <summary>
        /// Gets or sets the text value.
        /// </summary>
        public string Text
        {
            get => _inputField?.text ?? "";
            set { if (_inputField != null) _inputField.text = value; }
        }

        /// <summary>
        /// Gets or sets the placeholder text.
        /// </summary>
        public string Placeholder
        {
            get => _placeholder?.text ?? "";
            set { if (_placeholder != null) _placeholder.text = value; }
        }

        /// <summary>
        /// Gets or sets the character limit (0 = no limit).
        /// </summary>
        public int CharacterLimit
        {
            get => _inputField?.characterLimit ?? 0;
            set { if (_inputField != null) _inputField.characterLimit = value; }
        }

        #endregion

        #region Factory Method

        /// <summary>
        /// Creates a new input field.
        /// </summary>
        public static UIInputField Create(Transform parent, string placeholder = "Enter text...", string initialValue = "")
        {
            return new UIInputField(parent, placeholder, initialValue);
        }

        #endregion

        #region Constructor

        private UIInputField(Transform parent, string placeholder, string initialValue)
        {
            GameObject = new GameObject("UIInputField");
            GameObject.transform.SetParent(parent, false);

            RectTransform = GameObject.AddComponent<RectTransform>();
            RectTransform.sizeDelta = new Vector2(200, 35);

            // Add layout element
            var layoutElement = GameObject.AddComponent<LayoutElement>();
            layoutElement.minWidth = 80;
            layoutElement.minHeight = 30;
            layoutElement.preferredHeight = 35;
            layoutElement.flexibleWidth = 1;

            // Background
            _backgroundImage = GameObject.AddComponent<Image>();
            _backgroundImage.sprite = UISprites.GetButtonSprite();
            _backgroundImage.type = Image.Type.Sliced;
            _backgroundImage.color = UIColors.Lighten(UIColors.PanelBackground, 0.05f);

            // Text area
            var textAreaObj = new GameObject("TextArea");
            textAreaObj.transform.SetParent(GameObject.transform, false);

            var textAreaRect = textAreaObj.AddComponent<RectTransform>();
            textAreaRect.anchorMin = Vector2.zero;
            textAreaRect.anchorMax = Vector2.one;
            textAreaRect.offsetMin = new Vector2(10, 5);
            textAreaRect.offsetMax = new Vector2(-10, -5);

            // Mask for text clipping
            textAreaObj.AddComponent<RectMask2D>();

            // Placeholder
            var placeholderObj = new GameObject("Placeholder");
            placeholderObj.transform.SetParent(textAreaObj.transform, false);

            var placeholderRect = placeholderObj.AddComponent<RectTransform>();
            placeholderRect.anchorMin = Vector2.zero;
            placeholderRect.anchorMax = Vector2.one;
            placeholderRect.offsetMin = Vector2.zero;
            placeholderRect.offsetMax = Vector2.zero;

            _placeholder = placeholderObj.AddComponent<TextMeshProUGUI>();
            _placeholder.text = placeholder;
            _placeholder.font = UISprites.GetGameFont();
            _placeholder.fontSize = 14;
            _placeholder.color = UIColors.MutedText;
            _placeholder.alignment = TextAlignmentOptions.Left;
            _placeholder.fontStyle = FontStyles.Italic;

            // Text
            var textObj = new GameObject("Text");
            textObj.transform.SetParent(textAreaObj.transform, false);

            var textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            _text = textObj.AddComponent<TextMeshProUGUI>();
            _text.text = initialValue;
            _text.font = UISprites.GetGameFont();
            _text.fontSize = 14;
            _text.color = UIColors.LabelText;
            _text.alignment = TextAlignmentOptions.Left;

            // Input field component
            _inputField = GameObject.AddComponent<TMP_InputField>();
            _inputField.textViewport = textAreaRect;
            _inputField.textComponent = _text;
            _inputField.placeholder = _placeholder;
            _inputField.text = initialValue;
            _inputField.fontAsset = UISprites.GetGameFont();
            _inputField.pointSize = 14;
            _inputField.caretColor = UIColors.LabelText;
            _inputField.selectionColor = UIColors.WithAlpha(UIColors.ValueText, 0.3f);

            _inputField.onValueChanged.AddListener(OnValueChanged);
            _inputField.onEndEdit.AddListener(OnEndEdit);
        }

        #endregion

        #region Private Methods

        private void OnValueChanged(string value)
        {
            _onValueChanged?.Invoke(value);
        }

        private void OnEndEdit(string value)
        {
            _onEndEdit?.Invoke(value);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the value changed callback.
        /// </summary>
        public UIInputField OnValueChanged(Action<string> callback)
        {
            _onValueChanged = callback;
            return this;
        }

        /// <summary>
        /// Sets the end edit callback (when focus is lost or enter is pressed).
        /// </summary>
        public UIInputField OnEndEdit(Action<string> callback)
        {
            _onEndEdit = callback;
            return this;
        }

        /// <summary>
        /// Sets the input field to accept only integers.
        /// </summary>
        public UIInputField AsInteger()
        {
            _inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
            return this;
        }

        /// <summary>
        /// Sets the input field to accept only decimal numbers.
        /// </summary>
        public UIInputField AsDecimal()
        {
            _inputField.contentType = TMP_InputField.ContentType.DecimalNumber;
            return this;
        }

        /// <summary>
        /// Sets the input field as a password field.
        /// </summary>
        public UIInputField AsPassword()
        {
            _inputField.contentType = TMP_InputField.ContentType.Password;
            return this;
        }

        /// <summary>
        /// Sets the input field as multiline.
        /// </summary>
        public UIInputField AsMultiline(int lines = 3)
        {
            _inputField.lineType = TMP_InputField.LineType.MultiLineNewline;
            RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, 35 + (lines - 1) * 18);
            return this;
        }

        /// <summary>
        /// Sets the character limit.
        /// </summary>
        public UIInputField SetCharacterLimit(int limit)
        {
            _inputField.characterLimit = limit;
            return this;
        }

        /// <summary>
        /// Sets the background color.
        /// </summary>
        public UIInputField SetBackgroundColor(Color color)
        {
            _backgroundImage.color = color;
            return this;
        }

        /// <summary>
        /// Sets the text color.
        /// </summary>
        public UIInputField SetTextColor(Color color)
        {
            _text.color = color;
            return this;
        }

        /// <summary>
        /// Sets whether the input field is interactable.
        /// </summary>
        public UIInputField SetInteractable(bool interactable)
        {
            _inputField.interactable = interactable;
            return this;
        }

        /// <summary>
        /// Focuses the input field.
        /// </summary>
        public UIInputField Focus()
        {
            _inputField.Select();
            _inputField.ActivateInputField();
            return this;
        }

        /// <summary>
        /// Clears the input field.
        /// </summary>
        public UIInputField Clear()
        {
            _inputField.text = "";
            return this;
        }

        #endregion
    }
}
