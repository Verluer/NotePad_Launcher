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
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;
using NotePad_Launcher.ServiceUI;
using System.Collections;
using Domain.IService.IFileSystem;
using Domain.IService.IValidation;
using Service.FileSystem;
using Domain.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace NotePad_Launcher.MVVM.FunctionalWindows.FileListWindow;

[RegisterService(ServiceLifetime.Transient, asSelf: true)]
public class FileListWindowVM : INotifyPropertyChanged

{
    private readonly IFileSystemManager _fileSystemManager;
    private readonly IFileDialog _fileDialog;
    private readonly IDataStorage _dataStorage;
    private readonly IValidationService _validationService;

    public event Action? MaximizeRequested;
    public event Action? MinimizeRequested;
    public event Action? CloseRequested;

    private ICommand? _closeCommand;
    private ICommand? _minimizeCommand;
    private ICommand? _maximizeCommand;
    private ICommand? _folderCommand;
    private ICommand? _newFolderCreateCommand;
    private ICommand? _newFileCreateCommand;
    private ICommand? _folderEditNameCommand;
    private ICommand? _folderDeleteCommand;
    private ICommand? _updateCommand;
    private ICommand? _nextFolderCommand;
    private ICommand? _backFolderCommand;
    private ICommand? _menuRenameFileCommand;
    private ICommand? _menuDeleteFileCommand;
    private ICommand? _menuMoveFileCommand;

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<FileModel> FileListItem { get; set; } = new ObservableCollection<FileModel>();

    public ObservableCollection<string> FolderFileString { get; set; } = new ObservableCollection<string>();

