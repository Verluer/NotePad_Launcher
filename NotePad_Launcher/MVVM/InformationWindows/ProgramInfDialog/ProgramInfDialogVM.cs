using NotePad_Launcher.ViewModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using NotePad_Launcher.MVVM.Commands;
using NotePad_Launcher.ServiceUI;
using Domain.Model;
using Domain.IService;

namespace NotePad_Launcher.MVVM.InformationWindows.ProgramInfDialog;

public class ProgramInfDialogVM : INotifyPropertyChanged
{
    private ICommand? _closeCommand;
    public event Action? CloseRequested;

    public event PropertyChangedEventHandler? PropertyChanged;

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
    private readonly IConfigService _configService;
    private AppConfigModel _config;
    public AppConfigModel Config
    {
        get => _config;
        set
        {
            if (_config != value)
            {
                _config = value;
                OnPropertyChanged(nameof(Config));
            }
        }
    }

    public ProgramInfDialogVM(IConfigService configService)
    {
        _configService = configService;
        Config = _configService.Load();
        LocalizationService.Instance.LoadLanguage(Config.Language);
    }
    public ICommand CloseCommand => _closeCommand ??= new OtherRelayCommands(ExecuteCloseCommand, CanExecute);
    private void ExecuteCloseCommand(object? parameter)
    {
        CloseRequested?.Invoke();
    }
    private bool CanExecute(object? parameter) => true;
}