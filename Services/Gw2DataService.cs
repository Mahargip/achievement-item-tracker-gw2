using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AchievementItemTracker.Models;
using Blish_HUD;
using Blish_HUD.Modules.Managers;
using Gw2Sharp.WebApi.V2.Models;

namespace AchievementItemTracker.Services
{
    public class Gw2DataService
    {
        private static readonly Logger Logger = Logger.GetLogger<Gw2DataService>();
        private readonly Gw2ApiManager? _gw2ApiManager;

        private static readonly TokenPermission[] RequiredPermissions = new[]
        {
            TokenPermission.Account,
            TokenPermission.Inventories,
            TokenPermission.Characters,
            TokenPermission.Progression
        };

        private readonly Dictionary<int, Item> _itemCache = new Dictionary<int, Item>();
        private readonly Dictionary<int, Achievement> _achievementCache = new Dictionary<int, Achievement>();

        public AccountInventorySnapshot CurrentSnapshot { get; private set; } = new AccountInventorySnapshot();

        public event EventHandler<AccountInventorySnapshot>? InventoryRefreshed;
        public event EventHandler<string>? SyncStatusChanged;

        public bool IsSyncing { get; private set; }
        public string LastSyncMessage { get; private set; } = "Not synced yet";

        public Gw2DataService(Gw2ApiManager gw2ApiManager)
        {
            _gw2ApiManager = gw2ApiManager;
            if (_gw2ApiManager != null)
            {
                _gw2ApiManager.SubtokenUpdated += OnSubtokenUpdated;
            }
        }

        private void OnSubtokenUpdated(object sender, ValueEventArgs<IEnumerable<TokenPermission>> e)
        {
            Logger.Info("GW2 API Subtoken updated. Triggering inventory refresh.");
            Task.Run(RefreshInventoryAsync);
        }

        public bool HasPermissions()
        {
            return _gw2ApiManager != null && _gw2ApiManager.HasPermissions(RequiredPermissions);
        }

