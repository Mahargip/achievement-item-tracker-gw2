using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using AchievementItemTracker.Models;
using AchievementItemTracker.Services;
using AchievementItemTracker.UI.Views;
using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AchievementItemTracker
{
    [Export(typeof(Blish_HUD.Modules.Module))]
    public class AchievementItemTrackerModule : Blish_HUD.Modules.Module
    {
        private static readonly Logger Logger = Logger.GetLogger<AchievementItemTrackerModule>();

        internal SettingsManager SettingsManager => ModuleParameters.SettingsManager;
        internal ContentsManager ContentsManager => ModuleParameters.ContentsManager;
        internal DirectoriesManager DirectoriesManager => ModuleParameters.DirectoriesManager;
        internal Gw2ApiManager Gw2ApiManager => ModuleParameters.Gw2ApiManager;

        public static AchievementItemTrackerModule Instance { get; private set; } = null!;

        // Services
        private Gw2DataService _gw2DataService = null!;
        private AchievementIndexService _indexService = null!;

        // UI
        private CornerIcon? _cornerIcon;
        private ContextMenuStrip? _contextMenu;
        private TrackerWindow? _trackerWindow;
        private PinnedTrackerWidget? _pinnedWidget;
        private AsyncTexture2D? _windowBackground;
        private AsyncTexture2D? _cornerIconTexture;
        private AsyncTexture2D? _cornerIconHoverTexture;

        // Settings
        private SettingEntry<KeyBinding>? _hotkeySetting;
        private SettingEntry<int>? _pinnedAchievementIdSetting;
        private SettingEntry<bool>? _showPinnedWidgetSetting;

        // Update loop timer
        private double _runningTime;

        [ImportingConstructor]
        public AchievementItemTrackerModule([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters)
        {
            Instance = this;
        }

        protected override void DefineSettings(SettingCollection settings)
        {
            _hotkeySetting = settings.DefineSetting(
                "ToggleWindowHotkey",
                new KeyBinding(Keys.None),
                () => "Toggle Window Keybind",
                () => "Hotkey to open or close the Achievement Item Tracker window."
            );

            _pinnedAchievementIdSetting = settings.DefineSetting(
                "PinnedAchievementId",
                0,
                () => "Pinned Achievement ID",
                () => "Stores the ID of the achievement pinned to the screen."
            );

            _showPinnedWidgetSetting = settings.DefineSetting(
                "ShowPinnedWidget",
                false,
                () => "Show Pinned Overlay",
                () => "Display the compact pinned achievement overlay on the screen."
            );
        }

        protected override async Task LoadAsync()
        {
            try
            {
                Logger.Info("Initializing Achievement Item Tracker module...");

                // 1. Initialize Services
                _gw2DataService = new Gw2DataService(Gw2ApiManager);
                _indexService = new AchievementIndexService();

                // 2. Load Achievement Index
                string moduleDir = DirectoriesManager.GetFullDirectoryPath("achievement-item-tracker");
                _indexService.LoadIndex(ContentsManager, moduleDir);

                // 3. Load Textures
                _windowBackground = AsyncTexture2D.FromAssetId(155997);

                try
                {
                    _cornerIconTexture = ContentsManager.GetTexture("icon.png");
                    _cornerIconHoverTexture = ContentsManager.GetTexture("icon_hover.png");
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "Could not load custom icon, using default texture.");
                }

                await Task.CompletedTask;

                // 4. Create Main Tracker Window
                _trackerWindow = new TrackerWindow(_windowBackground, _gw2DataService, _indexService);
                _trackerWindow.PinnedAchievementChanged += OnPinnedAchievementChanged;

                // 5. Create Pinned HUD Overlay
                _pinnedWidget = new PinnedTrackerWidget();
                _pinnedWidget.Closed += (s, e) =>
                {
                    if (_pinnedAchievementIdSetting != null)
                    {
                        _pinnedAchievementIdSetting.Value = 0;
                    }
                    if (_showPinnedWidgetSetting != null)
                    {
                        _showPinnedWidgetSetting.Value = false;
                    }
                };

                // 6. Create Corner Icon & Context Menu
                _cornerIcon = new CornerIcon
                {
                    Icon = _cornerIconTexture ?? ContentService.Textures.Pixel,
                    HoverIcon = _cornerIconHoverTexture ?? _cornerIconTexture,
                    BasicTooltipText = "Achievement Item Tracker",
                    Priority = 84729104,
                    Parent = GameService.Graphics.SpriteScreen
                };
                _cornerIcon.Click += (s, e) => _trackerWindow.ToggleWindow();

                _contextMenu = new ContextMenuStrip();
                _contextMenu.AddMenuItem("Open Tracker Window").Click += (s, e) => _trackerWindow.ToggleWindow();
                _contextMenu.AddMenuItem("Sync Inventory Now").Click += async (s, e) => await _gw2DataService.RefreshInventoryAsync();
                _contextMenu.AddMenuItem("Toggle Screen Pin Widget").Click += (s, e) =>
                {
                    if (_pinnedWidget != null)
                    {
                        _pinnedWidget.Visible = !_pinnedWidget.Visible;
                    }
                };
                _cornerIcon.Menu = _contextMenu;

                // 7. Setup Keybind
                if (_hotkeySetting?.Value != null)
                {
                    _hotkeySetting.Value.Activated += OnHotkeyActivated;
                }

                // 8. Restore Pinned Achievement only if user explicitly enabled it
                int savedPinnedId = _pinnedAchievementIdSetting?.Value ?? 0;
                bool shouldShowPinned = _showPinnedWidgetSetting?.Value ?? false;
                if (savedPinnedId > 0 && shouldShowPinned)
                {
                    var savedAch = _indexService.GetById(savedPinnedId);
                    if (savedAch != null)
                    {
                        _ = RestorePinnedAchievementAsync(savedAch);
                    }
                }

                // 9. Initial background inventory sync
                _ = Task.Run(async () =>
                {
                    await Task.Delay(2000); // Give Blish HUD a moment to initialize token
                    await _gw2DataService.RefreshInventoryAsync();
                });

                Logger.Info("Achievement Item Tracker module loaded successfully!");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to load Achievement Item Tracker module.");
            }
        }

        private void OnHotkeyActivated(object sender, EventArgs e)
        {
            _trackerWindow?.ToggleWindow();
        }

        private async void OnPinnedAchievementChanged(object sender, AchievementIndexItem? item)
        {
            if (item == null)
            {
                if (_pinnedWidget != null) _pinnedWidget.Visible = false;
                if (_pinnedAchievementIdSetting != null) _pinnedAchievementIdSetting.Value = 0;
                if (_showPinnedWidgetSetting != null) _showPinnedWidgetSetting.Value = false;
                return;
            }

            if (_pinnedAchievementIdSetting != null)
            {
                _pinnedAchievementIdSetting.Value = item.Id;
            }
            if (_showPinnedWidgetSetting != null)
            {
                _showPinnedWidgetSetting.Value = true;
            }

            await RestorePinnedAchievementAsync(item);
        }

        private async Task RestorePinnedAchievementAsync(AchievementIndexItem item)
        {
            try
            {
                var reqs = await _gw2DataService.GetAchievementRequirementsAsync(item.Id);
                _pinnedWidget?.SetAchievement(item, reqs);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Failed to restore pinned achievement {item.Name} ({item.Id})");
            }
        }

        protected override void Update(GameTime gameTime)
        {
            _runningTime += gameTime.ElapsedGameTime.TotalMilliseconds;

            // Auto-refresh inventory every 5 minutes in background
            if (_runningTime > 300_000)
            {
                _runningTime = 0;
                _ = Task.Run(async () =>
                {
                    await _gw2DataService.RefreshInventoryAsync();
                    if (_pinnedWidget != null && _pinnedWidget.CurrentAchievement != null)
                    {
                        await RestorePinnedAchievementAsync(_pinnedWidget.CurrentAchievement);
                    }
                });
            }
        }

        protected override void Unload()
        {
            if (_hotkeySetting?.Value != null)
            {
                _hotkeySetting.Value.Activated -= OnHotkeyActivated;
            }

            if (_trackerWindow != null)
            {
                _trackerWindow.PinnedAchievementChanged -= OnPinnedAchievementChanged;
                _trackerWindow.Dispose();
                _trackerWindow = null;
            }

            _pinnedWidget?.Dispose();
            _pinnedWidget = null;

            _cornerIcon?.Dispose();
            _cornerIcon = null;

            _contextMenu?.Dispose();
            _contextMenu = null;

            _cornerIconTexture?.Dispose();
            _cornerIconTexture = null;

            _cornerIconHoverTexture?.Dispose();
            _cornerIconHoverTexture = null;

            _gw2DataService?.Dispose();

            Instance = null!;
            Logger.Info("Achievement Item Tracker unloaded.");
        }
    }
}
