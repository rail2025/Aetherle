using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Aetherle.Core.Rules;

public class GuessValidator
{
    private HashSet<string> allowedGuesses = new(StringComparer.OrdinalIgnoreCase);

    public async Task LoadDictionaryAsync()
    {
        try
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("Aetherle.Data.allowed_guesses.json");

            if (stream == null) return;

            using var reader = new StreamReader(stream);
            var jsonText = await reader.ReadToEndAsync();

            var data = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, List<string>>>>(jsonText);

            allowedGuesses.Clear();
            if (data != null)
            {
                foreach (var category in data.Values)
                {
                    foreach (var lengthGroup in category.Values)
                    {
                        foreach (var word in lengthGroup)
                        {
                            allowedGuesses.Add(word);
                        }
                    }
                }
            }
        }
        catch
        {
            allowedGuesses = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }
    }

    public bool IsValidGuess(string guess)
    {
        if (string.IsNullOrWhiteSpace(guess)) return false;

        string sanitized = guess.Trim().ToUpper();

        if (allowedGuesses.Count == 0)
        {
            return sanitized.Length >= 5 && sanitized.Length <= 8 && sanitized.All(char.IsLetter);
        }

        return allowedGuesses.Contains(sanitized);
    }
}
