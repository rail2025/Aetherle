using Aetherle.Core.Services;
using Aetherle.UI.Components;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using System;
using System.Numerics;

namespace Aetherle.UI.Windows;

public class MainWindow : Window
{
    private readonly Plugin plugin;
    private readonly GridRenderer gridRenderer = new();
    private readonly KeyboardRenderer keyboardRenderer = new();

    private readonly Vector4 colorPinkActive = new(1.0f, 0.08f, 0.58f, 1.0f);
    private readonly Vector4 colorPinkBg = new(1.0f, 0.08f, 0.58f, 0.7f);
    private readonly Vector4 colorWin = new(0.2f, 0.8f, 0.2f, 1f);
    private readonly Vector4 colorLose = new(0.8f, 0.2f, 0.2f, 1f);
    private readonly Vector4 colorLosePink = new(0f, 0f, 0f, 1f);

    internal DateTime? yeetStartTime = null;

    public MainWindow(Plugin plugin) : base("Aetherle")
    {
        this.plugin = plugin;
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(400, 600),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
    }

    public override void PreDraw()
    {
        if (plugin.Configuration.HotPinkMode)
        {
            ImGui.PushStyleColor(ImGuiCol.TitleBgActive, colorPinkActive);
            ImGui.PushStyleColor(ImGuiCol.WindowBg, colorPinkBg);
        }
    }

    public override void PostDraw()
    {
        if (plugin.Configuration.HotPinkMode)
        {
            ImGui.PopStyleColor(2);
        }
    }

    public override void Draw()
    {
        if (plugin.CurrentPuzzle == null || string.IsNullOrEmpty(plugin.CurrentPuzzle.Word))
        {
            ImGui.Text("Loading puzzle...");
            return;
        }

        var state = plugin.SessionService.State;

        ImGui.Spacing();

        ImGui.TextColored(new Vector4(1, 1, 0, 1), "AETHERLE");
        ImGui.SameLine();
        if (ImGui.Button("Settings"))
        {
            plugin.ConfigWindow.IsOpen = !plugin.ConfigWindow.IsOpen;
        }
        ImGui.Separator();
        ImGui.TextColored(new Vector4(0.5f, 0.5f, 0.5f, 1), $"Category: {plugin.SessionService.CurrentCategory}");
        ImGui.Spacing();

        GridRenderer.Draw(state, plugin.SessionService.TargetWord.Length, plugin.Configuration.HotPinkMode);

        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();

        if (state.IsComplete)
        {
            string msg = state.IsWin ? "Victory!" : $"Game Over. The word was {plugin.SessionService.TargetWord}";
            Vector4 color = state.IsWin ? colorWin : (plugin.Configuration.HotPinkMode ? colorLosePink : colorLose);

            ImGui.SetCursorPosX((ImGui.GetWindowWidth() - ImGui.CalcTextSize(msg).X) * 0.5f);
            ImGui.TextColored(color, msg);

            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Spacing();

            plugin.StatsWindow.DrawStatsContent();

            ImGui.Spacing();
            float buttonWidth = 100f;
            ImGui.SetCursorPosX((ImGui.GetWindowWidth() - (buttonWidth * 2 + ImGui.GetStyle().ItemSpacing.X)) * 0.5f);

            if (ImGui.Button("Share Result", new Vector2(buttonWidth, 0)))
            {
                var guesses = new System.Collections.Generic.List<string>();
                foreach (var g in state.Guesses)
                {
                    char[] chars = new char[g.Letters.Count];
                    for (int i = 0; i < g.Letters.Count; i++) chars[i] = g.Letters[i].Value;
                    guesses.Add(new string(chars));
                }
                ShareResultService.CopyToClipboard(plugin.CurrentPuzzle.Word, guesses);
            }
            ImGui.SameLine();
            if (ImGui.Button("Reset Stats", new Vector2(buttonWidth, 0)))
            {
                plugin.Configuration.DailyGamesCompletions = 0;
                plugin.Configuration.LastPlayDate = DateTime.MinValue;
                plugin.Configuration.GamesPlayed = 0;
                plugin.Configuration.GamesWon = 0;
                plugin.Configuration.CurrentStreak = 0;
                plugin.Configuration.MaxStreak = 0;
                for (int i = 1; i <= 6; i++) plugin.Configuration.GuessDistribution[i] = 0;
                Plugin.PluginInterface.SavePluginConfig(plugin.Configuration);
            }

            ImGui.Spacing();

            plugin.GetYeeted.CheckStatus(
                () => plugin.SessionService.StartRandomSession(),
                () => this.IsOpen = false
            );

            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Spacing();
        }

        keyboardRenderer.Draw(plugin, state, c => plugin.SessionService.AddLetter(c));

        //if (ImGui.IsKeyPressed(ImGuiKey.Enter)) plugin.SessionService.SubmitGuess();
        //if (ImGui.IsKeyPressed(ImGuiKey.Backspace)) plugin.SessionService.RemoveLetter();
    }
}
