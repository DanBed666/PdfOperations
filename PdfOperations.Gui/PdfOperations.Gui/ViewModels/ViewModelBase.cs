using CommunityToolkit.Mvvm.ComponentModel;
using PdfOperations.Gui.Models;
using PdfOperations.Gui.Services;

namespace PdfOperations.Gui.ViewModels;

public abstract partial class ViewModelBase : ObservableObject
{
    public LocalizationService Loc => LocalizationService.Instance;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private StatusKind statusKind = StatusKind.Info;

    public bool IsNotBusy => !IsBusy;

    partial void OnIsBusyChanged(bool value)
    {
        OnPropertyChanged(nameof(IsNotBusy));
    }

    public string StatusBackground => StatusKind switch
    {
        StatusKind.Success => "#ECFDF3",
        StatusKind.Error => "#FEF2F2",
        _ => "#EFF6FF"
    };

    public string StatusBorder => StatusKind switch
    {
        StatusKind.Success => "#86EFAC",
        StatusKind.Error => "#FCA5A5",
        _ => "#BFDBFE"
    };

    public string StatusForeground => StatusKind switch
    {
        StatusKind.Success => "#166534",
        StatusKind.Error => "#991B1B",
        _ => "#1F2937"
    };

    partial void OnStatusKindChanged(StatusKind value)
    {
        OnPropertyChanged(nameof(StatusBackground));
        OnPropertyChanged(nameof(StatusBorder));
        OnPropertyChanged(nameof(StatusForeground));
    }
}
