using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace PdfOperations.Gui.Helpers;

public static class FilePickerHelper
{
    public static async Task<string[]> PickFilesAsync(
        Control owner,
        string title,
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
            FileTypeFilter =
            [
                new FilePickerFileType("Input files")
                {
                    Patterns = patterns
                }
            ]
        });

        return files
            .Select(file => file.Path.LocalPath)
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .ToArray();
    }

    public static async Task<string?> PickFolderAsync(
        Control owner,
        string title = "Select output folder")
    {
        TopLevel? topLevel = TopLevel.GetTopLevel(owner);

        if (topLevel is null)
            return null;

        var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = title,
            AllowMultiple = false
        });

        return folders.FirstOrDefault()?.Path.LocalPath;
    }
}