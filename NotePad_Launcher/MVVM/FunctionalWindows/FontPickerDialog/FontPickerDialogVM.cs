using NotePad_Launcher.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using NotePad_Launcher.MVVM.Commands;
using Domain.Attributes;
using Microsoft.Extensions.DependencyInjection;
using NotePad_Launcher.IServiceUI;

namespace NotePad_Launcher.MVVM.FunctionalWindows.FontPickerDialog;

[RegisterService(ServiceLifetime.Transient, asSelf: true)]
public class FontPickerDialogVM : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private ICommand? _closeCommand;
    private ICommand? _confirmCommand;
    public event Action? CloseRequested;
    private readonly IDataStorage _dataStorage;
    private readonly ILocalizationService _localizationService;
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
            if (SetField(ref _selectedFont, value))
            {
                LoadFontStylesWeights();
            }
        }
    }

    private TextBlock _selectedFontStyle;
    public TextBlock SelectedFontStyle
    {
        get => _selectedFontStyle;
        set => SetField(ref _selectedFontStyle, value);
    }

    private TextBlock _selectedFontWeight;
    public TextBlock SelectedFontWeight
    {
        get => _selectedFontWeight;
        set => SetField(ref _selectedFontWeight, value);
    }
    private string _title;
    public string Title
    {
        get => _title;
        set => SetField(ref _title, value);
    }
  
    private double _selectedFontSize;
    public double SelectedFontSize
    {
        get => _selectedFontSize;
        set => SetField(ref _selectedFontSize, value);
    }

    public FontPickerDialogVM(IDataStorage dataStorage, ILocalizationService localizationService)
    {
        _dataStorage = dataStorage;
        _localizationService = localizationService;

        Fonts = new ObservableCollection<FontFamily>(System.Windows.Media.Fonts.SystemFontFamilies.OrderBy(f => f.Source));
        FontStyles = new ObservableCollection<TextBlock>();
        FontWeights = new ObservableCollection<TextBlock>();
        FontSizes = new ObservableCollection<double> { 8, 9, 10, 11, 12, 14, 16, 18, 20, 24, 28, 32, 36, 48, 72 };

        var (fontSize, fontFamily, fontStyle, fontWeight) = _dataStorage.GetFontFamilySizeCallback();

        Title = _localizationService["FontPickerTitle"];

        if (Fonts.Any())
        {
            SelectedFontSize = fontSize;
            SelectedFont = fontFamily;
            for (int i = 0; i < FontStyles.Count; i++)
            {
                if (FontStyles[i].FontStyle == fontStyle)
                {
                    SelectedFontStyle = FontStyles[i];
                }
            }
            for (int i = 0; i < FontWeights.Count; i++)
            {
                if (FontWeights[i].FontWeight == fontWeight)
                {
                    SelectedFontWeight = FontWeights[i];
                }
            }
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
        _dataStorage.PushUpdatedFamilySize(SelectedFontSize, SelectedFont, SelectedFontStyle.FontStyle, SelectedFontWeight.FontWeight);
        CloseRequested?.Invoke();
    }
    private bool CanExecute(object? parameter) => true;
}