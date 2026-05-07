using Aetherle.UI.Components;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using Aetherle.Core.Services;
using System.Numerics;

namespace Aetherle.UI.Windows;

public class MainWindow : Window
{
    private readonly Plugin plugin;
    private readonly GridRenderer gridRenderer = new();
    private readonly KeyboardRenderer keyboardRenderer = new();
    public MainWindow(Plugin plugin) : base("Aetherle")
    {
        this.plugin = plugin;
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(400, 600),
            MaximumSize = new Vector2(400, 600)
        };
    }

    public override void Draw()
    {
        if (plugin.CurrentPuzzle == null || string.IsNullOrEmpty(plugin.CurrentPuzzle.Word))
        {
            ImGui.Text("Loading puzzle...");
            return;
        }

        var state = plugin.SessionService.State;

        if (state.IsComplete)
        {
            GetYeeted.CheckStatus(
                plugin,
                () => plugin.SessionService.StartNewSession(plugin.CurrentPuzzle.Word),
                () => this.IsOpen = false
            );
        }

        ImGui.Spacing();

        GridRenderer.Draw(state, plugin.CurrentPuzzle.Metadata.WordLength);

        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();

        keyboardRenderer.Draw(state, c => plugin.SessionService.AddLetter(c));

        if (ImGui.IsKeyPressed(ImGuiKey.Enter)) plugin.SessionService.SubmitGuess();
        if (ImGui.IsKeyPressed(ImGuiKey.Backspace)) plugin.SessionService.RemoveLetter();
    }
}
