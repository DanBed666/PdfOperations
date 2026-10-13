using Avalonia.Controls;
using Avalonia.Interactivity;
using PdfOperations.Gui.Helpers;
using PdfOperations.Gui.ViewModels;

namespace PdfOperations.Gui.Views;

public partial class ManyToManyView : UserControl
{
    public ManyToManyView()
    {
        InitializeComponent();
    }

    private async void BrowseInputFile_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not ManyToManyViewModel viewModel)
            return;

        string[] paths = await FilePickerHelper.PickFilesAsync(
            this,
            viewModel.FileDialogTitle,
            viewModel.FilePatterns);

        if (paths.Length == 0)
            return;

        viewModel.InputFilesList.SetFiles(paths);
    }

    private async void BrowseOutputDirectory_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not ManyToManyViewModel viewModel)
            return;

        string? path = await FilePickerHelper.PickFolderAsync(this);

        if (!string.IsNullOrWhiteSpace(path))
            viewModel.OutputDirectory = path;
    }
    
    private async void AddInputFiles_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not ManyToManyViewModel viewModel)
            return;

        string[] paths = await FilePickerHelper.PickFilesAsync(
            this,
            viewModel.FileDialogTitle,
            viewModel.FilePatterns);

        if (paths.Length == 0)
            return;

        viewModel.InputFilesList.AddFiles(paths);
    }
}