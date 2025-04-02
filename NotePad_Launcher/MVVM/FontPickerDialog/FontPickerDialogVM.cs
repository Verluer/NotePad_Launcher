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
    public event Action? CloseRequested;
    public ObservableCollection<FontFamily> Fonts { get; }
    public ObservableCollection<string> FontStyles { get; }
    public ObservableCollection<int> FontSizes { get; }

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
    private int _selectedFontSize;
    public int SelectedFontSize
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
    public FontPickerDialogVM()
    {
        Fonts = new ObservableCollection<FontFamily>(System.Windows.Media.Fonts.SystemFontFamilies.OrderBy(f => f.Source));
        FontStyles = new ObservableCollection<string>();
        FontSizes = new ObservableCollection<int> { 8, 9, 10, 11, 12, 14, 16, 18, 20, 24, 28, 32, 36, 48, 72 };

        if (Fonts.Any())
        {
            SelectedFont = Fonts.First();
            SelectedFontSize = FontSizes[4];
        }
    }
    private void LoadFontStyles()
    {
        FontStyles.Clear();
        FontStyles.Add("Regular");
        FontStyles.Add("Bold");
        FontStyles.Add("Italic");
        FontStyles.Add("Bold Italic");

        SelectedFontStyle = "Regular";
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
    private void ExecuteCloseCommand(object? parameter)
    {
        CloseRequested?.Invoke();
    }
    private bool CanExecute(object? parameter) => true;
}