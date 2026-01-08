using System;
using OpenWood.Core.API;
using UnityEngine;

namespace OpenWood.Core.Cheats
{
    /// <summary>
    /// Comprehensive cheat menu for Littlewood.
    /// Provides an IMGUI interface for testing and debugging using the OpenWood API.
    /// Styled to match the game's UI as much as possible.
    /// </summary>
    /// <remarks>
    /// This is a reference implementation showing how to use the OpenWood modding APIs.
    /// Mod developers can use this as an example for building their own UI.
    /// </remarks>
    public class CheatMenu : MonoBehaviour
    {
        #region Singleton

        /// <summary>
        /// The singleton instance of the CheatMenu.
        /// </summary>
        public static CheatMenu Instance { get; private set; }

        #endregion

        #region Private Fields

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
        private string _teleportX = "0";
        private string _teleportY = "0";

        // Toggle states (bound to API)
        private bool _noClip = false;

        // Cursor state
        private bool _savedCursorVisible;
        private CursorLockMode _savedCursorLockMode;

        #endregion

        #region Public Properties

        /// <summary>
        /// Key to toggle the cheat menu visibility.
        /// </summary>
        public static KeyCode ToggleKey { get; set; } = KeyCode.F3;

        /// <summary>
        /// Gets whether the menu is currently visible.
        /// </summary>
        public bool IsVisible => _isVisible;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the CheatMenu. Called automatically by the plugin.
        /// </summary>
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
            float width = 520f;
            float height = 480f;
            _windowRect = new Rect(
                (Screen.width - width) / 2,
                (Screen.height - height) / 2,
                width,
                height
            );
        }

        #endregion

        #region Unity Lifecycle

        private void Update()
        {
            // Toggle menu with hotkey
            if (Input.GetKeyDown(ToggleKey))
            {
                ToggleMenu();
            }

            // Handle keyboard navigation when visible
            if (_isVisible)
            {
                HandleKeyboardNavigation();
            }
        }

        private void OnGUI()
        {
            if (!_isVisible) return;

            InitStyles();
            _windowRect = GUI.Window(_windowId, _windowRect, DrawWindow, "", _windowStyle);
        }

        #endregion

        #region Menu Control

        /// <summary>
        /// Toggles the menu visibility.
        /// </summary>
        public void ToggleMenu()
        {
            _isVisible = !_isVisible;
            
            if (_isVisible)
            {
                _savedCursorVisible = Cursor.visible;
                _savedCursorLockMode = Cursor.lockState;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.visible = _savedCursorVisible;
                Cursor.lockState = _savedCursorLockMode;
            }
        }

        /// <summary>
        /// Shows the menu.
        /// </summary>
        public void Show()
        {
            if (!_isVisible) ToggleMenu();
        }

