using Microsoft.Xna.Framework;

namespace AchievementItemTracker.Models
{
    public class AchievementItemRequirement
    {
        public int BitIndex { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string? IconUrl { get; set; }
        public string Rarity { get; set; } = "Basic";
        public int RequiredCount { get; set; } = 1;

        public int InventoryCount { get; set; }
        public int BankCount { get; set; }
        public int MaterialsCount { get; set; }
        public int SharedInventoryCount { get; set; }
        public int OwnedCount => InventoryCount + BankCount + MaterialsCount + SharedInventoryCount;

        public bool IsBitCompleted { get; set; }
        public bool IsCompleted => IsBitCompleted || (OwnedCount >= RequiredCount && RequiredCount > 0);

        public string? HintText { get; set; }

        public Color GetRarityColor()
        {
            switch (Rarity?.ToLowerInvariant())
            {
                case "junk":
                    return new Color(170, 170, 170);
                case "basic":
                    return Color.White;
                case "fine":
                    return new Color(98, 164, 218); // Blue
                case "masterwork":
                    return new Color(26, 147, 5);   // Green
                case "rare":
                    return new Color(255, 210, 0);  // Yellow/Gold
                case "exotic":
                    return new Color(255, 164, 5);  // Orange
                case "ascended":
                    return new Color(251, 62, 141); // Pink/Magenta
                case "legendary":
                    return new Color(155, 107, 255);// Purple
                default:
                    return Color.White;
            }
        }
    }
}
