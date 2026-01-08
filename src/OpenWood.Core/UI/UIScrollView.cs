using UnityEngine;
using UnityEngine.UI;

namespace OpenWood.Core.UI
{
    /// <summary>
    /// A scroll view for scrollable content areas.
    /// </summary>
    public class UIScrollView : UIElement
    {
        #region Private Fields

        private readonly ScrollRect _scrollRect;
        private readonly RectTransform _contentRect;
        private readonly Image _backgroundImage;
        private readonly Scrollbar _verticalScrollbar;
        private readonly Scrollbar _horizontalScrollbar;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the scroll rect component.
        /// </summary>
        public ScrollRect ScrollRect => _scrollRect;

        /// <summary>
        /// Gets the content RectTransform where child elements should be added.
        /// </summary>
        public RectTransform Content => _contentRect;

        /// <summary>
        /// Gets the content Transform for adding children.
        /// </summary>
        public Transform ContentTransform => _contentRect.transform;

        /// <summary>
        /// Gets or sets vertical scrolling.
        /// </summary>
        public bool VerticalScroll
        {
            get => _scrollRect?.vertical ?? true;
            set { if (_scrollRect != null) _scrollRect.vertical = value; }
        }

        /// <summary>
        /// Gets or sets horizontal scrolling.
        /// </summary>
        public bool HorizontalScroll
        {
            get => _scrollRect?.horizontal ?? false;
            set { if (_scrollRect != null) _scrollRect.horizontal = value; }
        }

        #endregion

        #region Factory Method

        /// <summary>
        /// Creates a new scroll view.
        /// </summary>
        public static UIScrollView Create(Transform parent, float width = 300, float height = 200)
        {
            return new UIScrollView(parent, width, height);
        }

        #endregion

        #region Constructor

