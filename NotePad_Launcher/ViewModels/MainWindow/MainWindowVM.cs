using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Domain.Enum;
using Domain.IService;
using Domain.Model;
using ICSharpCode.AvalonEdit.Document;
using Microsoft.VisualBasic.Logging;
using NotePad_Launcher.Contracts;
using NotePad_Launcher.Services;
using NotePad_Launcher.ViewModels;
using Service;


namespace NotePad_Launcher.ViewModels.MainWindow;

public class MainWindowVM : INotifyPropertyChanged
{
    #region Variables
    private ICommand? _closeCommand;
    private ICommand? _minimizeCommand;
    private ICommand? _maximizeCommand;
    private ICommand? _openFileCommand;
    private ICommand? _createFileCommand;
    private ICommand? _saveFileCommand;
    private ICommand? _saveFileDialogCommand;
    private ICommand? _fileListCommand;
    private ICommand? _deleteFileCommand;
    private ICommand? _toggleWordWrapCommand;
    private ICommand? _logCommand;
    private ICommand? _rsaCommand;
    private ICommand? _elgamalCommand;
    private ICommand? _rabinaCommand;
    private ICommand? _eccCommand;

    public event Action? MaximizeRequested;
    public event Action? MinimizeRequested;
    public Action UpdateWordWrapAction;
    private string FilePath;

    private readonly IFileService _fileService;
    private readonly IFileDialog _fileDialog;
    private bool _checkSaveFile = true;
    public bool CheckSaveFile
    {
        get => _checkSaveFile;
        set
        {
            if (_checkSaveFile != value)
            {
                _checkSaveFile = value;
                OnPropertyChanged();
            }
        }
    }
    public MainWindowVM()
    {
        _fileService = new FileService();
        _fileDialog = new FileDialog();
        FileTextDocument = new TextDocument();
        _fileService.ExDirectoryFile();
    }
    private TextDocument _fileTextDocument;
    public TextDocument FileTextDocument
    {
        get => _fileTextDocument;
        set
        {
            if (_fileTextDocument != value)
            {
                if (_fileTextDocument != null)
                {
                    _fileTextDocument.Changed -= OnDocumentChanged;
                }

                _fileTextDocument = value;

                if (_fileTextDocument != null)
                {
                    _fileTextDocument.Changed += OnDocumentChanged;
                }

                OnPropertyChanged();
            }
        }
    }
    private string _fileName;
    public string FileName
    {
        get => _fileName;
        set
        {
            if (_fileName != value)
            {
                _fileName = value;
                OnPropertyChanged();
            }
        }
    }
    private bool _isWordWrapEnabled;
    public bool IsWordWrapEnabled
    {
        get => _isWordWrapEnabled;
        set
        {
            if (_isWordWrapEnabled != value)
            {
                _isWordWrapEnabled = value;
                OnPropertyChanged();

                UpdateWordWrapAction?.Invoke();
            }
        }
    }


    public event PropertyChangedEventHandler? PropertyChanged;


    #endregion

    #region Functions
    private void OnDocumentChanged(object? sender, EventArgs e)
    {
        CheckSaveFile = _fileService.CheckTextChange(FilePath, FileTextDocument.Text);
    }

    public bool CheckingSaveFile(bool saveFile)
    {
        if (!saveFile) 
        {
            var result = _fileDialog.ShowYesNoDialog(
                "Текстовой файл не был сохранен, вы хотите продолжить?", "");

            return result == MessageBoxResult.Yes;
        }

        return true;
    }
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void OpenEncryptionWindow(EncryptionMethod method)
    {
        var encryptionWindow = new EncryptionWindow(method);
        encryptionWindow.EncryptionResultAction = (newText) =>
        {
            FileTextDocument.Text = newText;
        };
        encryptionWindow.Show();
    }
    public void LogMessage(string message)
    {
        string logFilePath = "D:\\VIsual Studio\\VS project\\NotePad_Launcher\\NotePad_Launcher\\bin\\Debug\\net8.0-windows\\Documents\\log.txt";
        File.AppendAllText(logFilePath, DateTime.Now + ": " + message + Environment.NewLine);
    }
    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
    #endregion