    private string _selectedFolder;
    public string SelectedFolder
    {
        get => _selectedFolder;
        set
        {
            if (SetField(ref _selectedFolder, value))
            {
                LoadFileList(value);
            }
        }
    }
    private int _selectedIndex;
    public int SelectedIndex
    {
        get => _selectedIndex;
        set => SetField(ref _selectedIndex, value);
    }
    public FileListWindowVM(IFileSystemManager fileSystemManager, IFileDialog fileDialog, IDataStorage dataStorage, IValidationService validationService)
    {
        _fileSystemManager = fileSystemManager;
        _fileDialog = fileDialog;
        _validationService = validationService;

        UploadFolder(0);
        _dataStorage = dataStorage;
    }
    public void UploadFolder(int selectedIndex)
    {
        if (FolderFileString != null) FolderFileString.Clear();
        List<string> folderNames = _fileSystemManager.LoadFolderFile(App.Config.DocsPath);
        foreach (var folderName in folderNames)
        {
            FolderFileString.Add(folderName);
        }
        if (FolderFileString.Any()) SelectedIndex = selectedIndex;

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
    private void LoadFileList(string nameFolder)
    {
        try
        {
            var pathFile = Path.Combine(App.Config.DocsPath, nameFolder);
            var files = _fileSystemManager.GetTextFiles(pathFile); 

            if (FileListItem != null) FileListItem.Clear();

            foreach (var file in files)
            {
                FileListItem.Add(file); 
            }
        }
        catch (Exception ex)
        {
            return;
        }
    }
    public ICommand CloseCommand => _closeCommand ??= new OtherRelayCommands(ExecuteCloseCommand, CanExecute);
    public ICommand MinimizeCommand => _minimizeCommand ??= new OtherRelayCommands(ExecuteMinimizeCommand, CanExecute);
    public ICommand MaximizeCommand => _maximizeCommand ??= new OtherRelayCommands(ExecuteMaximizeCommand, CanExecute);
    public ICommand FolderCommand => _folderCommand ??= new OtherRelayCommands(ExecuteFolderCommand, CanExecute);
    public ICommand NewFolderCreateCommand => _newFolderCreateCommand ??= new OtherRelayCommands(ExecuteNewFolderCreateCommand, CanExecute);
    public ICommand NewFileCreateCommand => _newFileCreateCommand ??= new OtherRelayCommands(ExecuteNewFileCreateCommand, CanExecute);
    public ICommand FolderEditNameCommand => _folderEditNameCommand ??= new OtherRelayCommands(ExecuteFolderEditNameCommand, CanExecute);
    public ICommand FolderDeleteCommand => _folderDeleteCommand ??= new OtherRelayCommands(ExecuteFolderDeleteCommand, CanExecute);
    public ICommand UpdateCommand => _updateCommand ??= new OtherRelayCommands(ExecuteUpdateCommand, CanExecute);
    public ICommand NextFolderCommand => _nextFolderCommand ??= new OtherRelayCommands(ExecuteNextFolderCommand, CanExecute);
    public ICommand BackFolderCommand => _backFolderCommand ??= new OtherRelayCommands(ExecuteBackFolderCommand, CanExecute);
    public ICommand MenuRenameFileCommand => _menuRenameFileCommand ??= new OtherRelayCommands(ExecuteMenuRenameFileCommand, CanExecute);
    public ICommand MenuDeleteFileCommand => _menuDeleteFileCommand ??= new OtherRelayCommands(ExecuteMenuDeleteFileCommand, CanExecute);
    public ICommand MenuMoveFileCommand => _menuMoveFileCommand ??= new OtherRelayCommands(ExecuteMenuMoveFileCommand, CanExecute);
    private void ExecuteFolderCommand(object? parameter)
    {
        string selectedPath = Path.Combine(App.Config.DocsPath, SelectedFolder);
        if (_validationService.DirectoryExists(selectedPath))
        {
            System.Diagnostics.Process.Start("explorer.exe", selectedPath);
        }
    }
    private void ExecuteNewFolderCreateCommand(object? parameter)
    {
        string nameFolder = _fileDialog.InputTextDialog("Create New Folder", "Enter names for the new folder-directory:", "NewFolder", true, false);
        if (nameFolder == null) return;
        string pathFolder = Path.Combine(App.Config.DocsPath, $"{nameFolder}");
        if (!_validationService.DirectoryExists(pathFolder))
        {
            _fileSystemManager.DirectoryCreate(pathFolder);
            UploadFolder(SelectedIndex);
            _fileDialog.ShowMessage($"Folder successfully created {pathFolder}", "Created Folder");
            return;
        }
        int i = 1;
        while (true)
        {
            string tempPathFolder = Path.Combine(App.Config.DocsPath, $"{nameFolder}{i}");
            if (!_validationService.DirectoryExists(tempPathFolder))
            {
                _fileSystemManager.DirectoryCreate(tempPathFolder);  
                UploadFolder(SelectedIndex);
                _fileDialog.ShowMessage($"Folder successfully created {tempPathFolder}", "Created Folder");
                break;
            }
            i++;
        }
    }
    private void ExecuteNewFileCreateCommand(object? parameter)
    {
        string selectedPath = Path.Combine(App.Config.DocsPath, SelectedFolder);
        string fileName = _fileDialog.InputTextDialog("Create text file", "Enter a name for the text file:", "NewTextFile", true, false);
        if (fileName == null) return;
        _fileSystemManager.CreateFile(selectedPath, fileName);
        UploadFolder(SelectedIndex);
    }
    private void ExecuteFolderEditNameCommand(object? parameter)
    {
        string nameFolder = _fileDialog.InputTextDialog("Edit name", "Edit name Folder:", SelectedFolder, true, false);
        if (nameFolder == null) return;
        string oldPathFolder = Path.Combine(App.Config.DocsPath, SelectedFolder);
        string pathFolder = Path.Combine(App.Config.DocsPath, $"{nameFolder}");

        if (!_validationService.DirectoryExists(pathFolder))
        {
            _fileSystemManager.DirectoryMove(oldPathFolder, pathFolder);
            UploadFolder(SelectedIndex);
        }
        else _fileDialog.ShowMessage("Such a folder already exists.", "Folder Already Exists");

    }
    private void ExecuteFolderDeleteCommand(object? parameter)
    {
        string pathFolder = Path.Combine(App.Config.DocsPath, SelectedFolder);
        var result = _fileDialog.ShowYesNoDialog($"Are you sure you want to delete this directory?\n{pathFolder}", "Delete");

        if (result == MessageBoxResult.Yes)
        {
            _fileSystemManager.DirectoryDelete(pathFolder, true);
            UploadFolder(0);
        }
        else return;



    }
    private void ExecuteUpdateCommand(object? parameter)
    {
        UploadFolder(SelectedIndex);
    }
    private void ExecuteNextFolderCommand(object? parameter)
    {
        int currentIndex = -1;
        for (int i = 0; i < FolderFileString.Count; i++)
        {
            if (FolderFileString[i] == SelectedFolder)
            {
                currentIndex = i;
                break;
            }
        }
        if (currentIndex >= 0 && currentIndex < FolderFileString.Count - 1)
        {
            var nextFolder = currentIndex + 1;
            SelectedIndex = nextFolder;
        }
        else
        {
            return;
        }
    }
    private void ExecuteBackFolderCommand(object? parameter)
    {
        int currentIndex = -1;
        for (int i = 0; i < FolderFileString.Count; i++)
        {
            if (FolderFileString[i] == SelectedFolder)
            {
                currentIndex = i;
                break;
            }
        }

        if (currentIndex > 0)
        {
            var previousFolder = currentIndex - 1;
            SelectedIndex = previousFolder;
        }
        else
        {
            return;
        }
    }

    private void ExecuteMenuRenameFileCommand(object? parameter)
    {
        if (parameter is IList list)
        {
            var itemsFileModel = list.Cast<FileModel>().ToList();
            foreach (var model in itemsFileModel)
            {
                var newName = _fileDialog.InputTextDialog("Rename file", "Enter the new name of the text file", model.FileName, true, false);
                if (newName == null) { return; }
                var directoryPath = Path.GetDirectoryName(model.FilePath);
                var newPath = Path.Combine(directoryPath, $"{newName}.txt");
                _fileSystemManager.FileMove(model.FilePath, newPath);
                UploadFolder(SelectedIndex);
            }
        }
        else
        { _fileDialog.ShowMessage("error loading renaming elements", "Error"); }
    }
    private void ExecuteMenuDeleteFileCommand(object? parameter)
    {
        if (parameter is IList list)
        {
            var itemsFileModel = list.Cast<FileModel>().ToList();
            foreach (var model in itemsFileModel)
            {
                _dataStorage.PushUpdatedSelectionFile(model, true);
                UploadFolder(SelectedIndex);
            }
        }
        else { _fileDialog.ShowMessage("error loading removal items", "Error"); }
    }
    private void ExecuteMenuMoveFileCommand(object? parameter)
    {
        if (parameter is IList list)
        {
            var itemsFileModel = list.Cast<FileModel>().ToList();
            foreach (var model in itemsFileModel)
            {
                var result = _fileDialog.FolderFileDialog(App.Config.DocsPath);
                if (result == null) return;
                var name = Path.GetFileName(model.FilePath);
                var newPath = Path.Combine(result, name);
                _fileSystemManager.FileMove(model.FilePath, newPath);
                UploadFolder(SelectedIndex);
            }
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
}
