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

        bool[] isDown = new bool[256];
        bool anyKeyPressed = false;

        for (int i = (int)VirtualKey.A; i <= (int)VirtualKey.Z; i++)
        {
            isDown[i] = this.keyState[(VirtualKey)i];
            if (isDown[i]) anyKeyPressed = true;
        }

        isDown[(int)VirtualKey.RETURN] = this.keyState[VirtualKey.RETURN];
        isDown[(int)VirtualKey.BACK] = this.keyState[VirtualKey.BACK];

        if (isDown[(int)VirtualKey.RETURN] || isDown[(int)VirtualKey.BACK]) anyKeyPressed = true;

        if (anyKeyPressed)
        {
            this.keyState.ClearAll();
        }
        else
        {
            for (int i = 0; i < 256; i++) wasDown[i] = false;
            return;
        }

        for (int i = (int)VirtualKey.A; i <= (int)VirtualKey.Z; i++)
        {
            if (isDown[i] && !wasDown[i])
            {
                char letter = (char)('A' + (i - (int)VirtualKey.A));
                this.plugin.SessionService.AddLetter(letter);
            }
        }

        if (isDown[(int)VirtualKey.RETURN] && !wasDown[(int)VirtualKey.RETURN]) this.plugin.SessionService.SubmitGuess();
        if (isDown[(int)VirtualKey.BACK] && !wasDown[(int)VirtualKey.BACK]) this.plugin.SessionService.RemoveLetter();

        for (int i = 0; i < 256; i++) wasDown[i] = isDown[i];
    }
}
