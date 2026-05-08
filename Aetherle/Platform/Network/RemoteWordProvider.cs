using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using Aetherle.Core.Models;

namespace Aetherle.Platform.Network;

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
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("Aetherle.Data.backup_words.json");
            using var reader = new System.IO.StreamReader(stream!);
            var jsonText = await reader.ReadToEndAsync();
            var data = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, List<string>>>>(jsonText);

            if (data == null)
            {
                return new DailyPuzzle { Word = "AETHER", Metadata = new PuzzleMetadata { Category = "System", WordLength = 6 } };
            }

            var wordListWithCategories = new List<(string Category, string Word)>();
            foreach (var category in data)
            {
                foreach (var lengthGroup in category.Value)
                {
                    foreach (var word in lengthGroup.Value)
                    {
                        wordListWithCategories.Add((category.Key, word));
                    }
                }
            }

            if (wordListWithCategories.Count == 0)
            {
                return new DailyPuzzle { Word = "AETHER", Metadata = new PuzzleMetadata { Category = "System", WordLength = 6 } };
            }

            int dayOffset = (int)(DateTime.UtcNow.Date - new DateTime(2024, 1, 1)).TotalDays;
            int index = dayOffset % wordListWithCategories.Count;
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
