using System;
using System.Threading.Tasks;
using System.Threading;
using Velopack;
using Velopack.Sources;

namespace PdfOperations.Gui.Services;

public sealed class AppUpdateService : IAppUpdateService
{
    private readonly UpdateManager manager;
    private readonly UpdateProgressSource source;

    public AppUpdateService()
    {
        source = new UpdateProgressSource(new GithubSource(
            "https://github.com/DanBed666/PdfOperations", accessToken: null, prerelease: false));
        manager = new UpdateManager(source);
    }

    public bool IsSupported =>
        manager.IsInstalled && manager.IsPortable;

    public Task<UpdateInfo?> CheckAsync() =>
        manager.CheckForUpdatesAsync();

    public async Task DownloadAsync(UpdateInfo update, Action<UpdateDownloadProgress> progress,
        CancellationToken cancellationToken)
    {
        source.Begin(progress);
        try
        {
            // Delta reconstruction can block synchronously inside Velopack.
            await Task.Run(() => manager.DownloadUpdatesAsync(update, source.ReportProgress, cancellationToken),
                cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
        }
        finally { source.End(); }
    }

    public void ApplyAndRestart(UpdateInfo update) =>
        manager.ApplyUpdatesAndRestart(update.TargetFullRelease);
}
