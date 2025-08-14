using NotePad_Launcher.ViewModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using NotePad_Launcher.MVVM.Commands;
using NotePad_Launcher.ServiceUI;
using Domain.Model;
using Domain.IService;
using Domain.Attributes;
using Microsoft.Extensions.DependencyInjection;
using NotePad_Launcher.IServiceUI;

namespace NotePad_Launcher.MVVM.InformationWindows.ProgramInfDialog;

[RegisterService(ServiceLifetime.Transient, asSelf: true)]
public class ProgramInfDialogVM : INotifyPropertyChanged
{
    private ICommand? _closeCommand;

    private readonly ILocalizationService _localizationService;

    public event Action? CloseRequested;

    public event PropertyChangedEventHandler? PropertyChanged;

    private string _title;
    public string Title
    {
        get => _title;
        set => SetField(ref _title, value);
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

    public ProgramInfDialogVM(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
        Title = _localizationService["ProgramInfTitle"];
    }
    public ICommand CloseCommand => _closeCommand ??= new OtherRelayCommands(ExecuteCloseCommand, CanExecute);
    private void ExecuteCloseCommand(object? parameter)
    {
        CloseRequested?.Invoke();
    }
    private bool CanExecute(object? parameter) => true;
}