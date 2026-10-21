using Avalonia;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.IO;
using System.Threading;
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
            operationCancellation = new CancellationTokenSource();
            IsCancellationRequested = false;
            Interlocked.Increment(ref progressVersion);
            Volatile.Write(ref reportedCompletedFiles, 0);

            CompletedFiles = 0;
            TotalFiles = 0;
        }
        else
        {
            operationCancellation?.Dispose();
            operationCancellation = null;
        }

        OnPropertyChanged(nameof(IsNotBusy));
        OnPropertyChanged(nameof(HasFileProgress));
        CancelOperationCommand.NotifyCanExecuteChanged();
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
    
    private CancellationTokenSource? operationCancellation;
    private int progressVersion;
    private int reportedCompletedFiles;

    protected CancellationToken OperationToken =>
        operationCancellation?.Token ?? CancellationToken.None;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CancelOperationCommand))]
    private bool isCancellationRequested;

    private bool CanCancelOperation() =>
        IsBusy && !IsCancellationRequested && operationCancellation is not null;

    [RelayCommand(CanExecute = nameof(CanCancelOperation))]
    private void CancelOperation()
    {
        if (!CanCancelOperation())
            return;

        IsCancellationRequested = true;
        StatusKind = StatusKind.Info;
        StatusMessage = Loc.Cancelling;
        operationCancellation!.Cancel();
    }

    public bool HasErrorDetails => !string.IsNullOrWhiteSpace(ErrorDetails);
    
    public virtual void RefreshLocalization()
    {
        OnPropertyChanged(string.Empty);
    }

    public void ShowError(Exception error)
    {
        if (error is OperationCanceledException cancelled
            && IsCancellationRequested && cancelled.CancellationToken == OperationToken)
        {
            CompletedFiles = Volatile.Read(ref reportedCompletedFiles);
            bool cleanupFailed = error.InnerException is AggregateException
                || error.Data[TemporaryDirectoryCleanup.ErrorDataKey] is string;
            StatusKind = cleanupFailed ? StatusKind.Error : StatusKind.Info;
            StatusMessage = Loc.OperationCancelled(CompletedFiles, TotalFiles);
            if (cleanupFailed)
            {
                StatusMessage += " " + Loc.CancellationCleanupFailed;
                ErrorDetails = error.ToString();
                if (error.Data[TemporaryDirectoryCleanup.ErrorDataKey] is string details)
                    ErrorDetails += Environment.NewLine + Environment.NewLine + details;
            }
            return;
        }

        StatusKind = StatusKind.Error;
        StatusMessage = Loc.ErrorSummary(error);
        ErrorDetails = error.ToString();
        if (error.Data[TemporaryDirectoryCleanup.ErrorDataKey] is string cleanupDetails)
            ErrorDetails += Environment.NewLine + Environment.NewLine + cleanupDetails;
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
        int version = Volatile.Read(ref progressVersion);
        Volatile.Write(ref reportedCompletedFiles, 0);
        IProgress<int> updates = new Progress<int>(completed =>
        {
            if (version == Volatile.Read(ref progressVersion) && IsBusy)
                CompletedFiles = completed;
        });
        return new CallbackProgress(completed =>
        {
            if (version != Volatile.Read(ref progressVersion))
                return;
            // Keep an exact count even if cancellation reaches the UI before a queued update.
            Volatile.Write(ref reportedCompletedFiles, completed);
            updates.Report(completed);
        });
    }

    private sealed class CallbackProgress(Action<int> report) : IProgress<int>
    {
        public void Report(int value) => report(value);
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
