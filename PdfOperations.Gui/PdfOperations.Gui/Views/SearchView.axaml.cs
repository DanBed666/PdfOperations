using Avalonia.Controls;
using Avalonia.Interactivity;
using PdfOperations.Gui.Helpers;
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
        if (DataContext is not SearchViewModel viewModel)
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

    private async void BrowseOutputDirectory_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not SearchViewModel viewModel)
            return;

        string? path = await FilePickerHelper.PickFolderAsync(this);

        if (!string.IsNullOrWhiteSpace(path))
            viewModel.OutputDirectory = path;
    }
}