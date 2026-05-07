using System;
using System.Collections.Generic;
using System.Text;

namespace Aetherle.Core.Models;

public sealed class PuzzleMetadata
{
    public string Category { get; init; } = "General";
    public string Hint { get; init; } = string.Empty;
    public int WordLength { get; init; } = 5;
    public string Difficulty { get; init; } = "Normal";
}
