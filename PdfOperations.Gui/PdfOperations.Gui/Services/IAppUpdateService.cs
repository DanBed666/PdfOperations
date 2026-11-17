using System;
using System.Threading;
using System.Threading.Tasks;
using Velopack;

namespace PdfOperations.Gui.Services;

public enum UpdateDownloadPhase { Downloading, Preparing }

public readonly record struct UpdateDownloadProgress(UpdateDownloadPhase Phase, int Percent);

public interface IAppUpdateService
{
    bool IsSupported { get; }
    Task<UpdateInfo?> CheckAsync();
    Task DownloadAsync(UpdateInfo update, Action<UpdateDownloadProgress> progress, CancellationToken cancellationToken);
    void ApplyAndRestart(UpdateInfo update);
}
