using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Domain.Enum;
using Domain.IService;
using ICSharpCode.AvalonEdit.Document;
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
    private ICommand? _rsaCommand;
    private ICommand? _elgamalCommand;
    private ICommand? _rabinaCommand;
    private ICommand? _eccCommand;

    public event Action? MaximizeRequested;
    public event Action? MinimizeRequested;
    private string FilePath;

    private readonly IFileService _fileService;
    private readonly IFileDialog _fileDialog;

    private TextDocument _fileTextDocument = new TextDocument();
    public MainWindowVM()
    {
        _fileService = new FileService();
        _fileDialog = new FileDialog();
    }
    public TextDocument FileTextDocument
    {
        get => _fileTextDocument;
        set
        {
            _fileTextDocument = value;
            OnPropertyChanged();
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
        FilePath = _fileDialog.OpenTextFileDialog();
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