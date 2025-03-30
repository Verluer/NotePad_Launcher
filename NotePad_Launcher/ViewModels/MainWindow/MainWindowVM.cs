using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Domain.Enum;
using NotePad_Launcher.Contracts;
using NotePad_Launcher.ViewModels;


namespace NotePad_Launcher.ViewModels.MainWindow;

public class MainWindowVM : INotifyPropertyChanged
{
    private ICommand? _closeCommand;
    private ICommand? _minimizeCommand;
    private ICommand? _maximizeCommand;
    private ICommand? _rsaCommand;
    private ICommand? _elgamalCommand;
    private ICommand? _rabinaCommand;
    private ICommand? _eccCommand;

    private string _fileText;
    public string FileText
    {
        get => _fileText;
        set
        {
            _fileText = value;
            OnPropertyChanged();
        }
    }
    public IWindowService WindowService { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    private void OpenEncryptionWindow(EncryptionMethod method)
    {
        var encryptionWindow = new EncryptionWindow(method);
        encryptionWindow.EncryptionResultAction = (newText) =>
        {
            FileText = newText; // Обновляем FileText в ViewModel
        };
        encryptionWindow.Show();
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
    public ICommand MaximizeCommand => _maximizeCommand ??= new OtherRelayCommands(ExecuteMaximizeCommand, CanExecute);
    public ICommand RSACommand => _rsaCommand ??= new OtherRelayCommands(ExecuteRSACommand, CanExecute);
    public ICommand ElgamalCommand => _elgamalCommand ??= new OtherRelayCommands(ExecuteElgamalCommand, CanExecute);
    public ICommand RabinaCommand => _rabinaCommand ??= new OtherRelayCommands(ExecuteRabinaCommand, CanExecute);
    public ICommand ECCCommand => _eccCommand ??= new OtherRelayCommands(ExecuteECCCommand, CanExecute);

    // Действие (событие) кнопок
    private void ExecuteCloseCommand(object? parameter)
    {
        Application.Current.Shutdown();
    }
    private void ExecuteMinimizeCommand(object? parameter)
    {
        WindowService?.Minimize();
    }

    private void ExecuteMaximizeCommand(object? parameter)
    {
        WindowService?.Maximize();
    }
    private void ExecuteRSACommand(object? parameter)
    {
        OpenEncryptionWindow(EncryptionMethod.RSA);
    }

    private void ExecuteElgamalCommand(object? parameter)
    {
        OpenEncryptionWindow(EncryptionMethod.Elgamal);
    }

    private void ExecuteRabinaCommand(object? parameter)
    {
        OpenEncryptionWindow(EncryptionMethod.Rabina);
    }

    private void ExecuteECCCommand(object? parameter)
    {
        OpenEncryptionWindow(EncryptionMethod.ECC);
    }

    // Свойство
    private bool CanExecute(object? parameter) => true;
}