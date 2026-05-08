using Aetherle.Core.Models;
using Dalamud.Bindings.ImGui;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Aetherle.UI.Components;

public class KeyboardRenderer
{
    private readonly string[] rows = { "QWERTYUIOP", "ASDFGHJKL", "ZXCVBNM" };

    public void Draw(GameState state, Action<char> onKeyClick)
    {
        var letterStates = GetLetterStates(state);

        foreach (var row in rows)
        {
            float spacing = ImGui.GetStyle().ItemSpacing.X;
            float keyWidth = 32f;
            float keyHeight = 40f;
            float totalWidth = (row.Length * keyWidth) + ((row.Length - 1) * spacing);
            ImGui.SetCursorPosX((ImGui.GetWindowWidth() - totalWidth) * 0.5f);

            foreach (var c in row)
            {
                letterStates.TryGetValue(c, out var s);

                ImGui.PushStyleColor(ImGuiCol.Button, GetColorForState(s));
                if (ImGui.Button(c.ToString(), new Vector2(keyWidth, keyHeight)))
                {
                    onKeyClick(c);
                }
                ImGui.PopStyleColor();
                ImGui.SameLine();
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

    private static Vector4 GetColorForState(LetterState state) => state switch
    {
        LetterState.Correct => new Vector4(0.32f, 0.61f, 0.33f, 1f),
        LetterState.Present => new Vector4(0.78f, 0.7f, 0.31f, 1f),
        LetterState.Absent => new Vector4(0.22f, 0.22f, 0.23f, 1f),
        _ => new Vector4(0.5f, 0.5f, 0.5f, 1f)
    };
}
