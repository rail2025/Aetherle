using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using System;
using System.Numerics;

namespace Aetherle.UI.Windows;

public class StatsWindow : Window, IDisposable
{
    private readonly Plugin plugin;

    public StatsWindow(Plugin plugin) : base("Aetherle Statistics")
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(250, 150),
            MaximumSize = new Vector2(500, 500)
        };
        this.plugin = plugin;
    }

    public void Dispose() { }

    public override void Draw()
    {
        ImGui.Spacing();
        DrawStatsContent();
    }

    public void DrawStatsContent()
    {
        ImGui.Columns(3, "stats", false);

        CenterText($"{plugin.Configuration.GamesPlayed}");
        CenterText("Played");
        ImGui.NextColumn();

        CenterText($"{plugin.Configuration.WinPercentage}");
        CenterText("Win %");
        ImGui.NextColumn();

        CenterText($"{plugin.Configuration.CurrentStreak}");
        CenterText("Streak");
        ImGui.Columns(1);
    }

    private static void CenterText(string text)
    {
        float width = ImGui.GetColumnWidth();
        float textWidth = ImGui.CalcTextSize(text).X;
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + (width - textWidth) / 2);
        ImGui.TextUnformatted(text);
    }
}
