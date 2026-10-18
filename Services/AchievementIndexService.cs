using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AchievementItemTracker.Models;
using Blish_HUD;
using Blish_HUD.Modules.Managers;
using Newtonsoft.Json;

namespace AchievementItemTracker.Services
{
    public class AchievementIndexService
    {
        private static readonly Logger Logger = Logger.GetLogger<AchievementIndexService>();
        private readonly List<AchievementIndexItem> _items = new List<AchievementIndexItem>();
        private bool _isLoaded;

        public bool IsLoaded => _isLoaded;
        public int TotalCount => _items.Count;

        public void LoadIndex(ContentsManager contentsManager, string moduleDir)
        {
            try
            {
                string json = string.Empty;

                // First try contents manager (packaged ref folder)
                if (contentsManager != null)
                {
                    try
                    {
                        using (var stream = contentsManager.GetFileStream("achievements.json"))
                        {
                            if (stream != null)
                            {
                                using (var reader = new StreamReader(stream))
                                {
                                    json = reader.ReadToEnd();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex, "Could not load achievements.json from ContentsManager, trying local path.");
                    }
                }

                // Fallback to local ref folder if running unzipped or in dev
                if (string.IsNullOrEmpty(json))
                {
                    string localPath = Path.Combine(moduleDir, "ref", "achievements.json");
                    if (File.Exists(localPath))
                    {
                        json = File.ReadAllText(localPath);
                    }
                }

                if (!string.IsNullOrEmpty(json))
                {
                    var parsed = JsonConvert.DeserializeObject<List<AchievementIndexItem>>(json);
                    if (parsed != null)
                    {
                        _items.Clear();
                        _items.AddRange(parsed);
                        _isLoaded = true;
                        Logger.Info($"Loaded {_items.Count} achievements into search index.");
                    }
                }
                else
                {
                    Logger.Warn("achievements.json not found in ContentsManager or local directory.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to load achievements index.");
            }
        }

        public IReadOnlyList<AchievementIndexItem> Search(string query, bool onlyItemAchievements = false, int maxResults = 30)
        {
            if (!_isLoaded || _items.Count == 0)
            {
                return Array.Empty<AchievementIndexItem>();
            }

            if (string.IsNullOrWhiteSpace(query))
            {
                // Return interesting collection achievements by default
                return _items
                    .Where(a => a.ItemCount > 0)
                    .OrderByDescending(a => a.ItemCount)
                    .Take(maxResults)
                    .ToList();
            }

            string cleanQuery = query.Trim().ToLowerInvariant();
            string[] queryWords = cleanQuery.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var filtered = _items.Where(a =>
            {
                if (onlyItemAchievements && a.ItemCount == 0)
                {
                    return false;
                }

                string nameLower = a.Name.ToLowerInvariant();
                return queryWords.All(word => nameLower.Contains(word));
            });

            // Score and sort results:
            // 1. Exact match on name
            // 2. Starts with query
            // 3. Has item bits
            // 4. Alphabetical
            return filtered
                .OrderByDescending(a => a.Name.Equals(cleanQuery, StringComparison.OrdinalIgnoreCase))
                .ThenByDescending(a => a.Name.StartsWith(cleanQuery, StringComparison.OrdinalIgnoreCase))
                .ThenByDescending(a => a.ItemCount > 0)
                .ThenBy(a => a.Name)
                .Take(maxResults)
                .ToList();
        }

        public AchievementIndexItem? GetById(int id)
        {
            return _items.FirstOrDefault(a => a.Id == id);
        }
    }
}
