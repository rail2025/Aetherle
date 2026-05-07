using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using System;
using System.Numerics;

namespace Aetherle.UI.Windows;

public class ConfigWindow : Window, IDisposable
{
    private readonly Plugin plugin;

    public ConfigWindow(Plugin plugin) : base("Aetherle Settings", ImGuiWindowFlags.NoCollapse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(200, 100),
            MaximumSize = new Vector2(400, 200)
        };
        this.plugin = plugin;
    }

    public void Dispose() { }

    public override void Draw()
    {
        bool hotPink = plugin.Configuration.HotPinkMode;
        if (ImGui.Checkbox("Hot Pink Mode", ref hotPink))
        {
            plugin.Configuration.HotPinkMode = hotPink;
            Plugin.PluginInterface.SavePluginConfig(plugin.Configuration);
        }

        bool hardMode = plugin.Configuration.HardMode;
        if (ImGui.Checkbox("Hard Mode", ref hardMode))
        {
            plugin.Configuration.HardMode = hardMode;
            Plugin.PluginInterface.SavePluginConfig(plugin.Configuration);
        }
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip("Any revealed hints must be used in subsequent guesses.");
        }
    }
}
