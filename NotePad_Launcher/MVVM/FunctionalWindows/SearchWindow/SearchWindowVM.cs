using NotePad_Launcher.MVVM.Commands;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Domain.IService;
using Microsoft.Extensions.DependencyInjection;
using System.Text.RegularExpressions;
using System.Windows.Forms.VisualStyles;
using Service;

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
    private string _selectedOption;

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
    public SearchWindowVM(IDataStorage dataStorage)
    {
        _searchService = App.ServiceProvider.GetRequiredService<ISearchService>();
        _dataStorage = dataStorage;
        _serviceFunctions = new ServiceFunctions();
        _fileDialog = new FileDialog();
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
        MatchCollection matches = _searchService.SearchPattern(fileText, SearchPattern);

        if (SelectedOption == "Down")
        {
            currentMatchIndex++;
        }
        else if (SelectedOption == "Up")
        {
            currentMatchIndex--;
        }
        if (currentMatchIndex >= 0 && currentMatchIndex < matches.Count)
        {
            Match currentMatch = matches[currentMatchIndex];
            _dataStorage.ResultSearch(currentMatch.Index, currentMatch.Length);
        }
        else
        {
            _fileDialog.ShowMessage($"Не удалось найти {SearchPattern}", "Error");

            if (SelectedOption == "Down")
                currentMatchIndex--;
            else if (SelectedOption == "Up")
                currentMatchIndex++;
        }
        previousOption = SelectedOption;
    }
    private void ExecuteMinimizeCommand(object? parameter)
    {
        MinimizeRequested?.Invoke();
    }
    private bool CanExecute(object? parameter) => true;

}