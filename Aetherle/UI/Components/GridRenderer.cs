using Aetherle.Core.Models;
using Dalamud.Bindings.ImGui;
using System.Numerics;

namespace Aetherle.UI.Components;

public class GridRenderer
{
    public static void Draw(GameState state, int wordLength)
    {
        float cellSize = 50f;

        for (int row = 0; row < 6; row++)
        {
            float totalWidth = (wordLength * cellSize) + ((wordLength - 1) * ImGui.GetStyle().ItemSpacing.X);
            ImGui.SetCursorPosX((ImGui.GetWindowWidth() - totalWidth) * 0.5f);

            for (int col = 0; col < wordLength; col++)
            {
                Vector4 bgColor = new Vector4(0.2f, 0.2f, 0.2f, 1f);
                char letter = ' ';

                if (row < state.Guesses.Count)
                {
                    var guessLetter = state.Guesses[row].Letters[col];
                    letter = guessLetter.Value;
                    bgColor = GetColorForState(guessLetter.State);
                }
                else if (row == state.Guesses.Count && col < state.CurrentGuess.Length)
                {
                    letter = state.CurrentGuess[col];
                }

                DrawCell(letter, bgColor, cellSize);
                if (col < wordLength - 1) ImGui.SameLine();
            }
        }
    }

    private static void DrawCell(char letter, Vector4 color, float size)
    {
        var drawList = ImGui.GetWindowDrawList();
        var pos = ImGui.GetCursorScreenPos();

        drawList.AddRectFilled(pos, pos + new Vector2(size, size), ImGui.ColorConvertFloat4ToU32(color), 5f);

        if (letter != ' ')
        {
            string text = letter.ToString();
            var textSize = ImGui.CalcTextSize(text);
            var textPos = pos + (new Vector2(size, size) - textSize) * 0.5f;
            drawList.AddText(textPos, ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 1, 1)), text);
        }

        ImGui.Dummy(new Vector2(size, size));
    }

    private static Vector4 GetColorForState(LetterState state) => state switch
    {
        LetterState.Correct => new Vector4(0.32f, 0.61f, 0.33f, 1f),
        LetterState.Present => new Vector4(0.78f, 0.7f, 0.31f, 1f),
        LetterState.Absent => new Vector4(0.22f, 0.22f, 0.23f, 1f),
        _ => new Vector4(0.12f, 0.12f, 0.13f, 1f)
    };
}