    #region ICommand Button
    public ICommand LogCommand => _logCommand ??= new OtherRelayCommands(ExecuteLogCommand, CanExecute);
    #region Tools bar 
    public ICommand CloseCommand => _closeCommand ??= new OtherRelayCommands(ExecuteCloseCommand, CanExecute);
    public ICommand MinimizeCommand => _minimizeCommand ??= new OtherRelayCommands(ExecuteMinimizeCommand, CanExecute);
    public ICommand MaximizeCommand => _maximizeCommand ??= new OtherRelayCommands(ExecuteMaximizeCommand, CanExecute);
    #endregion
        #region File
    public ICommand OpenFileCommand => _openFileCommand ??= new OtherRelayCommands(ExecuteOpenFileDialog, CanExecute);
    public ICommand CreateFileCommand => _createFileCommand ??= new OtherRelayCommands(ExecuteCreateFile, CanExecute);
    public ICommand SaveFileCommand => _saveFileCommand ??= new OtherRelayCommands(ExecuteSaveFile, CanExecute);
    public ICommand SaveFileDialogCommand => _saveFileDialogCommand ??= new OtherRelayCommands(ExecuteSaveFileDialog, CanExecute);
    public ICommand FileListCommand => _fileListCommand ??= new OtherRelayCommands(ExecuteFileList, CanExecute);
    public ICommand DeleteFileCommand => _deleteFileCommand ??= new OtherRelayCommands(ExecuteDeleteFile, CanExecute);
    public ICommand ToggleWordWrapCommand => _toggleWordWrapCommand ??= new OtherRelayCommands(ExecuteWordWrap, CanExecute);
        #endregion
        #region Encryption
    public ICommand RSACommand => _rsaCommand ??= new OtherRelayCommands(ExecuteRSACommand, CanExecute);
    public ICommand ElgamalCommand => _elgamalCommand ??= new OtherRelayCommands(ExecuteElgamalCommand, CanExecute);
    public ICommand RabinaCommand => _rabinaCommand ??= new OtherRelayCommands(ExecuteRabinaCommand, CanExecute);
    public ICommand ECCCommand => _eccCommand ??= new OtherRelayCommands(ExecuteECCCommand, CanExecute);

    #endregion
    #endregion

    #region Execute Button Parameter
    // Действие (событие) кнопок
    private void ExecuteLogCommand(object? parameter)
    {
        LogMessage(CheckSaveFile.ToString());
    }
    private void ExecuteCloseCommand(object? parameter)
    {
        Application.Current.Shutdown();
    }
    private void ExecuteMinimizeCommand(object? parameter)
    {
        MinimizeRequested?.Invoke();
    }
    private void ExecuteMaximizeCommand(object? parameter)
    {
        MaximizeRequested?.Invoke();
    }

    private void ExecuteOpenFileDialog(object? parameter)
    {
        if (!CheckingSaveFile(CheckSaveFile))
        {
            return;
        }
        FilePath = _fileDialog.OpenTextFileDialog(FilePath);
        if (string.IsNullOrEmpty(FilePath)) return; 
        var filePath = FilePath;
        var openFile = _fileService.OpenFile(filePath);
            FilePath = openFile.FilePath;
            FileName = openFile.FileName;
            FileTextDocument.Text = string.Empty;
            FileTextDocument.Text = openFile.FileText;
    }

    private void ExecuteCreateFile(object? parameter)
    {
        if (!CheckingSaveFile(CheckSaveFile))
        {
            return;
        }

        var nameFile = _fileService.CreateFile();
        FileName = nameFile.FileName;
        FileTextDocument.Text = string.Empty;
        FilePath = nameFile.FilePath;
    }

    private void ExecuteSaveFile(object? parameter)
    {
        var model = new FileModel
        {
            FileText = FileTextDocument.Text,
            FileName = FileName,
            FilePath = FilePath
        };
        if (!string.IsNullOrEmpty(model.FileText.Trim()))
        {
            var saveFile = _fileService.SaveFile(model);
            FilePath = saveFile.FilePath;
            CheckSaveFile = true;
            _fileDialog.ShowMessage("Текстовой файл успешно сохранен", "Сохранение");
        }
        else
            _fileDialog.ShowMessage("Введите текст для текстового файла", "Сохранение");
    }
    private void ExecuteSaveFileDialog(object? parameter)
    {

            FilePath = _fileDialog.SaveFileDialog(FilePath);
            _fileService.WriteAllText(FilePath, FileTextDocument.Text);
            FileName = _fileService.GetFileName(FilePath);
            CheckSaveFile = true;
            _fileDialog.ShowMessage($"Файл сохранен:\n{FilePath}", "Сохранение");
    }
    private void ExecuteFileList(object? parameter)
    {
        var fileListWindow = new FileListWindow(_fileService);
        fileListWindow.Show();
    }
    private void ExecuteDeleteFile(object? parameter)
    {
        if (_fileService.FileExists(FilePath))
        {
            var result = _fileDialog.ShowYesNoDialog(
                "Вы точно хотите удалить этот текстовой файл??");
            if (result == MessageBoxResult.Yes)
            {
                FileTextDocument.Text = string.Empty;
                _fileService.DeleteFile(FilePath);
                FileName = string.Empty;
                FilePath = string.Empty;
                CheckSaveFile = true;
            }
        }
    }
    private void ExecuteWordWrap(object? parameter)
    {
        IsWordWrapEnabled = !IsWordWrapEnabled;
    }

    private void ExecuteRSACommand(object? parameter)
    {
        OpenEncryptionWindow(EncryptionMethod.RSA);
    }

    private void ExecuteElgamalCommand(object? parameter)
    {
        OpenEncryptionWindow(EncryptionMethod.Elgamal);
    }

    private void ExecuteRabinaCommand(object? parameter)
    {
        OpenEncryptionWindow(EncryptionMethod.Rabina);
    }

    private void ExecuteECCCommand(object? parameter)
    {
        OpenEncryptionWindow(EncryptionMethod.ECC);
    }

    #endregion

    #region CanExecute Button Parameter 
    private bool CanExecute(object? parameter) => true;


    #endregion
}