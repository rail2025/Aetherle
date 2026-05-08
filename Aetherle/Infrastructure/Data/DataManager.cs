using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Aetherle.Core.Models;

namespace Aetherle.Infrastructure.Data;

public class DataManager
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    public string ConfigDirectory { get; }
    private readonly string cachePath;

    public DataManager(string configDir)
    {
        ConfigDirectory = configDir;
        cachePath = Path.Combine(configDir, "daily_cache.json");
    }
    private List<string> wordList = new();

    public List<string> GetAllWords()
    {
        return this.wordList ?? new List<string>();
    }

    public async Task<List<string>> LoadBackupWordsAsync()
    {
        string path = Path.Combine(this.ConfigDirectory, "backup_words.json");
        if (!File.Exists(path)) return new List<string>();

        try
        {
            var json = await File.ReadAllTextAsync(path);
            var data = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, List<string>>>>(json, JsonOptions);

            this.wordList.Clear();
            if (data != null)
            {
                foreach (var category in data.Values)
                {
                    foreach (var lengthGroup in category.Values)
                    {
                        this.wordList.AddRange(lengthGroup);
                    }
                }
            }
            return this.wordList;
        }
        catch (Exception ex)
        {
            Plugin.Log.Error($"Failed to load backup words: {ex.Message}");
            return new List<string>();
        }
    }

    public async Task CacheDailyPuzzleAsync(DailyPuzzle puzzle)
    {
        try
        {
            var json = JsonSerializer.Serialize(puzzle, JsonOptions);
            await File.WriteAllTextAsync(this.cachePath, json);
        }
        catch (Exception ex)
        {
            Plugin.Log.Debug($"Failed to cache daily puzzle: {ex.Message}");
        }
    }

    public async Task<DailyPuzzle?> LoadCachedPuzzleAsync()
    {
        if (!File.Exists(this.cachePath)) return null;

        try
        {
            var json = await File.ReadAllTextAsync(this.cachePath);
            return JsonSerializer.Deserialize<DailyPuzzle>(json, JsonOptions);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
