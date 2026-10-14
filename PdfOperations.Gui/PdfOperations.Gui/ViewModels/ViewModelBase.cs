using Avalonia;
using Avalonia.Styling;
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

    [ObservableProperty]
    private bool overwriteExistingFiles;

    public bool IsNotBusy => !IsBusy;

    partial void OnIsBusyChanged(bool value)
    {
        OnPropertyChanged(nameof(IsNotBusy));
    }

    public string StatusBackground => IsDarkTheme
        ? StatusKind switch
        {
            StatusKind.Success => "#123524",
            StatusKind.Error => "#451A1A",
            _ => "#1E293B"
        }
        : StatusKind switch
        {
            StatusKind.Success => "#ECFDF3",
            StatusKind.Error => "#FEF2F2",
            _ => "#EFF6FF"
        };

    public string StatusBorder => IsDarkTheme
        ? StatusKind switch
        {
            StatusKind.Success => "#22C55E",
            StatusKind.Error => "#EF4444",
            _ => "#3B82F6"
        }
        : StatusKind switch
        {
            StatusKind.Success => "#86EFAC",
            StatusKind.Error => "#FCA5A5",
            _ => "#BFDBFE"
        };

    public string StatusForeground => IsDarkTheme
        ? StatusKind switch
        {
            StatusKind.Success => "#DCFCE7",
            StatusKind.Error => "#FEE2E2",
            _ => "#DBEAFE"
        }
        : StatusKind switch
        {
            StatusKind.Success => "#166534",
            StatusKind.Error => "#991B1B",
            _ => "#1F2937"
        };

    private static bool IsDarkTheme =>
        Application.Current?.ActualThemeVariant == ThemeVariant.Dark ||
        Application.Current?.RequestedThemeVariant == ThemeVariant.Dark;

    public void RefreshStatusColors()
    {
        OnPropertyChanged(nameof(StatusBackground));
        OnPropertyChanged(nameof(StatusBorder));
        OnPropertyChanged(nameof(StatusForeground));
    }

    partial void OnStatusKindChanged(StatusKind value)
    {
        RefreshStatusColors();
    }
}
