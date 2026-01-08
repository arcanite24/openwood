using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OpenWood.Core.UI
{
    /// <summary>
    /// A panel styled like the game's native UI panels.
    /// </summary>
    public class UIPanel : UIElement
    {
        #region Private Fields

        private readonly Image _backgroundImage;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the background image component.
        /// </summary>
        public Image BackgroundImage => _backgroundImage;

        #endregion

        #region Factory Method

        /// <summary>
        /// Creates a new panel.
        /// </summary>
        public static UIPanel Create(Transform parent, float width, float height)
        {
            return new UIPanel(parent, width, height);
        }

        #endregion

        #region Constructor

        private UIPanel(Transform parent, float width, float height)
        {
            GameObject = new GameObject("UIPanel");
            GameObject.transform.SetParent(parent, false);

            RectTransform = GameObject.AddComponent<RectTransform>();
            RectTransform.sizeDelta = new Vector2(width, height);

            _backgroundImage = GameObject.AddComponent<Image>();
            _backgroundImage.sprite = UISprites.GetPanelSprite();
            _backgroundImage.type = Image.Type.Sliced;
            _backgroundImage.color = UIColors.PanelBackground;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the panel's background color.
        /// </summary>
        public UIPanel SetColor(Color color)
        {
            _backgroundImage.color = color;
            return this;
        }

        /// <summary>
        /// Sets a custom sprite for the panel background.
        /// </summary>
        public UIPanel SetSprite(Sprite sprite)
        {
            _backgroundImage.sprite = sprite;
            return this;
        }

        /// <summary>
        /// Adds a vertical layout group to the panel.
        /// </summary>
        public UIPanel WithVerticalLayout(float spacing = 5f, RectOffset padding = null)
        {
            var layout = GameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = spacing;
            layout.padding = padding ?? new RectOffset(10, 10, 10, 10);
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            return this;
        }

        /// <summary>
        /// Adds a horizontal layout group to the panel.
        /// </summary>
        public UIPanel WithHorizontalLayout(float spacing = 5f, RectOffset padding = null)
        {
            var layout = GameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = spacing;
            layout.padding = padding ?? new RectOffset(10, 10, 10, 10);
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;
            layout.childControlWidth = false;
            layout.childControlHeight = true;
            return this;
        }

        /// <summary>
        /// Adds a grid layout group to the panel.
        /// </summary>
        public UIPanel WithGridLayout(Vector2 cellSize, Vector2 spacing, RectOffset padding = null)
        {
            var layout = GameObject.AddComponent<GridLayoutGroup>();
            layout.cellSize = cellSize;
            layout.spacing = spacing;
            layout.padding = padding ?? new RectOffset(10, 10, 10, 10);
            layout.startCorner = GridLayoutGroup.Corner.UpperLeft;
            layout.startAxis = GridLayoutGroup.Axis.Horizontal;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.constraint = GridLayoutGroup.Constraint.Flexible;
            return this;
        }

        /// <summary>
        /// Adds a content size fitter.
        /// </summary>
        public UIPanel WithContentSizeFitter(ContentSizeFitter.FitMode horizontalFit = ContentSizeFitter.FitMode.Unconstrained,
                                              ContentSizeFitter.FitMode verticalFit = ContentSizeFitter.FitMode.PreferredSize)
        {
            var fitter = GameObject.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = horizontalFit;
            fitter.verticalFit = verticalFit;
            return this;
        }

        #endregion
    }
}
