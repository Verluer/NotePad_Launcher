using Domain.Enum;
using Domain.IService;
using Domain.Model;
using Microsoft.Extensions.DependencyInjection;
using NotePad_Launcher.MVVM.Commands;
using Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NotePad_Launcher.MVVM.FunctionalWindows.SettingsWindow
{
    public class SettingsWindowVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly IDataStorage _dataStorage;
        private readonly IConfigService _configService;
        public event Action? MinimizeRequested;
        public event Action? CloseRequested;
        private ICommand? _closeCommand;
        private ICommand? _minimizeCommand;
        private ICommand? _saveCommand;
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
        private string _textBoxDocumentDirect;
        public string TextBoxDocumentDirect
        {
            get => _textBoxDocumentDirect;
            set
            {
                if (_textBoxDocumentDirect != value)
                {
                    _textBoxDocumentDirect = value;
                    OnPropertyChanged();
                }
            }
        }
        public SettingsWindowVM(IDataStorage dataStorage, IConfigService configService)
        {
            _dataStorage = dataStorage;
            _configService = configService;
            Config = _configService.Load();
            TextBoxDocumentDirect = Config.DocsPath;

        }
        public ICommand CloseCommand => _closeCommand ??= new OtherRelayCommands(ExecuteCloseCommand, CanExecute);
        public ICommand MinimizeCommand => _minimizeCommand ??= new OtherRelayCommands(ExecuteMinimizeCommand, CanExecute);
        public ICommand SaveCommand => _saveCommand ??= new OtherRelayCommands(ExecuteSaveCommand, CanExecute);
        private void ExecuteCloseCommand(object? parameter)
        {
            CloseRequested?.Invoke();
        }
        private void ExecuteMinimizeCommand(object? parameter)
        {
            MinimizeRequested?.Invoke();
        }
        private void ExecuteSaveCommand(object? parameter)
        {
            Config.DocsPath = TextBoxDocumentDirect;
            _configService.Save(Config);
            CloseRequested?.Invoke();
        }
        private bool CanExecute(object? parameter) => true;

    }
}
