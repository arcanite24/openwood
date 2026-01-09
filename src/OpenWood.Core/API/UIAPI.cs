using System;
using System.Collections.Generic;
using OpenWood.Core.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OpenWood.Core.API
{
    /// <summary>
    /// API for creating native game-styled UI elements.
    /// Uses the game's built-in Canvas and UI sprites to create consistent-looking mod interfaces.
    /// </summary>
    public static class UIAPI
    {
        #region Private Fields

        private static bool _initialized;
        private static Canvas _modCanvas;
        private static CanvasScaler _canvasScaler;
        private static EventSystem _eventSystem;
        private static GameObject _modUIRoot;
        
        // Cached game references
        private static GameScript _gameScript;
        
        // Registered mod windows
        private static readonly List<UIWindow> _windows = new List<UIWindow>();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets whether the UI API is ready to use.
        /// </summary>
        public static bool IsReady => _initialized && _modCanvas != null;

        /// <summary>
        /// Gets the mod UI Canvas.
        /// </summary>
        public static Canvas ModCanvas => _modCanvas;

        /// <summary>
        /// Gets the game's EventSystem.
        /// </summary>
        public static EventSystem GameEventSystem => _eventSystem;

        /// <summary>
        /// Gets the game's main GameScript instance.
        /// </summary>
        public static GameScript GameScript => _gameScript;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the UI API. Called automatically by the plugin.
        /// </summary>
        internal static void Initialize()
        {
            if (_initialized) return;

            Plugin.Log.LogDebug("Initializing UI API...");
            
            // We'll set up the canvas once the game is loaded
            Events.GameEvents.OnGameStart += OnGameStart;
            
            _initialized = true;
        }

        private static void OnGameStart()
        {
            SetupModCanvas();
            CacheGameReferences();
            UISprites.Initialize();
            Plugin.Log.LogInfo("UI API ready - native UI elements available");
        }

        private static void SetupModCanvas()
        {
            // Create a dedicated canvas for mod UI
            _modUIRoot = new GameObject("OpenWood_UI");
            UnityEngine.Object.DontDestroyOnLoad(_modUIRoot);

            _modCanvas = _modUIRoot.AddComponent<Canvas>();
            _modCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _modCanvas.sortingOrder = 1000; // Render above game UI

            _canvasScaler = _modUIRoot.AddComponent<CanvasScaler>();
            _canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            _canvasScaler.referenceResolution = new Vector2(1920, 1080);
            _canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            _canvasScaler.matchWidthOrHeight = 0.5f;

            _modUIRoot.AddComponent<GraphicRaycaster>();

            Plugin.Log.LogDebug("Mod UI Canvas created");
        }

        private static void CacheGameReferences()
        {
            _gameScript = UnityEngine.Object.FindObjectOfType<GameScript>();
            _eventSystem = UnityEngine.Object.FindObjectOfType<EventSystem>();
            
            if (_gameScript == null)
            {
                Plugin.Log.LogWarning("GameScript not found - some UI features may be limited");
            }
            
            if (_eventSystem == null)
            {
                Plugin.Log.LogWarning("EventSystem not found - creating new one for mod UI");
                var eventSystemObj = new GameObject("OpenWood_EventSystem");
                _eventSystem = eventSystemObj.AddComponent<EventSystem>();
                eventSystemObj.AddComponent<StandaloneInputModule>();
                UnityEngine.Object.DontDestroyOnLoad(eventSystemObj);
            }
        }

        #endregion

        #region Window Management

        /// <summary>
        /// Creates a new native-styled window.
        /// </summary>
        /// <param name="title">Window title</param>
        /// <param name="width">Window width in pixels</param>
        /// <param name="height">Window height in pixels</param>
        /// <returns>A new UIWindow instance</returns>
        public static UIWindow CreateWindow(string title, float width = 400, float height = 300)
        {
            if (!IsReady)
            {
                Plugin.Log.LogWarning("UI API not ready - window creation deferred");
                return null;
            }

            var window = new UIWindow(title, width, height, _modCanvas.transform);
            _windows.Add(window);
            return window;
        }

        /// <summary>
        /// Destroys a window and removes it from the system.
        /// </summary>
        public static void DestroyWindow(UIWindow window)
        {
            if (window == null) return;
            
            _windows.Remove(window);
            window.Destroy();
        }

        /// <summary>
        /// Gets all registered windows.
        /// </summary>
        public static IReadOnlyList<UIWindow> GetWindows() => _windows.AsReadOnly();

        #endregion

        #region UI Element Creation

        /// <summary>
        /// Creates a panel with the game's native tan background.
        /// </summary>
        public static UIPanel CreatePanel(Transform parent, float width, float height)
        {
            return UIPanel.Create(parent, width, height);
        }

        /// <summary>
        /// Creates a button with the game's native button style.
        /// </summary>
        public static UIButton CreateButton(Transform parent, string text, Action onClick = null)
        {
            return UIButton.Create(parent, text, onClick);
        }

        /// <summary>
        /// Creates a text label with the game's font.
        /// </summary>
        public static UILabel CreateLabel(Transform parent, string text)
        {
            return UILabel.Create(parent, text);
        }

        /// <summary>
        /// Creates an item slot styled like the game's inventory slots.
        /// </summary>
        public static UIItemSlot CreateItemSlot(Transform parent, float size = 50)
        {
            return UIItemSlot.Create(parent, size);
        }

        /// <summary>
        /// Creates a horizontal layout group.
        /// </summary>
        public static GameObject CreateHorizontalLayout(Transform parent, float spacing = 5f)
        {
            var go = new GameObject("HorizontalLayout");
            go.transform.SetParent(parent, false);

            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, 35); // Default height

            var layoutElem = go.AddComponent<LayoutElement>();
            layoutElem.minHeight = 35;
            layoutElem.preferredHeight = 35;
            layoutElem.flexibleWidth = 1;

            var layout = go.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = spacing;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;
            layout.childControlWidth = false;
            layout.childControlHeight = true;

            return go;
        }

        /// <summary>
        /// Creates a vertical layout group.
        /// </summary>
        public static GameObject CreateVerticalLayout(Transform parent, float spacing = 5f)
        {
            var go = new GameObject("VerticalLayout");
            go.transform.SetParent(parent, false);

            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, 0);

            var layoutElem = go.AddComponent<LayoutElement>();
            layoutElem.flexibleWidth = 1;
            layoutElem.flexibleHeight = 1;

            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.spacing = spacing;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.childControlWidth = true;
            layout.childControlHeight = false;

            return go;
        }

        /// <summary>
        /// Creates a scroll view.
        /// </summary>
        public static UIScrollView CreateScrollView(Transform parent, float width, float height)
        {
            return UIScrollView.Create(parent, width, height);
        }

        /// <summary>
        /// Creates a toggle/checkbox.
        /// </summary>
        public static UIToggle CreateToggle(Transform parent, string label, bool initialValue = false, Action<bool> onValueChanged = null)
        {
            return UIToggle.Create(parent, label, initialValue, onValueChanged);
        }

        /// <summary>
        /// Creates a slider.
        /// </summary>
        public static UISlider CreateSlider(Transform parent, string label, float min = 0, float max = 1, float initialValue = 0.5f)
        {
            return UISlider.Create(parent, label, min, max, initialValue);
        }

        /// <summary>
        /// Creates an input field.
        /// </summary>
        public static UIInputField CreateInputField(Transform parent, string placeholder = "Enter text...", string initialValue = "")
        {
            return UIInputField.Create(parent, placeholder, initialValue);
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Sets the selected UI element (for controller/keyboard navigation).
        /// </summary>
        public static void SetSelected(GameObject uiElement)
        {
            if (_eventSystem == null) return;
            
            _eventSystem.SetSelectedGameObject(null);
            _eventSystem.SetSelectedGameObject(uiElement);
        }

        /// <summary>
        /// Creates a simple texture of a solid color.
        /// </summary>
        public static Texture2D CreateSolidTexture(int width, int height, Color color)
        {
            var pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = color;

            var tex = new Texture2D(width, height);
            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }

        /// <summary>
        /// Creates a sprite from a solid color.
        /// </summary>
        public static Sprite CreateSolidSprite(int width, int height, Color color)
        {
            var tex = CreateSolidTexture(width, height, color);
            return Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
        }

        /// <summary>
        /// Gets the game's item sprite by ID.
        /// </summary>
        public static Sprite GetItemSprite(int itemId)
        {
            return UISprites.GetItemSprite(itemId);
        }

        /// <summary>
        /// Gets the game's NPC portrait sprite.
        /// </summary>
        public static Sprite GetNPCPortrait(int npcId)
        {
            return UISprites.GetPortraitSprite(npcId);
        }

        #endregion
    }
}
