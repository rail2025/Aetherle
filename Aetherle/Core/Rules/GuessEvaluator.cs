using System.Collections.Generic;
using System.Linq;
using Aetherle.Core.Models;

namespace Aetherle.Core.Rules;

public static class GuessEvaluator
{
    public static GuessResult Evaluate(string guess, string targetWord)
    {
        var results = new LetterState[guess.Length];
        var targetSpan = targetWord.ToCharArray();

        for (int i = 0; i < guess.Length; i++)
        {
            if (i < targetWord.Length && guess[i] == targetWord[i])
            {
                results[i] = LetterState.Correct;
                targetSpan[i] = '\0';
            }
            else
            {
                results[i] = LetterState.Absent;
            }
        }

        for (int i = 0; i < guess.Length; i++)
        {
            if (results[i] == LetterState.Correct)
                continue;

            for (int j = 0; j < targetSpan.Length; j++)
            {
                if (targetSpan[j] != '\0' && guess[i] == targetSpan[j])
                {
                    results[i] = LetterState.Present;
                    targetSpan[j] = '\0';
                    break;
                }
            }
        }

        var letterResults = guess.Select((c, i) => new LetterResult
        {
            Letter = c,
            State = results[i]
        }).ToList();

        return new GuessResult
        {
            Guess = guess,
            Letters = letterResults
        };
    }
}
