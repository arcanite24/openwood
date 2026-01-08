using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace OpenWood.Core.UI
{
    /// <summary>
    /// Base class for all UI element wrappers.
    /// </summary>
    public abstract class UIElement
    {
        /// <summary>
        /// The root GameObject of this UI element.
        /// </summary>
        public GameObject GameObject { get; protected set; }

        /// <summary>
        /// The RectTransform of this element.
        /// </summary>
        public RectTransform RectTransform { get; protected set; }

        /// <summary>
        /// The Transform of this element.
        /// </summary>
        public Transform Transform => GameObject?.transform;

        /// <summary>
        /// Gets or sets whether this element is visible.
        /// </summary>
        public bool IsVisible
        {
            get => GameObject?.activeSelf ?? false;
            set => GameObject?.SetActive(value);
        }

        /// <summary>
        /// Sets the anchored position.
        /// </summary>
        public UIElement SetPosition(float x, float y)
        {
            if (RectTransform != null)
            {
                RectTransform.anchoredPosition = new Vector2(x, y);
            }
            return this;
        }

        /// <summary>
        /// Sets the size of this element.
        /// </summary>
        public UIElement SetSize(float width, float height)
        {
            if (RectTransform != null)
            {
                RectTransform.sizeDelta = new Vector2(width, height);
            }
            return this;
        }

        /// <summary>
        /// Sets the anchor preset.
        /// </summary>
        public UIElement SetAnchor(AnchorPreset preset)
        {
            if (RectTransform == null) return this;

            switch (preset)
            {
                case AnchorPreset.TopLeft:
                    RectTransform.anchorMin = new Vector2(0, 1);
                    RectTransform.anchorMax = new Vector2(0, 1);
                    RectTransform.pivot = new Vector2(0, 1);
                    break;
                case AnchorPreset.TopCenter:
                    RectTransform.anchorMin = new Vector2(0.5f, 1);
                    RectTransform.anchorMax = new Vector2(0.5f, 1);
                    RectTransform.pivot = new Vector2(0.5f, 1);
                    break;
                case AnchorPreset.TopRight:
                    RectTransform.anchorMin = new Vector2(1, 1);
                    RectTransform.anchorMax = new Vector2(1, 1);
                    RectTransform.pivot = new Vector2(1, 1);
                    break;
                case AnchorPreset.MiddleLeft:
                    RectTransform.anchorMin = new Vector2(0, 0.5f);
                    RectTransform.anchorMax = new Vector2(0, 0.5f);
                    RectTransform.pivot = new Vector2(0, 0.5f);
                    break;
                case AnchorPreset.MiddleCenter:
                    RectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    RectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                    RectTransform.pivot = new Vector2(0.5f, 0.5f);
                    break;
                case AnchorPreset.MiddleRight:
                    RectTransform.anchorMin = new Vector2(1, 0.5f);
                    RectTransform.anchorMax = new Vector2(1, 0.5f);
                    RectTransform.pivot = new Vector2(1, 0.5f);
                    break;
                case AnchorPreset.BottomLeft:
                    RectTransform.anchorMin = new Vector2(0, 0);
                    RectTransform.anchorMax = new Vector2(0, 0);
                    RectTransform.pivot = new Vector2(0, 0);
                    break;
                case AnchorPreset.BottomCenter:
                    RectTransform.anchorMin = new Vector2(0.5f, 0);
                    RectTransform.anchorMax = new Vector2(0.5f, 0);
                    RectTransform.pivot = new Vector2(0.5f, 0);
                    break;
                case AnchorPreset.BottomRight:
                    RectTransform.anchorMin = new Vector2(1, 0);
                    RectTransform.anchorMax = new Vector2(1, 0);
                    RectTransform.pivot = new Vector2(1, 0);
                    break;
                case AnchorPreset.StretchTop:
                    RectTransform.anchorMin = new Vector2(0, 1);
                    RectTransform.anchorMax = new Vector2(1, 1);
                    RectTransform.pivot = new Vector2(0.5f, 1);
                    break;
                case AnchorPreset.StretchMiddle:
                    RectTransform.anchorMin = new Vector2(0, 0.5f);
                    RectTransform.anchorMax = new Vector2(1, 0.5f);
                    RectTransform.pivot = new Vector2(0.5f, 0.5f);
                    break;
                case AnchorPreset.StretchBottom:
                    RectTransform.anchorMin = new Vector2(0, 0);
                    RectTransform.anchorMax = new Vector2(1, 0);
                    RectTransform.pivot = new Vector2(0.5f, 0);
                    break;
                case AnchorPreset.StretchFull:
                    RectTransform.anchorMin = Vector2.zero;
                    RectTransform.anchorMax = Vector2.one;
                    RectTransform.pivot = new Vector2(0.5f, 0.5f);
                    RectTransform.offsetMin = Vector2.zero;
                    RectTransform.offsetMax = Vector2.zero;
                    break;
            }

            return this;
        }

        /// <summary>
        /// Destroys this UI element.
        /// </summary>
        public virtual void Destroy()
        {
            if (GameObject != null)
            {
                UnityEngine.Object.Destroy(GameObject);
            }
        }
    }

    /// <summary>
    /// Anchor presets for UI positioning.
    /// </summary>
    public enum AnchorPreset
    {
        TopLeft,
        TopCenter,
        TopRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        BottomLeft,
        BottomCenter,
        BottomRight,
        StretchTop,
        StretchMiddle,
        StretchBottom,
        StretchFull
    }
}
