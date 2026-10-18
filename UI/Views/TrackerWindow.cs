using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AchievementItemTracker.Models;
using AchievementItemTracker.Services;
using AchievementItemTracker.UI.Controls;
using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;

namespace AchievementItemTracker.UI.Views
{
    public class TrackerWindow : StandardWindow
    {
        private static readonly Logger Logger = Logger.GetLogger<TrackerWindow>();

        private readonly Gw2DataService _gw2DataService;
        private readonly AchievementIndexService _indexService;

        // UI Controls - Search bar & Header
        private readonly TextBox _searchBox;
        private readonly Checkbox _filterItemsOnlyCheckbox;
        private readonly StandardButton _refreshButton;
        private readonly Label _statusLabel;

        // Search Results Panel
        private readonly Panel _searchContainer;
        private readonly FlowPanel _searchResultsFlow;

        // Selected Achievement Details Panel
        private readonly Panel _detailContainer;
        private readonly StandardButton _backButton;
        private readonly StandardButton _pinButton;
        private readonly Label _achievementTitleLabel;
        private readonly Label _achievementReqLabel;
        private readonly Label _progressLabel;
        private readonly FlowPanel _itemsFlow;
        private readonly LoadingSpinner _spinner;

        private AchievementIndexItem? _currentAchievement;
        private List<AchievementItemRequirement> _currentRequirements = new List<AchievementItemRequirement>();

        public event EventHandler<AchievementIndexItem?>? PinnedAchievementChanged;

        public TrackerWindow(AsyncTexture2D background, Gw2DataService gw2DataService, AchievementIndexService indexService)
            : base(background, new Rectangle(25, 26, 730, 660), new Rectangle(35, 45, 690, 600))
        {
            _gw2DataService = gw2DataService;
            _indexService = indexService;

            Title = "Achievement Item Tracker";
            Subtitle = "Item Requirements & Inventory Totals";
            Id = "AchievementItemTracker_MainWindow_v1";
            SavesPosition = true;
            Parent = GameService.Graphics.SpriteScreen;

            // 1. Search Box
            _searchBox = new TextBox
            {
                Location = new Point(5, 5),
                Size = new Point(360, 30),
                PlaceholderText = "Search achievement (e.g. Skyscale, Bioluminescence)...",
                Parent = this
            };
            _searchBox.TextChanged += (s, e) => TriggerSearch();

            // 2. Filter Checkbox
            _filterItemsOnlyCheckbox = new Checkbox
            {
                Text = "Collections only",
                Checked = true,
                Location = new Point(375, 10),
                Parent = this
            };
            _filterItemsOnlyCheckbox.CheckedChanged += (s, e) => TriggerSearch();

            // 3. Status Subtitle
            _statusLabel = new Label
            {
                Text = _gw2DataService.LastSyncMessage,
                Font = GameService.Content.DefaultFont12,
                TextColor = Color.LightGray * 0.8f,
                Location = new Point(5, 40),
                Width = 660,
                Height = 18,
                Parent = this
            };

            // 4. Refresh Inventory Button
            _refreshButton = new StandardButton
            {
                Text = "↻ Sync Items",
                Location = new Point(545, 5),
                Size = new Point(125, 30),
                BasicTooltipText = "Fetch latest items from your Bank, Bags, and Material Storage",
                Parent = this
            };
            _refreshButton.Click += async (s, e) =>
            {
                _refreshButton.Enabled = false;
                _statusLabel.Text = "Syncing with GW2 API...";
                await _gw2DataService.RefreshInventoryAsync();
                _refreshButton.Enabled = true;
            };

            // 5. Search Container
            _searchContainer = new Panel
            {
                Location = new Point(5, 62),
                Size = new Point(665, 520),
                Visible = true,
                Parent = this
            };

            _searchResultsFlow = new FlowPanel
            {
                FlowDirection = ControlFlowDirection.SingleTopToBottom,
                ControlPadding = new Vector2(0, 4),
                CanScroll = true,
                Size = new Point(665, 520),
                Parent = _searchContainer
            };

            // 6. Detail Container
            _detailContainer = new Panel
            {
                Location = new Point(5, 62),
                Size = new Point(665, 520),
                Visible = false,
                Parent = this
            };

            _backButton = new StandardButton
            {
                Text = "← Back to Search",
                Location = new Point(0, 0),
                Size = new Point(130, 26),
                Parent = _detailContainer
            };
            _backButton.Click += (s, e) => ShowSearch();

            _pinButton = new StandardButton
            {
                Text = "📌 Pin to Screen",
                Location = new Point(140, 0),
                Size = new Point(130, 26),
                BasicTooltipText = "Keeps this tracker floating on your screen while playing",
                Parent = _detailContainer
            };
            _pinButton.Click += (s, e) =>
            {
                PinnedAchievementChanged?.Invoke(this, _currentAchievement);
            };

            _achievementTitleLabel = new Label
            {
                Font = GameService.Content.DefaultFont18,
                TextColor = Color.Gold,
                Location = new Point(0, 32),
                Width = 665,
                Height = 26,
                Parent = _detailContainer
            };

            _achievementReqLabel = new Label
            {
                Font = GameService.Content.DefaultFont12,
                TextColor = Color.LightGray,
                Location = new Point(0, 60),
                Width = 665,
                Height = 32,
                WrapText = true,
                Parent = _detailContainer
            };

            _progressLabel = new Label
            {
                Font = GameService.Content.DefaultFont14,
                TextColor = Color.LightGreen,
                Location = new Point(0, 94),
                Width = 665,
                Height = 20,
                Parent = _detailContainer
            };

            _spinner = new LoadingSpinner
            {
                Location = new Point(310, 200),
                Size = new Point(48, 48),
                Visible = false,
                Parent = _detailContainer
            };

            _itemsFlow = new FlowPanel
            {
                FlowDirection = ControlFlowDirection.SingleTopToBottom,
                ControlPadding = new Vector2(0, 4),
                CanScroll = true,
                Location = new Point(0, 120),
                Size = new Point(665, 395),
                Parent = _detailContainer
            };

            // Event Subscriptions
            _gw2DataService.SyncStatusChanged += (s, msg) => _statusLabel.Text = msg;
            _gw2DataService.InventoryRefreshed += (s, snapshot) => OnInventoryUpdated();

            // Populate initial popular achievements
            TriggerSearch();
        }

