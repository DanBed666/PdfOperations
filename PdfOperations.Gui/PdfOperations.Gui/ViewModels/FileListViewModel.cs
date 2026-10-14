using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PdfOperations.Gui.Models;

namespace PdfOperations.Gui.ViewModels;

public partial class FileListViewModel : ViewModelBase
{
    public ObservableCollection<SelectedFileItem> Files { get; } = new();

    public bool HasFiles => Files.Count > 0;

    [ObservableProperty]
    private SelectedFileItem? selectedFile;
    
    public event Action? FilesChanged;
    
    public AvaloniaList<SelectedFileItem> SelectedFiles { get; } = new();
    
    [ObservableProperty]
    private bool showMoveButtons;

    public void SetFiles(IEnumerable<string> paths)
    {
        Files.Clear();

        foreach (string path in paths)
        {
            Files.Add(new SelectedFileItem { Path = path });
        }

        RefreshNumbers();
        
        NotifyFilesChanged();
    }

    public void AddFiles(IEnumerable<string> paths)
    {
        foreach (string path in paths)
        {
            if (Files.Any(file => file.Path == path))
                continue;

            Files.Add(new SelectedFileItem { Path = path });
        }

        RefreshNumbers();
        
        NotifyFilesChanged();
    }

    public void RemoveFiles(IEnumerable<SelectedFileItem> selectedFiles)
    {
        foreach (SelectedFileItem selectedFile in selectedFiles.ToArray())
        {
            Files.Remove(selectedFile);
        }

        SelectedFile = null;
        SelectedFiles.Clear();
        RefreshNumbers();
        
        NotifyFilesChanged();
    }

    public void Clear()
    {
        Files.Clear();
        SelectedFile = null;
        RefreshNumbers();
        
        NotifyFilesChanged();
    }

    public string[] ToPathArray()
    {
        return Files.Select(file => file.Path).ToArray();
    }

    public bool IsEmpty()
    {
        return Files.Count == 0;
    }

    private void RefreshNumbers()
    {
        for (int i = 0; i < Files.Count; i++)
        {
            Files[i].Number = i + 1;
        }
    }

    private void NotifyFilesChanged()
    {
        OnPropertyChanged(nameof(HasFiles));
        FilesChanged?.Invoke();
    }
    
    [RelayCommand]
    public void MoveSelectedUp()
    {
        if (SelectedFile is null)
            return;

        SelectedFileItem movedFile = SelectedFile;
        int index = Files.IndexOf(movedFile);

        if (index <= 0)
            return;

        Files.Move(index, index - 1);
        SelectedFile = movedFile;

        RefreshNumbers();
        NotifyFilesChanged();
    }

    [RelayCommand]
    public void MoveSelectedDown()
    {
        if (SelectedFile is null)
            return;

        SelectedFileItem movedFile = SelectedFile;
        int index = Files.IndexOf(movedFile);

        if (index < 0 || index >= Files.Count - 1)
            return;

        Files.Move(index, index + 1);
        SelectedFile = movedFile;

        RefreshNumbers();
        NotifyFilesChanged();
    }
    
    [RelayCommand]
    public void RemoveSelectedFile()
    {
        if (SelectedFiles.Count > 0)
        {
            RemoveFiles(SelectedFiles.ToArray());
            SelectedFiles.Clear();
            return;
        }

        if (SelectedFile is null)
            return;

        RemoveFiles(new[] { SelectedFile });
    }

    [RelayCommand]
    public void ClearFiles()
    {
        Clear();
    }
}
