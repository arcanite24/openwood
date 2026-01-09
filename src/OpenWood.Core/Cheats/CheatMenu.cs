using System.Collections.Generic;
using OpenWood.Core.API;
using OpenWood.Core.UI;
using UnityEngine;

namespace OpenWood.Core.Cheats
{
    /// <summary>
    /// Comprehensive cheat menu for Littlewood.
    /// Provides a native UI interface for testing and debugging using the OpenWood API.
    /// </summary>
    public class CheatMenu : MonoBehaviour
    {
        #region Singleton

        public static CheatMenu Instance { get; private set; }

        #endregion

        #region Private Fields

        private bool _isVisible = false;
        private int _currentTab = 0;
        private bool _uiCreated = false;

        private readonly string[] _tabs = { "Player", "Items", "Time", "NPCs", "World", "Teleport", "Debug" };

        // UI Elements
        private UIWindow _window;
        private List<UIButton> _tabButtons = new List<UIButton>();
        private List<GameObject> _tabPanels = new List<GameObject>();
        private UIScrollView _contentScrollView;

        // Dynamic labels
        private UILabel _moneyLabel;
        private UILabel _dayExpLabel;
        private UILabel _dateLabel;
        private UILabel _statusBarDate;
        private UILabel _statusBarMoney;

        // Cursor state
        private bool _savedCursorVisible;
        private CursorLockMode _savedCursorLockMode;

        #endregion

        #region Public Properties

        public static KeyCode ToggleKey { get; set; } = KeyCode.F3;
        public bool IsVisible => _isVisible;

        #endregion

        #region Initialization

        public static void Initialize()
        {
            if (Instance != null) return;

            var go = new GameObject("OpenWood_CheatMenu");
            Instance = go.AddComponent<CheatMenu>();
            DontDestroyOnLoad(go);

            Plugin.Log.LogInfo("Cheat Menu initialized. Press F3 to toggle.");
        }

        private void Start()
        {
            Events.GameEvents.OnGameStart += OnGameStart;
        }

        private void OnGameStart()
        {
            CreateUI();
        }

        private void OnDestroy()
        {
            Events.GameEvents.OnGameStart -= OnGameStart;

            if (_window != null)
            {
                UIAPI.DestroyWindow(_window);
            }
        }

        #endregion

        #region UI Creation

        private void CreateUI()
        {
            if (_uiCreated || !UIAPI.IsReady) return;

            Plugin.Log.LogDebug("Creating CheatMenu UI...");

            // Create main window
            _window = UIAPI.CreateWindow("OpenWood Cheat Menu", 550, 520);
            _window.Center();
            _window.Hide();

            var content = _window.ContentArea;

            // Hint label
            UIAPI.CreateLabel(content, "Navigate: Q/E = Switch Tabs | 1-3 = Quick Actions")
                .SetFontSize(11);

            // Tab bar
            var tabBar = UIAPI.CreateHorizontalLayout(content, 3);
            var tabBarLayout = tabBar.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>();
            tabBarLayout.childControlWidth = true;
            tabBarLayout.childForceExpandWidth = true;

            for (int i = 0; i < _tabs.Length; i++)
            {
                int tabIndex = i;
                var tabBtn = UIAPI.CreateButton(tabBar.transform, _tabs[i], () => SwitchTab(tabIndex));
                tabBtn.SetSize(70, 32);
                _tabButtons.Add(tabBtn);
            }

            // Content scroll view
            _contentScrollView = UIAPI.CreateScrollView(content, 530, 350);
            _contentScrollView.WithVerticalLayout(5, 10);

            // Create all tab panels
            CreatePlayerTab();
            CreateItemsTab();
            CreateTimeTab();
            CreateNPCsTab();
            CreateWorldTab();
            CreateTeleportTab();
            CreateDebugTab();

            // Status bar
            var statusBar = UIAPI.CreatePanel(content, 530, 30);
            statusBar.SetColor(UIColors.Darken(UIColors.PanelBackground, 0.05f));
            var statusLayout = statusBar.GameObject.AddComponent<UnityEngine.UI.HorizontalLayoutGroup>();
            statusLayout.padding = new RectOffset(10, 10, 5, 5);
            statusLayout.childControlWidth = true;
            statusLayout.childForceExpandWidth = true;

            _statusBarDate = UIAPI.CreateLabel(statusBar.Transform, "Day 1, Spring, Year 1");
            _statusBarMoney = UIAPI.CreateLabel(statusBar.Transform, "Dew: 0").AsValue().RightAligned();

            SwitchTab(0);

            _uiCreated = true;
            Plugin.Log.LogInfo("CheatMenu UI created successfully");
        }

        private GameObject CreateTabPanel(string name)
        {
            var panel = new GameObject(name);
            panel.transform.SetParent(_contentScrollView.ContentTransform, false);

            var rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);

