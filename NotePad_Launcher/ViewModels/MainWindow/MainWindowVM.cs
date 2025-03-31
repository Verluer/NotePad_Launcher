using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Domain.Enum;
using Domain.IService;
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
    private ICommand? _logCommand;
    private ICommand? _rsaCommand;
    private ICommand? _elgamalCommand;
    private ICommand? _rabinaCommand;
    private ICommand? _eccCommand;

    public event Action? MaximizeRequested;
    public event Action? MinimizeRequested;
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
            MessageBoxResult result = _fileDialog.ShowYesNoDialog(
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