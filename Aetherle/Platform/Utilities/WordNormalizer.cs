using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Aetherle.Platform.Utilities;

public static class WordNormalizer
{
    public static string Normalize(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        var upper = input.ToUpperInvariant();
        var noPunctuation = Regex.Replace(upper, "['\\-]", string.Empty);
        var trimmed = noPunctuation.Trim();

        return Regex.Replace(trimmed, "[^A-Z]", string.Empty);
    }
}
