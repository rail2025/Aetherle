using System;
using System.Collections.Generic;
using System.Text;

namespace Aetherle.Core.Models;
public sealed class LetterResult
{
    public char Letter { get; init; }
    public LetterState State { get; init; }
}
