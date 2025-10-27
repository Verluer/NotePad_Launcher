using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Domain.Attributes;
using Domain.Enum;
using Domain.IService.IFileSystem;
using Domain.IService.ISystemApp;
using Domain.IService.ITextUtils;
using Domain.IService.IValidation;
using Domain.Model;
using ICSharpCode.AvalonEdit.Document;
using Microsoft.Extensions.DependencyInjection;
using NotePad_Launcher.IServiceUI;
using NotePad_Launcher.MVVM.Commands;
using NotePad_Launcher.MVVM.FontPickerDialog;
using NotePad_Launcher.MVVM.FunctionalWindows.FindReplaceWindow;
using NotePad_Launcher.MVVM.FunctionalWindows.SettingsWindow;
using NotePad_Launcher.MVVM.ProgramInfDialog;
using NotePad_Launcher.ServiceUI;
using Service;
using Service.FileSystem;
using FontFamily = System.Windows.Media.FontFamily;
using FontStyle = System.Windows.FontStyle;


namespace NotePad_Launcher.ViewModels.MainWindow;

[RegisterService(ServiceLifetime.Singleton, asSelf: true)]
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
    private ICommand? _toggleSyntaxHighlightingCommand;
    private ICommand? _logCommand;
    private ICommand? _encryptedMethodCommand;
    private ICommand? _movingGithubCommand;
    private ICommand? _programInfCommand;
    private ICommand? _fontPickerCommand;
    private ICommand? _searchPatternCommand;
    private ICommand? _clearCommand;

    public event PropertyChangedEventHandler? PropertyChanged;

    public event Action? MaximizeRequested;
    public event Action? MinimizeRequested;
    public event Action? CloseRequested;

    public event Action<EncryptionMethod> EncryptedMethodExecuted;
    public event Action<FindReplaceMethod> SearchReplaceMethodExecuted;

    public event Action? UpdateWordWrapRequested;
    public event Action? UpdateSyntaxHighlightingRequested;

    private readonly IFileSystemManager _fileSystemManager;
    private readonly IFileDialog _fileDialog;
    private readonly IServiceFunctions _serviceFunctions;
    private readonly IDataStorage _dataStorage;
    private readonly IWindowService _windowService;
    private readonly IConfigService _configService;
    private readonly IValidationService _validationService;
    private readonly ILocalizationService _localizationService;
    private readonly ITextService _textService;
    private bool _checkSaveFile = true;
    public string FilePath;
    public bool CheckSaveFile
    {
        get => _checkSaveFile;
        set => SetField(ref _checkSaveFile, value);
    }
    private TextDocument _fileTextDocument;
    public TextDocument FileTextDocument
    {
        get => _fileTextDocument;
        set
        {
            if (!EqualityComparer<TextDocument>.Default.Equals(_fileTextDocument, value))
            {
                if (_fileTextDocument != null)
                    _fileTextDocument.Changed -= OnDocumentChanged;

                if (SetField(ref _fileTextDocument, value))
                {
                    if (_fileTextDocument != null)
                        _fileTextDocument.Changed += OnDocumentChanged;
                }
            }

        }
    }
    private string _fileName;
    public string FileName
    {
        get => _fileName;
        set => SetField(ref _fileName, value);
    }
    private double _selectedFontSize = 14;
    public double SelectedFontSize
    {
        get => _selectedFontSize;
        set => SetField(ref _selectedFontSize, value);
    }

    private FontFamily _selectedFontFamily;

    public FontFamily SelectedFontFamily
    {
        get => _selectedFontFamily;
        set => SetField(ref _selectedFontFamily, value);
    }
    private FontStyle _selectedFontStyle;
    public FontStyle SelectedFontStyle
    {
        get => _selectedFontStyle;
        set => SetField(ref _selectedFontStyle, value);
    }
    private FontWeight _selectedFontWeight;
    public FontWeight SelectedFontWeight
    {
        get => _selectedFontWeight;
        set => SetField(ref _selectedFontWeight, value);
    }

    private bool _isWordWrapEnabled;
    public bool IsWordWrapEnabled
    {
        get => _isWordWrapEnabled;
        set
        {
            if (SetField(ref _isWordWrapEnabled, value))
            {
                UpdateWordWrapRequested?.Invoke();
                App.Config.WordWrap = IsWordWrapEnabled;
                _configService.Save(App.Config);
            }
        }
    }
    private bool _isSyntaxHighlightingEnabled;
    public bool IsSyntaxHighlightingEnabled
    {
        get => _isSyntaxHighlightingEnabled;
        set
        {
            if (SetField(ref _isSyntaxHighlightingEnabled, value))
            {
                UpdateSyntax();
                App.Config.SyntaxToggle = IsSyntaxHighlightingEnabled;
                _configService.Save(App.Config);
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
        set => SetField(ref _startupFilePath, value);
    }


    #endregion
    public MainWindowVM(ILocalizationService localizationService, IWindowService windowService, IFileDialog fileDialog,
        IDataStorage dataStorage, IConfigService configService, IValidationService validationService, ITextService textService, IFileSystemManager fileSystemManager)
    {
        _fileSystemManager = fileSystemManager;
        _windowService = windowService;
        _fileDialog = fileDialog;
        _dataStorage = dataStorage;
        _configService = configService;
        _validationService = validationService;
        _localizationService = localizationService;
        _textService = textService;

        if (App.Config.DocsPath == "FirstLaunch")
        {
            _fileSystemManager.CreateDocumentsDirectory();
            string DocumentPath = _fileSystemManager.ExDirectoryFile("Documents");
            App.Config.DocsPath = DocumentPath;
            _configService.Save(App.Config);
        }

        IsSyntaxHighlightingEnabled = App.Config.SyntaxToggle;
        IsWordWrapEnabled = App.Config.WordWrap;

        _dataStorage.GetTextCallback = () => FileTextDocument.Text;
        _dataStorage.TextUpdated += OnTextUpdated;
        SelectedFontFamily = new FontFamily("Arial");
        _dataStorage.GetFontFamilySizeCallback = () => (SelectedFontSize, SelectedFontFamily, SelectedFontStyle, SelectedFontWeight);
        _dataStorage.FamilySizeUpdated += OnFamilySizeUpdated;
        FileTextDocument = new TextDocument();
        StartupFilePath = _dataStorage.StartupFilePath;

        if (string.IsNullOrEmpty(FilePath) && !string.IsNullOrEmpty(StartupFilePath))
        {
            var startFile = _fileSystemManager.OpenFile(StartupFilePath);
            FilePath = startFile.FilePath;
            FileName = startFile.FileName;
            FileTextDocument.Text = string.Empty;
            FileTextDocument.Text = startFile.FileText;
        }
        _dataStorage.SelectionFileUpdated += UpdateFileInfo;
        _dataStorage.UpdateSyntaxHighlighting += UpdateSyntax;
        _dataStorage.UpdateWordWrap += UpdateWordWrap;
    }
    #region Functions
    private void UpdateSyntax()
    {
        UpdateSyntaxHighlightingRequested?.Invoke();
    }
    private void UpdateWordWrap()
    {
        UpdateWordWrapRequested?.Invoke();
    }
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
    public void UpdateFileInfo(FileModel fileModel, bool deleteFile)
    {
        if (!CheckingSaveFile(CheckSaveFile))
        {
            return;
        }
        if (deleteFile)
        {
            var result = _fileDialog.ShowYesNoDialog(_localizationService["MainMessageConfirmDelete"], "");
            if (result == MessageBoxResult.Yes)
            {
                if (FilePath == fileModel.FilePath)
                {
                    FileTextDocument.Text = string.Empty;
                    _fileSystemManager.FileDelete(fileModel.FilePath);
                    FileName = string.Empty;
                    FilePath = string.Empty;
                    CheckSaveFile = true;
                    return;
                }
                else
                {
                    _fileSystemManager.FileDelete(fileModel.FilePath);
                    return;
                }
            }
            else return;
        }
        CurrentFile = fileModel;
        FilePath = CurrentFile.FilePath;
        FileTextDocument.Text = CurrentFile.FileText;
        FileName = CurrentFile.FileName;
    }
    private void OnDocumentChanged(object? sender, EventArgs e)
    {
        CheckSaveFile = _textService.CheckTextChange(FilePath, FileTextDocument.Text);
    }

    public bool CheckingSaveFile(bool saveFile)
    {
        if (!saveFile)
        {
            var result = _fileDialog.ShowYesNoDialog(
                $"{_localizationService["MainMessageNotSaved"]}", "");

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
    public ICommand ClearCommand => _clearCommand ??= new OtherRelayCommands(ExecuteClear, CanExecute);


    public ICommand ToggleWordWrapCommand => _toggleWordWrapCommand ??= new OtherRelayCommands(ExecuteWordWrap, CanExecute);
    public ICommand ToggleSyntaxHighlightingCommand => _toggleSyntaxHighlightingCommand ??= new OtherRelayCommands(ExecuteSyntaxHighlighting, CanExecute);
    public ICommand EncryptedMethodCommand => _encryptedMethodCommand ??= new OtherRelayCommands(ExecuteEncryptedMethod, CanExecute);
    public ICommand MovingGithubCommand => _movingGithubCommand ??= new OtherRelayCommands(ExecuteMovingGitHub, CanExecute);
    public ICommand ProgramInfCommand => _programInfCommand ??= new OtherRelayCommands(ExecuteProgramInf, CanExecute);
    public ICommand FontPickerCommand => _fontPickerCommand ??= new OtherRelayCommands(ExecuteFontPickerCommand, CanExecute);

    #endregion

    #region Execute Button Parameter
    private void ExecuteLogCommand(object? parameter)
    {
        _windowService.OpenWindowDialog<SettingsWindow>();
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

    private void ExecuteOpenFileDialog(object? parameter)
    {
        if (!CheckingSaveFile(CheckSaveFile))
        {
            return;
        }
        FilePath = _fileDialog.OpenTextFileDialog(FilePath, "txt");
        if (string.IsNullOrEmpty(FilePath)) return;
        var filePath = FilePath;
        var openFile = _fileSystemManager.OpenFile(filePath);
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
        string currectPathConfig = _fileDialog.InputTextDialog("Create New File", "Select create file folder:", "", false, true);
        currectPathConfig = Path.Combine(App.Config.DocsPath, currectPathConfig);
        var nameFile = _fileSystemManager.CreateFile(currectPathConfig, "NewFileText");
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
        string currectPathConfig = Path.GetDirectoryName(model.FilePath);
        string testPathConfig = Path.GetDirectoryName(currectPathConfig);

        if (App.Config.SaveSetting == "SaveDirectory" && App.Config.DocsPath != testPathConfig || string.IsNullOrEmpty(model.FilePath))
        {
            try
            {
                currectPathConfig = _fileDialog.InputTextDialog("Save File", "Select save folder:", "", false, true);
                if (currectPathConfig == null) return;
                currectPathConfig = Path.Combine(App.Config.DocsPath, currectPathConfig);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        if (!string.IsNullOrWhiteSpace(model.FileText))
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.FileName))
            {
                var tempModel = _fileSystemManager.CreateFile(currectPathConfig, "NewFileText");
                model.FileName = tempModel.FileName;
                model.FilePath = tempModel.FilePath;
                FileName = tempModel.FileName;
            }
            var saveFile = _fileSystemManager.SaveFile(model, App.Config.SaveSetting, currectPathConfig);
            FilePath = saveFile.FilePath;
            FileName = saveFile.FileName;
            CheckSaveFile = true;
            _fileDialog.ShowMessage($"{_localizationService["MainMessageSaved"]}", $"{_localizationService["MainMessageSavedTitle"]}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        else if (!string.IsNullOrWhiteSpace(model.FileName))
        {
            try
            {
                var saveFile = _fileSystemManager.SaveFile(model, App.Config.SaveSetting, currectPathConfig);
            FilePath = saveFile.FilePath;
            FileName = saveFile.FileName;
            CheckSaveFile = true;
            _fileDialog.ShowMessage(_localizationService["MainMessageSaved"], _localizationService["MainMessageSavedTitle"]);
        }
            catch (Exception ex)
            {
            MessageBox.Show(ex.ToString());
        }
    }
        else
        {
            _fileDialog.ShowMessage(_localizationService["MainMessageTextNull"], _localizationService["MainMessageSavedTitle"]);
        }
    }
    private void ExecuteSaveFileDialog(object? parameter)
    {
        var selectedPath = _fileDialog.SaveFileDialog(FilePath, FileName);
        if (!string.IsNullOrEmpty(selectedPath))
        {
            FilePath = selectedPath;
            _fileSystemManager.WriteAllText(FilePath, FileTextDocument.Text);
            FileName = Path.GetFileNameWithoutExtension(FilePath);
            CheckSaveFile = true;
            _fileDialog.ShowMessage($"{_localizationService["MainMessageSaved"]}:\n{FilePath}", _localizationService["MainMessageSavedTitle"]);
        }
    }
    private void ExecuteFileList(object? parameter)
    {
        _windowService.OpenWindow<FileListWindow>();

    }
    private void ExecuteDeleteFile(object? parameter)
    {
        if (_validationService.FileExists(FilePath))
        {
            var result = _fileDialog.ShowYesNoDialog(_localizationService["MainMessageConfirmDelete"], "");
            if (result == MessageBoxResult.Yes)
            {
                FileTextDocument.Text = string.Empty;
                _fileSystemManager.FileDelete(FilePath);
                FileName = string.Empty;
                FilePath = string.Empty;
                CheckSaveFile = true;
            }
        }
    }
    private void ExecuteSearchPattern(object? parameter)
    {
        if (parameter is FindReplaceMethod method)
        {
            _dataStorage.searchReplaceMethod = method;
            SearchReplaceMethodExecuted?.Invoke(method);
            _windowService.OpenWindow<FindReplaceWindow>();
        }
    }
    private void ExecuteClear(object? parameter)
    {
        if (!CheckingSaveFile(CheckSaveFile))
        {
            return;
        }
        FileTextDocument.Text = string.Empty;
        FileName = string.Empty;
        FilePath = string.Empty;
        CheckSaveFile = true;
    }
    private void ExecuteWordWrap(object? parameter)
    {
        IsWordWrapEnabled = !IsWordWrapEnabled;
    }
    private void ExecuteSyntaxHighlighting(object? parameter)
    {
       IsSyntaxHighlightingEnabled = !IsSyntaxHighlightingEnabled;
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