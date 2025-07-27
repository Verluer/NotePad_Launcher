using Domain.IService;
using Domain.Model;
using System;
using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using NotePad_Launcher.MVVM.Commands;
using Service;
using NotePad_Launcher.ViewModels;
using static System.Windows.Forms.Design.AxImporter;
using System.Windows;
using System.Windows.Controls;

namespace NotePad_Launcher.MVVM.FunctionalWindows.FileListWindow;

public class FileListWindowVM : INotifyPropertyChanged

{
    private readonly IFileService _fileService;
    private readonly IFileDialog _fileDialog;

    public event Action? MaximizeRequested;
    public event Action? MinimizeRequested;
    public event Action? CloseRequested;

    private ICommand? _closeCommand;
    private ICommand? _minimizeCommand;
    private ICommand? _maximizeCommand;
    private ICommand? _newFolderCreateCommand;

    public event PropertyChangedEventHandler? PropertyChanged;
    private List<FileModel> _fileListItem;

    public List<FileModel> FileListItem
    {
        get => _fileListItem;
        set => SetField(ref _fileListItem, value);
    }

    public ObservableCollection<FileModel> FileList { get; private set; }
    public ObservableCollection<FolderEntry> FolderFile { get; }

    private string _selectedFolder;
    public string SelectedFolder
    {
        get => _selectedFolder;
        set
        {
            if (_selectedFolder != value)
            {
                _selectedFolder = value;
                OnPropertyChanged(nameof(SelectedFolder));
                LoadFileList(value);
            }
        }
    }
    public FileListWindowVM(IFileService service, IFileDialog fileDialog)
    {
        _fileService = service;
        _fileDialog = fileDialog;
        FolderFile = new ObservableCollection<FolderEntry>();
        UploadFolder();
    }
    public void UploadFolder()
    {
        FolderFile.Clear();
        List<string> folderNames = _fileService.LoadFolderFile(App.Config.DocsPath);
        foreach (var folderName in folderNames)
        {
            FolderFile.Add(new FolderEntry(folderName, this));
        }
        if (FolderFile.Any())
            FolderFile[0].IsSelected = true;
    }
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
    public void SelectFolder(string name)
    {
        SelectedFolder = name;

        foreach (var item in FolderFile)
            item.IsSelected = item.Name == name;
    }

    private void LoadFileList(string Option)
    {
        try
        {
            var pathFile = Path.Combine(App.Config.DocsPath, Option);
            var files = _fileService.GetTextFiles(pathFile);
            FileListItem = files;
        }
        catch (Exception ex)
        {
            _fileDialog.ShowMessage($"Ошибка: {ex.Message}", "Warning");
        }
    }
    public ICommand CloseCommand => _closeCommand ??= new OtherRelayCommands(ExecuteCloseCommand, CanExecute);
    public ICommand MinimizeCommand => _minimizeCommand ??= new OtherRelayCommands(ExecuteMinimizeCommand, CanExecute);
    public ICommand MaximizeCommand => _maximizeCommand ??= new OtherRelayCommands(ExecuteMaximizeCommand, CanExecute);
    public ICommand NewFolderCreateCommand => _newFolderCreateCommand ??= new OtherRelayCommands(ExecuteNewFolderCreateCommand, CanExecute);

    private void ExecuteNewFolderCreateCommand(object? parameter)
    {
        int i = 1;
        while (true)
        {
            string tempPathFolder = Path.Combine(App.Config.DocsPath, $"NewFolder{i}");
            if (!Directory.Exists(tempPathFolder))
            {
                Directory.CreateDirectory(tempPathFolder);
                UploadFolder();
                MessageBox.Show($"Папка успешно создана {tempPathFolder}");
                break;
            }
            i++;
        }
    }
    private void ExecuteCloseCommand(object? parameter)
    {
        CloseRequested?.Invoke();
    }
    private void ExecuteMinimizeCommand(object? parameter)
    {
        MinimizeRequested?.Invoke();
    }
    private void ExecuteMaximizeCommand(object? parameter)
    {
        MaximizeRequested?.Invoke();
    }
    private bool CanExecute(object? parameter) => true;
    public class FolderEntry : INotifyPropertyChanged
    {
        private bool _isSelected;

        public string Name { get; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));

                    if (_isSelected)
                        parent.SelectFolder(Name);
                }
            }
        }

        private readonly FileListWindowVM parent;

        public FolderEntry(string name, FileListWindowVM parentViewModel)
        {
            Name = name;
            parent = parentViewModel;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
