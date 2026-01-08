using System;
using System.Collections.Generic;
using UnityEngine;

namespace OpenWood.Core.UI
{
    /// <summary>
    /// Simple UI window for mods to display information.
    /// </summary>
    public class ModWindow
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public Rect WindowRect { get; set; }
        public bool IsVisible { get; set; }
        public bool IsDraggable { get; set; } = true;
        public Action<int> DrawContent { get; set; }

        private int _windowId;

        public ModWindow(string id, string title, Rect rect)
        {
            Id = id;
            Title = title;
            WindowRect = rect;
            _windowId = id.GetHashCode();
        }

        public void Show() => IsVisible = true;
        public void Hide() => IsVisible = false;
        public void Toggle() => IsVisible = !IsVisible;

        internal void Draw()
        {
            if (!IsVisible) return;

            WindowRect = GUILayout.Window(_windowId, WindowRect, DrawWindowContent, Title);
        }

        private void DrawWindowContent(int windowId)
        {
            DrawContent?.Invoke(windowId);

            if (IsDraggable)
            {
                GUI.DragWindow();
            }
        }
    }

    /// <summary>
    /// Manages mod UI rendering.
    /// </summary>
    public static class ModUI
    {
        private static readonly List<ModWindow> _windows = new List<ModWindow>();
        private static readonly List<Action> _guiCallbacks = new List<Action>();
        private static GUIStyle _boxStyle;
        private static GUIStyle _buttonStyle;
        private static GUIStyle _labelStyle;

        /// <summary>
        /// Key binding to toggle the mod menu.
        /// </summary>
        public static KeyCode ToggleKey { get; set; } = KeyCode.F1;

        /// <summary>
        /// Whether the main mod menu is visible.
        /// </summary>
        public static bool IsMenuVisible { get; private set; }

        /// <summary>
        /// Register a window to be drawn.
        /// </summary>
        public static void RegisterWindow(ModWindow window)
        {
            if (!_windows.Contains(window))
            {
                _windows.Add(window);
            }
        }

        /// <summary>
        /// Unregister a window.
        /// </summary>
        public static void UnregisterWindow(ModWindow window)
        {
            _windows.Remove(window);
        }

        /// <summary>
        /// Register a custom GUI callback.
        /// </summary>
        public static void RegisterGUI(Action callback)
        {
            if (!_guiCallbacks.Contains(callback))
            {
                _guiCallbacks.Add(callback);
            }
        }

        /// <summary>
        /// Unregister a custom GUI callback.
        /// </summary>
        public static void UnregisterGUI(Action callback)
        {
            _guiCallbacks.Remove(callback);
        }

        /// <summary>
        /// Show a notification message on screen.
        /// </summary>
        public static void ShowNotification(string message, float duration = 3f)
        {
            // TODO: Implement notification system
            Plugin.Log.LogInfo($"[Notification] {message}");
        }

        internal static void OnGUI()
        {
            // Check for toggle key
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == ToggleKey)
            {
                IsMenuVisible = !IsMenuVisible;
                Event.current.Use();
            }

            // Initialize styles
            InitStyles();

            // Draw all registered windows
            foreach (var window in _windows)
            {
                window.Draw();
            }

            // Call custom GUI callbacks
            foreach (var callback in _guiCallbacks)
            {
                try
                {
                    callback?.Invoke();
                }
                catch (Exception ex)
                {
                    Plugin.Log.LogError($"Error in GUI callback: {ex}");
                }
            }
        }

        private static void InitStyles()
        {
            if (_boxStyle != null) return;

            _boxStyle = new GUIStyle(GUI.skin.box)
            {
                padding = new RectOffset(10, 10, 10, 10)
            };

            _buttonStyle = new GUIStyle(GUI.skin.button)
            {
                padding = new RectOffset(10, 10, 5, 5)
            };

            _labelStyle = new GUIStyle(GUI.skin.label)
            {
                wordWrap = true
            };
        }

        /// <summary>
        /// Create a labeled toggle button.
        /// </summary>
        public static bool Toggle(string label, bool value)
        {
            return GUILayout.Toggle(value, label);
        }

        /// <summary>
        /// Create a labeled slider.
        /// </summary>
        public static float Slider(string label, float value, float min, float max)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{label}: {value:F2}", GUILayout.Width(150));
            value = GUILayout.HorizontalSlider(value, min, max);
            GUILayout.EndHorizontal();
            return value;
        }

        /// <summary>
        /// Create a labeled text field.
        /// </summary>
        public static string TextField(string label, string value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(100));
            value = GUILayout.TextField(value);
            GUILayout.EndHorizontal();
            return value;
        }
    }
}
