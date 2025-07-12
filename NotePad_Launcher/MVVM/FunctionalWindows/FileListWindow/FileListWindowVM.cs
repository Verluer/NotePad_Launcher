using Domain.IService;
using Domain.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using NotePad_Launcher.MVVM.Commands;
using Service;
using NotePad_Launcher.ViewModels;

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

    public event PropertyChangedEventHandler? PropertyChanged;
    private List<FileModel> _fileListItem;

    public List<FileModel> FileListItem
    {
        get => _fileListItem;
        set => SetField(ref _fileListItem, value);
    }

    public ObservableCollection<FileModel> FileList { get; private set; }

    public FileListWindowVM(IFileService service, IFileDialog fileDialog)
    {
        _fileService = service;
        _fileDialog = fileDialog;
        LoadFileList();
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
    private void LoadFileList()
    {
        try
        {
            var files = _fileService.GetTextFiles();
            FileListItem = files; // Привязываем список файлов
        }
        catch (Exception ex)
        {
            _fileDialog.ShowMessage($"Ошибка: {ex.Message}", "Warning");
        }
    }
    public ICommand CloseCommand => _closeCommand ??= new OtherRelayCommands(ExecuteCloseCommand, CanExecute);
    public ICommand MinimizeCommand => _minimizeCommand ??= new OtherRelayCommands(ExecuteMinimizeCommand, CanExecute);
    public ICommand MaximizeCommand => _maximizeCommand ??= new OtherRelayCommands(ExecuteMaximizeCommand, CanExecute);

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