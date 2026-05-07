using System;
using System.Linq;
using Aetherle.Core.Models;

namespace Aetherle.Core.Services;

public class GameSessionService
{
    private readonly Plugin plugin;
    private string targetWord = string.Empty;
    public GameState State { get; private set; } = new();

    public GameSessionService(Plugin plugin) => this.plugin = plugin;

    public void StartNewSession(string word)
    {
        targetWord = word.ToUpper();
        State = new GameState();
    }

    public GameState GetCurrentState() => State;

    public void AddLetter(char c)
    {
        if (State.IsComplete || State.CurrentGuess.Length >= targetWord.Length) return;
        State.CurrentGuess += char.ToUpper(c);
    }

    public void RemoveLetter()
    {
        if (State.IsComplete || State.CurrentGuess.Length == 0) return;
        State.CurrentGuess = State.CurrentGuess[..^1];
    }

    public void SubmitGuess()
    {
        if (State.IsComplete || State.CurrentGuess.Length != targetWord.Length) return;

        var guess = new Guess();
        for (int i = 0; i < targetWord.Length; i++)
        {
            char c = State.CurrentGuess[i];
            LetterState s = LetterState.Absent;

            if (c == targetWord[i]) s = LetterState.Correct;
            else if (targetWord.Contains(c)) s = LetterState.Present;

            guess.Letters.Add(new Letter(c, s));
        }

        State.Guesses.Add(guess);
        State.CurrentGuess = "";

        if (guess.Letters.All(l => l.State == LetterState.Correct) || State.Guesses.Count >= 6)
        {
            State.IsWin = guess.Letters.All(l => l.State == LetterState.Correct);
            State.IsComplete = true;

            var cfg = this.plugin.Configuration;
            if (cfg.LastPlayDate.Date != DateTime.UtcNow.Date)
            {
                cfg.DailyGamesCompletions = 0;
                cfg.LastPlayDate = DateTime.UtcNow;
            }

            cfg.DailyGamesCompletions++;
            UpdateStats(State.IsWin);
        }
    }

    private void UpdateStats(bool won)
    {
        var cfg = this.plugin.Configuration;
        cfg.GamesPlayed++;
        if (won)
        {
            cfg.GamesWon++;
            cfg.CurrentStreak++;
        }
        else
        {
            cfg.CurrentStreak = 0;
        }
        Plugin.PluginInterface.SavePluginConfig(cfg);
    }
}
