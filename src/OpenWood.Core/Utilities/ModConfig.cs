using System;
using System.IO;
using System.Text;
using UnityEngine;
using BepInEx;

namespace OpenWood.Core.Utilities
{
    /// <summary>
    /// Configuration utilities for mods.
    /// Provides JSON-based config files as an alternative to BepInEx config.
    /// </summary>
    public class ModConfig<T> where T : class, new()
    {
        private readonly string _configPath;
        private T _data;

        /// <summary>
        /// The configuration data.
        /// </summary>
        public T Data => _data;

        /// <summary>
        /// Create a new config manager for a mod.
        /// </summary>
        /// <param name="modId">Unique identifier for the mod</param>
        /// <param name="fileName">Config file name (without extension)</param>
        public ModConfig(string modId, string fileName = "config")
        {
            var configDir = Path.Combine(Paths.ConfigPath, modId);
            if (!Directory.Exists(configDir))
            {
                Directory.CreateDirectory(configDir);
            }

            _configPath = Path.Combine(configDir, $"{fileName}.json");
            Load();
        }

        /// <summary>
        /// Load configuration from disk.
        /// </summary>
        public void Load()
        {
            try
            {
                if (File.Exists(_configPath))
                {
                    var json = File.ReadAllText(_configPath, Encoding.UTF8);
                    _data = JsonUtility.FromJson<T>(json) ?? new T();
                }
                else
                {
                    _data = new T();
                    Save(); // Create default config file
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Failed to load config from {_configPath}: {ex.Message}");
                _data = new T();
            }
        }

        /// <summary>
        /// Save configuration to disk.
        /// </summary>
        public void Save()
        {
            try
            {
                var json = JsonUtility.ToJson(_data, true);
                File.WriteAllText(_configPath, json, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Failed to save config to {_configPath}: {ex.Message}");
            }
        }

        /// <summary>
        /// Reset configuration to defaults.
        /// </summary>
        public void Reset()
        {
            _data = new T();
            Save();
        }
    }
}
