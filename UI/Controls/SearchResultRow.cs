using System;
using AchievementItemTracker.Models;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Microsoft.Xna.Framework;

namespace AchievementItemTracker.UI.Controls
{
    public class SearchResultRow : Panel
    {
        private readonly AchievementIndexItem _item;
        private readonly Label _nameLabel;
        private readonly Label _itemCountLabel;

        public event EventHandler<AchievementIndexItem>? Selected;

        public SearchResultRow(AchievementIndexItem item, int width)
        {
            _item = item;
            Width = width;
            Height = 36;
            BackgroundColor = new Color(20, 20, 24, 180);

            _nameLabel = new Label
            {
                Text = item.Name,
                Font = GameService.Content.DefaultFont14,
                TextColor = Color.White,
                Location = new Point(12, 8),
                Width = Math.Max(100, width - 110),
                Height = 20,
                Parent = this
            };

            string countText = item.ItemCount > 0 ? $"{item.ItemCount} items" : "Achievement";
            Color countColor = item.ItemCount > 0 ? Color.Gold : Color.LightGray * 0.7f;

            _itemCountLabel = new Label
            {
                Text = countText,
                Font = GameService.Content.DefaultFont12,
                TextColor = countColor,
                HorizontalAlignment = HorizontalAlignment.Right,
                Location = new Point(width - 95, 8),
                Width = 85,
                Height = 20,
                Parent = this
            };

            if (!string.IsNullOrWhiteSpace(item.Requirement))
            {
                BasicTooltipText = $"{item.Name}\n{item.Requirement}";
                _nameLabel.BasicTooltipText = BasicTooltipText;
            }

            Click += OnRowClicked;
            _nameLabel.Click += OnRowClicked;
            _itemCountLabel.Click += OnRowClicked;
        }

        private void OnRowClicked(object sender, MouseEventArgs e)
        {
            Selected?.Invoke(this, _item);
        }

        protected override void OnMouseEntered(MouseEventArgs e)
        {
            BackgroundColor = new Color(48, 52, 65, 230);
            base.OnMouseEntered(e);
        }

        protected override void OnMouseLeft(MouseEventArgs e)
        {
            BackgroundColor = new Color(20, 20, 24, 180);
            base.OnMouseLeft(e);
        }
    }
}
