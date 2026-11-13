using System;
using System.Reflection;
using PdfOperations.Gui.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace PdfOperations.Gui.ViewModels;

public partial class AboutViewModel : ViewModelBase
{
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

    [RelayCommand]
    private async Task CheckUpdatesAsync()
    {
        HasUpdate = false;
        StatusKind = StatusKind.Info;
        StatusMessage = Loc.CheckingUpdates;

        try
        {
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
        catch (Exception ex) when (ex is HttpRequestException or OperationCanceledException
                                       or JsonException or FormatException)
        {
            StatusKind = StatusKind.Error;
            StatusMessage = Loc.UpdateCheckFailed;
        }
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
