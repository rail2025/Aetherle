using Dalamud.Configuration;
using System;
using System.Collections.Generic;

namespace Aetherle;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public bool HotPinkMode { get; set; } = false;
    public bool HardMode { get; set; } = false;
    public int GamesPlayed { get; set; } = 0;
    public int GamesWon { get; set; } = 0;
    public int CurrentStreak { get; set; } = 0;
    public int MaxStreak { get; set; } = 0;
    public int WinPercentage => GamesPlayed == 0 ? 0 : (int)Math.Round((double)GamesWon / GamesPlayed * 100);

    public Dictionary<int, int> GuessDistribution { get; set; } = new()
    {
        { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }, { 6, 0 }
    };

    public int DailyGamesCompletions { get; set; } = 0;
    public DateTime LastPlayDate { get; set; } = DateTime.MinValue;

    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