        public void TriggerSearch()
        {
            if (!_searchContainer.Visible)
            {
                ShowSearch();
            }

            _searchResultsFlow.ClearChildren();

            string query = _searchBox.Text;
            bool onlyItems = _filterItemsOnlyCheckbox.Checked;

            var results = _indexService.Search(query, onlyItems, 50);

            if (results.Count == 0)
            {
                var noResults = new Label
                {
                    Text = "No achievements found matching your search.",
                    Font = GameService.Content.DefaultFont14,
                    TextColor = Color.LightGray * 0.7f,
                    Location = new Point(20, 20),
                    Width = 400,
                    Parent = _searchResultsFlow
                };
                return;
            }

            foreach (var item in results)
            {
                var row = new SearchResultRow(item, 645);
                row.Selected += (s, selectedItem) => SelectAchievement(selectedItem);
                row.Parent = _searchResultsFlow;
            }
        }

        public async void SelectAchievement(AchievementIndexItem item)
        {
            _currentAchievement = item;
            _searchContainer.Visible = false;
            _detailContainer.Visible = true;

            _achievementTitleLabel.Text = item.Name;
            _achievementReqLabel.Text = item.Requirement;
            _progressLabel.Text = "Loading requirement details from GW2 API...";

            _itemsFlow.ClearChildren();
            _spinner.Visible = true;

            try
            {
                _currentRequirements = await _gw2DataService.GetAchievementRequirementsAsync(item.Id);
                RenderRequirements();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Failed to load requirements for achievement {item.Name} ({item.Id})");
                _progressLabel.Text = "Failed to load requirements. Check API connection or try again.";
            }
            finally
            {
                _spinner.Visible = false;
            }
        }

        private void RenderRequirements()
        {
            _itemsFlow.ClearChildren();

            if (_currentRequirements.Count == 0)
            {
                new Label
                {
                    Text = "No item requirements found in this achievement's API bits.\n(Some achievements use plain text objectives instead of item collections).",
                    Font = GameService.Content.DefaultFont14,
                    TextColor = Color.Orange,
                    Location = new Point(10, 20),
                    Width = 600,
                    Height = 60,
                    WrapText = true,
                    Parent = _itemsFlow
                };
                _progressLabel.Text = "No item bits";
                return;
            }

            int completedCount = _currentRequirements.Count(r => r.IsCompleted);
            int totalCount = _currentRequirements.Count;
            int percentage = totalCount > 0 ? (int)((completedCount / (float)totalCount) * 100) : 0;

            _progressLabel.Text = $"Progress: {completedCount} / {totalCount} Items Ready ({percentage}%)";
            _progressLabel.TextColor = completedCount == totalCount ? Color.LightGreen : Color.Gold;

            foreach (var req in _currentRequirements)
            {
                var row = new ItemRequirementRow(req, 645)
                {
                    Parent = _itemsFlow
                };
            }
        }

        private void OnInventoryUpdated()
        {
            if (_currentAchievement != null && _detailContainer.Visible)
            {
                // Refresh requirements with updated inventory numbers
                Task.Run(async () =>
                {
                    _currentRequirements = await _gw2DataService.GetAchievementRequirementsAsync(_currentAchievement.Id);
                    RenderRequirements();
                });
            }
        }

        private void ShowSearch()
        {
            _detailContainer.Visible = false;
            _searchContainer.Visible = true;
        }
    }
}
