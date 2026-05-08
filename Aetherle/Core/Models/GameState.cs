using System.Collections.Generic;

namespace Aetherle.Core.Models;

public record Letter(char Value, LetterState State);

public class Guess
{
    public List<Letter> Letters { get; set; } = new();
}

public class GameState
{
    public string CurrentGuess { get; set; } = string.Empty;
    public string TargetWord { get; set; }
    public GameState(string word = "")
    {
        TargetWord = word;
    }
    public List<Guess> Guesses { get; set; } = new();
    public bool IsComplete { get; set; }
    public bool IsWin { get; set; }
}
