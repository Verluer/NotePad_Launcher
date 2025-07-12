using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Domain.Enum;
using Domain.IService;
using Domain.Model;
using ICSharpCode.AvalonEdit.Document;
using Microsoft.Extensions.DependencyInjection;
using NotePad_Launcher.MVVM.Commands;
using NotePad_Launcher.MVVM.FontPickerDialog;
using NotePad_Launcher.MVVM.FunctionalWindows.SearchWindow;
using NotePad_Launcher.MVVM.FunctionalWindows.SettingsWindow;
using NotePad_Launcher.MVVM.ProgramInfDialog;
using NotePad_Launcher.ServiceUI;
using Service;
using FontFamily = System.Windows.Media.FontFamily;
using FontStyle = System.Windows.FontStyle;


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
    private ICommand? _encryptedMethodCommand;
    private ICommand? _movingGithubCommand;
    private ICommand? _programInfCommand;
    private ICommand? _fontPickerCommand;
    private ICommand? _searchPatternCommand;

    public event Action? MaximizeRequested;
    public event Action? MinimizeRequested;
    public event Action<EncryptionMethod> EncryptedMethodExecuted;
    public event Action<SearchReplaceMethod> SearchReplaceMethodExecuted;

    public Action UpdateWordWrapAction;

    private readonly IFileService _fileService;
    private readonly IFileDialog _fileDialog;
    private readonly IServiceFunctions _serviceFunctions;
    private readonly IDataStorage _dataStorage;
    private readonly IFileAssociationService _fileAssociationService;
    private readonly IWindowService _windowService;
    private readonly IConfigService _configService;

    private bool _checkSaveFile = true;
    public string FilePath;
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
    private double _selectedFontSize = 14;
    public double SelectedFontSize
    {
        get => _selectedFontSize;
        set
        {
            if (_selectedFontSize != value)
            {
                _selectedFontSize = value;
                OnPropertyChanged();
            }
        }
    }

    private FontFamily _selectedFontFamily;

    public FontFamily SelectedFontFamily
    {
        get => _selectedFontFamily;
        set
        {
            if (_selectedFontFamily != value)
            {
                _selectedFontFamily = value;
                OnPropertyChanged();
            }
        }
    }
    private FontStyle _selectedFontStyle;
    public FontStyle SelectedFontStyle
    {
        get => _selectedFontStyle;
        set
        {
            if (_selectedFontStyle != value)
            {
                _selectedFontStyle = value;
                OnPropertyChanged();
            }
        }
    }
    private FontWeight _selectedFontWeight;
    public FontWeight SelectedFontWeight
    {
        get => _selectedFontWeight;
        set
        {
            if (_selectedFontWeight != value)
            {
                _selectedFontWeight = value;
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
    private FileModel _currentFile;

    public FileModel CurrentFile
    {
        get => _currentFile;
        set => SetField(ref _currentFile, value);
    }
    private string _startupFilePath;
    public string StartupFilePath
    {
        get => _startupFilePath;
        set
        {
            if (_startupFilePath != value)
            {
                _startupFilePath = value;
                OnPropertyChanged();
            }
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged;


    #endregion
    public MainWindowVM()
    {
        _fileService = App.ServiceProvider.GetRequiredService<IFileService>();
        _windowService = App.ServiceProvider.GetRequiredService<IWindowService>();
        _fileDialog = App.ServiceProvider.GetRequiredService<IFileDialog>();
        _serviceFunctions = new ServiceFunctions();
        _dataStorage = App.ServiceProvider.GetRequiredService<IDataStorage>();
        _fileAssociationService = App.ServiceProvider.GetRequiredService<IFileAssociationService>();
        _fileService.CreateDocumentsDirectory();
        if (App.Config.DocsPath == "FirstLaunch")
        {
            string DocumentPath = _fileService.ExDirectoryFile("Documents");
            App.Config.DocsPath = DocumentPath;
            _configService.Save(App.Config);
        }
        LocalizationService.Instance.LoadLanguage(App.Config.Language);
        _dataStorage.GetTextCallback = () => FileTextDocument.Text;
        _dataStorage.TextUpdated += OnTextUpdated;
        SelectedFontFamily = new FontFamily("Arial");
        _dataStorage.GetFontFamilySizeCallback = () => (SelectedFontSize, SelectedFontFamily, SelectedFontStyle, SelectedFontWeight);
        _dataStorage.FamilySizeUpdated += OnFamilySizeUpdated;
        FileTextDocument = new TextDocument();
        _fileAssociationService.RegisterTxtFileAssociation();
        StartupFilePath = _dataStorage.StartupFilePath;
        if (string.IsNullOrEmpty(FilePath) && !string.IsNullOrEmpty(StartupFilePath))
        {
            var startFile = _fileService.OpenFile(StartupFilePath);
            FilePath = startFile.FilePath;
            FileName = startFile.FileName;
            FileTextDocument.Text = string.Empty;
            FileTextDocument.Text = startFile.FileText;

        }
    }
    #region Functions

    private void OnFamilySizeUpdated(double fontSize, FontFamily fontFamily, FontStyle fontStyle, FontWeight fontWeight)
    {
        SelectedFontFamily = fontFamily;
        SelectedFontSize = fontSize;
        SelectedFontStyle = fontStyle;
        SelectedFontWeight = fontWeight;
    }
    private void OnTextUpdated(string newText)
    {
        FileTextDocument.Text = newText;
    }
    public void UpdateFileInfo(FileModel fileModel)
    {
        if (!CheckingSaveFile(CheckSaveFile))
        {
            return;
        }

        CurrentFile = fileModel;
        FilePath = CurrentFile.FilePath;
        FileTextDocument.Text = CurrentFile.FileText;
        FileName = CurrentFile.FileName;
    }
    private void OnDocumentChanged(object? sender, EventArgs e)
    {
        CheckSaveFile = _fileService.CheckTextChange(FilePath, FileTextDocument.Text);
    }

    public bool CheckingSaveFile(bool saveFile)
    {
        if (!saveFile)
        {
            var result = _fileDialog.ShowYesNoDialog(
                $"{LocalizationService.Instance["MainMessageNotSaved"]}", "");

            return result == MessageBoxResult.Yes;
        }

        return true;
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
    #endregion

    #region ICommand Button
    public ICommand LogCommand => _logCommand ??= new OtherRelayCommands(ExecuteLogCommand, CanExecute);
    #region Tools bar 
    public ICommand CloseCommand => _closeCommand ??= new OtherRelayCommands(ExecuteCloseCommand, CanExecute);
    public ICommand MinimizeCommand => _minimizeCommand ??= new OtherRelayCommands(ExecuteMinimizeCommand, CanExecute);
    public ICommand MaximizeCommand => _maximizeCommand ??= new OtherRelayCommands(ExecuteMaximizeCommand, CanExecute);
    #endregion
    public ICommand OpenFileCommand => _openFileCommand ??= new OtherRelayCommands(ExecuteOpenFileDialog, CanExecute);
    public ICommand CreateFileCommand => _createFileCommand ??= new OtherRelayCommands(ExecuteCreateFile, CanExecute);
    public ICommand SaveFileCommand => _saveFileCommand ??= new OtherRelayCommands(ExecuteSaveFile, CanExecute);
    public ICommand SaveFileDialogCommand => _saveFileDialogCommand ??= new OtherRelayCommands(ExecuteSaveFileDialog, CanExecute);
    public ICommand FileListCommand => _fileListCommand ??= new OtherRelayCommands(ExecuteFileList, CanExecute);
    public ICommand DeleteFileCommand => _deleteFileCommand ??= new OtherRelayCommands(ExecuteDeleteFile, CanExecute);
    public ICommand SearchPatternCommand => _searchPatternCommand ??= new OtherRelayCommands(ExecuteSearchPattern, CanExecute);
    
    
    public ICommand ToggleWordWrapCommand => _toggleWordWrapCommand ??= new OtherRelayCommands(ExecuteWordWrap, CanExecute);
    public ICommand EncryptedMethodCommand => _encryptedMethodCommand ??= new OtherRelayCommands(ExecuteEncryptedMethod, CanExecute);
    public ICommand MovingGithubCommand => _movingGithubCommand ??= new OtherRelayCommands(ExecuteMovingGitHub, CanExecute);
    public ICommand ProgramInfCommand => _programInfCommand ??= new OtherRelayCommands(ExecuteProgramInf, CanExecute);
    public ICommand FontPickerCommand => _fontPickerCommand ??= new OtherRelayCommands(ExecuteFontPickerCommand, CanExecute);

    #endregion

    #region Execute Button Parameter
    // Действие (событие) кнопок
    private void ExecuteLogCommand(object? parameter)
    {
        _windowService.OpenWindowDialog<SettingsWindow>();
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
        FilePath = _fileDialog.OpenTextFileDialog(FilePath, "txt");
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

        var nameFile = _fileService.CreateFile(App.Config.DocsPath);
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
            var saveFile = _fileService.SaveFile(model, App.Config.SaveSetting, App.Config.DocsPath);
            FilePath = saveFile.FilePath;
            CheckSaveFile = true;
            _fileDialog.ShowMessage(LocalizationService.Instance["MainMessageSaved"], LocalizationService.Instance["MainMessageSavedTitle"]);
        }
        else
            _fileDialog.ShowMessage(LocalizationService.Instance["MainMessageTextNull"], LocalizationService.Instance["MainMessageSavedTitle"]);
    }
    private void ExecuteSaveFileDialog(object? parameter)
    {
        var selectedPath = _fileDialog.SaveFileDialog(FilePath);
        if (!string.IsNullOrEmpty(selectedPath))
        {
            FilePath = selectedPath;
            _fileService.WriteAllText(FilePath, FileTextDocument.Text);
            FileName = _fileService.GetFileNameWithout(FilePath);
            CheckSaveFile = true;
            _fileDialog.ShowMessage($"{LocalizationService.Instance["MainMessageSaved"]}:\n{FilePath}", LocalizationService.Instance["MainMessageSavedTitle"]);
        }
    }
    private void ExecuteFileList(object? parameter)
    {
        _windowService.OpenWindow<NotePad_Launcher.FileListWindow>();
        
    }
    private void ExecuteDeleteFile(object? parameter)
    {
        if (_fileService.FileExists(FilePath))
        {
            var result = _fileDialog.ShowYesNoDialog(LocalizationService.Instance["MainMessageConfirmDelete"], "");
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
    private void ExecuteSearchPattern(object? parameter)
    {
        if (parameter is SearchReplaceMethod method)
        {
            _dataStorage.searchReplaceMethod = method;
            SearchReplaceMethodExecuted?.Invoke(method);
            _windowService.OpenWindow<SearchWindow>();
        }
    }
    private void ExecuteWordWrap(object? parameter)
    {
        IsWordWrapEnabled = !IsWordWrapEnabled;
    }

    private void ExecuteEncryptedMethod(object? parameter)
    {
        if (parameter is EncryptionMethod method)
        {
            _dataStorage.CurrentMethod = method;
            EncryptedMethodExecuted?.Invoke(method);
            _windowService.OpenWindow<NotePad_Launcher.EncryptionWindow>();
        }
    }
    private void ExecuteMovingGitHub(object? parameter)
    {
        string url = "https://github.com/Verluer/NotePad_Launcher";
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });
    }
    private void ExecuteProgramInf(object? parameter)
    {
        _windowService.OpenWindowDialog<ProgramInfDialog>();
    }
    private void ExecuteFontPickerCommand(object? parameter)
    {
        _windowService.OpenWindowDialog<FontPickerDialog>();
    }

    #endregion

    #region CanExecute Button Parameter 
    private bool CanExecute(object? parameter) => true;


    #endregion
}