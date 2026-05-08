using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using System;
using System.Numerics;

namespace Aetherle.Core.Services;

public class GetYeeted
{
    private readonly Plugin plugin;
    private static DateTime? YeetStartTime = null;
    private const int YeetDelaySeconds = 10;

    public GetYeeted(Plugin plugin) => this.plugin = plugin;

    public static void CheckStatus(Plugin plugin, Action restartAction, Action closeAction)
    {
        var cfg = plugin.Configuration;

        if (cfg.LastPlayDate.Date != DateTime.UtcNow.Date)
        {
            cfg.DailyGamesCompletions = 0;
        }

        if (cfg.DailyGamesCompletions < 4)
        {
            CenterButton("Replay new word", new Vector2(100, 30), restartAction);
        }
        else if (cfg.DailyGamesCompletions < 6)
        {
            CenterText("Stop playing this and go stretch and drink water.", new Vector4(1, 1, 1, 1));

            var timeLeft = DateTime.UtcNow.Date.AddDays(1) - DateTime.UtcNow;
            string timeStr = $"Wait for daily reset in: {timeLeft.Hours:D2}:{timeLeft.Minutes:D2}:{timeLeft.Seconds:D2}";
            CenterText(timeStr, new Vector4(1, 1, 1, 1));

            ImGui.Spacing();
            CenterButton("Play Anyway", new Vector2(100, 30), restartAction);
        }
        else
        {
            if (YeetStartTime == null) YeetStartTime = DateTime.UtcNow;

            var elapsed = (DateTime.UtcNow - YeetStartTime.Value).TotalSeconds;
            var remaining = Math.Max(0, YeetDelaySeconds - (int)elapsed);

            CenterText("I said stop! Try more tomorrow.", new Vector4(1, 1, 0, 1));
            ImGui.Spacing();

            string btnLabel = $"Close Plugin ({remaining}s)";
            if (ImGui.Button(btnLabel, new Vector2(ImGui.GetWindowWidth(), 40)) || remaining <= 0)
            {
                YeetStartTime = null;
                closeAction();
            }
        }
    }

    private static void CenterText(string text, Vector4 color)
    {
        float windowWidth = ImGui.GetWindowSize().X;
        float textWidth = ImGui.CalcTextSize(text).X;
        ImGui.SetCursorPosX((windowWidth - textWidth) * 0.5f);
        ImGui.TextColored(color, text);
    }

    private static void CenterButton(string label, Vector2 size, Action action)
    {
        float windowWidth = ImGui.GetWindowSize().X;
        ImGui.SetCursorPosX((windowWidth - size.X) * 0.5f);
        if (ImGui.Button(label, size))
        {
            YeetStartTime = null;
            action();
        }
    }
}