        private UIScrollView(Transform parent, float width, float height)
        {
            GameObject = new GameObject("UIScrollView");
            GameObject.transform.SetParent(parent, false);

            RectTransform = GameObject.AddComponent<RectTransform>();
            RectTransform.sizeDelta = new Vector2(width, height);

            // Add layout element
            var layoutElement = GameObject.AddComponent<LayoutElement>();
            layoutElement.minWidth = 100;
            layoutElement.minHeight = 50;
            layoutElement.preferredWidth = width;
            layoutElement.preferredHeight = height;
            layoutElement.flexibleWidth = 1;
            layoutElement.flexibleHeight = 1;

            // Background
            _backgroundImage = GameObject.AddComponent<Image>();
            _backgroundImage.sprite = UISprites.GetPanelSprite();
            _backgroundImage.type = Image.Type.Sliced;
            _backgroundImage.color = UIColors.Darken(UIColors.PanelBackground, 0.05f);

            // Mask for clipping content
            var mask = GameObject.AddComponent<Mask>();
            mask.showMaskGraphic = true;

            // Viewport
            var viewportObj = new GameObject("Viewport");
            viewportObj.transform.SetParent(GameObject.transform, false);

            var viewportRect = viewportObj.AddComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = new Vector2(5, 5);
            viewportRect.offsetMax = new Vector2(-20, -5); // Leave room for scrollbar

            var viewportImage = viewportObj.AddComponent<Image>();
            viewportImage.color = Color.clear;

            var viewportMask = viewportObj.AddComponent<Mask>();
            viewportMask.showMaskGraphic = false;

            // Content
            var contentObj = new GameObject("Content");
            contentObj.transform.SetParent(viewportObj.transform, false);

            _contentRect = contentObj.AddComponent<RectTransform>();
            _contentRect.anchorMin = new Vector2(0, 1);
            _contentRect.anchorMax = new Vector2(1, 1);
            _contentRect.pivot = new Vector2(0, 1);
            _contentRect.offsetMin = Vector2.zero;
            _contentRect.offsetMax = Vector2.zero;

            // Add content size fitter for auto-sizing
            var contentSizeFitter = contentObj.AddComponent<ContentSizeFitter>();
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // Vertical scrollbar
            var vScrollObj = new GameObject("VerticalScrollbar");
            vScrollObj.transform.SetParent(GameObject.transform, false);

            var vScrollRect = vScrollObj.AddComponent<RectTransform>();
            vScrollRect.anchorMin = new Vector2(1, 0);
            vScrollRect.anchorMax = new Vector2(1, 1);
            vScrollRect.pivot = new Vector2(1, 0.5f);
            vScrollRect.sizeDelta = new Vector2(12, 0);
            vScrollRect.offsetMin = new Vector2(-15, 5);
            vScrollRect.offsetMax = new Vector2(-3, -5);

            var vScrollImage = vScrollObj.AddComponent<Image>();
            vScrollImage.sprite = UISprites.GetButtonSprite();
            vScrollImage.type = Image.Type.Sliced;
            vScrollImage.color = UIColors.ScrollbarTrack;

            // Scrollbar sliding area
            var vSlideAreaObj = new GameObject("SlidingArea");
            vSlideAreaObj.transform.SetParent(vScrollObj.transform, false);

            var vSlideAreaRect = vSlideAreaObj.AddComponent<RectTransform>();
            vSlideAreaRect.anchorMin = Vector2.zero;
            vSlideAreaRect.anchorMax = Vector2.one;
            vSlideAreaRect.offsetMin = Vector2.zero;
            vSlideAreaRect.offsetMax = Vector2.zero;

            // Scrollbar handle
            var vHandleObj = new GameObject("Handle");
            vHandleObj.transform.SetParent(vSlideAreaObj.transform, false);

            var vHandleRect = vHandleObj.AddComponent<RectTransform>();
            vHandleRect.sizeDelta = Vector2.zero;

            var vHandleImage = vHandleObj.AddComponent<Image>();
            vHandleImage.sprite = UISprites.GetButtonSprite();
            vHandleImage.type = Image.Type.Sliced;
            vHandleImage.color = UIColors.ScrollbarHandle;

            _verticalScrollbar = vScrollObj.AddComponent<Scrollbar>();
            _verticalScrollbar.handleRect = vHandleRect;
            _verticalScrollbar.targetGraphic = vHandleImage;
            _verticalScrollbar.direction = Scrollbar.Direction.BottomToTop;

            // ScrollRect component
            _scrollRect = GameObject.AddComponent<ScrollRect>();
            _scrollRect.viewport = viewportRect;
            _scrollRect.content = _contentRect;
            _scrollRect.horizontal = false;
            _scrollRect.vertical = true;
            _scrollRect.verticalScrollbar = _verticalScrollbar;
            _scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            _scrollRect.scrollSensitivity = 30f;
            _scrollRect.movementType = ScrollRect.MovementType.Clamped;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a vertical layout group to the content.
        /// </summary>
        public UIScrollView WithVerticalLayout(int spacing = 5, int padding = 5)
        {
            var layout = _contentRect.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = spacing;
            layout.padding = new RectOffset(padding, padding, padding, padding);
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            return this;
        }

        /// <summary>
        /// Adds a horizontal layout group to the content.
        /// </summary>
        public UIScrollView WithHorizontalLayout(int spacing = 5, int padding = 5)
        {
            _scrollRect.horizontal = true;
            _scrollRect.vertical = false;
            
            var layout = _contentRect.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = spacing;
            layout.padding = new RectOffset(padding, padding, padding, padding);
            layout.childControlWidth = false;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;
            return this;
        }

        /// <summary>
        /// Adds a grid layout group to the content.
        /// </summary>
        public UIScrollView WithGridLayout(int cellWidth = 50, int cellHeight = 50, int spacing = 5, int padding = 5)
        {
            var layout = _contentRect.gameObject.AddComponent<GridLayoutGroup>();
            layout.cellSize = new Vector2(cellWidth, cellHeight);
            layout.spacing = new Vector2(spacing, spacing);
            layout.padding = new RectOffset(padding, padding, padding, padding);
            layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            layout.constraintCount = Mathf.Max(1, (int)((_scrollRect.viewport.rect.width - padding * 2) / (cellWidth + spacing)));
            return this;
        }

        /// <summary>
        /// Scrolls to the top.
        /// </summary>
        public UIScrollView ScrollToTop()
        {
            _scrollRect.normalizedPosition = new Vector2(0, 1);
            return this;
        }

        /// <summary>
        /// Scrolls to the bottom.
        /// </summary>
        public UIScrollView ScrollToBottom()
        {
            _scrollRect.normalizedPosition = new Vector2(0, 0);
            return this;
        }

        /// <summary>
        /// Sets the background color.
        /// </summary>
        public UIScrollView SetBackgroundColor(Color color)
        {
            _backgroundImage.color = color;
            return this;
        }

        /// <summary>
        /// Hides the background.
        /// </summary>
        public UIScrollView HideBackground()
        {
            _backgroundImage.enabled = false;
            return this;
        }

        /// <summary>
        /// Sets the scroll sensitivity.
        /// </summary>
        public UIScrollView SetScrollSensitivity(float sensitivity)
        {
            _scrollRect.scrollSensitivity = sensitivity;
            return this;
        }

        /// <summary>
        /// Clears all content.
        /// </summary>
        public UIScrollView ClearContent()
        {
            foreach (Transform child in _contentRect)
            {
                Object.Destroy(child.gameObject);
            }
            return this;
        }

        #endregion
    }
}
