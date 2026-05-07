using System.Collections.Generic;

namespace Aetherle.Core.Models;

public sealed class GuessResult
{
    public string Guess { get; init; } = string.Empty;
    public IReadOnlyList<LetterResult> Letters { get; init; } = [];
}
