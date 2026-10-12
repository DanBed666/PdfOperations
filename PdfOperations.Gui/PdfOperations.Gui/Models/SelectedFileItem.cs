using CommunityToolkit.Mvvm.ComponentModel;

namespace PdfOperations.Gui.Models;

public partial class SelectedFileItem : ObservableObject
{
    [ObservableProperty]
    private int number;

    [ObservableProperty]
    private string path = "";

    public string DisplayText => $"{Number}. {Path}";

    partial void OnNumberChanged(int value)
    {
        OnPropertyChanged(nameof(DisplayText));
    }

    partial void OnPathChanged(string value)
    {
        OnPropertyChanged(nameof(DisplayText));
    }
}