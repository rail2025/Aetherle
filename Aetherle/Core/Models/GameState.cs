using System.Collections.Generic;

namespace Aetherle.Core.Models;

public class Letter
{
    public char Value { get; init; }
    public LetterState State { get; init; }

    public Letter(char value, LetterState state)
    {
        Value = value;
        State = state;
    }
}

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
