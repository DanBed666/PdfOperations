using CommunityToolkit.Mvvm.ComponentModel;

using PdfOperations.Gui.Helpers;

namespace PdfOperations.Gui.Models;

public partial class SelectedFileItem : ObservableObject
{
    [ObservableProperty]
    private int number;

    [ObservableProperty]
    private string path = "";

    public string DisplayPath => PathDisplayHelper.Format(Path);

    public string DisplayText => $"{Number}. {System.IO.Path.GetFileName(Path)}";
    public string DirectoryPath => System.IO.Path.GetDirectoryName(Path) ?? "";

    partial void OnNumberChanged(int value)
    {
        OnPropertyChanged(nameof(DisplayText));
    }

    partial void OnPathChanged(string value)
    {
        OnPropertyChanged(nameof(DisplayPath));
        OnPropertyChanged(nameof(DisplayText));
        OnPropertyChanged(nameof(DirectoryPath));
    }
}
