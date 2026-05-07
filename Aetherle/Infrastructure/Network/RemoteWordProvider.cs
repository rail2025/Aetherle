using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using Aetherle.Core.Models;

namespace Aetherle.Infrastructure.Network;

public class RemoteWordProvider
{
    private readonly HttpClient client = new();

    public async Task<DailyPuzzle> FetchDailyPuzzleAsync()
    {
        try
        {
            var response = await client.GetStringAsync("https://rail2025.github.io/puzzle.json");
            var puzzle = JsonSerializer.Deserialize<DailyPuzzle>(response);
            return puzzle ?? throw new Exception("Failed to deserialize remote puzzle.");
        }
        catch
        {
            var jsonText = await File.ReadAllTextAsync(Path.Combine("Data", "backup_words.json"));
            var data = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, List<string>>>>(jsonText);

            if (data == null)
            {
                return new DailyPuzzle { Word = "AETHER", Metadata = new PuzzleMetadata { Category = "System", WordLength = 6 } };
            }

            var wordListWithCategories = data.SelectMany(categoryItem =>
                categoryItem.Value?.SelectMany(lengthItem =>
                    lengthItem.Value?.Select(wordText => new { Category = categoryItem.Key, Word = wordText }) ?? Enumerable.Empty<dynamic>()
                ) ?? Enumerable.Empty<dynamic>()
            ).ToList();

            if (wordListWithCategories.Count == 0)
            {
                return new DailyPuzzle { Word = "AETHER", Metadata = new PuzzleMetadata { Category = "System", WordLength = 6 } };
            }

            int index = Math.Abs(DateTime.UtcNow.Date.GetHashCode()) % wordListWithCategories.Count;
            var chosenWord = wordListWithCategories[index];

            return new DailyPuzzle
            {
                Word = chosenWord.Word,
                Metadata = new PuzzleMetadata
                {
                    Category = chosenWord.Category,
                    WordLength = chosenWord.Word.Length
                }
            };
        }
    }
}
