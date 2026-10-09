using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using PdfOperations.Gui.ViewModels;

namespace PdfOperations.Gui.Views;

public partial class ReplacementView : UserControl
{
    public ReplacementView()
    {
        InitializeComponent();
    }

    private async void BrowseInputFiles_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not ReplacementViewModel viewModel)
            return;

        var topLevel = TopLevel.GetTopLevel(this);

        if (topLevel is null)
            return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select document files",
            AllowMultiple = true,
            FileTypeFilter =
            [
                new FilePickerFileType("Editable documents")
                {
                    Patterns = ["*.docx", "*.odg", "*.odt"]
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

    private async void BrowsePlaceholderFile_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not ReplacementViewModel viewModel)
            return;

        var topLevel = TopLevel.GetTopLevel(this);

        if (topLevel is null)
            return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select Excel replacement file",
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("Excel files")
                {
                    Patterns = ["*.xlsx"]
                }
            ]
        });

        string? path = files.FirstOrDefault()?.Path.LocalPath;

        if (!string.IsNullOrWhiteSpace(path))
            viewModel.PlaceholderFile = path;
    }

    private async void BrowseOutputDirectory_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not ReplacementViewModel viewModel)
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