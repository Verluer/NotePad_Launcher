using Domain.IService;
using Microsoft.Extensions.DependencyInjection;
using NotePad_Launcher.MVVM.Commands;
using Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private readonly IFileService _fileService;
        public event PropertyChangedEventHandler? PropertyChanged;

        private ICommand? _closeCommand;
        private ICommand? _okCommand;
        private ICommand? _cancelCommand;
        public event Action? CloseRequested;
        public event Action? OkRequested;
        public ObservableCollection<string> FolderFileString { get; set; } = new ObservableCollection<string>();

        private string _selectedFolder;
        public string SelectedFolder
        {
            get => _selectedFolder;
            set
            {
                if (SetField(ref _selectedFolder, value))
                {
                    if (IsComboBoxVisible = true) Result = value;
                }
            }
        }
        private int _selectedIndex;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set => SetField(ref _selectedIndex, value);
        }
        private string _title;
        public string Title
        {
            get => _title;
            set => SetField(ref _title, value);
        }
        private string _message;
        public string Message
        {
            get => _message;
            set => SetField(ref _message, value);
        }
        private string _inputText;
        public string InputText
        {
            get => _inputText;
            set
            {
                if (SetField(ref _inputText, value))
                {
                    if(IsTextBoxVisible = true) Result = value;
                }
            }
        }
        private bool _isTextBoxVisible = true;

        public bool IsTextBoxVisible
        {
            get => _isTextBoxVisible;
            set => SetField(ref _isTextBoxVisible, value);
        }
        private bool _isComboBoxVisible = true;

        public bool IsComboBoxVisible
        {
            get => _isComboBoxVisible;
            set => SetField(ref _isComboBoxVisible, value);
        }
        public string Result { get; set; } = "";
        public InputTextDialogVM(string title, string message, string inputText, bool isTextBox, bool isComboBox)
        {
            _fileService = App.ServiceProvider.GetRequiredService<IFileService>();
            Title = title;
            InputText = inputText;
            Message = message;
            IsTextBoxVisible = isTextBox;
            IsComboBoxVisible = isComboBox;
            if (IsComboBoxVisible == true)
            {
                UploadFolder();
            }
            
        }
        public void UploadFolder()
        {
            if (FolderFileString != null) FolderFileString.Clear();
            List<string> folderNames = _fileService.LoadFolderFile(App.Config.DocsPath);
            foreach (var folderName in folderNames)
            {
                FolderFileString.Add(folderName);
            }
            if (FolderFileString.Any()) SelectedIndex = 0;

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
