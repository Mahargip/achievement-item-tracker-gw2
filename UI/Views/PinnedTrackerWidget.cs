using System;
using System.Collections.Generic;
using System.Linq;
using AchievementItemTracker.Models;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Microsoft.Xna.Framework;

namespace AchievementItemTracker.UI.Views
{
    public class PinnedTrackerWidget : Panel
    {
        private readonly Label _titleLabel;
        private readonly Label _progressLabel;
        private readonly StandardButton _closeButton;
        private readonly FlowPanel _itemsFlow;

        private bool _isDragging;
        private Point _dragOffset;

        public AchievementIndexItem? CurrentAchievement { get; private set; }

        public PinnedTrackerWidget()
        {
            Size = new Point(280, 360);
            Location = new Point(300, 200);
            BackgroundColor = new Color(15, 15, 18, 210);
            Visible = false;
            Parent = GameService.Graphics.SpriteScreen;

            // Title
            _titleLabel = new Label
            {
                Text = "Pinned Achievement",
                Font = GameService.Content.DefaultFont14,
                TextColor = Color.Gold,
                Location = new Point(10, 8),
                Width = 200,
                Height = 20,
                Parent = this
            };

            // Close button
            _closeButton = new StandardButton
            {
                Text = "×",
                Location = new Point(252, 6),
                Size = new Point(22, 22),
                BasicTooltipText = "Unpin and close overlay",
                Parent = this
            };
            _closeButton.Click += (s, e) =>
            {
                Visible = false;
                CurrentAchievement = null;
            };

            // Progress
            _progressLabel = new Label
            {
                Text = "0 / 0",
                Font = GameService.Content.DefaultFont12,
                TextColor = Color.LightGreen,
                Location = new Point(10, 30),
                Width = 240,
                Height = 16,
                Parent = this
            };

            // Items List
            _itemsFlow = new FlowPanel
            {
                FlowDirection = ControlFlowDirection.SingleTopToBottom,
                ControlPadding = new Vector2(0, 3),
                CanScroll = true,
                Location = new Point(8, 50),
                Size = new Point(264, 300),
                Parent = this
            };
        }

        public void SetAchievement(AchievementIndexItem? achievement, List<AchievementItemRequirement> requirements)
        {
            CurrentAchievement = achievement;

            if (achievement == null || requirements == null || requirements.Count == 0)
            {
                Visible = false;
                return;
            }

            _titleLabel.Text = achievement.Name;
            _titleLabel.BasicTooltipText = achievement.Name;

            int completed = requirements.Count(r => r.IsCompleted);
            _progressLabel.Text = $"{completed} / {requirements.Count} Items Ready";

            _itemsFlow.ClearChildren();

            foreach (var req in requirements)
            {
                var row = new Panel
                {
                    Width = 248,
                    Height = 28,
                    BackgroundColor = new Color(28, 28, 34, 190),
                    Parent = _itemsFlow
                };

                // Item Name
                var nameLabel = new Label
                {
                    Text = req.ItemName,
                    Font = GameService.Content.DefaultFont12,
                    TextColor = req.GetRarityColor(),
                    Location = new Point(6, 6),
                    Width = 155,
                    Height = 16,
                    Parent = row
                };

                // Count
                string badge = req.IsBitCompleted ? "✓" : $"{req.OwnedCount}/{req.RequiredCount}";
                Color badgeColor = req.IsCompleted ? Color.LightGreen : (req.OwnedCount > 0 ? Color.Gold : new Color(255, 110, 110));

                var countLabel = new Label
                {
                    Text = badge,
                    Font = GameService.Content.DefaultFont12,
                    TextColor = badgeColor,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Location = new Point(165, 6),
                    Width = 75,
                    Height = 16,
                    Parent = row
                };

                string tooltip = $"{req.ItemName}\nOwned: {req.OwnedCount} / {req.RequiredCount}\n(Bags: {req.InventoryCount}, Bank: {req.BankCount}, Materials: {req.MaterialsCount}, Shared: {req.SharedInventoryCount})";
                row.BasicTooltipText = tooltip;
                nameLabel.BasicTooltipText = tooltip;
                countLabel.BasicTooltipText = tooltip;
            }

            Visible = true;
        }

        protected override void OnLeftMouseButtonPressed(MouseEventArgs e)
        {
            _isDragging = true;
            _dragOffset = Input.Mouse.Position - Location;
            base.OnLeftMouseButtonPressed(e);
        }

        protected override void OnLeftMouseButtonReleased(MouseEventArgs e)
        {
            _isDragging = false;
            base.OnLeftMouseButtonReleased(e);
        }

        protected override void OnMouseMoved(MouseEventArgs e)
        {
            if (_isDragging)
            {
                Location = Input.Mouse.Position - _dragOffset;
            }
            base.OnMouseMoved(e);
        }
    }
}
