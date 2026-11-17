using System;
using System.Threading;
using System.Threading.Tasks;
using Velopack;
using Velopack.Logging;
using Velopack.Sources;

namespace PdfOperations.Gui.Services;

// Observe actual transfers, including full-package fallback, rather than guessing the phase from a percentage.
internal sealed class UpdateProgressSource(IUpdateSource inner) : IUpdateSource
{
    private readonly object gate = new();
    private Action<UpdateDownloadProgress>? report;
    private int activeDownloads;
    private int percent;

    public void Begin(Action<UpdateDownloadProgress> callback)
    {
        lock (gate)
        {
            if (report is not null)
                throw new InvalidOperationException("An update download is already running.");
            report = callback;
            activeDownloads = 0;
            percent = 0;
            ReportState();
        }
    }

    public void End()
    {
        lock (gate) { report = null; }
    }

    public void ReportProgress(int value)
    {
        lock (gate)
        {
            // 100% is reserved for successful completion, including verification and cleanup.
            percent = Math.Clamp(value, 0, 99);
            ReportState();
        }
    }

    private void ReportState() => report?.Invoke(new(
        activeDownloads > 0 ? UpdateDownloadPhase.Downloading : UpdateDownloadPhase.Preparing, percent));

    public Task<VelopackAssetFeed> GetReleaseFeed(IVelopackLogger logger, string? appId, string channel,
        Guid? stagingId = null, VelopackAsset? latestLocalRelease = null) =>
        inner.GetReleaseFeed(logger, appId, channel, stagingId, latestLocalRelease);

    public async Task DownloadReleaseEntry(IVelopackLogger logger, VelopackAsset releaseEntry,
        string localFile, Action<int> progress, CancellationToken cancelToken = default)
    {
        // Velopack may try a full-package fallback after cancellation of a delta transfer.
        cancelToken.ThrowIfCancellationRequested();
        lock (gate)
        {
            activeDownloads++;
            if (releaseEntry.Type == VelopackAssetType.Full)
                percent = 0;
            ReportState();
        }
        try
        {
            await inner.DownloadReleaseEntry(logger, releaseEntry, localFile, progress, cancelToken)
                .ConfigureAwait(false);
            cancelToken.ThrowIfCancellationRequested();
        }
        finally
        {
            lock (gate)
            {
                activeDownloads--;
                ReportState();
            }
        }
    }
}
