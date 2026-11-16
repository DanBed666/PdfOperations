using Avalonia;
using System;
using Velopack;
using PdfOperations.Gui.Services;
using Velopack.Locators;

namespace PdfOperations.Gui;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static int Main(string[] args)
    {
        VelopackApp.Build()
            .SetAutoApplyOnStartup(false)
            .Run();
        
        if (WordConversionWorker.TryRun(args, out int exitCode))
            return exitCode;
        
        var locator = VelopackLocator.Current;

        using var instances = InstanceCoordinator.OpenApplication(
            locator.RootAppDir ?? AppContext.BaseDirectory,
            locator.UpdateExePath);

        if (instances is null)
            return 1;
        
        return BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace();
}
