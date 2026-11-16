using System;
using System.Threading.Tasks;
using Velopack;
using Velopack.Sources;

namespace PdfOperations.Gui.Services;

public sealed class AppUpdateService
{
    private readonly UpdateManager manager = new(new GithubSource(
        "https://github.com/DanBed666/PdfOperations",
        accessToken: null,
        prerelease: false));

    public bool IsSupported =>
        manager.IsInstalled && manager.IsPortable;

    public Task<UpdateInfo?> CheckAsync() =>
        manager.CheckForUpdatesAsync();

    public Task DownloadAsync(UpdateInfo update, Action<int> progress) =>
        manager.DownloadUpdatesAsync(update, progress);

    public void ApplyAndRestart(UpdateInfo update) =>
        manager.ApplyUpdatesAndRestart(update.TargetFullRelease);
}