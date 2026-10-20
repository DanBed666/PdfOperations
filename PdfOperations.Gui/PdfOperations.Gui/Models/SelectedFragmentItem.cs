using CommunityToolkit.Mvvm.ComponentModel;

using PdfOperations.Gui.Helpers;

namespace PdfOperations.Gui.Models;

public partial class SelectedFragmentItem : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayText))]
    private int number;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayText))]
    [NotifyPropertyChangedFor(nameof(DisplayPath))]
    [NotifyPropertyChangedFor(nameof(DirectoryPath))]
    private string fileName = "";

    [ObservableProperty]
    private string pageNumbers = "";

    public string DisplayPath => PathDisplayHelper.Format(FileName);

    public string DisplayText => $"{Number}. {System.IO.Path.GetFileName(FileName)}";
    public string DirectoryPath => System.IO.Path.GetDirectoryName(FileName) ?? "";
}
