using Aetherle.Core.Models;
using Dalamud.Bindings.ImGui;
using Aetherle;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Aetherle.UI.Components;

public class KeyboardRenderer
{
    private readonly string[] rows = { "QWERTYUIOP", "ASDFGHJKL", "ZXCVBNM" };

    public void Draw(Plugin plugin, GameState state, Action<char> onKeyClick)
    {
        var letterStates = GetLetterStates(state);
        bool hotPink = plugin.Configuration.HotPinkMode;

        float spacing = ImGui.GetStyle().ItemSpacing.X;
        float keyWidth = 32f;
        float keyHeight = 40f;

        for (int i = 0; i < rows.Length; i++)
        {
            var row = rows[i];
            float rowWidth = (row.Length * keyWidth) + ((row.Length - 1) * spacing);

            if (i == 2) rowWidth += (keyWidth * 3f) + (spacing * 2);

            ImGui.SetCursorPosX((ImGui.GetWindowWidth() - rowWidth) * 0.5f);

            if (i == 2)
            {
                if (ImGui.Button("ENTER", new Vector2(keyWidth * 1.5f, keyHeight)))
                {
                    plugin.SessionService.SubmitGuess();
                }
                ImGui.SameLine();
            }

            foreach (var c in row)
            {
                letterStates.TryGetValue(c, out var s);

                ImGui.PushStyleColor(ImGuiCol.Button, GetColorForState(s, hotPink));
                if (ImGui.Button(c.ToString(), new Vector2(keyWidth, keyHeight)))
                {
                    onKeyClick(c);
                }
                ImGui.PopStyleColor();
                ImGui.SameLine();
            }

            if (i == 2)
            {
                if (ImGui.Button("BACK", new Vector2(keyWidth * 1.5f, keyHeight)))
                {
                    plugin.SessionService.RemoveLetter();
                }
            }
            ImGui.NewLine();
        }
    }

    private static Dictionary<char, LetterState> GetLetterStates(GameState state)
    {
        var results = new Dictionary<char, LetterState>();
        foreach (var guess in state.Guesses)
        {
            foreach (var l in guess.Letters)
            {
                if (!results.TryGetValue(l.Value, out var currentState) || (int)l.State > (int)currentState)
                {
                    results[l.Value] = l.State;
                }
            }
        }
        return results;
    }

    private static Vector4 GetColorForState(LetterState state, bool hotPink) => state switch
    {
        LetterState.Correct => hotPink ? new Vector4(1.0f, 0.08f, 0.58f, 1f) : new Vector4(0.32f, 0.61f, 0.33f, 1f),
        LetterState.Present => new Vector4(0.78f, 0.7f, 0.31f, 1f),
        LetterState.Absent => new Vector4(0.22f, 0.22f, 0.23f, 1f),
        _ => new Vector4(0.5f, 0.5f, 0.5f, 1f)
    };
}
