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
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType(viewModel.Loc.InputPdfFilesWithFormats)
                {
                    Patterns = ["*.pdf"]
                }
            ]
        });

        string? path = files.FirstOrDefault()?.Path.LocalPath;

        if (!string.IsNullOrWhiteSpace(path))
        {
            viewModel.SelectedFragmentFile = path;

            if (string.IsNullOrWhiteSpace(viewModel.OutputDirectory))
            {
                string? directory = System.IO.Path.GetDirectoryName(path);

                if (!string.IsNullOrWhiteSpace(directory))
                    viewModel.OutputDirectory = directory;
            }

            try
            {
                viewModel.SelectedFragmentFileInfo =
                    $"{System.IO.Path.GetFileName(path)} - liczba stron: {Info.GetPdfPagesSingle(path)}";
            }
            catch (Exception ex)
            {
                viewModel.SelectedFragmentFileInfo =
                    $"Nie udało się odczytać liczby stron: {ex.Message}";
            }
        }
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
