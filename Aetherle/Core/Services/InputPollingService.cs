using System;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Plugin.Services;

namespace Aetherle.Core.Services;

public class InputPollingService : IDisposable
{
    private readonly Plugin plugin;
    private readonly IFramework framework;
    private readonly IKeyState keyState;
    private readonly bool[] wasDown = new bool[256];

    public InputPollingService(Plugin plugin, IFramework framework, IKeyState keyState)
    {
        this.plugin = plugin;
        this.framework = framework;
        this.keyState = keyState;

        this.framework.Update += OnUpdate;
    }

    public void Dispose()
    {
        this.framework.Update -= OnUpdate;
    }

    private void OnUpdate(IFramework framework)
    {
        if (Plugin.MainWindow is not { IsOpen: true, IsFocused: true }) return;

        for (var key = VirtualKey.A; key <= VirtualKey.Z; key++)
        {
            if (this.keyState[key] && !wasDown[(int)key])
            {
                this.plugin.SessionService.AddLetter((char)('A' + (key - VirtualKey.A)));
            }
            wasDown[(int)key] = this.keyState[key];
        }

        if (this.keyState[VirtualKey.RETURN] && !wasDown[(int)VirtualKey.RETURN]) this.plugin.SessionService.SubmitGuess();
        wasDown[(int)VirtualKey.RETURN] = this.keyState[VirtualKey.RETURN];

        if (this.keyState[VirtualKey.BACK] && !wasDown[(int)VirtualKey.BACK]) this.plugin.SessionService.RemoveLetter();
        wasDown[(int)VirtualKey.BACK] = this.keyState[VirtualKey.BACK];
    }
}
