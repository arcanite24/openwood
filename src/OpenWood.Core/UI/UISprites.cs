using System.Collections.Generic;
using OpenWood.Core.Utilities;
using TMPro;
using UnityEngine;

namespace OpenWood.Core.UI
{
    /// <summary>
    /// Provides access to the game's UI sprites and fonts.
    /// Caches loaded resources for performance.
    /// </summary>
    public static class UISprites
    {
        #region Private Fields

        private static bool _initialized;
        private static Sprite _panelSprite;
        private static Sprite _buttonSprite;
        private static Sprite _slotSprite;
        private static Sprite _checkmarkSprite;
        private static TMP_FontAsset _gameFont;
        private static readonly Dictionary<int, Sprite> _itemSprites = new Dictionary<int, Sprite>();
        private static readonly Dictionary<int, Sprite> _portraitSprites = new Dictionary<int, Sprite>();
        private static Sprite[] _allItemSprites;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the sprite system. Called automatically when needed.
        /// </summary>
        public static void Initialize()
        {
            if (_initialized) return;
            
            LoadSprites();
            LoadFont();
            
            _initialized = true;
            Plugin.Log.LogDebug("UISprites initialized");
        }

        private static void LoadSprites()
        {
            try
            {
                // Try to load sprites from the game
                // The game uses Resources.LoadAll<Sprite>("itemSprite") pattern
                _allItemSprites = Resources.LoadAll<Sprite>("itemSprite");
                
                if (_allItemSprites != null && _allItemSprites.Length > 0)
                {
                    Plugin.Log.LogDebug($"Loaded {_allItemSprites.Length} item sprites");
                }

                // Try to find GameScript for its sprite references
                var gameScript = Object.FindObjectOfType<GameScript>();
                if (gameScript != null)
                {
                    // These sprites are public fields on GameScript
                    _panelSprite = ReflectionHelper.GetField<Sprite>(gameScript, "slicedTan");
                    _buttonSprite = ReflectionHelper.GetField<Sprite>(gameScript, "slicedTan2") ?? _panelSprite;
                    _slotSprite = ReflectionHelper.GetField<Sprite>(gameScript, "inventorySlot");
                    
                    Plugin.Log.LogDebug("Loaded game sprites from GameScript");
                }
            }
            catch (System.Exception ex)
            {
                Plugin.Log.LogWarning($"Failed to load game sprites: {ex.Message}");
            }

            // Create fallback sprites if loading failed
            if (_panelSprite == null)
            {
                _panelSprite = CreateFallbackSprite(UIColors.PanelBackground);
            }
            if (_buttonSprite == null)
            {
                _buttonSprite = CreateFallbackSprite(UIColors.ButtonNormal);
            }
            if (_slotSprite == null)
            {
                _slotSprite = CreateFallbackSprite(UIColors.ItemSlotBackground);
            }
            if (_checkmarkSprite == null)
            {
                _checkmarkSprite = CreateCheckmarkSprite();
            }
        }

        private static void LoadFont()
        {
            try
            {
                // Try to find a TMP font in the scene or resources
                var tmpTexts = Object.FindObjectsOfType<TextMeshProUGUI>();
                if (tmpTexts != null && tmpTexts.Length > 0)
                {
                    foreach (var tmp in tmpTexts)
                    {
                        if (tmp.font != null)
                        {
                            _gameFont = tmp.font;
                            Plugin.Log.LogDebug($"Found game font: {_gameFont.name}");
                            break;
                        }
                    }
                }
                
                // Try loading from resources
                if (_gameFont == null)
                {
                    var fonts = Resources.LoadAll<TMP_FontAsset>("");
                    if (fonts != null && fonts.Length > 0)
                    {
                        _gameFont = fonts[0];
                        Plugin.Log.LogDebug($"Loaded font from resources: {_gameFont.name}");
                    }
                }
            }
            catch (System.Exception ex)
            {
                Plugin.Log.LogWarning($"Failed to load game font: {ex.Message}");
            }

            // Fallback to TMP default
            if (_gameFont == null)
            {
                _gameFont = TMP_Settings.defaultFontAsset;
            }
        }

        #endregion

        #region Public Accessors

        /// <summary>
        /// Gets the panel/window background sprite (slicedTan).
        /// </summary>
        public static Sprite GetPanelSprite()
        {
            if (!_initialized) Initialize();
            return _panelSprite;
        }

        /// <summary>
        /// Gets the button sprite (slicedTan2).
        /// </summary>
        public static Sprite GetButtonSprite()
        {
            if (!_initialized) Initialize();
            return _buttonSprite;
        }

