using System.Collections.Generic;
using System.Linq;
using Aetherle.Core.Models;

namespace Aetherle.Core.Rules;

public static class GuessEvaluator
{
    public static GuessResult Evaluate(string guess, string targetWord)
    {
        var results = new LetterState[guess.Length];
        var targetChars = targetWord.ToCharArray();

        for (int i = 0; i < guess.Length; i++)
        {
            if (i < targetWord.Length && guess[i] == targetWord[i])
            {
                results[i] = LetterState.Correct;
                targetChars[i] = '\0';
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

            for (int j = 0; j < targetChars.Length; j++)
            {
                if (targetChars[j] != '\0' && guess[i] == targetChars[j])
                {
                    results[i] = LetterState.Present;
                    targetChars[j] = '\0';
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
