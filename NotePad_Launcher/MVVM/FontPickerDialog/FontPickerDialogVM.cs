using NotePad_Launcher.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace NotePad_Launcher.MVVM.FontPickerDialog;

public class FontPickerDialogVM : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private ICommand? _closeCommand;
    private ICommand? _confirmCommand;
    public event Action? CloseRequested;
    private readonly IStringService _stringService;
    public ObservableCollection<FontFamily> Fonts { get; }
    public ObservableCollection<string> FontStyles { get; }
    public ObservableCollection<double> FontSizes { get; }

    private FontFamily _selectedFont;
    public FontFamily SelectedFont
    {
        get => _selectedFont;
        set
        {
            if (_selectedFont != value)
            {
                _selectedFont = value;
                OnPropertyChanged();
                LoadFontStyles();
            }
        }
    }

    private string _selectedFontStyle;
    public string SelectedFontStyle
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
    private double _selectedFontSize;
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
    public FontPickerDialogVM(IStringService stringSerivce)
    {
        _stringService = stringSerivce;
        Fonts = new ObservableCollection<FontFamily>(System.Windows.Media.Fonts.SystemFontFamilies.OrderBy(f => f.Source));
        FontStyles = new ObservableCollection<string>();
        FontSizes = new ObservableCollection<double> { 8, 9, 10, 11, 12, 14, 16, 18, 20, 24, 28, 32, 36, 48, 72 };
        var (fontSize, fontFamily) = _stringService.GetFontFamilySizeCallback();

        if (Fonts.Any())
        {
            SelectedFontSize = fontSize;
            SelectedFont = fontFamily;
        }
    }
    private void LoadFontStyles()
    {
        FontStyles.Clear();
        FontStyles.Add("Mot Working");
        FontStyles.Add("Regular");
        FontStyles.Add("Bold");
        FontStyles.Add("Italic");
        FontStyles.Add("Bold Italic");

        SelectedFontStyle = "Not Working";
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
    public ICommand ConfirmCommand => _confirmCommand ??= new OtherRelayCommands(ExecuteConfirmCommand, CanExecute);
    private void ExecuteCloseCommand(object? parameter)
    {
        CloseRequested?.Invoke();
    }
    private void ExecuteConfirmCommand(object? parameter)
    {
        _stringService.PushUpdatedFamilySize(SelectedFontSize, SelectedFont);
        CloseRequested?.Invoke();
    }
    private bool CanExecute(object? parameter) => true;
}