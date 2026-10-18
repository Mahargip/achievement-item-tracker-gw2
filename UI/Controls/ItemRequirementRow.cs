using System;
using AchievementItemTracker.Models;
using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AchievementItemTracker.UI.Controls
{
    public class ItemRequirementRow : Panel
    {
        private readonly AchievementItemRequirement _requirement;
        private readonly Image _itemIcon;
        private readonly Label _nameLabel;
        private readonly Label _hintLabel;
        private readonly Label _countLabel;

        public AchievementItemRequirement Requirement => _requirement;

        public ItemRequirementRow(AchievementItemRequirement req, int width)
        {
            _requirement = req;

            Width = width;
            Height = 46;
            BackgroundColor = new Color(22, 22, 26, 200);

            // Item Icon
            _itemIcon = new Image
            {
                Size = new Point(32, 32),
                Location = new Point(7, 7),
                Parent = this
            };

            if (!string.IsNullOrEmpty(req.IconUrl))
            {
                _itemIcon.Texture = AsyncTexture2D.FromAssetId(0); // placeholder or load url
                try
                {
                    // Gw2Sharp icon url loading
                    _itemIcon.Texture = GameService.Content.GetRenderServiceTexture(req.IconUrl);
                }
                catch
                {
                    _itemIcon.Texture = ContentService.Textures.Error;
                }
            }

            // Item Name
            _nameLabel = new Label
            {
                Text = req.ItemName,
                TextColor = req.GetRarityColor(),
                Font = GameService.Content.DefaultFont14,
                Location = new Point(48, 6),
                Width = Math.Max(100, width - 200),
                Height = 18,
                Parent = this
            };

            // Hint / Text (sub-line)
            string hint = string.IsNullOrWhiteSpace(req.HintText) ? $"Item ID: {req.ItemId}" : req.HintText!;
            _hintLabel = new Label
            {
                Text = hint,
                TextColor = Color.LightGray * 0.75f,
                Font = GameService.Content.DefaultFont12,
                Location = new Point(48, 24),
                Width = Math.Max(100, width - 200),
                Height = 16,
                Parent = this
            };

            // Count Badge (e.g. 0 / 5)
            string countText;
            Color countColor;

            if (req.IsBitCompleted)
            {
                countText = "[ COMPLETED ✓ ]";
                countColor = Color.LightGreen;
            }
            else if (req.IsCompleted)
            {
                countText = $"[ {req.OwnedCount} / {req.RequiredCount} ✓ ]";
                countColor = Color.LightGreen;
            }
            else
            {
                countText = $"[ {req.OwnedCount} / {req.RequiredCount} ]";
                countColor = req.OwnedCount > 0 ? Color.Gold : new Color(255, 110, 110);
            }

            _countLabel = new Label
            {
                Text = countText,
                TextColor = countColor,
                Font = GameService.Content.DefaultFont14,
                HorizontalAlignment = HorizontalAlignment.Right,
                Location = new Point(width - 150, 12),
                Width = 140,
                Height = 22,
                Parent = this
            };

            // Detailed Hover Tooltip
            string tooltip = $"{req.ItemName} ({req.Rarity})\n" +
                             $"Required: {req.RequiredCount}\n" +
                             "------------------------------------\n" +
                             $"In Character Bags:    {req.InventoryCount}\n" +
                             $"In Bank:              {req.BankCount}\n" +
                             $"In Material Storage:  {req.MaterialsCount}\n" +
                             $"In Shared Slots:      {req.SharedInventoryCount}\n" +
                             "------------------------------------\n" +
                             $"Total Owned:          {req.OwnedCount} / {req.RequiredCount}\n" +
                             $"Status: {(req.IsCompleted ? "Ready / Finished" : "Incomplete")}";

            BasicTooltipText = tooltip;
            _itemIcon.BasicTooltipText = tooltip;
            _nameLabel.BasicTooltipText = tooltip;
            _countLabel.BasicTooltipText = tooltip;
        }

        protected override void OnMouseEntered(MouseEventArgs e)
        {
            BackgroundColor = new Color(40, 42, 50, 230);
            base.OnMouseEntered(e);
        }

        protected override void OnMouseLeft(MouseEventArgs e)
        {
            BackgroundColor = new Color(22, 22, 26, 200);
            base.OnMouseLeft(e);
        }
    }
}
