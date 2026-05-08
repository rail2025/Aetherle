using System;
using System.Collections.Generic;
using System.Text;

namespace Aetherle.Core.Models;

public sealed class DailyPuzzle
{
    public string Word { get; init; } = string.Empty;
    public PuzzleMetadata Metadata { get; init; } = new();
}
