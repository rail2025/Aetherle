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
            var jsonText = await File.ReadAllTextAsync(@"Data\allowed_guesses.json");
            var words = JsonSerializer.Deserialize<List<string>>(jsonText);
            if (words != null)
            {
                allowedGuesses = new HashSet<string>(words, StringComparer.OrdinalIgnoreCase);
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
