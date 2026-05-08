using System;
using System.Threading.Tasks;
using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using Aetherle.Core.Services;
using Aetherle.Core.Models;
using Aetherle.Platform.Data;
using Aetherle.Platform.Network;
using Aetherle.UI.Windows;

namespace Aetherle;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IFramework Framework { get; private set; } = null!;
    [PluginService] internal static IKeyState KeyState { get; private set; } = null!;
    [PluginService] internal static IPluginLog Log { get; private set; } = null!;

    public string Name => "Aetherle";
    private const string CommandName = "/aetherle";

    public Configuration Configuration { get; init; }
    public WindowSystem WindowSystem { get; init; }

    public ConfigWindow ConfigWindow { get; init; }
    public static MainWindow MainWindow { get; private set; } = null!;
    public StatsWindow StatsWindow { get; init; }
    public AboutWindow AboutWindow { get; init; }

    public DailyPuzzle CurrentPuzzle { get; private set; }
    public GameSessionService SessionService { get; init; }
    public InputPollingService InputService { get; init; }
    public DailyPuzzleService PuzzleService { get; init; }
    public GetYeeted GetYeeted { get; init; }
    public DataManager Data { get; init; }
    public Core.Rules.GuessValidator Validator { get; init; } = new();

    public Plugin()
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        Plugin.Log.Info($"CONFIG DIR: {PluginInterface.GetPluginConfigDirectory()}");
        PluginInterface.SavePluginConfig(Configuration);

        Data = new DataManager(PluginInterface.GetPluginConfigDirectory());
        PuzzleService = new DailyPuzzleService(Data, new RemoteWordProvider());

        Platform.Time.UtcResetService.CheckAndResetDailyLimits(Configuration);
        WindowSystem = new WindowSystem("Aetherle");
        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this);
        StatsWindow = new StatsWindow(this);
        AboutWindow = new AboutWindow();
        CurrentPuzzle = new DailyPuzzle();

        this.SessionService = new GameSessionService(this);
        this.InputService = new InputPollingService(this, Framework, KeyState);
        this.GetYeeted = new GetYeeted(this);

        _ = LoadInitialPuzzleAsync();

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);
        WindowSystem.AddWindow(StatsWindow);
        WindowSystem.AddWindow(AboutWindow);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Open the Aetherle game window."
        });

        PluginInterface.UiBuilder.Draw += WindowSystem.Draw;
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUi;
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUi;
    }

    private async Task LoadInitialPuzzleAsync()
    {
        await Validator.LoadDictionaryAsync();
        await Data.LoadBackupWordsAsync(); 
        CurrentPuzzle = await PuzzleService.GetTodayPuzzleAsync();
        SessionService.StartNewSession(CurrentPuzzle.Word);
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();
        CommandManager.RemoveHandler(CommandName);
        InputService.Dispose();
    }

    private static void OnCommand(string command, string args) => MainWindow.IsOpen = true;
    private void ToggleConfigUi() => ConfigWindow.IsOpen = !ConfigWindow.IsOpen;
    private void ToggleMainUi() => MainWindow.IsOpen = !MainWindow.IsOpen;
}
