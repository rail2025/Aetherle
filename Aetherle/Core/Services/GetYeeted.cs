using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using System;
using System.Numerics;

namespace Aetherle.Core.Services;

public class GetYeeted
{
    private readonly Plugin plugin;
    private const int YeetDelaySeconds = 10;

    public GetYeeted(Plugin plugin) => this.plugin = plugin;

    public void CheckStatus(Action restartAction, Action closeAction)
    {
        var cfg = plugin.Configuration;

        if (cfg.LastPlayDate.Date != DateTime.UtcNow.Date)
        {
            cfg.DailyGamesCompletions = 0;
        }

        if (cfg.DailyGamesCompletions < 4)
        {
            if (ImGui.Button("Replay new word", new Vector2(ImGui.GetContentRegionAvail().X, 30)))
                restartAction();
        }
        else if (cfg.DailyGamesCompletions < 6)
        {
            ImGui.TextWrapped("Stop playing this and go stretch and drink water.");

            var timeLeft = DateTime.UtcNow.Date.AddDays(1) - DateTime.UtcNow;
            ImGui.TextWrapped($"Wait for daily reset in: {timeLeft:hh\\:mm\\:ss}");

            ImGui.Spacing();
            if (ImGui.Button("Play Anyway", new Vector2(ImGui.GetContentRegionAvail().X, 30)))
                restartAction();
        }
        else
        {
            if (Plugin.MainWindow.yeetStartTime == null) Plugin.MainWindow.yeetStartTime = DateTime.UtcNow;

            var elapsed = (DateTime.UtcNow - Plugin.MainWindow.yeetStartTime.Value).TotalSeconds;
            var remaining = Math.Max(0, YeetDelaySeconds - (int)elapsed);

            ImGui.TextColored(new Vector4(1, 1, 0, 1), "I said stop! Try more tomorrow.");
            ImGui.Spacing();

            string btnLabel = $"Close Plugin ({remaining}s)";
            if (ImGui.Button(btnLabel, new Vector2(ImGui.GetContentRegionAvail().X, 40)) || remaining <= 0)
            {
                Plugin.MainWindow.yeetStartTime = null;
                closeAction();
            }
        }
    }
}