        public async Task<AccountInventorySnapshot?> RefreshInventoryAsync()
        {
            if (IsSyncing)
            {
                return CurrentSnapshot;
            }

            IsSyncing = true;
            UpdateStatus("Syncing inventory from GW2 API...");

            try
            {
                if (!HasPermissions())
                {
                    UpdateStatus("Missing API permissions (Inventories, Characters, Progression).");
                    IsSyncing = false;
                    return null;
                }

                var client = _gw2ApiManager?.Gw2ApiClient;
                if (client == null)
                {
                    UpdateStatus("API client unavailable.");
                    IsSyncing = false;
                    return null;
                }

                var snapshot = new AccountInventorySnapshot
                {
                    LastUpdated = DateTime.UtcNow
                };

                // 1. Fetch Material Storage
                try
                {
                    var materials = await client.V2.Account.Materials.GetAsync();
                    if (materials != null)
                    {
                        foreach (var mat in materials)
                        {
                            if (mat != null && mat.Count > 0)
                            {
                                AddCount(snapshot.MaterialCounts, mat.Id, mat.Count);
                                AddCount(snapshot.TotalCounts, mat.Id, mat.Count);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "Failed to fetch account materials.");
                }

                // 2. Fetch Bank
                try
                {
                    var bank = await client.V2.Account.Bank.GetAsync();
                    if (bank != null)
                    {
                        foreach (var item in bank)
                        {
                            if (item != null && item.Count > 0)
                            {
                                AddCount(snapshot.BankCounts, item.Id, item.Count);
                                AddCount(snapshot.TotalCounts, item.Id, item.Count);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "Failed to fetch account bank.");
                }

                // 3. Fetch Shared Inventory Slots
                try
                {
                    var shared = await client.V2.Account.Inventory.GetAsync();
                    if (shared != null)
                    {
                        foreach (var item in shared)
                        {
                            if (item != null && item.Count > 0)
                            {
                                AddCount(snapshot.SharedCounts, item.Id, item.Count);
                                AddCount(snapshot.TotalCounts, item.Id, item.Count);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "Failed to fetch shared inventory.");
                }

                // 4. Fetch Character Inventories
                try
                {
                    var characters = await client.V2.Characters.AllAsync();
                    if (characters != null)
                    {
                        foreach (var character in characters)
                        {
                            if (character?.Bags == null) continue;

                            foreach (var bag in character.Bags)
                            {
                                if (bag?.Inventory == null) continue;

                                foreach (var item in bag.Inventory)
                                {
                                    if (item != null && item.Count > 0)
                                    {
                                        AddCount(snapshot.CharacterCounts, item.Id, item.Count);
                                        AddCount(snapshot.TotalCounts, item.Id, item.Count);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "Failed to fetch character inventories.");
                }

                // 5. Fetch Account Achievements Progression
                try
                {
                    var accountAchievements = await client.V2.Account.Achievements.GetAsync();
                    if (accountAchievements != null)
                    {
                        foreach (var ach in accountAchievements)
                        {
                            if (ach == null) continue;

                            if (ach.Done)
                            {
                                snapshot.CompletedAchievementIds.Add(ach.Id);
                            }

                            if (ach.Bits != null && ach.Bits.Count > 0)
                            {
                                snapshot.CompletedBitsByAchievement[ach.Id] = new HashSet<int>(ach.Bits);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "Failed to fetch account achievements progress.");
                }

                CurrentSnapshot = snapshot;
                UpdateStatus($"Synced at {DateTime.Now:HH:mm:ss} ({snapshot.TotalCounts.Count} unique items tracked)");
                InventoryRefreshed?.Invoke(this, snapshot);
                return snapshot;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error refreshing inventory snapshot.");
                UpdateStatus($"Sync failed: {ex.Message}");
                return null;
            }
            finally
            {
                IsSyncing = false;
            }
        }

        public async Task<Achievement?> GetAchievementAsync(int achievementId)
        {
            if (_achievementCache.TryGetValue(achievementId, out var cached))
            {
                return cached;
            }

            try
            {
                var client = _gw2ApiManager?.Gw2ApiClient;
                if (client == null) return null;

                var ach = await client.V2.Achievements.GetAsync(achievementId);
                if (ach != null)
                {
                    _achievementCache[achievementId] = ach;
                }
                return ach;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Failed to fetch achievement details for ID {achievementId}");
                return null;
            }
        }

        public async Task<List<AchievementItemRequirement>> GetAchievementRequirementsAsync(int achievementId)
        {
            var results = new List<AchievementItemRequirement>();
            var ach = await GetAchievementAsync(achievementId);
            if (ach == null || ach.Bits == null)
            {
                return results;
            }

            // Gather item IDs from bits
            var itemBitsToFetch = new List<(int bitIndex, int itemId, string? text)>();

            for (int i = 0; i < ach.Bits.Count; i++)
            {
                var bit = ach.Bits[i];
                if (bit is AchievementItemBit itemBit)
                {
                    itemBitsToFetch.Add((i, itemBit.Id, null));
                }
            }

            if (itemBitsToFetch.Count == 0)
            {
                return results;
            }

            // Bulk fetch missing item definitions
            var missingIds = itemBitsToFetch
                .Select(b => b.itemId)
                .Distinct()
                .Where(id => !_itemCache.ContainsKey(id))
                .ToList();

            if (missingIds.Count > 0)
            {
                try
                {
                    var client = _gw2ApiManager?.Gw2ApiClient;
                    if (client != null)
                    {
                        var fetchedItems = await client.V2.Items.ManyAsync(missingIds);
                        if (fetchedItems != null)
                        {
                            foreach (var item in fetchedItems)
                            {
                                if (item != null)
                                {
                                    _itemCache[item.Id] = item;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, $"Failed to fetch {missingIds.Count} items from GW2 API.");
                }
            }

            // Build requirement objects
            foreach (var (bitIndex, itemId, text) in itemBitsToFetch)
            {
                _itemCache.TryGetValue(itemId, out var itemDef);
                var breakdown = CurrentSnapshot.GetBreakdown(itemId);
                bool isBitDone = CurrentSnapshot.IsBitCompleted(achievementId, bitIndex);

                int requiredCount = ParseRequiredCount(text);

                var req = new AchievementItemRequirement
                {
                    BitIndex = bitIndex,
                    ItemId = itemId,
                    ItemName = itemDef?.Name ?? $"Item #{itemId}",
                    IconUrl = itemDef != null ? itemDef.Icon.Url.ToString() : null,
                    Rarity = itemDef?.Rarity?.Value.ToString() ?? "Basic",
                    RequiredCount = requiredCount,
                    InventoryCount = breakdown.inventory,
                    BankCount = breakdown.bank,
                    MaterialsCount = breakdown.materials,
                    SharedInventoryCount = breakdown.shared,
                    IsBitCompleted = isBitDone,
                    HintText = text
                };

                results.Add(req);
            }

            return results;
        }

        private static int ParseRequiredCount(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 1;

            // Matches e.g. "Bring 12 Pieces of Skyscale Food", "Collect 5 Charged Quartz", "Deliver 250 ..."
            var match = Regex.Match(text, @"(?:Bring|Collect|Deliver|Need|Give|Purchase|Obtain)?\s*(\d+)\s+", RegexOptions.IgnoreCase);
            if (match.Success && int.TryParse(match.Groups[1].Value, out int count) && count > 0 && count < 100000)
            {
                return count;
            }

            return 1;
        }

        private static void AddCount(Dictionary<int, int> dict, int id, int count)
        {
            if (dict.TryGetValue(id, out int existing))
            {
                dict[id] = existing + count;
            }
            else
            {
                dict[id] = count;
            }
        }

        private void UpdateStatus(string message)
        {
            LastSyncMessage = message;
            SyncStatusChanged?.Invoke(this, message);
        }

        public void Dispose()
        {
            if (_gw2ApiManager != null)
            {
                _gw2ApiManager.SubtokenUpdated -= OnSubtokenUpdated;
            }
        }
    }
}
