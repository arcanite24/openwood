using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OpenWood.Core.UI
{
    /// <summary>
    /// A draggable window with title bar and content area styled like the game's UI.
    /// </summary>
    public class UIWindow : UIElement
    {
        #region Private Fields

        private readonly Image _backgroundImage;
        private readonly Image _titleBarImage;
        private readonly TextMeshProUGUI _titleText;
        private readonly Button _closeButton;
        private readonly RectTransform _contentArea;
        private readonly UIDragHandler _dragHandler;

        #endregion

        #region Public Properties

        /// <summary>
        /// The content area transform where child elements should be added.
        /// </summary>
        public RectTransform ContentArea => _contentArea;

        /// <summary>
        /// Gets or sets the window title.
        /// </summary>
        public string Title
        {
            get => _titleText?.text ?? "";
            set { if (_titleText != null) _titleText.text = value; }
        }

        /// <summary>
        /// Event fired when the close button is clicked.
        /// </summary>
        public event Action OnClose;

        /// <summary>
        /// Gets or sets whether the window can be dragged.
        /// </summary>
        public bool IsDraggable
        {
            get => _dragHandler?.enabled ?? false;
            set { if (_dragHandler != null) _dragHandler.enabled = value; }
        }

        #endregion

        #region Constructor

        internal UIWindow(string title, float width, float height, Transform parent)
        {
            // Create main window object
            GameObject = new GameObject($"UIWindow_{title.Replace(" ", "_")}");
            GameObject.transform.SetParent(parent, false);

            RectTransform = GameObject.AddComponent<RectTransform>();
            RectTransform.sizeDelta = new Vector2(width, height);
            RectTransform.anchoredPosition = Vector2.zero;
            SetAnchor(AnchorPreset.MiddleCenter);

            // Background panel
            _backgroundImage = GameObject.AddComponent<Image>();
            _backgroundImage.sprite = UISprites.GetPanelSprite();
            _backgroundImage.type = Image.Type.Sliced;
            _backgroundImage.color = UIColors.PanelBackground;

            // Title bar
            var titleBar = new GameObject("TitleBar");
            titleBar.transform.SetParent(GameObject.transform, false);

            var titleBarRect = titleBar.AddComponent<RectTransform>();
            titleBarRect.anchorMin = new Vector2(0, 1);
            titleBarRect.anchorMax = new Vector2(1, 1);
            titleBarRect.pivot = new Vector2(0.5f, 1);
            titleBarRect.sizeDelta = new Vector2(0, 40);
            titleBarRect.anchoredPosition = Vector2.zero;

            _titleBarImage = titleBar.AddComponent<Image>();
            _titleBarImage.sprite = UISprites.GetButtonSprite();
            _titleBarImage.type = Image.Type.Sliced;
            _titleBarImage.color = UIColors.TitleBar;

            // Drag handler on title bar
            _dragHandler = titleBar.AddComponent<UIDragHandler>();
            _dragHandler.Target = RectTransform;

            // Title text
            var titleTextObj = new GameObject("TitleText");
            titleTextObj.transform.SetParent(titleBar.transform, false);

            var titleTextRect = titleTextObj.AddComponent<RectTransform>();
            titleTextRect.anchorMin = Vector2.zero;
            titleTextRect.anchorMax = Vector2.one;
            titleTextRect.offsetMin = new Vector2(10, 0);
            titleTextRect.offsetMax = new Vector2(-40, 0);

            _titleText = titleTextObj.AddComponent<TextMeshProUGUI>();
            _titleText.text = title;
            _titleText.font = UISprites.GetGameFont();
            _titleText.fontSize = 18;
            _titleText.fontStyle = FontStyles.Bold;
            _titleText.color = UIColors.TitleText;
            _titleText.alignment = TextAlignmentOptions.MidlineLeft;

            // Close button
            var closeBtn = new GameObject("CloseButton");
            closeBtn.transform.SetParent(titleBar.transform, false);

            var closeBtnRect = closeBtn.AddComponent<RectTransform>();
            closeBtnRect.anchorMin = new Vector2(1, 0.5f);
            closeBtnRect.anchorMax = new Vector2(1, 0.5f);
            closeBtnRect.pivot = new Vector2(1, 0.5f);
            closeBtnRect.sizeDelta = new Vector2(30, 30);
            closeBtnRect.anchoredPosition = new Vector2(-5, 0);

            var closeBtnImage = closeBtn.AddComponent<Image>();
            closeBtnImage.sprite = UISprites.GetButtonSprite();
            closeBtnImage.type = Image.Type.Sliced;
            closeBtnImage.color = UIColors.CloseButton;

            _closeButton = closeBtn.AddComponent<Button>();
            _closeButton.targetGraphic = closeBtnImage;
            _closeButton.onClick.AddListener(() =>
            {
                OnClose?.Invoke();
                Hide();
            });

            // X text on close button
            var closeText = new GameObject("CloseText");
            closeText.transform.SetParent(closeBtn.transform, false);

            var closeTextRect = closeText.AddComponent<RectTransform>();
            closeTextRect.anchorMin = Vector2.zero;
            closeTextRect.anchorMax = Vector2.one;
            closeTextRect.offsetMin = Vector2.zero;
            closeTextRect.offsetMax = Vector2.zero;

            var closeTextTmp = closeText.AddComponent<TextMeshProUGUI>();
            closeTextTmp.text = "X";
            closeTextTmp.font = UISprites.GetGameFont();
            closeTextTmp.fontSize = 16;
            closeTextTmp.fontStyle = FontStyles.Bold;
            closeTextTmp.color = Color.white;
            closeTextTmp.alignment = TextAlignmentOptions.Center;

            // Content area
            var content = new GameObject("Content");
            content.transform.SetParent(GameObject.transform, false);

            _contentArea = content.AddComponent<RectTransform>();
            _contentArea.anchorMin = Vector2.zero;
            _contentArea.anchorMax = Vector2.one;
            _contentArea.offsetMin = new Vector2(10, 10);
            _contentArea.offsetMax = new Vector2(-10, -50);

            // Add vertical layout group to automatically arrange children
            var contentLayout = content.AddComponent<VerticalLayoutGroup>();
            contentLayout.spacing = 5;
            contentLayout.padding = new RectOffset(5, 5, 5, 5);
            contentLayout.childAlignment = TextAnchor.UpperCenter;
            contentLayout.childForceExpandWidth = true;
            contentLayout.childForceExpandHeight = false;
            contentLayout.childControlWidth = true;
            contentLayout.childControlHeight = false;

            // Add content size fitter for layout compatibility
            var layoutElement = content.AddComponent<LayoutElement>();
            layoutElement.flexibleWidth = 1;
            layoutElement.flexibleHeight = 1;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Shows the window.
        /// </summary>
        public UIWindow Show()
        {
            IsVisible = true;
            return this;
        }

        /// <summary>
        /// Hides the window.
        /// </summary>
        public UIWindow Hide()
        {
            IsVisible = false;
            return this;
        }

        /// <summary>
        /// Toggles window visibility.
        /// </summary>
        public UIWindow Toggle()
        {
            IsVisible = !IsVisible;
            return this;
        }

        /// <summary>
        /// Centers the window on screen.
        /// </summary>
        public UIWindow Center()
        {
            SetAnchor(AnchorPreset.MiddleCenter);
            RectTransform.anchoredPosition = Vector2.zero;
            return this;
        }

        /// <summary>
        /// Adds a vertical layout to the content area.
        /// </summary>
        public VerticalLayoutGroup AddVerticalLayout(float spacing = 5f, RectOffset padding = null)
        {
            var layout = _contentArea.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = spacing;
            layout.padding = padding ?? new RectOffset(5, 5, 5, 5);
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            return layout;
        }

        /// <summary>
        /// Adds a horizontal layout to the content area.
        /// </summary>
        public HorizontalLayoutGroup AddHorizontalLayout(float spacing = 5f, RectOffset padding = null)
        {
            var layout = _contentArea.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = spacing;
            layout.padding = padding ?? new RectOffset(5, 5, 5, 5);
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;
            layout.childControlWidth = false;
            layout.childControlHeight = true;
            return layout;
        }

        /// <summary>
        /// Sets the background color of the window.
        /// </summary>
        public UIWindow SetBackgroundColor(Color color)
        {
            if (_backgroundImage != null)
                _backgroundImage.color = color;
            return this;
        }

        /// <summary>
        /// Sets the title bar color.
        /// </summary>
        public UIWindow SetTitleBarColor(Color color)
        {
            if (_titleBarImage != null)
                _titleBarImage.color = color;
            return this;
        }

        #endregion
    }

    /// <summary>
    /// Simple drag handler for UI elements.
    /// </summary>
    public class UIDragHandler : MonoBehaviour, UnityEngine.EventSystems.IDragHandler, UnityEngine.EventSystems.IBeginDragHandler
    {
        public RectTransform Target { get; set; }
        private Vector2 _dragOffset;

        public void OnBeginDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (Target == null) return;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                Target.parent as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out var localPoint);
            _dragOffset = Target.anchoredPosition - localPoint;
        }

        public void OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (Target == null) return;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                Target.parent as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out var localPoint);
            Target.anchoredPosition = localPoint + _dragOffset;
        }
    }
}
