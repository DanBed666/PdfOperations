using System.Linq;
using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using PdfOperations.Gui.Helpers;
using PdfOperations.Gui.Models;
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

    private async void DownloadReplacementTemplate_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not ReplacementViewModel viewModel || viewModel.IsBusy)
            return;

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null)
            return;

        try
        {
            using var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = viewModel.Loc.DownloadReplacementTemplate,
                SuggestedFileName = "replacement-template.xlsx",
                DefaultExtension = "xlsx",
                ShowOverwritePrompt = true,
                FileTypeChoices =
                [
                    new FilePickerFileType(viewModel.Loc.InputExcelFilesWithFormats)
                    {
                        Patterns = ["*.xlsx"]
                    }
                ]
            });

            if (file is null || viewModel.IsBusy)
                return;

            // Build the workbook before opening a destination that may already exist.
            using var template = new MemoryStream();
            ExcelExporter.SaveReplacementTemplate(template);
            template.Position = 0;
            await using (var output = await file.OpenWriteAsync())
            {
                if (output.CanSeek)
                    output.SetLength(0);
                await template.CopyToAsync(output);
            }

            viewModel.StatusKind = StatusKind.Success;
            viewModel.StatusMessage = viewModel.Loc.DoneSaved(file.TryGetLocalPath() ?? file.Name);
        }
        catch (Exception ex)
        {
            viewModel.ShowError(ex);
        }
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
