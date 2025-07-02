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
    Match ReplaceMatch = null;
    private int lastMatchOffset = -1;
    private int previousCaretOffset = -1;
    private string previousSearchPattern = string.Empty;
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
        get => _replacePattern;
        set
        {
            if (_replacePattern != value)
            {
                _replacePattern = value;
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
    public ICommand ReplaceCommand => _replaceCommand ??= new OtherRelayCommands(ExecuteReplaceCommand, CanExecute);
    public ICommand ReplaceAllCommand => _replaceAllCommand ??= new OtherRelayCommands(ExecuteReplaceAllCommand, CanExecute);
    private void ExecuteCloseCommand(object? parameter)
    {
        CloseRequested?.Invoke();
    }
    private Match Search()
    {
        var fileText = _dataStorage.GetTextCallback();
        MatchCollection matches = _searchService.SearchPattern(fileText, SearchPattern, IsRegisterAware);

        if (matches.Count == 0)
        {
            _fileDialog.ShowMessage($"Не удалось найти {SearchPattern}", "Error");
            return null;
        }

        int currentCaretOffset = _dataStorage.GetCaretOffset();

        // 1. Сброс при смене паттерна — только в начало
        // 2. Сброс при ручном перемещении курсора — с позиции курсора
        if (SearchPattern != previousSearchPattern)
        {
            lastMatchOffset = 0;
        }
        else if (currentCaretOffset != previousCaretOffset)
        {
            lastMatchOffset = currentCaretOffset;
        }

        previousSearchPattern = SearchPattern;
        previousCaretOffset = currentCaretOffset;

        int index = -1;

        if (SelectedOption == "Down")
        {
            for (int i = 0; i < matches.Count; i++)
            {
                if (matches[i].Index >= lastMatchOffset)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                if (IsTextFairing)
                {
                    index = 0;
                }
                else
                {
                    _fileDialog.ShowMessage($"Достигнут конец документа", "Inf");
                    return null;
                }
            }
        }
        else if (SelectedOption == "Up")
        {
            for (int i = matches.Count - 1; i >= 0; i--)
            {
                if (matches[i].Index + matches[i].Length <= lastMatchOffset)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                if (IsTextFairing)
                {
                    index = matches.Count - 1;
                }
                else
                {
                    _fileDialog.ShowMessage($"Достигнуто начало документа", "Inf");
                    return null;
                }
            }
        }

        Match currentMatch = matches[index];

        // Обновляем offset после найденного слова
        lastMatchOffset = currentMatch.Index + currentMatch.Length;

        _dataStorage.ResultSearch(currentMatch.Index, currentMatch.Length);
        previousOption = SelectedOption;

        return currentMatch;
    }
    private void ExecuteSearchCommand(object? parameter)
    {
        ReplaceMatch = Search();
    }
    private void ExecuteReplaceCommand(object? parameter)
    {
        var (areaIndex, areaLength) = _dataStorage.GetSelectionCallback();
        var fileText = _dataStorage.GetTextCallback();
        string areaText = fileText.Substring(areaIndex, areaLength);
        if (!areaText.Equals(SearchPattern, StringComparison.OrdinalIgnoreCase))
        {
            ReplaceMatch = Search();
        }
        else
        {
            string resultReplace = _searchService.ReplaceText(fileText, ReplaceMatch.Index, ReplaceMatch.Length, ReplacePattern);
            _dataStorage.PushUpdatedText(resultReplace);
        }
    }
    private void ExecuteReplaceAllCommand(object? parameter)
    {
        var fileText = _dataStorage.GetTextCallback();
        string resultReplaceAll = _searchService.ReplaceAllText(fileText, SearchPattern, ReplacePattern, IsRegisterAware);
        _dataStorage.PushUpdatedText(resultReplaceAll);
    }
    private void ExecuteMinimizeCommand(object? parameter)
    {
        MinimizeRequested?.Invoke();
    }
    private bool CanExecute(object? parameter) => true;

}