        /// <summary>
        /// Hides the menu.
        /// </summary>
        public void Hide()
        {
            if (_isVisible) ToggleMenu();
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
                    if (Input.GetKeyDown(KeyCode.Alpha1)) PlayerAPI.AddMoney(1000);
                    if (Input.GetKeyDown(KeyCode.Alpha2)) PlayerAPI.AddMoney(10000);
                    if (Input.GetKeyDown(KeyCode.Alpha3)) PlayerAPI.ResetDayExperience();
                    break;
                case 1: // Items tab
                    if (Input.GetKeyDown(KeyCode.Alpha1)) InventoryAPI.AddItem(InventoryAPI.ItemID.Wood, 10);
                    if (Input.GetKeyDown(KeyCode.Alpha2)) InventoryAPI.AddItem(InventoryAPI.ItemID.Stone, 10);
                    if (Input.GetKeyDown(KeyCode.Alpha3)) InventoryAPI.AddItem(InventoryAPI.ItemID.WoodenPlank, 10);
                    break;
                case 2: // Time tab
                    if (Input.GetKeyDown(KeyCode.Alpha1)) TimeAPI.AdvanceDay();
                    if (Input.GetKeyDown(KeyCode.Alpha2)) TimeAPI.ToggleRain();
                    break;
                case 3: // NPCs tab
                    if (Input.GetKeyDown(KeyCode.Alpha1)) NPCAPI.MaxAllFriendships();
                    break;
            }
        }

        #endregion

        #region Style Initialization

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

            _windowStyle = new GUIStyle(GUI.skin.window)
            {
                normal = { background = bgTex, textColor = textColor },
                padding = new RectOffset(10, 10, 10, 10),
                border = new RectOffset(4, 4, 4, 4)
            };

            _headerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = headerColor }
            };

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

            _sectionStyle = new GUIStyle(GUI.skin.box)
            {
                normal = { background = sectionTex, textColor = textColor },
                padding = new RectOffset(10, 10, 8, 8),
                margin = new RectOffset(0, 0, 5, 5)
            };

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

            _labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                normal = { textColor = textColor }
            };

            _valueStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                normal = { textColor = headerColor }
            };

            _textFieldStyle = new GUIStyle(GUI.skin.textField)
            {
                fontSize = 13,
                normal = { textColor = textColor },
                padding = new RectOffset(6, 6, 4, 4)
            };

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

        #endregion

        #region Main Window Drawing

        private void DrawWindow(int windowId)
        {
            // Title bar
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("OpenWood Cheat Menu", _headerStyle);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("X", _smallButtonStyle, GUILayout.Width(25)))
            {
                Hide();
            }
            GUILayout.EndHorizontal();

            // Keyboard hints
            GUILayout.Label("Navigate: Q/E = Switch Tabs | Arrow Keys = Scroll | 1-3 = Quick Actions", _labelStyle);
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
            GUILayout.Label($"{TimeAPI.GetFormattedDate()}", _labelStyle);
            GUILayout.FlexibleSpace();
            GUILayout.Label($"Dew: {PlayerAPI.Money}", _valueStyle);
            GUILayout.EndHorizontal();
        }

        #endregion

        #region Player Tab

        private void DrawPlayerTab()
        {
            // Money section
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("Money", _headerStyle);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Current:", _labelStyle, GUILayout.Width(80));
            GUILayout.Label(PlayerAPI.Money.ToString(), _valueStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _moneyInput = GUILayout.TextField(_moneyInput, _textFieldStyle, GUILayout.Width(100));
            if (GUILayout.Button("Add", _buttonStyle, GUILayout.Width(60)))
            {
                if (int.TryParse(_moneyInput, out int amount))
                    PlayerAPI.AddMoney(amount);
            }
            if (GUILayout.Button("Set", _buttonStyle, GUILayout.Width(60)))
            {
                if (int.TryParse(_moneyInput, out int amount))
                    PlayerAPI.SetMoney(amount);
            }
            if (GUILayout.Button("+1000", _smallButtonStyle)) PlayerAPI.AddMoney(1000);
            if (GUILayout.Button("+10000", _smallButtonStyle)) PlayerAPI.AddMoney(10000);
            GUILayout.EndHorizontal();

            bool infiniteMoney = PlayerAPI.InfiniteMoney;
            bool newInfiniteMoney = GUILayout.Toggle(infiniteMoney, "  Infinite Money", _toggleStyle);
            if (newInfiniteMoney != infiniteMoney) PlayerAPI.InfiniteMoney = newInfiniteMoney;
            GUILayout.EndVertical();

            GUILayout.Space(5);

            // Experience section
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("Experience", _headerStyle);

            GUILayout.BeginHorizontal();
            GUILayout.Label($"Day EXP: {PlayerAPI.DayExperience} / 100", _labelStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _expInput = GUILayout.TextField(_expInput, _textFieldStyle, GUILayout.Width(100));
            if (GUILayout.Button("Add EXP", _buttonStyle, GUILayout.Width(80)))
            {
                if (int.TryParse(_expInput, out int amount))
                    PlayerAPI.AddDayExperience(amount);
            }
            if (GUILayout.Button("Max EXP", _buttonStyle, GUILayout.Width(80)))
            {
                PlayerAPI.MaxDayExperience();
            }
            if (GUILayout.Button("Reset EXP", _buttonStyle, GUILayout.Width(80)))
            {
                PlayerAPI.ResetDayExperience();
            }
            GUILayout.EndHorizontal();

            bool freezeTime = PlayerAPI.FreezeTime;
            bool newFreezeTime = GUILayout.Toggle(freezeTime, "  Freeze Day (No Fatigue)", _toggleStyle);
            if (newFreezeTime != freezeTime) PlayerAPI.FreezeTime = newFreezeTime;
            GUILayout.EndVertical();

            GUILayout.Space(5);

            // Movement section
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("Movement", _headerStyle);

            bool speedHack = PlayerAPI.SpeedHackEnabled;
            bool newSpeedHack = GUILayout.Toggle(speedHack, "  Speed Hack", _toggleStyle);
            if (newSpeedHack != speedHack) PlayerAPI.SpeedHackEnabled = newSpeedHack;
            
            if (PlayerAPI.SpeedHackEnabled)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label($"Speed: {PlayerAPI.SpeedMultiplier:F1}x", _labelStyle, GUILayout.Width(100));
                float newSpeed = GUILayout.HorizontalSlider(PlayerAPI.SpeedMultiplier, 0.5f, 5f);
                if (Math.Abs(newSpeed - PlayerAPI.SpeedMultiplier) > 0.01f)
                    PlayerAPI.SpeedMultiplier = newSpeed;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("1x", _smallButtonStyle)) PlayerAPI.SetSpeedMultiplier(1f);
                if (GUILayout.Button("1.5x", _smallButtonStyle)) PlayerAPI.SetSpeedMultiplier(1.5f);
                if (GUILayout.Button("2x", _smallButtonStyle)) PlayerAPI.SetSpeedMultiplier(2f);
                if (GUILayout.Button("3x", _smallButtonStyle)) PlayerAPI.SetSpeedMultiplier(3f);
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
            GUILayout.Label("Add Items", _headerStyle);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Item ID:", _labelStyle, GUILayout.Width(60));
            _itemIdInput = GUILayout.TextField(_itemIdInput, _textFieldStyle, GUILayout.Width(60));
            GUILayout.Label("Count:", _labelStyle, GUILayout.Width(50));
            _itemCountInput = GUILayout.TextField(_itemCountInput, _textFieldStyle, GUILayout.Width(40));
            if (GUILayout.Button("Add Item", _buttonStyle))
            {
                if (int.TryParse(_itemIdInput, out int id) && int.TryParse(_itemCountInput, out int count))
                {
                    InventoryAPI.AddItem(id, count);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(5);

            // Quick add buttons - WOOD & PLANKS
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("Wood & Planks", _headerStyle);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Wood", _smallButtonStyle)) InventoryAPI.AddItem(InventoryAPI.ItemID.Wood);
            if (GUILayout.Button("Magicwood", _smallButtonStyle)) InventoryAPI.AddItem(InventoryAPI.ItemID.Magicwood);
            if (GUILayout.Button("Goldenwood", _smallButtonStyle)) InventoryAPI.AddItem(InventoryAPI.ItemID.Goldenwood);
            if (GUILayout.Button("Almwood", _smallButtonStyle)) InventoryAPI.AddItem(InventoryAPI.ItemID.Almwood);
            if (GUILayout.Button("Leifwood", _smallButtonStyle)) InventoryAPI.AddItem(InventoryAPI.ItemID.Leifwood);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Wooden Plank", _smallButtonStyle)) InventoryAPI.AddItem(InventoryAPI.ItemID.WoodenPlank);
            if (GUILayout.Button("Fancy Plank", _smallButtonStyle)) InventoryAPI.AddItem(InventoryAPI.ItemID.FancyPlank);
            if (GUILayout.Button("Perfect Plank", _smallButtonStyle)) InventoryAPI.AddItem(InventoryAPI.ItemID.PerfectPlank);
            if (GUILayout.Button("Dusk Plank", _smallButtonStyle)) InventoryAPI.AddItem(InventoryAPI.ItemID.DuskPlank);
            if (GUILayout.Button("Dawn Plank", _smallButtonStyle)) InventoryAPI.AddItem(InventoryAPI.ItemID.DawnPlank);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(5);

            // STONE & BRICKS
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("Stone & Bricks", _headerStyle);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Stone", _smallButtonStyle)) InventoryAPI.AddItem(InventoryAPI.ItemID.Stone);
            if (GUILayout.Button("Magicite", _smallButtonStyle)) InventoryAPI.AddItem(InventoryAPI.ItemID.Magicite);
            if (GUILayout.Button("Orichalcum", _smallButtonStyle)) InventoryAPI.AddItem(InventoryAPI.ItemID.Orichalcum);
            if (GUILayout.Button("Wyvernite", _smallButtonStyle)) InventoryAPI.AddItem(InventoryAPI.ItemID.Wyvernite);
            if (GUILayout.Button("Dragalium", _smallButtonStyle)) InventoryAPI.AddItem(InventoryAPI.ItemID.Dragalium);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Plain Brick", _smallButtonStyle)) InventoryAPI.AddItem(InventoryAPI.ItemID.PlainBrick);
            if (GUILayout.Button("Fancy Brick", _smallButtonStyle)) InventoryAPI.AddItem(InventoryAPI.ItemID.FancyBrick);
            if (GUILayout.Button("Perfect Brick", _smallButtonStyle)) InventoryAPI.AddItem(InventoryAPI.ItemID.PerfectBrick);
            if (GUILayout.Button("Moonlight Orb", _smallButtonStyle)) InventoryAPI.AddItem(InventoryAPI.ItemID.MoonlightOrb);
            if (GUILayout.Button("Sunlight Orb", _smallButtonStyle)) InventoryAPI.AddItem(InventoryAPI.ItemID.SunlightOrb);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(5);

            // BULK ACTIONS
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("Bulk Actions", _headerStyle);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add All Wood Types (10)", _buttonStyle))
            {
                InventoryAPI.AddAllWoodTypes(10);
            }
            if (GUILayout.Button("Add All Stone Types (10)", _buttonStyle))
            {
                InventoryAPI.AddAllStoneTypes(10);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add All Planks (10)", _buttonStyle))
            {
                InventoryAPI.AddAllPlankTypes(10);
            }
            if (GUILayout.Button("Add All Bricks (10)", _buttonStyle))
            {
                InventoryAPI.AddAllBrickTypes(10);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Starter Pack", _buttonStyle))
            {
                InventoryAPI.AddStarterPack();
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
            GUILayout.Label("Current Date", _headerStyle);

            GUILayout.BeginHorizontal();
            GUILayout.Label($"Day: {TimeAPI.Day}", _valueStyle);
            GUILayout.Label($"Week: {TimeAPI.Week}", _valueStyle);
            GUILayout.Label($"Season: {TimeAPI.SeasonName}", _valueStyle);
            GUILayout.Label($"Year: {TimeAPI.Year}", _valueStyle);
            GUILayout.EndHorizontal();

            GUILayout.Label($"Days Played: {TimeAPI.DaysPlayed}", _labelStyle);
            GUILayout.EndVertical();

            GUILayout.Space(5);

            // Set time section
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("Set Date", _headerStyle);

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
                if (int.TryParse(_dayInput, out int day) &&
                    int.TryParse(_seasonInput, out int season) &&
                    int.TryParse(_yearInput, out int year))
                {
                    TimeAPI.SetDate(day, season, year);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(5);

            // Quick time controls
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("Quick Controls", _headerStyle);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Next Day", _buttonStyle)) TimeAPI.AdvanceDay();
            if (GUILayout.Button("Next Week", _buttonStyle)) TimeAPI.AdvanceWeek();
            if (GUILayout.Button("Next Season", _buttonStyle)) TimeAPI.AdvanceSeason();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Spring", _buttonStyle)) TimeAPI.CurrentSeason = TimeAPI.Season.Spring;
            if (GUILayout.Button("Summer", _buttonStyle)) TimeAPI.CurrentSeason = TimeAPI.Season.Summer;
            if (GUILayout.Button("Autumn", _buttonStyle)) TimeAPI.CurrentSeason = TimeAPI.Season.Autumn;
            if (GUILayout.Button("Winter", _buttonStyle)) TimeAPI.CurrentSeason = TimeAPI.Season.Winter;
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(5);

            // Weather
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("Weather", _headerStyle);

            GUILayout.BeginHorizontal();
            GUILayout.Label($"Raining: {(TimeAPI.IsRaining ? "Yes" : "No")}", _valueStyle);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(TimeAPI.IsRaining ? "Stop Rain" : "Start Rain", _buttonStyle))
            {
                TimeAPI.ToggleRain();
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
            GUILayout.Label("NPC Relationships", _headerStyle);

            GUILayout.BeginHorizontal();
            GUILayout.Label("NPC ID:", _labelStyle, GUILayout.Width(60));
            _npcIdInput = GUILayout.TextField(_npcIdInput, _textFieldStyle, GUILayout.Width(40));
            GUILayout.Label("Hearts:", _labelStyle, GUILayout.Width(50));
            _heartInput = GUILayout.TextField(_heartInput, _textFieldStyle, GUILayout.Width(40));

            if (GUILayout.Button("Set Level", _buttonStyle))
            {
                if (int.TryParse(_npcIdInput, out int npcId) && int.TryParse(_heartInput, out int hearts))
                {
                    NPCAPI.SetFriendship(npcId, hearts);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(5);

            // NPC quick buttons
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("Townsfolk Quick Set", _headerStyle);

            var npcs = new[] { 
                NPCAPI.NPC.Willow, NPCAPI.NPC.Dalton, NPCAPI.NPC.Dudley, NPCAPI.NPC.Laura,
                NPCAPI.NPC.Bubsy, NPCAPI.NPC.Ash, NPCAPI.NPC.Lilith, NPCAPI.NPC.Zana 
            };

            for (int row = 0; row < 2; row++)
            {
                GUILayout.BeginHorizontal();
                for (int col = 0; col < 4; col++)
                {
                    int idx = row * 4 + col;
                    if (idx < npcs.Length)
                    {
                        var npc = npcs[idx];
                        int level = NPCAPI.GetFriendship(npc);
                        if (GUILayout.Button($"{NPCAPI.GetName(npc)}\n[{level}]", _smallButtonStyle, GUILayout.Height(40)))
                        {
                            NPCAPI.SetFriendship(npc, NPCAPI.MaxFriendshipLevel);
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.Label("Click NPC to max hearts.", _labelStyle);
            GUILayout.EndVertical();

            GUILayout.Space(5);

            // Bulk actions
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("Bulk Actions", _headerStyle);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Max All Friendships", _buttonStyle))
            {
                NPCAPI.MaxAllFriendships();
            }
            if (GUILayout.Button("Reset All Friendships", _buttonStyle))
            {
                NPCAPI.ResetAllFriendships();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Max All Romance", _buttonStyle))
            {
                NPCAPI.MaxAllRomance();
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
            GUILayout.Label("Location Info", _headerStyle);

            GUILayout.Label($"Indoor: {WorldAPI.IsIndoors}", _labelStyle);
            GUILayout.Label($"Player Position: {WorldAPI.PlayerPosition}", _labelStyle);
            GUILayout.Label($"Facing: {WorldAPI.FacingDirection}", _labelStyle);
            GUILayout.Label($"Build Mode: {WorldAPI.IsInBuildMode}", _labelStyle);
            GUILayout.EndVertical();

            GUILayout.Space(5);

            // Unlock features
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("Unlocks", _headerStyle);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Unlock All Tools", _buttonStyle))
            {
                WorldAPI.UnlockAllTools();
            }
            if (GUILayout.Button("Unlock All Recipes", _buttonStyle))
            {
                WorldAPI.UnlockAllRecipes();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Unlock Museum", _buttonStyle))
            {
                WorldAPI.UnlockMuseum();
            }
            if (GUILayout.Button("Discover All Items", _buttonStyle))
            {
                WorldAPI.DiscoverAllItems();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(5);

            // Hobbies
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("Hobbies", _headerStyle);

            if (GUILayout.Button("Max All Hobby Levels", _buttonStyle))
            {
                PlayerAPI.MaxAllHobbies();
            }

            string[] hobbies = { "Woodcutting", "Mining", "Fishing", "Bug Catching", "Farming" };
            for (int i = 0; i < hobbies.Length; i++)
            {
                GUILayout.Label($"{hobbies[i]}: {PlayerAPI.GetHobbyExperience(i)}", _labelStyle);
            }
            GUILayout.EndVertical();
        }

        #endregion

        #region Teleport Tab

        private void DrawTeleportTab()
        {
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("Quick Teleport", _headerStyle);

            GUILayout.Label("Common Locations:", _labelStyle);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Town Center", _buttonStyle))
            {
                PlayerAPI.Teleport(WorldAPI.Locations.TownCenter);
            }
            if (GUILayout.Button("Farm Area", _buttonStyle))
            {
                PlayerAPI.Teleport(WorldAPI.Locations.FarmArea);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Forest Entrance", _buttonStyle))
            {
                PlayerAPI.Teleport(WorldAPI.Locations.ForestEntrance);
            }
            if (GUILayout.Button("Beach", _buttonStyle))
            {
                PlayerAPI.Teleport(WorldAPI.Locations.Beach);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(5);

            // Custom teleport
            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("Custom Teleport", _headerStyle);

            GUILayout.Label($"Current: ({PlayerAPI.Position.x:F2}, {PlayerAPI.Position.y:F2})", _valueStyle);

            GUILayout.BeginHorizontal();
            GUILayout.Label("X:", _labelStyle, GUILayout.Width(20));
            _teleportX = GUILayout.TextField(_teleportX, _textFieldStyle, GUILayout.Width(60));
            GUILayout.Label("Y:", _labelStyle, GUILayout.Width(20));
            _teleportY = GUILayout.TextField(_teleportY, _textFieldStyle, GUILayout.Width(60));
            
            if (GUILayout.Button("Teleport", _buttonStyle))
            {
                if (float.TryParse(_teleportX, out float x) && float.TryParse(_teleportY, out float y))
                {
                    PlayerAPI.Teleport(x, y);
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
            GUILayout.Label("Debug Options", _headerStyle);

            GUILayout.BeginHorizontal();
            GUILayout.Label($"Menu Open: {GameAPI.IsMenuOpen}", _valueStyle);
            GUILayout.Label($"Talking: {GameAPI.IsTalking}", _valueStyle);
            GUILayout.Label($"Interacting: {GameAPI.IsInteracting}", _valueStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label($"Fishing: {GameAPI.IsFishing}", _valueStyle);
            GUILayout.Label($"Building: {GameAPI.IsBuilding}", _valueStyle);
            GUILayout.Label($"Paused: {GameAPI.IsPaused}", _valueStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(5);

            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("Game State", _headerStyle);

            if (GUILayout.Button("Force Close All Menus", _buttonStyle))
            {
                GameAPI.CloseAllMenus();
            }

            if (GUILayout.Button("Dump Game State to Log", _buttonStyle))
            {
                GameAPI.DumpGameState();
            }
            GUILayout.EndVertical();

            GUILayout.Space(5);

            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("Statistics", _headerStyle);

            GUILayout.Label($"Days Played: {GameAPI.DaysPlayed}", _labelStyle);
            GUILayout.Label($"Total Dewdrops: {GameAPI.TotalMoney}", _labelStyle);
            GUILayout.EndVertical();

            GUILayout.Space(5);

            GUILayout.BeginVertical(_sectionStyle);
            GUILayout.Label("About & Controls", _headerStyle);
            GUILayout.Label($"OpenWood v{GameAPI.OpenWoodVersion}", _labelStyle);
            GUILayout.Space(5);
            GUILayout.Label("Keyboard Shortcuts:", _valueStyle);
            GUILayout.Label("  F3 - Toggle this menu", _labelStyle);
            GUILayout.Label("  Q / E - Previous/Next tab", _labelStyle);
            GUILayout.Label("  Arrow Keys - Scroll content", _labelStyle);
            GUILayout.Space(5);
            GUILayout.Label("Quick Actions (per tab):", _valueStyle);
            GUILayout.Label("  Player: 1=+1000$, 2=+10000$, 3=Reset EXP", _labelStyle);
            GUILayout.Label("  Items: 1=10 Wood, 2=10 Stone, 3=10 Planks", _labelStyle);
            GUILayout.Label("  Time: 1=Next Day, 2=Toggle Rain", _labelStyle);
            GUILayout.Label("  NPCs: 1=Max All Friendships", _labelStyle);
            GUILayout.EndVertical();
        }

        #endregion
    }
}
