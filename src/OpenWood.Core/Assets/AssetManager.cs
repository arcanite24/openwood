using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OpenWood.Core.Assets
{
    /// <summary>
    /// Manages loading and caching of mod assets.
    /// </summary>
    public static class AssetManager
    {
        private static bool _initialized;
        private static readonly Dictionary<string, Texture2D> _textureCache = new Dictionary<string, Texture2D>();
        private static readonly Dictionary<string, Sprite> _spriteCache = new Dictionary<string, Sprite>();
        private static readonly Dictionary<string, AudioClip> _audioCache = new Dictionary<string, AudioClip>();
        private static readonly Dictionary<string, AssetBundle> _bundleCache = new Dictionary<string, AssetBundle>();

        /// <summary>
        /// Base path for mod assets.
        /// </summary>
        public static string ModAssetsPath { get; private set; }

        internal static void Initialize()
        {
            if (_initialized) return;

            ModAssetsPath = Path.Combine(BepInEx.Paths.PluginPath, "OpenWood", "Assets");
            
            if (!Directory.Exists(ModAssetsPath))
            {
                Directory.CreateDirectory(ModAssetsPath);
            }

            _initialized = true;
            Plugin.Log.LogDebug($"AssetManager initialized. Assets path: {ModAssetsPath}");
        }

        /// <summary>
        /// Load a texture from the mod assets folder.
        /// </summary>
        /// <param name="relativePath">Path relative to the mod's assets folder</param>
        /// <returns>Loaded texture or null if not found</returns>
        public static Texture2D LoadTexture(string relativePath)
        {
            if (_textureCache.TryGetValue(relativePath, out var cached))
            {
                return cached;
            }

            var fullPath = Path.Combine(ModAssetsPath, relativePath);
            if (!File.Exists(fullPath))
            {
                Plugin.Log.LogWarning($"Texture not found: {fullPath}");
                return null;
            }

            try
            {
                var bytes = File.ReadAllBytes(fullPath);
                var texture = new Texture2D(2, 2);
                texture.LoadImage(bytes);
                texture.filterMode = FilterMode.Point; // Pixel art style
                _textureCache[relativePath] = texture;
                return texture;
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Failed to load texture {relativePath}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Load a sprite from a texture.
        /// </summary>
        public static Sprite LoadSprite(string relativePath, int pixelsPerUnit = 16)
        {
            var cacheKey = $"{relativePath}@{pixelsPerUnit}";
            if (_spriteCache.TryGetValue(cacheKey, out var cached))
            {
                return cached;
            }

            var texture = LoadTexture(relativePath);
            if (texture == null) return null;

            var sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                pixelsPerUnit
            );

            _spriteCache[cacheKey] = sprite;
            return sprite;
        }

        /// <summary>
        /// Load an asset bundle.
        /// </summary>
        public static AssetBundle LoadBundle(string relativePath)
        {
            if (_bundleCache.TryGetValue(relativePath, out var cached))
            {
                return cached;
            }

            var fullPath = Path.Combine(ModAssetsPath, relativePath);
            if (!File.Exists(fullPath))
            {
                Plugin.Log.LogWarning($"Asset bundle not found: {fullPath}");
                return null;
            }

            try
            {
                var bundle = AssetBundle.LoadFromFile(fullPath);
                _bundleCache[relativePath] = bundle;
                return bundle;
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Failed to load bundle {relativePath}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Unload all cached assets.
        /// </summary>
        public static void UnloadAll()
        {
            foreach (var texture in _textureCache.Values)
            {
                UnityEngine.Object.Destroy(texture);
            }
            _textureCache.Clear();

            foreach (var sprite in _spriteCache.Values)
            {
                UnityEngine.Object.Destroy(sprite);
            }
            _spriteCache.Clear();

            foreach (var bundle in _bundleCache.Values)
            {
                bundle.Unload(true);
            }
            _bundleCache.Clear();

            Plugin.Log.LogDebug("All cached assets unloaded");
        }
    }
}
