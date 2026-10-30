using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using PdfOperations.Gui.Services;

namespace PdfOperations.Gui.Helpers;

public static class FilePickerHelper
{
    public static async Task<string[]> PickFilesAsync(
        Control owner,
        string title,
        string fileTypeName,
        string[] patterns,
        bool allowMultiple = true)
    {
        TopLevel? topLevel = TopLevel.GetTopLevel(owner);

        if (topLevel is null)
            return [];

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = allowMultiple,
            FileTypeFilter = BuildFilters(fileTypeName, patterns)
        });

        return files
            .Select(file => file.Path.LocalPath)
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .ToArray();
    }

    private static FilePickerFileType[] BuildFilters(string name, string[] patterns)
    {
        var loc = LocalizationService.Instance;
        string[] images = ["*.jpg", "*.jpeg", "*.png", "*.bmp", "*.tif", "*.tiff"];
        var groups = patterns.Select(pattern => pattern.ToLowerInvariant()).Distinct()
            .GroupBy(pattern => images.Contains(pattern) ? "images" : pattern[2..].ToUpperInvariant())
            .ToArray();

        if (groups.Length <= 1)
            return [new FilePickerFileType(name) { Patterns = patterns }];

        var filters = groups.Select(group => new FilePickerFileType(
            $"{(group.Key == "images" ? loc.ImageFilesLabel : group.Key)} ({string.Join(", ", group)})")
            { Patterns = group.ToArray() });

        return filters.Prepend(new FilePickerFileType(loc.AllSupportedFiles)
            { Patterns = patterns }).ToArray();
    }

    public static async Task<string?> PickFolderAsync(
        Control owner,
        string? title = null)
    {
        TopLevel? topLevel = TopLevel.GetTopLevel(owner);

        if (topLevel is null)
            return null;

        var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = title ?? LocalizationService.Instance.SelectOutputDirectory,
            AllowMultiple = false
        });

        return folders.FirstOrDefault()?.Path.LocalPath;
    }
}