        /// <summary>
        /// Gets the inventory slot sprite.
        /// </summary>
        public static Sprite GetSlotSprite()
        {
            if (!_initialized) Initialize();
            return _slotSprite;
        }

        /// <summary>
        /// Gets a checkmark sprite for toggles.
        /// </summary>
        public static Sprite GetCheckmarkSprite()
        {
            if (!_initialized) Initialize();
            return _checkmarkSprite;
        }

        /// <summary>
        /// Gets an item sprite by ID.
        /// </summary>
        public static Sprite GetItemSprite(int itemId)
        {
            if (!_initialized) Initialize();

            if (_itemSprites.TryGetValue(itemId, out var cached))
            {
                return cached;
            }

            // Items are stored in the sprite sheet by index
            if (_allItemSprites != null && itemId >= 0 && itemId < _allItemSprites.Length)
            {
                var sprite = _allItemSprites[itemId];
                _itemSprites[itemId] = sprite;
                return sprite;
            }

            return null;
        }

        /// <summary>
        /// Gets an NPC portrait sprite by ID.
        /// </summary>
        public static Sprite GetPortraitSprite(int npcId)
        {
            if (!_initialized) Initialize();

            if (_portraitSprites.TryGetValue(npcId, out var cached))
            {
                return cached;
            }

            try
            {
                // Portraits are typically in a separate resource
                var portraits = Resources.LoadAll<Sprite>("portrait");
                if (portraits != null && npcId >= 0 && npcId < portraits.Length)
                {
                    var sprite = portraits[npcId];
                    _portraitSprites[npcId] = sprite;
                    return sprite;
                }
            }
            catch (System.Exception ex)
            {
                Plugin.Log.LogWarning($"Failed to load portrait {npcId}: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Gets the game's TMP font.
        /// </summary>
        public static TMP_FontAsset GetGameFont()
        {
            if (!_initialized) Initialize();
            return _gameFont;
        }

        /// <summary>
        /// Gets all loaded item sprites.
        /// </summary>
        public static Sprite[] GetAllItemSprites()
        {
            if (!_initialized) Initialize();
            return _allItemSprites;
        }

        #endregion

        #region Fallback Sprite Creation

        private static Sprite CreateFallbackSprite(Color color)
        {
            // Create a simple 9-slice sprite as fallback
            var texture = new Texture2D(32, 32);
            var pixels = new Color[32 * 32];

            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    // Create a simple bordered square
                    bool isBorder = x < 2 || x >= 30 || y < 2 || y >= 30;
                    pixels[y * 32 + x] = isBorder ? UIColors.Darken(color, 0.2f) : color;
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();
            texture.filterMode = FilterMode.Point;

            // Create sprite with 9-slice border
            return Sprite.Create(
                texture,
                new Rect(0, 0, 32, 32),
                new Vector2(0.5f, 0.5f),
                100f,
                0,
                SpriteMeshType.FullRect,
                new Vector4(4, 4, 4, 4) // Border for 9-slice
            );
        }

        private static Sprite CreateCheckmarkSprite()
        {
            var texture = new Texture2D(16, 16);
            var pixels = new Color[16 * 16];

            // Clear to transparent
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.clear;
            }

            // Draw a simple checkmark
            var checkColor = UIColors.ToggleCheckmark;
            
            // Draw the checkmark shape
            int[,] checkShape = {
                {4, 7}, {5, 8}, {6, 9}, {7, 10},
                {8, 9}, {9, 8}, {10, 7}, {11, 6}, {12, 5}
            };

            for (int i = 0; i < checkShape.GetLength(0); i++)
            {
                int x = checkShape[i, 0];
                int y = checkShape[i, 1];
                
                // Draw thick checkmark
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        int px = x + dx;
                        int py = y + dy;
                        if (px >= 0 && px < 16 && py >= 0 && py < 16)
                        {
                            pixels[py * 16 + px] = checkColor;
                        }
                    }
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();
            texture.filterMode = FilterMode.Point;

            return Sprite.Create(
                texture,
                new Rect(0, 0, 16, 16),
                new Vector2(0.5f, 0.5f),
                100f
            );
        }

        #endregion

        #region Reload

        /// <summary>
        /// Forces reloading of all sprites. Useful after scene changes.
        /// </summary>
        public static void Reload()
        {
            _initialized = false;
            _itemSprites.Clear();
            _portraitSprites.Clear();
            Initialize();
        }

        #endregion
    }
}
