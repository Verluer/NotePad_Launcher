using NotePad_Launcher.MVVM.Commands;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace NotePad_Launcher.MVVM.FunctionalWindows.SearchWindow;

public class SearchWindowVM : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public event Action? MinimizeRequested;
    public event Action? CloseRequested;

    private ICommand? _closeCommand;
    private ICommand? _minimizeCommand;
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

    private void ExecuteCloseCommand(object? parameter)
    {
        CloseRequested?.Invoke();
    }
    private void ExecuteMinimizeCommand(object? parameter)
    {
        MinimizeRequested?.Invoke();
    }
    private bool CanExecute(object? parameter) => true;

}