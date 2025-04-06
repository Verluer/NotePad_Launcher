
using NotePad_Launcher.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace NotePad_Launcher.MVVM.FontPickerDialog;

public class FontPickerDialogVM : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private ICommand? _closeCommand;
    private ICommand? _confirmCommand;
    public event Action? CloseRequested;
    private readonly IStringService _stringService;
    public ObservableCollection<FontFamily> Fonts { get; }
    public ObservableCollection<TextBlock> FontStyles { get; }
    public ObservableCollection<TextBlock> FontWeights { get; }
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
                LoadFontStylesWeights();
            }
        }
    }

    private TextBlock _selectedFontStyle;
    public TextBlock SelectedFontStyle
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

    private TextBlock _selectedFontWeight;
    public TextBlock SelectedFontWeight
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
        FontStyles = new ObservableCollection<TextBlock>();
        FontWeights = new ObservableCollection<TextBlock>();
        FontSizes = new ObservableCollection<double> { 8, 9, 10, 11, 12, 14, 16, 18, 20, 24, 28, 32, 36, 48, 72 };
        var (fontSize, fontFamily, fontStyle, fontWeight) = _stringService.GetFontFamilySizeCallback();
        if (Fonts.Any())
        {
            SelectedFontSize = fontSize;
            SelectedFont = fontFamily;
            TextBlock textBlockStyle = new TextBlock
            {
                Text = fontStyle.ToString(),
                FontStyle = fontStyle
            };
            FontStyles.Add(textBlockStyle);
            SelectedFontStyle = textBlockStyle;
            TextBlock textBlockWeight = new TextBlock
            {
                Text = fontWeight.ToString(),
                FontWeight = fontWeight
            };
            FontWeights.Add(textBlockWeight);
            SelectedFontWeight = textBlockWeight;
        }
    }
    private void LoadFontStylesWeights()
    {
        FontStyles.Clear();
        FontWeights.Clear();
        var styleSet = new HashSet<FontStyle>();
        var weightSet = new HashSet<FontWeight>();

        foreach (var typeface in SelectedFont.GetTypefaces())
        {

            var style = typeface.Style;
            var weight = typeface.Weight;
            if (styleSet.Add(style))
            {
                TextBlock textBlock = new TextBlock
                {
                    Text = style.ToString(),
                    FontStyle = style
                };
                FontStyles.Add(textBlock);
            }
            if (weightSet.Add(weight))
            {
                TextBlock textBlock = new TextBlock
                {
                    Text = weight.ToString(),
                    FontWeight = weight
                };
                FontWeights.Add(textBlock);
            }

        }
        if (FontStyles.Count > 0)
        {
            SelectedFontStyle = FontStyles[0];  
        }
        if (FontWeights.Count > 0)
        {
            SelectedFontWeight = FontWeights[0]; 
        }
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
        _stringService.PushUpdatedFamilySize(SelectedFontSize, SelectedFont, SelectedFontStyle.FontStyle, SelectedFontWeight.FontWeight);
        CloseRequested?.Invoke();
    }
    private bool CanExecute(object? parameter) => true;
}