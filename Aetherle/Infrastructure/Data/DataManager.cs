using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Aetherle.Core.Models;

namespace Aetherle.Infrastructure.Data;

public class DataManager(string configDir)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private readonly string configDirectory = configDir;
    private readonly string cachePath = Path.Combine(configDir, "daily_cache.json");

    public async Task<List<string>> LoadWordsFromFileAsync(string fileName)
    {
        string path = Path.Combine(this.configDirectory, fileName);
        if (!File.Exists(path)) return new List<string>();

        try
        {
            var json = await File.ReadAllTextAsync(path);
            return JsonSerializer.Deserialize<List<string>>(json, JsonOptions) ?? new List<string>();
        }
        catch (JsonException)
        {
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
