using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OpenWood.Core.UI
{
    /// <summary>
    /// An inventory-style item slot that displays an item sprite and count.
    /// </summary>
    public class UIItemSlot : UIElement
    {
        #region Private Fields

        private readonly Image _backgroundImage;
        private readonly Image _itemImage;
        private readonly TextMeshProUGUI _countText;
        private readonly Button _button;
        private int _itemId = -1;
        private int _count = 0;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the current item ID, or -1 if empty.
        /// </summary>
        public int ItemId => _itemId;

        /// <summary>
        /// Gets the current item count.
        /// </summary>
        public int Count => _count;

        /// <summary>
        /// Gets whether this slot is empty.
        /// </summary>
        public bool IsEmpty => _itemId < 0 || _count <= 0;

        /// <summary>
        /// Gets the button component for click handling.
        /// </summary>
        public Button Button => _button;

        #endregion

        #region Factory Method

        /// <summary>
        /// Creates a new item slot.
        /// </summary>
        public static UIItemSlot Create(Transform parent, float size = 50)
        {
            return new UIItemSlot(parent, size);
        }

        #endregion

        #region Constructor

        private UIItemSlot(Transform parent, float size)
        {
            GameObject = new GameObject("UIItemSlot");
            GameObject.transform.SetParent(parent, false);

            RectTransform = GameObject.AddComponent<RectTransform>();
            RectTransform.sizeDelta = new Vector2(size, size);

            // Add layout element
            var layoutElement = GameObject.AddComponent<LayoutElement>();
            layoutElement.minWidth = size;
            layoutElement.minHeight = size;
            layoutElement.preferredWidth = size;
            layoutElement.preferredHeight = size;

            // Background
            _backgroundImage = GameObject.AddComponent<Image>();
            _backgroundImage.sprite = UISprites.GetSlotSprite();
            _backgroundImage.type = Image.Type.Sliced;
            _backgroundImage.color = UIColors.ItemSlotBackground;

            // Button for interaction
            _button = GameObject.AddComponent<Button>();
            _button.targetGraphic = _backgroundImage;
            
            var colors = _button.colors;
            colors.normalColor = UIColors.ItemSlotBackground;
            colors.highlightedColor = UIColors.ItemSlotHighlighted;
            colors.pressedColor = UIColors.Darken(UIColors.ItemSlotBackground, 0.1f);
            _button.colors = colors;

            // Item image
            var itemObj = new GameObject("ItemImage");
            itemObj.transform.SetParent(GameObject.transform, false);

            var itemRect = itemObj.AddComponent<RectTransform>();
            itemRect.anchorMin = new Vector2(0.1f, 0.1f);
            itemRect.anchorMax = new Vector2(0.9f, 0.9f);
            itemRect.offsetMin = Vector2.zero;
            itemRect.offsetMax = Vector2.zero;

            _itemImage = itemObj.AddComponent<Image>();
            _itemImage.preserveAspect = true;
            _itemImage.enabled = false;

            // Count text
            var countObj = new GameObject("CountText");
            countObj.transform.SetParent(GameObject.transform, false);

            var countRect = countObj.AddComponent<RectTransform>();
            countRect.anchorMin = new Vector2(0.5f, 0f);
            countRect.anchorMax = new Vector2(1f, 0.35f);
            countRect.offsetMin = Vector2.zero;
            countRect.offsetMax = Vector2.zero;

            _countText = countObj.AddComponent<TextMeshProUGUI>();
            _countText.font = UISprites.GetGameFont();
            _countText.fontSize = 12;
            _countText.color = UIColors.ItemCountText;
            _countText.alignment = TextAlignmentOptions.BottomRight;
            _countText.enableWordWrapping = false;
            _countText.enabled = false;

            // Add shadow for count text visibility
            var shadow = countObj.AddComponent<Shadow>();
            shadow.effectColor = new Color(0, 0, 0, 0.8f);
            shadow.effectDistance = new Vector2(1, -1);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the item displayed in this slot.
        /// </summary>
        public UIItemSlot SetItem(int itemId, int count = 1)
        {
            _itemId = itemId;
            _count = count;

            if (itemId >= 0 && count > 0)
            {
                var sprite = UISprites.GetItemSprite(itemId);
                if (sprite != null)
                {
                    _itemImage.sprite = sprite;
                    _itemImage.enabled = true;
                }
                else
                {
                    _itemImage.enabled = false;
                }

                if (count > 1)
                {
                    _countText.text = count.ToString();
                    _countText.enabled = true;
                }
                else
                {
                    _countText.enabled = false;
                }
            }
            else
            {
                _itemImage.enabled = false;
                _countText.enabled = false;
            }

            return this;
        }

        /// <summary>
        /// Clears the slot.
        /// </summary>
        public UIItemSlot Clear()
        {
            _itemId = -1;
            _count = 0;
            _itemImage.enabled = false;
            _countText.enabled = false;
            return this;
        }

        /// <summary>
        /// Sets a click callback for the slot.
        /// </summary>
        public UIItemSlot OnClick(System.Action callback)
        {
            _button.onClick.RemoveAllListeners();
            if (callback != null)
            {
                _button.onClick.AddListener(() => callback());
            }
            return this;
        }

        /// <summary>
        /// Sets a click callback that receives the item info.
        /// </summary>
        public UIItemSlot OnClick(System.Action<int, int> callback)
        {
            _button.onClick.RemoveAllListeners();
            if (callback != null)
            {
                _button.onClick.AddListener(() => callback(_itemId, _count));
            }
            return this;
        }

        /// <summary>
        /// Sets whether the slot is interactable.
        /// </summary>
        public UIItemSlot SetInteractable(bool interactable)
        {
            _button.interactable = interactable;
            return this;
        }

        /// <summary>
        /// Sets a custom background color.
        /// </summary>
        public UIItemSlot SetBackgroundColor(Color color)
        {
            _backgroundImage.color = color;
            return this;
        }

        #endregion
    }
}
