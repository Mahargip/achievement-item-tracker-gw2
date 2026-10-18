using System;
using System.Collections.Generic;

namespace AchievementItemTracker.Models
{
    public class AccountInventorySnapshot
    {
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        public Dictionary<int, int> MaterialCounts { get; set; } = new Dictionary<int, int>();
        public Dictionary<int, int> BankCounts { get; set; } = new Dictionary<int, int>();
        public Dictionary<int, int> SharedCounts { get; set; } = new Dictionary<int, int>();
        public Dictionary<int, int> CharacterCounts { get; set; } = new Dictionary<int, int>();
        public Dictionary<int, int> TotalCounts { get; set; } = new Dictionary<int, int>();

        public Dictionary<int, HashSet<int>> CompletedBitsByAchievement { get; set; } = new Dictionary<int, HashSet<int>>();
        public HashSet<int> CompletedAchievementIds { get; set; } = new HashSet<int>();

        public int GetItemTotal(int itemId)
        {
            return TotalCounts.TryGetValue(itemId, out int count) ? count : 0;
        }

        public (int inventory, int bank, int materials, int shared, int total) GetBreakdown(int itemId)
        {
            MaterialCounts.TryGetValue(itemId, out int mat);
            BankCounts.TryGetValue(itemId, out int bank);
            SharedCounts.TryGetValue(itemId, out int shared);
            CharacterCounts.TryGetValue(itemId, out int inv);
            int total = mat + bank + shared + inv;
            return (inv, bank, mat, shared, total);
        }

        public bool IsBitCompleted(int achievementId, int bitIndex)
        {
            if (CompletedAchievementIds.Contains(achievementId))
            {
                return true;
            }

            return CompletedBitsByAchievement.TryGetValue(achievementId, out var bits) && bits.Contains(bitIndex);
        }
    }
}
