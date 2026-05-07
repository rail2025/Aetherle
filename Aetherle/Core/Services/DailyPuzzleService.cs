using System;
using System.Linq;
using System.Threading.Tasks;
using Aetherle.Core.Models;
using Aetherle.Infrastructure.Network;
using Aetherle.Infrastructure.Data;

namespace Aetherle.Core.Services;

public class DailyPuzzleService
{
    private readonly RemoteWordProvider provider;
    private readonly DataManager data;

    public DailyPuzzleService(DataManager data, RemoteWordProvider provider)
    {
        this.data = data;
        this.provider = provider;
    }

    public async Task<DailyPuzzle> GetTodayPuzzleAsync()
    {
        try
        {
            return await provider.FetchDailyPuzzleAsync();
        }
        catch (Exception ex) when (ex is System.Net.Http.HttpRequestException or System.Text.Json.JsonException)
        {
            var words = await this.data.LoadWordsFromFileAsync("backup_words.json");

            if (words.Count == 0)
            {
                return new DailyPuzzle
                {
                    Word = "AETHER",
                    Metadata = new PuzzleMetadata { Category = "System", WordLength = 6 }
                };
            }

            int dayOffset = (int)(DateTime.UtcNow.Date - new DateTime(2024, 1, 1)).TotalDays;
            string selectedWord = words[dayOffset % words.Count].ToUpper();

            return new DailyPuzzle
            {
                Word = selectedWord,
                Metadata = new PuzzleMetadata
                {
                    Category = "FFXIV",
                    WordLength = selectedWord.Length
                }
            };
        }
    }

    public static int GetDeterministicSeed()
    {
        return Math.Abs(DateTime.UtcNow.Date.GetHashCode());
    }
}
