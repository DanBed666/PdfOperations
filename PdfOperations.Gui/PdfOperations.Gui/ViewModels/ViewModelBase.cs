using Avalonia;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.IO;
using PdfOperations.Gui.Models;
using PdfOperations.Gui.Services;

namespace PdfOperations.Gui.ViewModels;

public abstract partial class ViewModelBase : ObservableObject
{
    public LocalizationService Loc => LocalizationService.Instance;

    private string statusMessage = "";

    public string StatusMessage
    {
        get => statusMessage;
        set
        {
            ErrorDetails = "";
            AreErrorDetailsExpanded = false;
            SetProperty(ref statusMessage, value);
            IsStatusVisible = !string.IsNullOrWhiteSpace(value);
        }
    }

    [ObservableProperty]
    private bool isStatusVisible;

    [RelayCommand]
    private void DismissStatus()
    {
        if (!IsBusy)
            IsStatusVisible = false;
    }

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private StatusKind statusKind = StatusKind.Info;

    [ObservableProperty]
    private bool overwriteExistingFiles;

    public bool IsNotBusy => !IsBusy;

    partial void OnIsBusyChanged(bool value)
    {
        if (value)
        {
            CompletedFiles = 0;
            TotalFiles = 0;
        }

        OnPropertyChanged(nameof(IsNotBusy));
        OnPropertyChanged(nameof(HasFileProgress));
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
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasErrorDetails))]
    private string errorDetails = "";

    [ObservableProperty]
    private bool areErrorDetailsExpanded;

    public bool HasErrorDetails => !string.IsNullOrWhiteSpace(ErrorDetails);
    
    public virtual void RefreshLocalization()
    {
        OnPropertyChanged(string.Empty);
    }

    public void ShowError(Exception error)
    {
        StatusKind = StatusKind.Error;
        StatusMessage = Loc.ErrorSummary(error);
        ErrorDetails = error.ToString();
    }

    public void RefreshStatusColors()
    {
        OnPropertyChanged(nameof(StatusBackground));
        OnPropertyChanged(nameof(StatusBorder));
        OnPropertyChanged(nameof(StatusForeground));
    }
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ProgressText))]
    private int completedFiles;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ProgressText))]
    [NotifyPropertyChangedFor(nameof(HasFileProgress))]
    private int totalFiles;

    public string ProgressText => Loc.FilesProgress(CompletedFiles, TotalFiles);

    public bool HasFileProgress => IsBusy && TotalFiles > 0;

    protected IProgress<int> BeginFileProgress(int total)
    {
        CompletedFiles = 0;
        TotalFiles = total;

        return new Progress<int>(completed => CompletedFiles = completed);
    }

    protected static void SetDefaultOutputDirectoryFromInputFiles(
        string[] files,
        string outputDirectory,
        Action<string> setOutputDirectory)
    {
        if (!string.IsNullOrWhiteSpace(outputDirectory))
            return;

        if (files.Length == 0)
            return;

        string? directory = Path.GetDirectoryName(files[0]);

        if (!string.IsNullOrWhiteSpace(directory))
            setOutputDirectory(directory);
    }

    protected static bool CanOpenDirectory(string directory)
    {
        return !string.IsNullOrWhiteSpace(directory) && Directory.Exists(directory);
    }

    partial void OnStatusKindChanged(StatusKind value)
    {
        RefreshStatusColors();
    }
}
