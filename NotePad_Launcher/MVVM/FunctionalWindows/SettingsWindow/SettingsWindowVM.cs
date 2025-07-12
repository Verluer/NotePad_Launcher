using Domain.Enum;
using Domain.IService;
using Domain.Model;
using Microsoft.Extensions.DependencyInjection;
using NotePad_Launcher.IServiceUI;
using NotePad_Launcher.MVVM.Commands;
using NotePad_Launcher.ServiceUI;
using Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NotePad_Launcher.MVVM.FunctionalWindows.SettingsWindow
{
    public class SettingsWindowVM : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly IDataStorage _dataStorage;
        private readonly IConfigService _configService;
        private readonly IFileDialog _fileDialog;
        public event Action? MinimizeRequested;
        public event Action? CloseRequested;
        private ICommand? _closeCommand;
        private ICommand? _minimizeCommand;
        private ICommand? _saveCommand;
        private ICommand? _openFolderCommand;

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
        private bool _isStackPanelGeneralVisible = true;

        public bool IsStackPanelGeneralVisible
        {
            get => _isStackPanelGeneralVisible;
            set
            {
                if (_isStackPanelGeneralVisible != value)
                {
                    _isStackPanelGeneralVisible = value;
                    OnPropertyChanged(nameof(IsStackPanelGeneralVisible));
                }
            }
        }
        private string _selectedSettings;
        public string SelectedSettings
        {
            get => _selectedSettings;
            set
            {
                if (_selectedSettings != value)
                {
                    _selectedSettings = value;
                    OnPropertyChanged(nameof(SelectedSettings));
                    if (value == "General") IsStackPanelGeneralVisible = true;
                    if (value == "Test") IsStackPanelGeneralVisible = false;
                }
            }
        }
        private string _selectedSaveConf;
        public string SelectedSaveConf
        {
            get => _selectedSaveConf;
            set
            {
                if (_selectedSaveConf != value)
                {
                    _selectedSaveConf = value;
                    OnPropertyChanged(nameof(SelectedSaveConf));
                    if (value == "SaveDirectory") ;
                    if (value == "SaveNormal") ;
                }
            }
        }
        private string _selectedLanguage;
        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                _selectedLanguage = value;
                OnPropertyChanged(nameof(SelectedLanguage));
            }
        }

        public List<string> Language { get; } = new List<string>
    {
        "en",
        "ua",
        "ru"
    };
        public SettingsWindowVM(IDataStorage dataStorage, IFileDialog fileDialog, IConfigService configService)
        {
            _configService = configService;
            _dataStorage = dataStorage;
            _fileDialog = fileDialog;
            TextBoxDocumentDirect = App.Config.DocsPath;
            SelectedSaveConf = App.Config.SaveSetting;
            SelectedLanguage = App.Config.Language;

        }
        public ICommand CloseCommand => _closeCommand ??= new OtherRelayCommands(ExecuteCloseCommand, CanExecute);
        public ICommand MinimizeCommand => _minimizeCommand ??= new OtherRelayCommands(ExecuteMinimizeCommand, CanExecute);
        public ICommand SaveCommand => _saveCommand ??= new OtherRelayCommands(ExecuteSaveCommand, CanExecute);
        public ICommand OpenFolderCommand => _openFolderCommand ??= new OtherRelayCommands(ExecuteOpenFolderCommand, CanExecute);
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
            App.Config.DocsPath = TextBoxDocumentDirect;
            App.Config.SaveSetting = SelectedSaveConf;
            App.Config.Language = SelectedLanguage;
            _configService.Save(App.Config);
            LocalizationService.Instance.LoadLanguage(SelectedLanguage);
            CloseRequested?.Invoke();
        }
        private void ExecuteOpenFolderCommand(object? parameter)
        {
            var owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
            TextBoxDocumentDirect = _fileDialog.FolderFileDialog(App.Config.DocsPath, owner);
        }
        private bool CanExecute(object? parameter) => true;

    }
}