            var layout = panel.AddComponent<UnityEngine.UI.VerticalLayoutGroup>();
            layout.spacing = 8;
            layout.padding = new RectOffset(5, 5, 5, 5);
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            var fitter = panel.AddComponent<UnityEngine.UI.ContentSizeFitter>();
            fitter.horizontalFit = UnityEngine.UI.ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;

            _tabPanels.Add(panel);
            return panel;
        }

        private void CreatePlayerTab()
        {
            var tab = CreateTabPanel("PlayerTab");
            
            UIAPI.CreateLabel(tab.transform, "=== PLAYER ===").AsHeader().Centered();
            
            _moneyLabel = UIAPI.CreateLabel(tab.transform, "Money: 0");
            _moneyLabel.SetFontSize(16);
            
            var moneyRow = UIAPI.CreateHorizontalLayout(tab.transform, 5);
            UIAPI.CreateButton(moneyRow.transform, "+1000 Dew", () => PlayerAPI.AddMoney(1000));
            UIAPI.CreateButton(moneyRow.transform, "+10000 Dew", () => PlayerAPI.AddMoney(10000));
            UIAPI.CreateButton(moneyRow.transform, "Max Money", () => PlayerAPI.SetMoney(999999));
            
            _dayExpLabel = UIAPI.CreateLabel(tab.transform, "Day EXP: 0 / 100");
            
            var expRow = UIAPI.CreateHorizontalLayout(tab.transform, 5);
            UIAPI.CreateButton(expRow.transform, "Add EXP", () => PlayerAPI.AddDayExperience(25));
            UIAPI.CreateButton(expRow.transform, "Max EXP", () => PlayerAPI.MaxDayExperience());
            UIAPI.CreateButton(expRow.transform, "Reset EXP", () => PlayerAPI.ResetDayExperience());
        }

        private void CreateItemsTab()
        {
            var tab = CreateTabPanel("ItemsTab");
            
            UIAPI.CreateLabel(tab.transform, "=== ITEMS ===").AsHeader().Centered();
            
            var itemRow1 = UIAPI.CreateHorizontalLayout(tab.transform, 5);
            UIAPI.CreateButton(itemRow1.transform, "+10 Wood", () => InventoryAPI.AddItem(InventoryAPI.ItemID.Wood, 10));
            UIAPI.CreateButton(itemRow1.transform, "+10 Stone", () => InventoryAPI.AddItem(InventoryAPI.ItemID.Stone, 10));
            UIAPI.CreateButton(itemRow1.transform, "+10 Planks", () => InventoryAPI.AddItem(InventoryAPI.ItemID.WoodenPlank, 10));
            UIAPI.CreateButton(itemRow1.transform, "+10 Bricks", () => InventoryAPI.AddItem(InventoryAPI.ItemID.PlainBrick, 10));
            
            var itemRow2 = UIAPI.CreateHorizontalLayout(tab.transform, 5);
            UIAPI.CreateButton(itemRow2.transform, "All Wood Types", () => InventoryAPI.AddAllWoodTypes(10));
            UIAPI.CreateButton(itemRow2.transform, "All Stone Types", () => InventoryAPI.AddAllStoneTypes(10));
            UIAPI.CreateButton(itemRow2.transform, "Starter Pack", () => InventoryAPI.AddStarterPack());
        }

        private void CreateTimeTab()
        {
            var tab = CreateTabPanel("TimeTab");
            
            UIAPI.CreateLabel(tab.transform, "=== TIME ===").AsHeader().Centered();
            
            _dateLabel = UIAPI.CreateLabel(tab.transform, "Day 1, Spring, Year 1");
            
            var timeRow = UIAPI.CreateHorizontalLayout(tab.transform, 5);
            UIAPI.CreateButton(timeRow.transform, "Next Day", () => TimeAPI.AdvanceDay());
            UIAPI.CreateButton(timeRow.transform, "Next Week", () => TimeAPI.AdvanceWeek());
            UIAPI.CreateButton(timeRow.transform, "Toggle Rain", () => TimeAPI.ToggleRain());
        }

        private void CreateNPCsTab()
        {
            var tab = CreateTabPanel("NPCsTab");
            
            UIAPI.CreateLabel(tab.transform, "=== NPCs ===").AsHeader().Centered();
            
            var npcRow = UIAPI.CreateHorizontalLayout(tab.transform, 5);
            UIAPI.CreateButton(npcRow.transform, "Max Friendships", () => NPCAPI.MaxAllFriendships());
            UIAPI.CreateButton(npcRow.transform, "Max Romance", () => NPCAPI.MaxAllRomance());
        }

