using NotePad_Launcher.MVVM.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace NotePad_Launcher.MVVM.DialogWindows.InputTextDialog
{
    public class InputTextDialogVM : INotifyPropertyChanged
    {
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

        public event PropertyChangedEventHandler? PropertyChanged;

        private ICommand? _closeCommand;
        private ICommand? _okCommand;
        private ICommand? _cancelCommand;
        public event Action? CloseRequested;
        public event Action? OkRequested;

        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _message;
        public string Message
        {
            get => _message;
            set
            {
                if (_message != value)
                {
                    _message = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _inputText;
        public string InputText
        {
            get => _inputText;
            set
            {
                if (_inputText != value)
                {
                    _inputText = value;
                    OnPropertyChanged();
                }
            }
        }
        public InputTextDialogVM(string title, string message, string inputText)
        {
            Title = title;
            InputText = inputText;
            Message = message;
        }
        public ICommand CloseCommand => _closeCommand ??= new OtherRelayCommands(ExecuteCloseCommand, CanExecute);
        public ICommand OkCommand => _okCommand ??= new OtherRelayCommands(ExecuteOkCommand, CanExecute);
        public ICommand CancelCommand => _cancelCommand ??= new OtherRelayCommands(ExecuteCloseCommand, CanExecute);

        private void ExecuteCloseCommand(object? parameter)
        {
            CloseRequested?.Invoke();
        }
        private void ExecuteOkCommand(object? parameter)
        {
            OkRequested?.Invoke();
        }
        private bool CanExecute(object? parameter) => true;
    }
}
