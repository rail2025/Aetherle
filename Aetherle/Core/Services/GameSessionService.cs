using Aetherle.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Aetherle.Core.Services;

public class GameSessionService
{
    private readonly Plugin plugin;
    private string targetWord = string.Empty;
    public string TargetWord => targetWord;
    public string CurrentCategory { get; private set; } = "Unknown";
    public GameState State { get; private set; } = new();

    public GameSessionService(Plugin plugin) => this.plugin = plugin;

    public void StartNewSession(string word, string category = "Daily")
    {
        targetWord = word.ToUpper();
        CurrentCategory = category;
        State = new GameState(targetWord);
    }

    public GameState GetCurrentState() => State;

    public void StartRandomSession()
    {
        try
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("Aetherle.Data.backup_words.json");

            if (stream == null)
            {
                StartNewSession("AETHER", "System");
                return;
            }

            using var reader = new StreamReader(stream);
            var jsonText = reader.ReadToEnd();
            var data = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, List<string>>>>(jsonText);

            if (data == null)
            {
                StartNewSession("AETHER", "System");
                return;
            }

            var allWords = new List<(string Word, string Category)>();
            foreach (var category in data)
            {
                foreach (var lengthGroup in category.Value)
                {
                    foreach (var word in lengthGroup.Value)
                    {
                        allWords.Add((word, category.Key));
                    }
                }
            }

            var rand = new Random();
            var choice = allWords[rand.Next(allWords.Count)];
            StartNewSession(choice.Word, choice.Category);
        }
        catch
        {
            StartNewSession("AETHER", "System");
        }
    }

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
            cfg.LastPlayDate = DateTime.UtcNow;

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
