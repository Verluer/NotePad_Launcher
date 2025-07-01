using NotePad_Launcher.MVVM.Commands;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Domain.IService;
using Microsoft.Extensions.DependencyInjection;
using System.Text.RegularExpressions;
using System.Windows.Forms.VisualStyles;
using Service;
using Domain.Enum;
using System.Windows;

namespace NotePad_Launcher.MVVM.FunctionalWindows.SearchWindow;

public class SearchWindowVM : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public event Action? MinimizeRequested;
    public event Action? CloseRequested;
    private readonly ISearchService _searchService;
    private readonly IDataStorage _dataStorage;
    private readonly IServiceFunctions _serviceFunctions;
    private readonly IFileDialog _fileDialog;
    private ICommand? _closeCommand;
    private ICommand? _minimizeCommand;
    private ICommand? _searchCommand;
    private ICommand? _replaceCommand;
    private ICommand? _replaceAllCommand;
    private static int currentMatchIndex = -1;
    private string previousOption = "None";
    private string _searchPattern;
    public string SearchPattern
    {
        get => _searchPattern;
        set
        {
            if (_searchPattern != value)
            {
                _searchPattern = value;
                OnPropertyChanged();
            }
        }
    }
    private string _titleMethod;
    public string TitleMethod
    {
        get => _titleMethod;
        set
        {
            if (_titleMethod != value)
            {
                _titleMethod = value;
                OnPropertyChanged();
            }
        }
    }
    private string _replacePattern;
    public string ReplacePattern
    {
        get => _searchPattern;
        set
        {
            if (_searchPattern != value)
            {
                _searchPattern = value;
                OnPropertyChanged();
            }
        }
    }
    private string _selectedOption = "Down";

    public string SelectedOption
    {
        get { return _selectedOption; }
        set
        {
            if (_selectedOption != value)
            {
                _selectedOption = value;
                OnPropertyChanged(nameof(SelectedOption));
            }
        }
    }
    private bool _isRegisterAware = false;

    public bool IsRegisterAware
    {
        get => _isRegisterAware;
        set
        {
            if (_isRegisterAware != value)
            {
                _isRegisterAware = value;
                OnPropertyChanged(nameof(IsRegisterAware));
            }
        }
    }
    private bool _isTextFairing = false;

    public bool IsTextFairing
    {
        get => _isTextFairing;
        set
        {
            if (_isTextFairing != value)
            {
                _isTextFairing = value;
                OnPropertyChanged(nameof(IsTextFairing));
            }
        }
    }
    private Thickness _buttonCloseMargin = new Thickness(0);
    public Thickness ButtonCloseMargin
    {
        get => _buttonCloseMargin;
        set
        {
            _buttonCloseMargin = value;
            OnPropertyChanged(nameof(ButtonCloseMargin));
        }
    }
    private bool _isButtonReplaceVisible = true;

    public bool IsButtonReplaceVisible
    {
        get => _isButtonReplaceVisible;
        set
        {
            if (_isButtonReplaceVisible != value)
            {
                _isButtonReplaceVisible = value;
                OnPropertyChanged(nameof(IsButtonReplaceVisible)); 
            }
        }
    }
    private bool _isButtonReplaceAllVisible = true;

    public bool IsButtonReplaceAllVisible
    {
        get => _isButtonReplaceAllVisible;
        set
        {
            if (_isButtonReplaceAllVisible != value)
            {
                _isButtonReplaceAllVisible = value;
                OnPropertyChanged(nameof(IsButtonReplaceAllVisible));
            }
        }
    }
   private bool _isTextBlockReplaceVisible = true;

    public bool IsTextBlockReplaceVisible
    {
        get => _isTextBlockReplaceVisible;
        set
        {
            if (_isTextBlockReplaceVisible != value)
            {
                _isTextBlockReplaceVisible = value;
                OnPropertyChanged(nameof(IsTextBlockReplaceVisible));
            }
        }
    }
    private bool _isTextBoxReplaceVisible = true;

    public bool IsTextBoxReplaceVisible
    {
        get => _isTextBoxReplaceVisible;
        set
        {
            if (_isTextBoxReplaceVisible != value)
            {
                _isTextBoxReplaceVisible = value;
                OnPropertyChanged(nameof(IsTextBoxReplaceVisible));
            }
        }
    }
    private bool _isRadioButtonUpVisible = true;

    public bool IsRadioButtonUpVisible
    {
        get => _isRadioButtonUpVisible;
        set
        {
            if (_isRadioButtonUpVisible != value)
            {
                _isRadioButtonUpVisible = value;
                OnPropertyChanged(nameof(IsRadioButtonUpVisible));
            }
        }
    }
    private bool _isRadioButtonDownVisible = true;

    public bool IsRadioButtonDownVisible
    {
        get => _isRadioButtonDownVisible;
        set
        {
            if (_isRadioButtonDownVisible != value)
            {
                _isRadioButtonDownVisible = value;
                OnPropertyChanged(nameof(IsRadioButtonDownVisible));
            }
        }
    }
    private SearchReplaceMethod _selectedMethod;
    public SearchReplaceMethod SelectedMethod { get; }
    public SearchWindowVM(IDataStorage dataStorage)
    {
        _searchService = App.ServiceProvider.GetRequiredService<ISearchService>();
        _dataStorage = dataStorage;
        SelectedMethod = _dataStorage.searchReplaceMethod;
        _serviceFunctions = new ServiceFunctions();
        _fileDialog = new FileDialog();
        switch (SelectedMethod)
        {
            case SearchReplaceMethod.Search:
                SerachUI();
                break;
            case SearchReplaceMethod.Replace:
                ReplaceUI();
                break;

        }
    }
    private void SerachUI()
    {
        IsButtonReplaceVisible = false;
        IsButtonReplaceAllVisible = false;
        ButtonCloseMargin = new Thickness(0, 10, 0, 0);
        IsTextBlockReplaceVisible = false;
        IsTextBoxReplaceVisible = false;
        TitleMethod = " Search";
    }
    private void ReplaceUI()
    {
        IsRadioButtonDownVisible = false;
        IsRadioButtonUpVisible = false;
        ButtonCloseMargin = new Thickness(0, 5, 0, 0);
        TitleMethod = " Replace";
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
    public ICommand CloseCommand => _closeCommand ??= new OtherRelayCommands(ExecuteCloseCommand, CanExecute);
    public ICommand MinimizeCommand => _minimizeCommand ??= new OtherRelayCommands(ExecuteMinimizeCommand, CanExecute);
    public ICommand SearchCommand => _searchCommand ??= new OtherRelayCommands(ExecuteSearchCommand, CanExecute);

    private void ExecuteCloseCommand(object? parameter)
    {
        CloseRequested?.Invoke();
    }

    private void ExecuteSearchCommand(object? parameter)
    {
        var fileText = _dataStorage.GetTextCallback();
        MatchCollection matches = _searchService.SearchPattern(fileText, SearchPattern, IsRegisterAware);

        if (matches.Count == 0)
        {
            _fileDialog.ShowMessage($"Не удалось найти {SearchPattern}", "Error");
            return;
        }

        if (SelectedOption == "Down")
        {
            currentMatchIndex++;
            if (currentMatchIndex >= matches.Count)
            {
                currentMatchIndex = IsTextFairing ? 0 : matches.Count - 1;
                if (!IsTextFairing)
                {
                    _fileDialog.ShowMessage($"Достигнут конец документа", "Inf");
                    return;
                }
            }
        }
        else if (SelectedOption == "Up")
        {
            currentMatchIndex--;
            if (currentMatchIndex < 0)
            {
                currentMatchIndex = IsTextFairing ? matches.Count - 1 : 0;
                if (!IsTextFairing)
                {
                    _fileDialog.ShowMessage($"Достигнуто начало документа", "Inf");
                    return;
                }
            }
        }

        Match currentMatch = matches[currentMatchIndex];
        _dataStorage.ResultSearch(currentMatch.Index, currentMatch.Length);
        previousOption = SelectedOption;
    }
    private void ExecuteMinimizeCommand(object? parameter)
    {
        MinimizeRequested?.Invoke();
    }
    private bool CanExecute(object? parameter) => true;

}