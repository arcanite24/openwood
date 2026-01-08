using System;
using System.Reflection;
using UnityEngine;

namespace OpenWood.Core.API
{
    /// <summary>
    /// API for controlling and querying player-related game state.
    /// Provides methods for managing money, experience, movement, and player stats.
    /// </summary>
    /// <example>
    /// <code>
    /// // Add money to the player
    /// PlayerAPI.AddMoney(1000);
    /// 
    /// // Enable speed boost
    /// PlayerAPI.SetSpeedMultiplier(2.0f);
    /// 
    /// // Get current position
    /// var pos = PlayerAPI.Position;
    /// </code>
    /// </example>
    public static class PlayerAPI
    {
        #region Private Fields

        private static bool _initialized;
        private static GameScript _gameScript;
        private static PlayerController _playerController;
        
        // Cached reflection members
        private static MethodInfo _addDewdropsMethod;
        private static FieldInfo _walkSpeedField;
        private static FieldInfo _runSpeedField;
        private static FieldInfo _editSpeedField;
        private static FieldInfo _boostSpeedField;
        
        // Speed hack state
        private static bool _speedHackEnabled;
        private static float _speedMultiplier = 1f;
        private static float _baseWalkSpeed;
        private static float _baseRunSpeed;
        private static float _baseEditSpeed;
        private static float _baseBoostSpeed;
        private static bool _baseSpeedsCached;
        
        // Toggleable cheats
        private static bool _infiniteMoney;
        private static bool _freezeTime;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the player's current money (Dewdrops).
        /// </summary>
        public static int Money
        {
            get => GameScript.dew;
            set
            {
                GameScript.dew = value;
                GameScript.actualDew = value;
            }
        }

        /// <summary>
        /// Gets or sets the player's day experience (fatigue).
        /// Range: 0-100. At 100, the day ends.
        /// </summary>
        public static int DayExperience
        {
            get => GameScript.dayEXP;
            set
            {
                GameScript.dayEXP = Mathf.Clamp(value, 0, 100);
                RefreshDayEXPBar();
            }
        }

        /// <summary>
        /// Gets the player's current position as a Vector2.
        /// </summary>
        public static Vector2 Position => GameScript.playerPos;

        /// <summary>
        /// Gets the direction the player is currently facing.
        /// </summary>
        public static int FacingDirection => GameScript.facingDir;

        /// <summary>
        /// Gets whether the player is currently indoors.
        /// </summary>
        public static bool IsIndoors => GameScript.isIndoor;

        /// <summary>
        /// Gets whether the player is currently sprinting.
        /// </summary>
        public static bool IsSprinting => PlayerController.isSprinting;

        /// <summary>
        /// Gets or sets whether infinite money mode is enabled.
        /// When enabled, money is automatically set to 99999.
        /// </summary>
        public static bool InfiniteMoney
        {
            get => _infiniteMoney;
            set => _infiniteMoney = value;
        }

        /// <summary>
        /// Gets or sets whether time freeze (no fatigue) is enabled.
        /// When enabled, day experience is kept at 0.
        /// </summary>
        public static bool FreezeTime
        {
            get => _freezeTime;
            set => _freezeTime = value;
        }

        /// <summary>
        /// Gets or sets whether speed hack is enabled.
        /// Use <see cref="SpeedMultiplier"/> to control the speed.
        /// </summary>
        public static bool SpeedHackEnabled
        {
            get => _speedHackEnabled;
            set
            {
                bool wasEnabled = _speedHackEnabled;
                _speedHackEnabled = value;
                
                if (!value && wasEnabled)
                {
                    ResetSpeeds();
                }
            }
        }

