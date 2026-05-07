using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using System.Collections.Generic;
using System.Text;

namespace Aetherle.Core.Services;

public class ShareResultService
{
    public static void CopyToClipboard(string word, List<string> guesses)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Aetherle: {word.Length} Letters");

        foreach (var guess in guesses)
        {
            for (int i = 0; i < word.Length; i++)
            {
                if (guess[i] == word[i]) sb.Append("🟩");
                else if (word.Contains(guess[i])) sb.Append("🟨");
                else sb.Append("⬛");
            }
            sb.AppendLine();
        }

        ImGui.SetClipboardText(sb.ToString());
    }
}
