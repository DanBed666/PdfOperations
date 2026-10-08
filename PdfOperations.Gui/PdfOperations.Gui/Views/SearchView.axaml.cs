using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using PdfOperations.Gui.ViewModels;

namespace PdfOperations.Gui.Views;

public partial class SearchView : UserControl
{
    public SearchView()
    {
        InitializeComponent();
    }

    private async void BrowseInputFiles_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not SearchViewModel viewModel)
            return;

        var topLevel = TopLevel.GetTopLevel(this);

        if (topLevel is null)
            return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select files",
            AllowMultiple = true,
            FileTypeFilter =
            [
                new FilePickerFileType("Supported files")
                {
                    Patterns = ["*.txt", "*.pdf", "*.jpg", "*.jpeg", "*.png", "*.bmp", "*.tif", "*.tiff"]
                }
            ]
        });

        string[] paths = files
            .Select(file => file.Path.LocalPath)
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .ToArray();

        if (paths.Length == 0)
            return;

        viewModel.InputFiles = paths;
        viewModel.InputFilesText = string.Join(Environment.NewLine, paths);
    }

    private async void BrowseOutputDirectory_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not SearchViewModel viewModel)
            return;

        var topLevel = TopLevel.GetTopLevel(this);

        if (topLevel is null)
            return;

        var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select output folder",
            AllowMultiple = false
        });

        string? path = folders.FirstOrDefault()?.Path.LocalPath;

        if (!string.IsNullOrWhiteSpace(path))
            viewModel.OutputDirectory = path;
    }
}