        private void CreateWorldTab()
        {
            var tab = CreateTabPanel("WorldTab");
            
            UIAPI.CreateLabel(tab.transform, "=== WORLD ===").AsHeader().Centered();
            
            var worldRow = UIAPI.CreateHorizontalLayout(tab.transform, 5);
            UIAPI.CreateButton(worldRow.transform, "Unlock Tools", () => WorldAPI.UnlockAllTools());
            UIAPI.CreateButton(worldRow.transform, "Unlock Recipes", () => WorldAPI.UnlockAllRecipes());
            UIAPI.CreateButton(worldRow.transform, "Discover All", () => WorldAPI.DiscoverAllItems());
            
            var hobbyRow = UIAPI.CreateHorizontalLayout(tab.transform, 5);
            UIAPI.CreateButton(hobbyRow.transform, "Max All Hobbies", () => PlayerAPI.MaxAllHobbies());
        }

        private void CreateTeleportTab()
        {
            var tab = CreateTabPanel("TeleportTab");
            
            UIAPI.CreateLabel(tab.transform, "=== TELEPORT ===").AsHeader().Centered();
            
            UIAPI.CreateLabel(tab.transform, "Quick teleport coming soon...");
        }

        private void CreateDebugTab()
        {
            var tab = CreateTabPanel("DebugTab");
            
            UIAPI.CreateLabel(tab.transform, "=== DEBUG ===").AsHeader().Centered();
            
            var debugRow = UIAPI.CreateHorizontalLayout(tab.transform, 5);
            UIAPI.CreateButton(debugRow.transform, "Close Menus", () => GameAPI.CloseAllMenus());
            UIAPI.CreateButton(debugRow.transform, "Dump State", () => GameAPI.DumpGameState());
        }

        private void SwitchTab(int tabIndex)
        {
            _currentTab = tabIndex;
            
            // Show only the selected tab panel
            for (int i = 0; i < _tabPanels.Count; i++)
            {
                _tabPanels[i].SetActive(i == tabIndex);
            }
            
            // Reset scroll position
            if (_contentScrollView != null && _contentScrollView.ScrollRect != null)
            {
                _contentScrollView.ScrollRect.verticalNormalizedPosition = 1f;
            }
            
            UpdateTabButtonStyles();
        }

        private void UpdateTabButtonStyles()
        {
            for (int i = 0; i < _tabButtons.Count; i++)
            {
                _tabButtons[i].SetColor(i == _currentTab ? UIColors.ToggleOn : UIColors.ButtonNormal);
            }
        }

        #endregion

        #region Unity Lifecycle

        private void Update()
        {
            if (Input.GetKeyDown(ToggleKey))
            {
                ToggleMenu();
            }

            if (_isVisible && _uiCreated)
            {
                HandleKeyboardNavigation();
                UpdateDynamicLabels();
            }
        }

        private void UpdateDynamicLabels()
        {
            if (_moneyLabel != null) _moneyLabel.Text = $"Money: {PlayerAPI.Money}";
            if (_dayExpLabel != null) _dayExpLabel.Text = $"Day EXP: {PlayerAPI.DayExperience} / 100";
            if (_dateLabel != null) _dateLabel.Text = $"Day {TimeAPI.Day} | Week {TimeAPI.Week} | {TimeAPI.SeasonName} | Year {TimeAPI.Year}";
            if (_statusBarDate != null) _statusBarDate.Text = TimeAPI.GetFormattedDate();
            if (_statusBarMoney != null) _statusBarMoney.Text = $"Dew: {PlayerAPI.Money}";
        }

        #endregion

        #region Menu Control

        public void ToggleMenu()
        {
            if (!_uiCreated)
            {
                CreateUI();
                if (!_uiCreated) return;
            }

            _isVisible = !_isVisible;

            if (_isVisible)
            {
                _savedCursorVisible = Cursor.visible;
                _savedCursorLockMode = Cursor.lockState;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                _window.Show();
            }
            else
            {
                Cursor.visible = _savedCursorVisible;
                Cursor.lockState = _savedCursorLockMode;
                _window.Hide();
            }
        }

        public void Show()
        {
            if (!_isVisible) ToggleMenu();
        }

        public void Hide()
        {
            if (_isVisible) ToggleMenu();
        }

        private void HandleKeyboardNavigation()
        {
            // Tab switching with Q/E
            if (Input.GetKeyDown(KeyCode.Q))
            {
                SwitchTab((_currentTab - 1 + _tabs.Length) % _tabs.Length);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                SwitchTab((_currentTab + 1) % _tabs.Length);
            }

            // Quick actions
            if (Input.GetKeyDown(KeyCode.Alpha1)) PlayerAPI.AddMoney(1000);
            if (Input.GetKeyDown(KeyCode.Alpha2)) PlayerAPI.AddMoney(10000);
            if (Input.GetKeyDown(KeyCode.Alpha3)) TimeAPI.AdvanceDay();
        }

        #endregion
    }
}
