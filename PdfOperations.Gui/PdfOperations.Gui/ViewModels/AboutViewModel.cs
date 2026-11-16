using System;
using System.Reflection;
using PdfOperations.Gui.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using PdfOperations.Gui.Services;
using Velopack;

namespace PdfOperations.Gui.ViewModels;

public partial class AboutViewModel : ViewModelBase
{
    private readonly AppUpdateService updateService = new();
    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DownloadUpdateCommand))]
    private UpdateInfo? availableUpdate;
    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DownloadUpdateCommand))]
    [NotifyCanExecuteChangedFor(nameof(RestartToUpdateCommand))]
    [NotifyCanExecuteChangedFor(nameof(CheckUpdatesCommand))]
    private bool isDownloading;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DownloadUpdateCommand))]
    [NotifyCanExecuteChangedFor(nameof(RestartToUpdateCommand))]
    [NotifyCanExecuteChangedFor(nameof(CheckUpdatesCommand))]
    private UpdateInfo? downloadedUpdate;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DownloadUpdateCommand))]
    [NotifyCanExecuteChangedFor(nameof(RestartToUpdateCommand))]
    [NotifyCanExecuteChangedFor(nameof(CheckUpdatesCommand))]
    private bool isCheckingUpdates;

    [ObservableProperty]
    private int downloadProgress;
    
    private readonly Func<bool> hasRunningOperations;
    
    public AboutViewModel(Func<bool> hasRunningOperations)
    {
        this.hasRunningOperations = hasRunningOperations;
    }

    private bool CanDownloadUpdate() =>
        updateService.IsSupported && AvailableUpdate is not null &&
        !IsDownloading && !IsCheckingUpdates && DownloadedUpdate is null;

    private bool CanCheckUpdates() =>
        !IsDownloading && !IsCheckingUpdates && DownloadedUpdate is null;
    public string Title => Loc.GetOperationName("About");
    public string Author => "DanBed666";
    public Uri RepositoryUrl { get; } = new("https://github.com/DanBed666/PdfOperations");
    public Uri CodexUrl { get; } = new("https://openai.com/codex/");

    public string AppVersion { get; } = typeof(AboutViewModel).Assembly
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
        .InformationalVersion.Split('+')[0]
        ?? typeof(AboutViewModel).Assembly.GetName().Version?.ToString(3)
        ?? "";
    
    private static readonly HttpClient Client = new() { Timeout = TimeSpan.FromSeconds(10) };
    public Uri ReleaseUrl { get; } = new("https://github.com/DanBed666/PdfOperations/releases/latest");

    [ObservableProperty]
    private bool hasUpdate;
    
    [RelayCommand(CanExecute = nameof(CanDownloadUpdate))]
    private async Task DownloadUpdateAsync()
    {
        if (!CanDownloadUpdate())
            return;
        var update = AvailableUpdate!;
        IsDownloading = true;
        DownloadProgress = 0;
        StatusKind = StatusKind.Info;
        StatusMessage = Loc.DownloadingUpdate;
        try
        {
            IProgress<int> progress = new Progress<int>(value =>
            {
                if (IsDownloading)
                    DownloadProgress = Math.Max(DownloadProgress, Math.Clamp(value, 0, 100));
            });
            await updateService.DownloadAsync(update, progress.Report);
            DownloadedUpdate = update;
            DownloadProgress = 100;
            StatusKind = StatusKind.Success;
            StatusMessage = Loc.UpdateDownloaded;
        }
        catch (Exception ex)
        {
            StatusKind = StatusKind.Error;
            StatusMessage = Loc.UpdateDownloadFailed;
            ErrorDetails = ex.ToString();
        }
        finally { IsDownloading = false; }
    }
    
    private bool CanRestartToUpdate() =>
        updateService.IsSupported && DownloadedUpdate is not null &&
        !IsDownloading && !IsCheckingUpdates;

    [RelayCommand(CanExecute = nameof(CanRestartToUpdate))]
    private void RestartToUpdate()
    {
        if (!CanRestartToUpdate())
            return;
        
        if (hasRunningOperations())
        {
            StatusKind = StatusKind.Warning;
            StatusMessage = Loc.UpdateWaitForOperations;
            return;
        }
        try
        {
            var coordinator = InstanceCoordinator.Current
                              ?? throw new InvalidOperationException("Instance coordination is unavailable.");

            var update = DownloadedUpdate!;

            bool started = coordinator.TryApply(
                () => updateService.ApplyAndRestart(update));

            if (!started)
            {
                StatusKind = StatusKind.Warning;
                StatusMessage = Loc.UpdateCloseOtherWindows;
            }
        }
        catch (Exception ex)
        {
            StatusKind = StatusKind.Error;
            StatusMessage = Loc.UpdateApplyFailed;
            ErrorDetails = ex.ToString();
        }
    }

    [RelayCommand(CanExecute = nameof(CanCheckUpdates))]
    private async Task CheckUpdatesAsync()
    {
        if (!CanCheckUpdates())
            return;

        IsCheckingUpdates = true;
        AvailableUpdate = null;
        HasUpdate = false;
        StatusKind = StatusKind.Info;
        StatusMessage = Loc.CheckingUpdates;

        try
        {
            if (updateService.IsSupported)
            {
                var update = await updateService.CheckAsync();

                if (update is null)
                {
                    StatusKind = StatusKind.Success;
                    StatusMessage = Loc.NoUpdates;
                    return;
                }

                string versionText = update.TargetFullRelease.Version
                    .ToString().Split('+')[0];

                if (!Version.TryParse(versionText, out var version))
                    throw new FormatException("Invalid update version.");

                AvailableUpdate = update;
                HasUpdate = true;
                StatusKind = StatusKind.Success;
                StatusMessage = Loc.UpdateAvailable(version);
                return;
            }
            
            using var request = new HttpRequestMessage(HttpMethod.Get,
                "https://api.github.com/repos/DanBed666/PdfOperations/releases/latest");
            request.Headers.UserAgent.ParseAdd("PdfOperations");
            using var response = await Client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            if (json.RootElement.ValueKind != JsonValueKind.Object ||
                !json.RootElement.TryGetProperty("tag_name", out var tag) ||
                tag.ValueKind != JsonValueKind.String ||
                !Version.TryParse(tag.GetString()?.TrimStart('v', 'V'), out var latest) ||
                !Version.TryParse(AppVersion, out var current))
                throw new FormatException();

            HasUpdate = latest > current;
            StatusKind = StatusKind.Success;
            StatusMessage = HasUpdate ? Loc.UpdateAvailable(latest) : Loc.NoUpdates;
        }
        catch (Exception ex)
        {
            AvailableUpdate = null;
            HasUpdate = false;
            StatusKind = StatusKind.Error;
            StatusMessage = Loc.UpdateCheckFailed;
            ErrorDetails = ex.ToString();
        }
        finally { IsCheckingUpdates = false; }
    }

    public ProjectCredit[] Libraries { get; } =
    [
        new("Avalonia", new Uri("https://github.com/AvaloniaUI/Avalonia")),
        new("CommunityToolkit.Mvvm", new Uri("https://github.com/CommunityToolkit/dotnet")),
        new("ClosedXML", new Uri("https://github.com/ClosedXML/ClosedXML")),
        new("Inter", new Uri("https://rsms.me/inter/"))
    ];

    public ProjectCredit[] Tools { get; } =
    [
        new("LibreOffice", new Uri("https://www.libreoffice.org/")),
        new("Poppler", new Uri("https://poppler.freedesktop.org/")),
        new("qpdf", new Uri("https://qpdf.sourceforge.io/")),
        new("Tesseract OCR", new Uri("https://github.com/tesseract-ocr/tesseract")),
        new("ImageMagick", new Uri("https://imagemagick.org/")),
        new("pdf2docx", new Uri("https://github.com/ArtifexSoftware/pdf2docx"))
    ];
}
