using CommunityToolkit.Mvvm.ComponentModel;
using PdfOperations.Gui.Services;

namespace PdfOperations.Gui.ViewModels;

public abstract partial class ViewModelBase : ObservableObject
{
    public LocalizationService Loc => LocalizationService.Instance;

    [ObservableProperty]
    private bool isBusy;

    public bool IsNotBusy => !IsBusy;

    partial void OnIsBusyChanged(bool value)
    {
        OnPropertyChanged(nameof(IsNotBusy));
    }
}
