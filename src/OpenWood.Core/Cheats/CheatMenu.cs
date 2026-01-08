using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace OpenWood.Core.Cheats
{
    /// <summary>
    /// Comprehensive cheat menu for Littlewood.
    /// Styled to match the game's UI as much as possible.
    /// </summary>
    public class CheatMenu : MonoBehaviour
    {
        public static CheatMenu Instance { get; private set; }

        // Menu state
        private bool _isVisible = false;
        private int _currentTab = 0;
        private Vector2 _scrollPosition = Vector2.zero;
        
        // Window settings
        private Rect _windowRect;
        private readonly int _windowId = "OpenWoodCheatMenu".GetHashCode();
        
        // Styles
        private GUIStyle _windowStyle;
        private GUIStyle _tabButtonStyle;
        private GUIStyle _tabButtonActiveStyle;
        private GUIStyle _sectionStyle;
        private GUIStyle _headerStyle;
        private GUIStyle _buttonStyle;
        private GUIStyle _smallButtonStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _valueStyle;
        private GUIStyle _textFieldStyle;
        private GUIStyle _sliderStyle;
        private GUIStyle _toggleStyle;
        private bool _stylesInitialized = false;

        // Tab definitions
        private readonly string[] _tabs = { "Player", "Items", "Time", "NPCs", "World", "Teleport", "Debug" };

        // Input fields
        private string _moneyInput = "1000";
        private string _expInput = "50";
        private string _itemIdInput = "1";
        private string _itemCountInput = "1";
        private string _npcIdInput = "1";
        private string _heartInput = "5";
        private string _dayInput = "1";
        private string _seasonInput = "0";
        private string _yearInput = "1";

        // Cheat state
        private bool _godMode = false;
        private bool _infiniteMoney = false;
        private bool _noClip = false;
        private bool _speedHack = false;
        private bool _speedHackPrevious = false;
        private float _speedMultiplier = 1f;
        private bool _instantActions = false;
        private bool _freezeTime = false;

        // Cursor state
        private bool _savedCursorVisible;
        private CursorLockMode _savedCursorLockMode;

        // Cached references
        private GameScript _gameScript;
        private PlayerController _playerController;
        private FieldInfo _inventoryField;
        private MethodInfo _addItemMethod;
        private MethodInfo _addDewdropsMethod;

        public static KeyCode ToggleKey { get; set; } = KeyCode.F3;

        public static void Initialize()
        {
            if (Instance != null) return;

            var go = new GameObject("OpenWood_CheatMenu");
            Instance = go.AddComponent<CheatMenu>();
            DontDestroyOnLoad(go);

            Plugin.Log.LogInfo("Cheat Menu initialized. Press F3 to toggle.");
        }

        private void Awake()
        {
            // Center the window
            float width = 500f;
            float height = 450f;
            _windowRect = new Rect(
                (Screen.width - width) / 2,
                (Screen.height - height) / 2,
                width,
                height
            );
        }

        private void Update()
        {
            // Toggle menu
            if (Input.GetKeyDown(ToggleKey))
            {
                _isVisible = !_isVisible;
                
                if (_isVisible)
                {
                    // Save cursor state and show cursor
                    _savedCursorVisible = Cursor.visible;
                    _savedCursorLockMode = Cursor.lockState;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    
                    CacheReferences();
                }
                else
                {
                    // Restore cursor state
                    Cursor.visible = _savedCursorVisible;
                    Cursor.lockState = _savedCursorLockMode;
                }
            }

            // Keyboard navigation when menu is open
            if (_isVisible)
            {
                HandleKeyboardNavigation();
            }

            // Apply continuous cheats
            ApplyContinuousCheats();
        }

        private void HandleKeyboardNavigation()
        {
            // Tab switching with Q/E or Left/Right brackets
            if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.LeftBracket))
            {
                _currentTab = (_currentTab - 1 + _tabs.Length) % _tabs.Length;
            }
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.RightBracket))
            {
                _currentTab = (_currentTab + 1) % _tabs.Length;
            }

            // Scroll with arrow keys
            if (Input.GetKey(KeyCode.UpArrow))
            {
                _scrollPosition.y -= 200f * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                _scrollPosition.y += 200f * Time.deltaTime;
            }

            // Quick action keys per tab
            HandleTabHotkeys();
        }

        private void HandleTabHotkeys()
        {
            switch (_currentTab)
            {
                case 0: // Player tab
                    if (Input.GetKeyDown(KeyCode.Alpha1)) AddMoney(1000);
                    if (Input.GetKeyDown(KeyCode.Alpha2)) AddMoney(10000);
                    if (Input.GetKeyDown(KeyCode.Alpha3)) { GameScript.dayEXP = 0; RefreshDayEXPBar(); }
                    break;
                case 1: // Items tab
                    if (Input.GetKeyDown(KeyCode.Alpha1)) { for (int i = 0; i < 10; i++) AddItem(40); } // Wood
                    if (Input.GetKeyDown(KeyCode.Alpha2)) { for (int i = 0; i < 10; i++) AddItem(80); } // Stone
                    if (Input.GetKeyDown(KeyCode.Alpha3)) { for (int i = 0; i < 10; i++) AddItem(60); } // Wooden Plank
                    break;
                case 2: // Time tab
                    if (Input.GetKeyDown(KeyCode.Alpha1)) AdvanceDay();
                    if (Input.GetKeyDown(KeyCode.Alpha2)) ToggleRain();
                    break;
                case 3: // NPCs tab
                    if (Input.GetKeyDown(KeyCode.Alpha1)) { for (int i = 1; i <= 20; i++) SetNPCFriendship(i, 10); }
                    break;
            }
        }

        private void CacheReferences()
        {
            try
            {
                _gameScript = FindObjectOfType<GameScript>();
                _playerController = FindObjectOfType<PlayerController>();

                if (_gameScript != null)
                {
                    var type = typeof(GameScript);
                    _inventoryField = type.GetField("inventory", BindingFlags.NonPublic | BindingFlags.Instance);
                    // Use specific parameter types to avoid ambiguous match
                    _addItemMethod = type.GetMethod("AddItem", BindingFlags.NonPublic | BindingFlags.Instance, 
                        null, new Type[] { typeof(int), typeof(int) }, null);
                    _addDewdropsMethod = type.GetMethod("AddDewdrops", BindingFlags.NonPublic | BindingFlags.Instance,
                        null, new Type[] { typeof(int) }, null);
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Failed to cache references: {ex.Message}");
            }
        }

        private void ApplyContinuousCheats()
        {
            if (_gameScript == null) return;

            // Infinite money
            if (_infiniteMoney && GameScript.dew < 99999)
            {
                GameScript.dew = 99999;
                GameScript.actualDew = 99999;
            }

            // Speed hack - must modify INSTANCE fields since static speed gets overwritten every frame
            if (_speedHack && _playerController != null)
            {
                ApplySpeedHack();
            }
            else if (!_speedHack && _speedHackPrevious && _playerController != null)
            {
                // Just turned off - reset to original speeds
                ResetSpeeds();
            }
            _speedHackPrevious = _speedHack;

            // Freeze time (prevent day EXP from reaching max)
            if (_freezeTime && GameScript.dayEXP > 0)
            {
                GameScript.dayEXP = 0;
                RefreshDayEXPBar();
            }
        }

        private float _baseWalkSpeed = 1.6f;
        private float _baseRunSpeed = 2f;
        private float _baseEditSpeed = 1.6f;
        private float _baseBoostSpeed = 2f;
        private bool _originalSpeedsCached = false;

        private void ApplySpeedHack()
        {
            try
            {
                var type = typeof(PlayerController);
                
                // Get the instance fields (not static speed)
                var walkSpeedField = type.GetField("walkSpeed", BindingFlags.Public | BindingFlags.Instance);
                var runSpeedField = type.GetField("runSpeed", BindingFlags.Public | BindingFlags.Instance);
                var editSpeedField = type.GetField("editSpeed", BindingFlags.Public | BindingFlags.Instance);
                var boostSpeedField = type.GetField("boostSpeed", BindingFlags.Public | BindingFlags.Instance);

                if (walkSpeedField == null || runSpeedField == null) return;

                // Cache original speeds on first use
                if (!_originalSpeedsCached)
                {
                    _baseWalkSpeed = (float)walkSpeedField.GetValue(_playerController);
                    _baseRunSpeed = (float)runSpeedField.GetValue(_playerController);
                    if (editSpeedField != null) _baseEditSpeed = (float)editSpeedField.GetValue(_playerController);
                    if (boostSpeedField != null) _baseBoostSpeed = (float)boostSpeedField.GetValue(_playerController);
                    _originalSpeedsCached = true;
                }

                // Set modified speeds based on multiplier
                walkSpeedField.SetValue(_playerController, _baseWalkSpeed * _speedMultiplier);
                runSpeedField.SetValue(_playerController, _baseRunSpeed * _speedMultiplier);
                if (editSpeedField != null) editSpeedField.SetValue(_playerController, _baseEditSpeed * _speedMultiplier);
                if (boostSpeedField != null) boostSpeedField.SetValue(_playerController, _baseBoostSpeed * _speedMultiplier);
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Speed hack failed: {ex.Message}");
            }
        }

        private void ResetSpeeds()
        {
            if (_playerController == null || !_originalSpeedsCached) return;
            
            try
            {
                var type = typeof(PlayerController);
                type.GetField("walkSpeed", BindingFlags.Public | BindingFlags.Instance)?.SetValue(_playerController, _baseWalkSpeed);
                type.GetField("runSpeed", BindingFlags.Public | BindingFlags.Instance)?.SetValue(_playerController, _baseRunSpeed);
                type.GetField("editSpeed", BindingFlags.Public | BindingFlags.Instance)?.SetValue(_playerController, _baseEditSpeed);
                type.GetField("boostSpeed", BindingFlags.Public | BindingFlags.Instance)?.SetValue(_playerController, _baseBoostSpeed);
            }
            catch { }
        }

        private void RefreshDayEXPBar()
        {
            if (_gameScript == null) return;
            
            try
            {
                var method = typeof(GameScript).GetMethod("RefreshDayEXPBar", BindingFlags.NonPublic | BindingFlags.Instance);
                method?.Invoke(_gameScript, null);
            }
            catch { }
        }

        private void OnGUI()
        {
            if (!_isVisible) return;

            InitStyles();

            // Draw window
            _windowRect = GUI.Window(_windowId, _windowRect, DrawWindow, "", _windowStyle);
        }

        private void InitStyles()
        {
            if (_stylesInitialized) return;

            // Colors matching Littlewood's palette
            var bgColor = new Color(0.15f, 0.12f, 0.18f, 0.95f);
            var headerColor = new Color(0.95f, 0.85f, 0.65f, 1f);
            var buttonColor = new Color(0.35f, 0.28f, 0.42f, 1f);
            var buttonHoverColor = new Color(0.45f, 0.38f, 0.52f, 1f);
            var buttonActiveColor = new Color(0.55f, 0.75f, 0.55f, 1f);
            var textColor = new Color(1f, 0.98f, 0.9f, 1f);

            // Create textures
            var bgTex = MakeTexture(2, 2, bgColor);
            var buttonTex = MakeTexture(2, 2, buttonColor);
            var buttonHoverTex = MakeTexture(2, 2, buttonHoverColor);
            var buttonActiveTex = MakeTexture(2, 2, buttonActiveColor);
            var sectionTex = MakeTexture(2, 2, new Color(0.2f, 0.17f, 0.25f, 0.8f));

            // Window style
            _windowStyle = new GUIStyle(GUI.skin.window)
            {
                normal = { background = bgTex, textColor = textColor },
                padding = new RectOffset(10, 10, 10, 10),
                border = new RectOffset(4, 4, 4, 4)
            };

            // Header style
            _headerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = headerColor }
            };

            // Tab button styles
            _tabButtonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                normal = { background = buttonTex, textColor = textColor },
                hover = { background = buttonHoverTex, textColor = textColor },
                active = { background = buttonActiveTex, textColor = Color.white },
                padding = new RectOffset(8, 8, 6, 6),
                margin = new RectOffset(2, 2, 2, 2)
            };

            _tabButtonActiveStyle = new GUIStyle(_tabButtonStyle)
            {
                normal = { background = buttonActiveTex, textColor = Color.white }
            };

            // Section style
            _sectionStyle = new GUIStyle(GUI.skin.box)
            {
                normal = { background = sectionTex, textColor = textColor },
                padding = new RectOffset(10, 10, 8, 8),
                margin = new RectOffset(0, 0, 5, 5)
            };

            // Button style
            _buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 13,
                normal = { background = buttonTex, textColor = textColor },
                hover = { background = buttonHoverTex, textColor = textColor },
                active = { background = buttonActiveTex, textColor = Color.white },
                padding = new RectOffset(12, 12, 8, 8),
                margin = new RectOffset(2, 2, 2, 2)
            };

            _smallButtonStyle = new GUIStyle(_buttonStyle)
            {
                fontSize = 11,
                padding = new RectOffset(6, 6, 4, 4)
            };

            // Label style
            _labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                normal = { textColor = textColor }
            };

            // Value style (for displaying values)
            _valueStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                normal = { textColor = headerColor }
            };

            // Text field style
            _textFieldStyle = new GUIStyle(GUI.skin.textField)
            {
                fontSize = 13,
                normal = { textColor = textColor },
                padding = new RectOffset(6, 6, 4, 4)
            };

            // Slider style
            _sliderStyle = new GUIStyle(GUI.skin.horizontalSlider);

            // Toggle style
            _toggleStyle = new GUIStyle(GUI.skin.toggle)
            {
                fontSize = 13,
                normal = { textColor = textColor },
                onNormal = { textColor = headerColor }
            };

            _stylesInitialized = true;
        }

        private Texture2D MakeTexture(int width, int height, Color color)
        {
            var pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = color;

            var tex = new Texture2D(width, height);
            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }

        private void DrawWindow(int windowId)
        {
            // Title bar
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("OpenWood Cheat Menu", _headerStyle);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("X", _smallButtonStyle, GUILayout.Width(25)))
            {
                _isVisible = false;
            }
            GUILayout.EndHorizontal();

            // Keyboard hints
            GUILayout.Label("Navigate: Q/E or [/] = Switch Tabs | ↑↓ = Scroll | 1-3 = Quick Actions", _labelStyle);

            GUILayout.Space(5);

            // Tab bar
            GUILayout.BeginHorizontal();
            for (int i = 0; i < _tabs.Length; i++)
            {
                var style = (i == _currentTab) ? _tabButtonActiveStyle : _tabButtonStyle;
                if (GUILayout.Button(_tabs[i], style))
                {
                    _currentTab = i;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            // Content area with scroll
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));

            switch (_currentTab)
            {
                case 0: DrawPlayerTab(); break;
                case 1: DrawItemsTab(); break;
                case 2: DrawTimeTab(); break;
                case 3: DrawNPCsTab(); break;
                case 4: DrawWorldTab(); break;
                case 5: DrawTeleportTab(); break;
                case 6: DrawDebugTab(); break;
            }

            GUILayout.EndScrollView();

            // Status bar
            GUILayout.Space(5);
            DrawStatusBar();

            // Make window draggable
            GUI.DragWindow(new Rect(0, 0, _windowRect.width, 30));
        }

        private void DrawStatusBar()
        {
            GUILayout.BeginHorizontal(_sectionStyle);
            GUILayout.Label($"Day: {GameScript.day} | Season: {GameScript.season} | Year: {GameScript.year}", _labelStyle);
            GUILayout.FlexibleSpace();
            GUILayout.Label($"Dew: {GameScript.dew}", _valueStyle);
            GUILayout.EndHorizontal();
        }

        #region Player Tab

        private void DrawPlayerTab()
        {
            // Money section
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("💰 Money", _headerStyle);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Current:", _labelStyle, GUILayout.Width(80));
            GUILayout.Label(GameScript.dew.ToString(), _valueStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _moneyInput = GUILayout.TextField(_moneyInput, _textFieldStyle, GUILayout.Width(100));
            if (GUILayout.Button("Add", _buttonStyle, GUILayout.Width(60)))
            {
                if (int.TryParse(_moneyInput, out int amount))
                    AddMoney(amount);
            }
            if (GUILayout.Button("Set", _buttonStyle, GUILayout.Width(60)))
            {
                if (int.TryParse(_moneyInput, out int amount))
                    SetMoney(amount);
            }
            if (GUILayout.Button("+1000", _smallButtonStyle)) AddMoney(1000);
            if (GUILayout.Button("+10000", _smallButtonStyle)) AddMoney(10000);
            GUILayout.EndHorizontal();

            _infiniteMoney = GUILayout.Toggle(_infiniteMoney, "  Infinite Money", _toggleStyle);
            GUILayout.EndVertical();

            GUILayout.Space(5);

            // Experience section
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("⭐ Experience", _headerStyle);

            GUILayout.BeginHorizontal();
            GUILayout.Label($"Day EXP: {GameScript.dayEXP} / 100", _labelStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _expInput = GUILayout.TextField(_expInput, _textFieldStyle, GUILayout.Width(100));
            if (GUILayout.Button("Add EXP", _buttonStyle, GUILayout.Width(80)))
            {
                if (int.TryParse(_expInput, out int amount))
                    AddDayEXP(amount);
            }
            if (GUILayout.Button("Max EXP", _buttonStyle, GUILayout.Width(80)))
            {
                GameScript.dayEXP = 100;
                RefreshDayEXPBar();
            }
            if (GUILayout.Button("Reset EXP", _buttonStyle, GUILayout.Width(80)))
            {
                GameScript.dayEXP = 0;
                RefreshDayEXPBar();
            }
            GUILayout.EndHorizontal();

            _freezeTime = GUILayout.Toggle(_freezeTime, "  Freeze Day (No Fatigue)", _toggleStyle);
            GUILayout.EndVertical();

            GUILayout.Space(5);

            // Movement section
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("🏃 Movement", _headerStyle);

            _speedHack = GUILayout.Toggle(_speedHack, "  Speed Hack", _toggleStyle);
            
            if (_speedHack)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label($"Speed: {_speedMultiplier:F1}x", _labelStyle, GUILayout.Width(100));
                _speedMultiplier = GUILayout.HorizontalSlider(_speedMultiplier, 0.5f, 5f);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("1x", _smallButtonStyle)) _speedMultiplier = 1f;
                if (GUILayout.Button("1.5x", _smallButtonStyle)) _speedMultiplier = 1.5f;
                if (GUILayout.Button("2x", _smallButtonStyle)) _speedMultiplier = 2f;
                if (GUILayout.Button("3x", _smallButtonStyle)) _speedMultiplier = 3f;
                GUILayout.EndHorizontal();
            }

            _noClip = GUILayout.Toggle(_noClip, "  No Clip (Not Implemented)", _toggleStyle);
            GUILayout.EndVertical();
        }

        #endregion

        #region Items Tab

        private void DrawItemsTab()
        {
            // Add items section
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("📦 Add Items", _headerStyle);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Item ID:", _labelStyle, GUILayout.Width(60));
            _itemIdInput = GUILayout.TextField(_itemIdInput, _textFieldStyle, GUILayout.Width(60));
            GUILayout.Label("Count:", _labelStyle, GUILayout.Width(50));
            _itemCountInput = GUILayout.TextField(_itemCountInput, _textFieldStyle, GUILayout.Width(40));
            if (GUILayout.Button("Add Item", _buttonStyle))
            {
                if (int.TryParse(_itemIdInput, out int id) && int.TryParse(_itemCountInput, out int count))
                {
                    for (int i = 0; i < count; i++)
                        AddItem(id);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(5);

            // Quick add buttons - WOOD (40-44)
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("🪵 Wood & Planks", _headerStyle);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Wood (40)", _smallButtonStyle)) AddItem(40);
            if (GUILayout.Button("Magicwood (41)", _smallButtonStyle)) AddItem(41);
            if (GUILayout.Button("Goldenwood (42)", _smallButtonStyle)) AddItem(42);
            if (GUILayout.Button("Almwood (43)", _smallButtonStyle)) AddItem(43);
            if (GUILayout.Button("Leifwood (44)", _smallButtonStyle)) AddItem(44);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Wooden Plank (60)", _smallButtonStyle)) AddItem(60);
            if (GUILayout.Button("Fancy Plank (61)", _smallButtonStyle)) AddItem(61);
            if (GUILayout.Button("Perfect Plank (62)", _smallButtonStyle)) AddItem(62);
            if (GUILayout.Button("Dusk Plank (63)", _smallButtonStyle)) AddItem(63);
            if (GUILayout.Button("Dawn Plank (64)", _smallButtonStyle)) AddItem(64);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(5);

            // STONE/ORE (80-84)
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("�ite Stone & Bricks", _headerStyle);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Stone (80)", _smallButtonStyle)) AddItem(80);
            if (GUILayout.Button("Magicite (81)", _smallButtonStyle)) AddItem(81);
            if (GUILayout.Button("Orichalcum (82)", _smallButtonStyle)) AddItem(82);
            if (GUILayout.Button("Wyvernite (83)", _smallButtonStyle)) AddItem(83);
            if (GUILayout.Button("Dragalium (84)", _smallButtonStyle)) AddItem(84);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Plain Brick (100)", _smallButtonStyle)) AddItem(100);
            if (GUILayout.Button("Fancy Brick (101)", _smallButtonStyle)) AddItem(101);
            if (GUILayout.Button("Perfect Brick (102)", _smallButtonStyle)) AddItem(102);
            if (GUILayout.Button("Moonlight Orb (103)", _smallButtonStyle)) AddItem(103);
            if (GUILayout.Button("Sunlight Orb (104)", _smallButtonStyle)) AddItem(104);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(5);

            // FRUIT (120-132)
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("🍎 Fruit", _headerStyle);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Slimeapple (120)", _smallButtonStyle)) AddItem(120);
            if (GUILayout.Button("Plumberry (121)", _smallButtonStyle)) AddItem(121);
            if (GUILayout.Button("Peachot (122)", _smallButtonStyle)) AddItem(122);
            if (GUILayout.Button("Goldenbell (123)", _smallButtonStyle)) AddItem(123);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Sourpuck (124)", _smallButtonStyle)) AddItem(124);
            if (GUILayout.Button("Goop Melon (125)", _smallButtonStyle)) AddItem(125);
            if (GUILayout.Button("Papayapa (126)", _smallButtonStyle)) AddItem(126);
            if (GUILayout.Button("Crescent Moon (127)", _smallButtonStyle)) AddItem(127);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(5);

            // VEGETABLES (140-152)
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("🥕 Vegetables", _headerStyle);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Carrot (140)", _smallButtonStyle)) AddItem(140);
            if (GUILayout.Button("Cabbage (141)", _smallButtonStyle)) AddItem(141);
            if (GUILayout.Button("Potato (142)", _smallButtonStyle)) AddItem(142);
            if (GUILayout.Button("Corn (143)", _smallButtonStyle)) AddItem(143);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Motato (144)", _smallButtonStyle)) AddItem(144);
            if (GUILayout.Button("Eggplant (145)", _smallButtonStyle)) AddItem(145);
            if (GUILayout.Button("Onion (146)", _smallButtonStyle)) AddItem(146);
            if (GUILayout.Button("Golden Carrot (149)", _smallButtonStyle)) AddItem(149);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(5);

            // FISH (160-189)
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("🐟 Fish", _headerStyle);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Minnow (160)", _smallButtonStyle)) AddItem(160);
            if (GUILayout.Button("Trout (161)", _smallButtonStyle)) AddItem(161);
            if (GUILayout.Button("Fire Carp (162)", _smallButtonStyle)) AddItem(162);
            if (GUILayout.Button("Golden Tuna (176)", _smallButtonStyle)) AddItem(176);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(5);

            // BUGS (200-229)
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("🦋 Bugs", _headerStyle);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Flutterfly (200)", _smallButtonStyle)) AddItem(200);
            if (GUILayout.Button("Monarch (202)", _smallButtonStyle)) AddItem(202);
            if (GUILayout.Button("Ladybug (205)", _smallButtonStyle)) AddItem(205);
            if (GUILayout.Button("Golden Titan (214)", _smallButtonStyle)) AddItem(214);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(5);

            // MISC (240-263)
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("📦 Miscellaneous", _headerStyle);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Egg (241)", _smallButtonStyle)) AddItem(241);
            if (GUILayout.Button("Golden Egg (242)", _smallButtonStyle)) AddItem(242);
            if (GUILayout.Button("Milk (243)", _smallButtonStyle)) AddItem(243);
            if (GUILayout.Button("Golden Milk (244)", _smallButtonStyle)) AddItem(244);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Fleece (247)", _smallButtonStyle)) AddItem(247);
            if (GUILayout.Button("Golden Fleece (248)", _smallButtonStyle)) AddItem(248);
            if (GUILayout.Button("Dust (296)", _smallButtonStyle)) AddItem(296);
            if (GUILayout.Button("Honeycomb (263)", _smallButtonStyle)) AddItem(263);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(5);

            // Tools & Special
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("🔧 Bulk Actions", _headerStyle);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add 10 of Each Wood", _buttonStyle))
            {
                for (int i = 0; i < 10; i++)
                {
                    AddItem(40); AddItem(41); AddItem(42); AddItem(43); AddItem(44);
                }
            }
            if (GUILayout.Button("Add 10 of Each Stone", _buttonStyle))
            {
                for (int i = 0; i < 10; i++)
                {
                    AddItem(80); AddItem(81); AddItem(82); AddItem(83); AddItem(84);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add 10 of Each Plank", _buttonStyle))
            {
                for (int i = 0; i < 10; i++)
                {
                    AddItem(60); AddItem(61); AddItem(62); AddItem(63); AddItem(64);
                }
            }
            if (GUILayout.Button("Add 10 of Each Brick", _buttonStyle))
            {
                for (int i = 0; i < 10; i++)
                {
                    AddItem(100); AddItem(101); AddItem(102); AddItem(103); AddItem(104);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        #endregion

        #region Time Tab

        private void DrawTimeTab()
        {
            // Current time display
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("📅 Current Date", _headerStyle);

            GUILayout.BeginHorizontal();
            GUILayout.Label($"Day: {GameScript.day}", _valueStyle);
            GUILayout.Label($"Week: {GameScript.week}", _valueStyle);
            GUILayout.Label($"Season: {GetSeasonName(GameScript.season)}", _valueStyle);
            GUILayout.Label($"Year: {GameScript.year}", _valueStyle);
            GUILayout.EndHorizontal();

            GUILayout.Label($"Days Played: {GameScript.daysPlayed}", _labelStyle);
            GUILayout.EndVertical();

            GUILayout.Space(5);

            // Set time section
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("⏰ Set Date", _headerStyle);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Day (1-7):", _labelStyle, GUILayout.Width(70));
            _dayInput = GUILayout.TextField(_dayInput, _textFieldStyle, GUILayout.Width(40));
            GUILayout.Label("Season (0-3):", _labelStyle, GUILayout.Width(90));
            _seasonInput = GUILayout.TextField(_seasonInput, _textFieldStyle, GUILayout.Width(40));
            GUILayout.Label("Year:", _labelStyle, GUILayout.Width(40));
            _yearInput = GUILayout.TextField(_yearInput, _textFieldStyle, GUILayout.Width(40));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Apply Date", _buttonStyle))
            {
                if (int.TryParse(_dayInput, out int day))
                    GameScript.day = Mathf.Clamp(day, 1, 7);
                if (int.TryParse(_seasonInput, out int season))
                    GameScript.season = Mathf.Clamp(season, 0, 3);
                if (int.TryParse(_yearInput, out int year))
                    GameScript.year = Mathf.Max(year, 1);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(5);

            // Quick time controls
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("⏭️ Quick Controls", _headerStyle);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Next Day", _buttonStyle)) AdvanceDay();
            if (GUILayout.Button("Next Week", _buttonStyle)) { for (int i = 0; i < 7; i++) AdvanceDay(); }
            if (GUILayout.Button("Next Season", _buttonStyle)) { for (int i = 0; i < 28; i++) AdvanceDay(); }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("🌸 Spring", _buttonStyle)) GameScript.season = 0;
            if (GUILayout.Button("☀️ Summer", _buttonStyle)) GameScript.season = 1;
            if (GUILayout.Button("🍂 Autumn", _buttonStyle)) GameScript.season = 2;
            if (GUILayout.Button("❄️ Winter", _buttonStyle)) GameScript.season = 3;
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(5);

            // Weather
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("🌧️ Weather", _headerStyle);

            GUILayout.BeginHorizontal();
            GUILayout.Label($"Raining: {(GameScript.raining ? "Yes" : "No")}", _valueStyle);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(GameScript.raining ? "Stop Rain" : "Start Rain", _buttonStyle))
            {
                ToggleRain();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        #endregion

        #region NPCs Tab

        private void DrawNPCsTab()
        {
            // NPC selection
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("👥 NPC Relationships", _headerStyle);

            GUILayout.BeginHorizontal();
            GUILayout.Label("NPC ID:", _labelStyle, GUILayout.Width(60));
            _npcIdInput = GUILayout.TextField(_npcIdInput, _textFieldStyle, GUILayout.Width(40));
            GUILayout.Label("Hearts:", _labelStyle, GUILayout.Width(50));
            _heartInput = GUILayout.TextField(_heartInput, _textFieldStyle, GUILayout.Width(40));

            if (GUILayout.Button("Set Level", _buttonStyle))
            {
                if (int.TryParse(_npcIdInput, out int npcId) && int.TryParse(_heartInput, out int hearts))
                {
                    SetNPCFriendship(npcId, hearts);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(5);

            // NPC quick buttons
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("❤️ Townsfolk Quick Set", _headerStyle);

            string[] npcs = { "Willow", "Dalton", "Dudley", "Laura", "Bubsy", "Ash", "Lilith", "Zana" };
            for (int row = 0; row < 2; row++)
            {
                GUILayout.BeginHorizontal();
                for (int col = 0; col < 4; col++)
                {
                    int npcId = row * 4 + col + 1;
                    if (npcId <= npcs.Length)
                    {
                        int level = GameScript.townsfolkLevel[npcId];
                        if (GUILayout.Button($"{npcs[npcId-1]}\n♥ {level}", _smallButtonStyle, GUILayout.Height(40)))
                        {
                            SetNPCFriendship(npcId, 10);
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Click NPC to max hearts. More NPCs: IDs 9-15", _labelStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(5);

            // Bulk actions
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("⚡ Bulk Actions", _headerStyle);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Max All Friendships", _buttonStyle))
            {
                for (int i = 1; i <= 20; i++)
                    SetNPCFriendship(i, 10);
            }
            if (GUILayout.Button("Reset All Friendships", _buttonStyle))
            {
                for (int i = 1; i <= 20; i++)
                    SetNPCFriendship(i, 0);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Max All Romance", _buttonStyle))
            {
                for (int i = 1; i <= 20; i++)
                {
                    if (GameScript.townsfolkRomanceLvl != null && i < GameScript.townsfolkRomanceLvl.Length)
                        GameScript.townsfolkRomanceLvl[i] = 10;
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        #endregion

        #region World Tab

        private void DrawWorldTab()
        {
            // Location info
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("🗺️ Location Info", _headerStyle);

            GUILayout.Label($"Indoor: {GameScript.isIndoor}", _labelStyle);
            GUILayout.Label($"Player Position: {GameScript.playerPos}", _labelStyle);
            GUILayout.Label($"Facing: {GameScript.facingDir}", _labelStyle);
            GUILayout.EndVertical();

            GUILayout.Space(5);

            // Unlock features
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("🔓 Unlocks", _headerStyle);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Unlock All Tools", _buttonStyle))
            {
                UnlockAllTools();
            }
            if (GUILayout.Button("Unlock All Recipes", _buttonStyle))
            {
                UnlockAllRecipes();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Unlock All Museum Items", _buttonStyle))
            {
                UnlockMuseum();
            }
            if (GUILayout.Button("Discover All Items", _buttonStyle))
            {
                DiscoverAllItems();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(5);

            // Building
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("🏗️ Building", _headerStyle);

            GUILayout.BeginHorizontal();
            GUILayout.Label($"Build Mode: {GameScript.building}", _valueStyle);
            GUILayout.Label($"Edit Mode: {GameScript.inEditMode}", _valueStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(5);

            // Hobbies
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("🎯 Hobbies", _headerStyle);

            if (GUILayout.Button("Max All Hobby Levels", _buttonStyle))
            {
                MaxAllHobbies();
            }

            string[] hobbies = { "Woodcutting", "Mining", "Fishing", "Bug Catching", "Farming" };
            for (int i = 0; i < Mathf.Min(hobbies.Length, GameScript.hobbyEXP.Length); i++)
            {
                GUILayout.Label($"{hobbies[i]}: {GameScript.hobbyEXP[i]}", _labelStyle);
            }
            GUILayout.EndVertical();
        }

        #endregion

        #region Teleport Tab

        private void DrawTeleportTab()
        {
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("📍 Quick Teleport", _headerStyle);

            GUILayout.Label("Common Locations:", _labelStyle);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Town Center (0, 0)", _buttonStyle))
            {
                TeleportPlayer(0, 0);
            }
            if (GUILayout.Button("Farm Area (-3, -3)", _buttonStyle))
            {
                TeleportPlayer(-3, -3);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Forest Entrance", _buttonStyle))
            {
                TeleportPlayer(5, 5);
            }
            if (GUILayout.Button("Beach", _buttonStyle))
            {
                TeleportPlayer(-5, 0);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(5);

            // Custom teleport
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("🎯 Custom Teleport", _headerStyle);

            GUILayout.Label($"Current: ({GameScript.playerPos.x:F2}, {GameScript.playerPos.y:F2})", _valueStyle);

            GUILayout.BeginHorizontal();
            GUILayout.Label("X:", _labelStyle, GUILayout.Width(20));
            string xInput = GUILayout.TextField(GameScript.playerPos.x.ToString("F1"), _textFieldStyle, GUILayout.Width(60));
            GUILayout.Label("Y:", _labelStyle, GUILayout.Width(20));
            string yInput = GUILayout.TextField(GameScript.playerPos.y.ToString("F1"), _textFieldStyle, GUILayout.Width(60));
            
            if (GUILayout.Button("Teleport", _buttonStyle))
            {
                if (float.TryParse(xInput, out float x) && float.TryParse(yInput, out float y))
                {
                    TeleportPlayer(x, y);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        #endregion

        #region Debug Tab

        private void DrawDebugTab()
        {
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("🔧 Debug Options", _headerStyle);

            GUILayout.BeginHorizontal();
            GUILayout.Label($"Menu Open: {GameScript.menuOpen}", _valueStyle);
            GUILayout.Label($"Talking: {GameScript.talking}", _valueStyle);
            GUILayout.Label($"Interacting: {GameScript.interacting}", _valueStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label($"Fishing: {GameScript.fishing}", _valueStyle);
            GUILayout.Label($"Building: {GameScript.building}", _valueStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(5);

            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("🎮 Game State", _headerStyle);

            if (GUILayout.Button("Force Close All Menus", _buttonStyle))
            {
                GameScript.menuOpen = false;
                GameScript.talking = false;
                GameScript.interacting = false;
            }

            if (GUILayout.Button("Dump Game State to Log", _buttonStyle))
            {
                DumpGameState();
            }
            GUILayout.EndVertical();

            GUILayout.Space(5);

            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("📊 Statistics", _headerStyle);

            GUILayout.Label($"Days Played: {GameScript.daysPlayed}", _labelStyle);
            GUILayout.Label($"Total Dewdrops Earned: {GameScript.dew}", _labelStyle);
            GUILayout.EndVertical();

            GUILayout.Space(5);

            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("ℹ️ About & Controls", _headerStyle);
            GUILayout.Label("OpenWood Cheat Menu v1.0", _labelStyle);
            GUILayout.Space(5);
            GUILayout.Label("⌨️ Keyboard Shortcuts:", _valueStyle);
            GUILayout.Label("  F3 - Toggle this menu", _labelStyle);
            GUILayout.Label("  Q / [ - Previous tab", _labelStyle);
            GUILayout.Label("  E / ] - Next tab", _labelStyle);
            GUILayout.Label("  ↑↓ - Scroll content", _labelStyle);
            GUILayout.Space(5);
            GUILayout.Label("⚡ Quick Actions (per tab):", _valueStyle);
            GUILayout.Label("  Player: 1=+1000$, 2=+10000$, 3=Reset EXP", _labelStyle);
            GUILayout.Label("  Items: 1=10 Oak, 2=10 Copper Ore, 3=10 Copper Bar", _labelStyle);
            GUILayout.Label("  Time: 1=Next Day, 2=Toggle Rain", _labelStyle);
            GUILayout.Label("  NPCs: 1=Max All Friendships", _labelStyle);
            GUILayout.EndVertical();
        }

        #endregion

        #region Cheat Methods

        private void AddMoney(int amount)
        {
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
                Plugin.Log.LogInfo($"Added {amount} dewdrops");
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Failed to add money: {ex.Message}");
            }
        }

        private void SetMoney(int amount)
        {
            GameScript.dew = amount;
            GameScript.actualDew = amount;
            Plugin.Log.LogInfo($"Set dewdrops to {amount}");
        }

        private void AddDayEXP(int amount)
        {
            GameScript.dayEXP = Mathf.Min(GameScript.dayEXP + amount, 100);
            Plugin.Log.LogInfo($"Added {amount} day EXP");
        }

        private void AddItem(int itemId, int quantity = 1)
        {
            try
            {
                if (_gameScript != null && _addItemMethod != null)
                {
                    // AddItem takes (int id, int quantity)
                    _addItemMethod.Invoke(_gameScript, new object[] { itemId, quantity });
                    Plugin.Log.LogInfo($"Added {quantity}x item {itemId}");
                }
                else
                {
                    Plugin.Log.LogWarning("Could not add item - GameScript not found");
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Failed to add item: {ex.Message}");
            }
        }

        private void SetNPCFriendship(int npcId, int level)
        {
            try
            {
                if (npcId >= 0 && npcId < GameScript.townsfolkLevel.Length)
                {
                    GameScript.townsfolkLevel[npcId] = Mathf.Clamp(level, 0, 10);
                    Plugin.Log.LogInfo($"Set NPC {npcId} friendship to {level}");
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Failed to set NPC friendship: {ex.Message}");
            }
        }

        private void AdvanceDay()
        {
            GameScript.daysPlayed++;
            GameScript.day++;
            if (GameScript.day > 7)
            {
                GameScript.day = 1;
                GameScript.week++;
                if (GameScript.week > 3)
                {
                    GameScript.week = 0;
                    GameScript.season++;
                    if (GameScript.season > 3)
                    {
                        GameScript.season = 0;
                        GameScript.year++;
                    }
                }
            }
            Plugin.Log.LogInfo($"Advanced to day {GameScript.day}, season {GameScript.season}, year {GameScript.year}");
        }

        private void ToggleRain()
        {
            GameScript.raining = !GameScript.raining;
            Plugin.Log.LogInfo($"Rain: {GameScript.raining}");
        }

        private void UnlockAllTools()
        {
            try
            {
                var toolUnlocked = typeof(GameScript).GetField("toolUnlocked", BindingFlags.Static | BindingFlags.Public);
                if (toolUnlocked != null)
                {
                    var tools = toolUnlocked.GetValue(null) as int[];
                    if (tools != null)
                    {
                        for (int i = 0; i < tools.Length; i++)
                            tools[i] = 1;
                        Plugin.Log.LogInfo("Unlocked all tools");
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Failed to unlock tools: {ex.Message}");
            }
        }

        private void UnlockAllRecipes()
        {
            Plugin.Log.LogInfo("Unlock all recipes - not yet implemented");
        }

        private void UnlockMuseum()
        {
            Plugin.Log.LogInfo("Unlock museum - not yet implemented");
        }

        private void DiscoverAllItems()
        {
            try
            {
                for (int i = 0; i < GameScript.discoverLevel.Length; i++)
                {
                    GameScript.discoverLevel[i] = 1;
                }
                Plugin.Log.LogInfo("Discovered all items");
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Failed to discover items: {ex.Message}");
            }
        }

        private void MaxAllHobbies()
        {
            try
            {
                for (int i = 0; i < GameScript.hobbyEXP.Length; i++)
                {
                    GameScript.hobbyEXP[i] = 999999;
                }
                Plugin.Log.LogInfo("Maxed all hobby levels");
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Failed to max hobbies: {ex.Message}");
            }
        }

        private void TeleportPlayer(float x, float y)
        {
            try
            {
                GameScript.playerPos = new Vector2(x, y);
                PlayerController.pos = new Vector2(x, y);
                
                if (_playerController != null)
                {
                    _playerController.transform.position = new Vector3(x, y, 0);
                }
                
                Plugin.Log.LogInfo($"Teleported to ({x}, {y})");
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Failed to teleport: {ex.Message}");
            }
        }

        private void DumpGameState()
        {
            Plugin.Log.LogInfo("=== GAME STATE DUMP ===");
            Plugin.Log.LogInfo($"Day: {GameScript.day}, Week: {GameScript.week}, Season: {GameScript.season}, Year: {GameScript.year}");
            Plugin.Log.LogInfo($"Dew: {GameScript.dew}, DayEXP: {GameScript.dayEXP}");
            Plugin.Log.LogInfo($"Position: {GameScript.playerPos}");
            Plugin.Log.LogInfo($"Raining: {GameScript.raining}");
            Plugin.Log.LogInfo($"Indoor: {GameScript.isIndoor}");
            Plugin.Log.LogInfo("======================");
        }

        private string GetSeasonName(int season)
        {
            switch (season)
            {
                case 0: return "Spring";
                case 1: return "Summer";
                case 2: return "Autumn";
                case 3: return "Winter";
                default: return "Unknown";
            }
        }

        #endregion
    }
}
