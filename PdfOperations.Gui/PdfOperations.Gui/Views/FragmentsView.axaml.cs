using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using PdfOperations.Gui.ViewModels;

namespace PdfOperations.Gui.Views;

public partial class FragmentsView : UserControl
{
    public FragmentsView()
    {
        InitializeComponent();
    }

    private async void BrowseFragmentFile_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not FragmentsViewModel viewModel)
            return;

        var topLevel = TopLevel.GetTopLevel(this);

        if (topLevel is null)
            return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = viewModel.Loc.SelectPdfFiles,
            AllowMultiple = true,
            FileTypeFilter =
            [
                new FilePickerFileType(viewModel.Loc.InputPdfFilesWithFormats)
                {
                    Patterns = ["*.pdf"]
                }
            ]
        });

        string[] paths = files
            .Select(file => file.TryGetLocalPath())
            .OfType<string>()
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .ToArray();

        if (paths.Length == 0)
            return;

        viewModel.SetSelectedFragmentFiles(paths);
    }

    private async void BrowseOutputDirectory_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not FragmentsViewModel viewModel)
            return;

        var topLevel = TopLevel.GetTopLevel(this);

        if (topLevel is null)
            return;

        var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = viewModel.Loc.SelectOutputDirectory,
            AllowMultiple = false
        });

        string? path = folders.FirstOrDefault()?.Path.LocalPath;

        if (!string.IsNullOrWhiteSpace(path))
            viewModel.OutputDirectory = path;
    }
}