        /// <summary>
        /// Gets or sets the speed multiplier when speed hack is enabled.
        /// Range: 0.5 to 5.0
        /// </summary>
        public static float SpeedMultiplier
        {
            get => _speedMultiplier;
            set => _speedMultiplier = Mathf.Clamp(value, 0.5f, 5f);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds money (Dewdrops) to the player's balance.
        /// Uses the game's native method when available for proper UI updates.
        /// </summary>
        /// <param name="amount">The amount to add (can be negative).</param>
        public static void AddMoney(int amount)
        {
            EnsureInitialized();
            
            try
            {
                if (_gameScript != null && _addDewdropsMethod != null)
                {
                    _addDewdropsMethod.Invoke(_gameScript, new object[] { amount });
                }
                else
                {
                    GameScript.dew += amount;
                    GameScript.actualDew += amount;
                }
                
                Plugin.Log.LogDebug($"Added {amount} dewdrops (total: {GameScript.dew})");
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Failed to add money: {ex.Message}");
            }
        }

        /// <summary>
        /// Sets the player's money to a specific amount.
        /// </summary>
        /// <param name="amount">The amount to set.</param>
        public static void SetMoney(int amount)
        {
            Money = Math.Max(0, amount);
            Plugin.Log.LogDebug($"Set dewdrops to {amount}");
        }

        /// <summary>
        /// Adds experience to the day's fatigue meter.
        /// </summary>
        /// <param name="amount">The amount to add.</param>
        public static void AddDayExperience(int amount)
        {
            DayExperience = GameScript.dayEXP + amount;
            Plugin.Log.LogDebug($"Added {amount} day EXP (total: {GameScript.dayEXP})");
        }

        /// <summary>
        /// Resets the day experience to 0, removing all fatigue.
        /// </summary>
        public static void ResetDayExperience()
        {
            DayExperience = 0;
            Plugin.Log.LogDebug("Reset day experience to 0");
        }

        /// <summary>
        /// Maxes out the day experience to 100, ending the day.
        /// </summary>
        public static void MaxDayExperience()
        {
            DayExperience = 100;
            Plugin.Log.LogDebug("Maxed day experience to 100");
        }

        /// <summary>
        /// Teleports the player to the specified position.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        public static void Teleport(float x, float y)
        {
            EnsureInitialized();
            
            try
            {
                GameScript.playerPos = new Vector2(x, y);
                PlayerController.pos = new Vector2(x, y);
                
                if (_playerController != null)
                {
                    _playerController.transform.position = new Vector3(x, y, 0);
                }
                
                Plugin.Log.LogDebug($"Teleported player to ({x}, {y})");
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Failed to teleport: {ex.Message}");
            }
        }

        /// <summary>
        /// Teleports the player to the specified position.
        /// </summary>
        /// <param name="position">Target position.</param>
        public static void Teleport(Vector2 position)
        {
            Teleport(position.x, position.y);
        }

        /// <summary>
        /// Sets the player's movement speed multiplier.
        /// This affects walking, running, and editing speeds.
        /// </summary>
        /// <param name="multiplier">Speed multiplier (1.0 = normal speed).</param>
        public static void SetSpeedMultiplier(float multiplier)
        {
            SpeedMultiplier = multiplier;
            SpeedHackEnabled = multiplier != 1.0f;
        }

        /// <summary>
        /// Gets the hobby experience for a specific hobby.
        /// </summary>
        /// <param name="hobbyIndex">Hobby index (0=Woodcutting, 1=Mining, 2=Fishing, 3=Bug Catching, 4=Farming).</param>
        /// <returns>The experience value for the hobby.</returns>
        public static int GetHobbyExperience(int hobbyIndex)
        {
            if (hobbyIndex >= 0 && hobbyIndex < GameScript.hobbyEXP.Length)
            {
                return GameScript.hobbyEXP[hobbyIndex];
            }
            return 0;
        }

        /// <summary>
        /// Sets the hobby experience for a specific hobby.
        /// </summary>
        /// <param name="hobbyIndex">Hobby index (0=Woodcutting, 1=Mining, 2=Fishing, 3=Bug Catching, 4=Farming).</param>
        /// <param name="experience">The experience value to set.</param>
        public static void SetHobbyExperience(int hobbyIndex, int experience)
        {
            if (hobbyIndex >= 0 && hobbyIndex < GameScript.hobbyEXP.Length)
            {
                GameScript.hobbyEXP[hobbyIndex] = experience;
                Plugin.Log.LogDebug($"Set hobby {hobbyIndex} experience to {experience}");
            }
        }

        /// <summary>
        /// Maxes out all hobby experience levels.
        /// </summary>
        public static void MaxAllHobbies()
        {
            for (int i = 0; i < GameScript.hobbyEXP.Length; i++)
            {
                GameScript.hobbyEXP[i] = 999999;
            }
            Plugin.Log.LogInfo("Maxed all hobby levels");
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Initializes the Player API. Called automatically by the plugin.
        /// </summary>
        internal static void Initialize()
        {
            if (_initialized) return;
            _initialized = true;
            
            CacheReferences();
            Plugin.Log.LogDebug("PlayerAPI initialized");
        }

        /// <summary>
        /// Called every frame to apply continuous effects.
        /// </summary>
        internal static void Tick()
        {
            if (!_initialized) return;
            
            // Apply infinite money
            if (_infiniteMoney && GameScript.dew < 99999)
            {
                GameScript.dew = 99999;
                GameScript.actualDew = 99999;
            }
            
            // Apply freeze time
            if (_freezeTime && GameScript.dayEXP > 0)
            {
                GameScript.dayEXP = 0;
                RefreshDayEXPBar();
            }
            
            // Apply speed hack
            if (_speedHackEnabled)
            {
                ApplySpeedHack();
            }
        }

        #endregion

        #region Private Methods

        private static void EnsureInitialized()
        {
            if (!_initialized)
            {
                Initialize();
            }
            
            // Re-cache if references are stale
            if (_gameScript == null)
            {
                CacheReferences();
            }
        }

        private static void CacheReferences()
        {
            try
            {
                _gameScript = UnityEngine.Object.FindObjectOfType<GameScript>();
                _playerController = UnityEngine.Object.FindObjectOfType<PlayerController>();

                if (_gameScript != null)
                {
                    var type = typeof(GameScript);
                    _addDewdropsMethod = type.GetMethod("AddDewdrops", 
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        null, new Type[] { typeof(int) }, null);
                }

                if (_playerController != null)
                {
                    var type = typeof(PlayerController);
                    _walkSpeedField = type.GetField("walkSpeed", BindingFlags.Public | BindingFlags.Instance);
                    _runSpeedField = type.GetField("runSpeed", BindingFlags.Public | BindingFlags.Instance);
                    _editSpeedField = type.GetField("editSpeed", BindingFlags.Public | BindingFlags.Instance);
                    _boostSpeedField = type.GetField("boostSpeed", BindingFlags.Public | BindingFlags.Instance);
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Failed to cache player references: {ex.Message}");
            }
        }

        private static void RefreshDayEXPBar()
        {
            if (_gameScript == null)
            {
                _gameScript = UnityEngine.Object.FindObjectOfType<GameScript>();
            }
            
            if (_gameScript == null) return;
            
            try
            {
                var method = typeof(GameScript).GetMethod("RefreshDayEXPBar", 
                    BindingFlags.NonPublic | BindingFlags.Instance);
                method?.Invoke(_gameScript, null);
            }
            catch { }
        }

        private static void ApplySpeedHack()
        {
            if (_playerController == null)
            {
                _playerController = UnityEngine.Object.FindObjectOfType<PlayerController>();
                if (_playerController == null) return;
            }

            try
            {
                if (_walkSpeedField == null) return;

                // Cache original speeds on first use
                if (!_baseSpeedsCached)
                {
                    _baseWalkSpeed = (float)_walkSpeedField.GetValue(_playerController);
                    _baseRunSpeed = (float)_runSpeedField.GetValue(_playerController);
                    if (_editSpeedField != null) 
                        _baseEditSpeed = (float)_editSpeedField.GetValue(_playerController);
                    if (_boostSpeedField != null) 
                        _baseBoostSpeed = (float)_boostSpeedField.GetValue(_playerController);
                    _baseSpeedsCached = true;
                }

                // Apply modified speeds
                _walkSpeedField.SetValue(_playerController, _baseWalkSpeed * _speedMultiplier);
                _runSpeedField.SetValue(_playerController, _baseRunSpeed * _speedMultiplier);
                _editSpeedField?.SetValue(_playerController, _baseEditSpeed * _speedMultiplier);
                _boostSpeedField?.SetValue(_playerController, _baseBoostSpeed * _speedMultiplier);
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Speed hack failed: {ex.Message}");
            }
        }

        private static void ResetSpeeds()
        {
            if (_playerController == null || !_baseSpeedsCached) return;

            try
            {
                _walkSpeedField?.SetValue(_playerController, _baseWalkSpeed);
                _runSpeedField?.SetValue(_playerController, _baseRunSpeed);
                _editSpeedField?.SetValue(_playerController, _baseEditSpeed);
                _boostSpeedField?.SetValue(_playerController, _baseBoostSpeed);
            }
            catch { }
        }

        #endregion
    }
}
