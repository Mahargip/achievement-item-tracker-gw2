using System.Collections.Generic;
using Newtonsoft.Json;

namespace AchievementItemTracker.Models
{
    public class AchievementIndexItem
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("req")]
        public string Requirement { get; set; } = string.Empty;

        [JsonProperty("type")]
        public string Type { get; set; } = "Default";

        [JsonProperty("item_count")]
        public int ItemCount { get; set; }

        [JsonProperty("total_bits")]
        public int TotalBits { get; set; }
    }

    public class AchievementDetail
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Requirement { get; set; } = string.Empty;
        public string? LockedText { get; set; }
        public string Type { get; set; } = "Default";
        public List<AchievementBitDetail> Bits { get; set; } = new List<AchievementBitDetail>();
        public int? CurrentProgress { get; set; }
        public int? MaxProgress { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class AchievementBitDetail
    {
        public int Index { get; set; }
        public string Type { get; set; } = string.Empty;
        public int? ItemId { get; set; }
        public string? Text { get; set; }
        public bool IsCompleted { get; set; }
    }
}
