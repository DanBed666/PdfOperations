using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using PdfOperations.Gui.Helpers;
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

        string[] paths = await FilePickerHelper.PickFilesAsync(
            this,
            viewModel.FileDialogTitle,
            viewModel.FileTypeName,
            viewModel.FilePatterns);

        if (paths.Length == 0)
            return;

        viewModel.InputFilesList.SetFiles(paths);
    }

    private async void AddInputFiles_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not ReplacementViewModel viewModel)
            return;

        string[] paths = await FilePickerHelper.PickFilesAsync(
            this,
            viewModel.FileDialogTitle,
            viewModel.FileTypeName,
            viewModel.FilePatterns);

        if (paths.Length == 0)
            return;

        viewModel.InputFilesList.AddFiles(paths);
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
            Title = viewModel.Loc.SelectExcelReplacementFile,
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType(viewModel.Loc.InputExcelFilesWithFormats)
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

        string? path = await FilePickerHelper.PickFolderAsync(this);

        if (!string.IsNullOrWhiteSpace(path))
            viewModel.OutputDirectory = path;
    }
}
