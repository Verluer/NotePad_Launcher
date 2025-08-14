using Domain.Attributes;
using Domain.Enum;
using Domain.IService.IFileSystem;
using Domain.IService.ISystemApp;
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
    [RegisterService(ServiceLifetime.Transient, asSelf: true)]
    public class SettingsWindowVM : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly IDataStorage _dataStorage;
        private readonly IConfigService _configService;
        private readonly IFileDialog _fileDialog;
        private readonly IFileSystemManager _fileSystemManager;
        private readonly ILocalizationService _localizationService;
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
        private string _title;
        public string Title
        {
            get => _title;
            set => SetField(ref _title, value);
        }
        private string _textBoxDocumentDirect;
        public string TextBoxDocumentDirect
        {
            get => _textBoxDocumentDirect;
            set => SetField(ref _textBoxDocumentDirect, value);
        }
        private bool _isStackPanelGeneralVisible = true;

        public bool IsStackPanelGeneralVisible
        {
            get => _isStackPanelGeneralVisible;
            set => SetField(ref _isStackPanelGeneralVisible, value);
        }
        private string _selectedSettings;
        public string SelectedSettings
        {
            get => _selectedSettings;
            set
            {
                if (SetField(ref _selectedSettings, value))
                {
                    if (value == "General") 
                    {
                        if (IsStackPanelGeneralVisible == false)
                        {
                            IsStackPanelGeneralVisible = true;
                        }
;                    }
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
                if (SetField(ref _selectedSaveConf, value))
                {
                    if (value == "SaveDirectory") ;
                    if (value == "SaveNormal") ;
                }
            }
        }
        private string _selectedLanguage;
        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set => SetField(ref _selectedLanguage, value);
        }
        private string _selectedAllHighlightings;
        public string SelectedAllHighlightings
        {
            get => _selectedAllHighlightings;
            set => SetField(ref _selectedAllHighlightings, value);
        }
        public List<string> Language { get; } = new List<string>();
        public List<string> AllHighlightings { get; } = new List<string>();
        public SettingsWindowVM(IDataStorage dataStorage, IFileDialog fileDialog, IConfigService configService, IFileSystemManager fileSystemManager, 
            ILocalizationService localization)
        {
            _configService = configService;
            _dataStorage = dataStorage;
            _fileDialog = fileDialog;
            _fileSystemManager = fileSystemManager;
            _localizationService = localization;

            Title = _localizationService["SettingsTitle"];
            LoadSettingsGeneral();
        }
        private void LoadSettingsGeneral()
        {
            SelectedSettings = "General";

            Language.Clear();
            Language.AddRange(_fileSystemManager.FindLocalization());

            AllHighlightings.Clear();
            AllHighlightings.AddRange(_dataStorage.AllHighlightings);

            TextBoxDocumentDirect = App.Config.DocsPath;
            SelectedSaveConf = App.Config.SaveSetting;
            SelectedLanguage = App.Config.Language;
            SelectedAllHighlightings = App.Config.SyntaxHighlighting;
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
            App.Config.SyntaxHighlighting = SelectedAllHighlightings;
            _dataStorage.PushUpdatedSyntax();
            _configService.Save(App.Config);
            _localizationService.LoadLanguage(SelectedLanguage);
            CloseRequested?.Invoke();
        }
        private void ExecuteOpenFolderCommand(object? parameter)
        {
            var owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
            TextBoxDocumentDirect = _fileDialog.FolderFileDialog(App.Config.DocsPath);
        }
        private bool CanExecute(object? parameter) => true;

    }
}
