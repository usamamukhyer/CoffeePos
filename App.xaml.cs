using DBCafeteria.Services;
using Microsoft.Maui.ApplicationModel;

namespace DBCafeteria;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        OfflineSyncService.Instance.Start();
        var shell = new AppShell();
        var window = new Window(shell);
        window.Resumed += (_, _) => OfflineSyncService.Instance.MarkActive();
        window.Stopped += (_, _) => OfflineSyncService.Instance.MarkInactive();
        window.Destroying += (_, _) => OfflineSyncService.Instance.MarkInactive();
        _ = RestoreSessionAndSyncAsync(shell);
        return window;
    }

    private static async Task RestoreSessionAndSyncAsync(AppShell shell)
    {
        var restored = await OrderSessionService.Instance.RestoreAsync();
        await MainThread.InvokeOnMainThreadAsync(async () =>
            await shell.GoToAsync(restored ? "//OrderSetup" : "//AuthLanding"));

        await OfflineSyncService.Instance.TriggerSyncAsync();
    }
}
