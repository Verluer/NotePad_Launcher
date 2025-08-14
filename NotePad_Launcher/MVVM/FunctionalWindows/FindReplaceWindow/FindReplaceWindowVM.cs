using Domain.Attributes;
using Domain.Enum;
using Domain.IService.ISystemApp;
using Domain.IService.ITextUtils;
using Microsoft.Extensions.DependencyInjection;
using NotePad_Launcher.MVVM.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using NotePad_Launcher.IServiceUI;

namespace NotePad_Launcher.MVVM.FunctionalWindows.FindReplaceWindow
{

    [RegisterService(ServiceLifetime.Transient, asSelf: true)]
    public class FindReplaceWindowVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action? MinimizeRequested;
        public event Action? CloseRequested;
        private readonly ISearchService _searchService;
        private readonly IDataStorage _dataStorage;
        private readonly IServiceFunctions _serviceFunctions;
        private readonly IFileDialog _fileDialog;
        private readonly IConfigService _configService;
        private readonly ILocalizationService _localizationService;
        private ICommand? _closeCommand;
        private ICommand? _minimizeCommand;
        private ICommand? _searchCommand;
        private ICommand? _replaceCommand;
        private ICommand? _replaceAllCommand;
        private static int currentMatchIndex = -1;
        private string previousOption = "None";
        private string _searchPattern;
        Match ReplaceMatch = null;
        private int lastMatchOffset = -1;
        private int previousCaretOffset = -1;
        private string previousSearchPattern = string.Empty;
        public string SearchPattern
        {
            get => _searchPattern;
            set => SetField(ref _searchPattern, value);
        }
        private string _titleMethod;
        public string TitleMethod
        {
            get => _titleMethod;
            set => SetField(ref _titleMethod, value);
        }
        private string _replacePattern;
        public string ReplacePattern
        {
            get => _replacePattern;
            set => SetField(ref _replacePattern, value);
        }
        private string _selectedOption = "Down";

        public string SelectedOption
        {
            get { return _selectedOption; }
            set => SetField(ref _selectedOption, value);
        }
        private bool _isRegisterAware = false;

        public bool IsRegisterAware
        {
            get => _isRegisterAware;
            set => SetField(ref _isRegisterAware, value);
        }
        private bool _isTextFairing = false;

        public bool IsTextFairing
        {
            get => _isTextFairing;
            set => SetField(ref _isTextFairing, value);
        }
        private Thickness _buttonCloseMargin = new Thickness(0);
        public Thickness ButtonCloseMargin
        {
            get => _buttonCloseMargin;
            set => SetField(ref _buttonCloseMargin, value);
        }
        private bool _isButtonReplaceVisible = true;

        public bool IsButtonReplaceVisible
        {
            get => _isButtonReplaceVisible;
            set => SetField(ref _isButtonReplaceAllVisible, value);
        }
        private bool _isButtonReplaceAllVisible = true;

        public bool IsButtonReplaceAllVisible
        {
            get => _isButtonReplaceAllVisible;
            set => SetField(ref _isButtonReplaceAllVisible, value);
        }
        private bool _isTextBlockReplaceVisible = true;

        public bool IsTextBlockReplaceVisible
        {
            get => _isTextBlockReplaceVisible;
            set => SetField(ref _isTextBlockReplaceVisible, value);
        }
        private bool _isTextBoxReplaceVisible = true;

        public bool IsTextBoxReplaceVisible
        {
            get => _isTextBoxReplaceVisible;
            set => SetField(ref _isTextBoxReplaceVisible, value);
        }
        private bool _isRadioButtonUpVisible = true;

        public bool IsRadioButtonUpVisible
        {
            get => _isRadioButtonUpVisible;
            set => SetField(ref _isRadioButtonUpVisible, value);
        }
        private bool _isRadioButtonDownVisible = true;

        public bool IsRadioButtonDownVisible
        {
            get => _isRadioButtonDownVisible;
            set => SetField(ref _isRadioButtonDownVisible, value);
        }
        private FindReplaceMethod _selectedMethod;
        public FindReplaceMethod SelectedMethod { get; }
        public FindReplaceWindowVM(IDataStorage dataStorage, IFileDialog fileDialog, ISearchService searchService, ILocalizationService localizationService)
        {
            _searchService = searchService;
            _dataStorage = dataStorage;
            SelectedMethod = _dataStorage.searchReplaceMethod;
            _fileDialog = fileDialog;
            _localizationService = localizationService;
            switch (SelectedMethod)
            {
                case FindReplaceMethod.Find:
                    SerachUI();
                    break;
                case FindReplaceMethod.Replace:
                    ReplaceUI();
                    break;

            }
        }
        private void SerachUI()
        {
            IsButtonReplaceVisible = false;
            IsButtonReplaceAllVisible = false;
            ButtonCloseMargin = new Thickness(0, 10, 0, 0);
            IsTextBlockReplaceVisible = false;
            IsTextBoxReplaceVisible = false;
            TitleMethod = _localizationService["FindTitle"];
        }
        private void ReplaceUI()
        {
            IsRadioButtonDownVisible = false;
            IsRadioButtonUpVisible = false;
            ButtonCloseMargin = new Thickness(0, 5, 0, 0);
            TitleMethod = _localizationService["ReplaceTitle"];
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
        public ICommand MinimizeCommand => _minimizeCommand ??= new OtherRelayCommands(ExecuteMinimizeCommand, CanExecute);
        public ICommand SearchCommand => _searchCommand ??= new OtherRelayCommands(ExecuteSearchCommand, CanExecute);
        public ICommand ReplaceCommand => _replaceCommand ??= new OtherRelayCommands(ExecuteReplaceCommand, CanExecute);
        public ICommand ReplaceAllCommand => _replaceAllCommand ??= new OtherRelayCommands(ExecuteReplaceAllCommand, CanExecute);
        private void ExecuteCloseCommand(object? parameter)
        {
            CloseRequested?.Invoke();
        }
        private void ExecuteMinimizeCommand(object? parameter)
        {
            MinimizeRequested?.Invoke();
        }
        private Match Search()
        {
            var fileText = _dataStorage.GetTextCallback();
            MatchCollection matches = _searchService.SearchPattern(fileText, SearchPattern, IsRegisterAware);

            if (matches.Count == 0)
            {
                _fileDialog.ShowMessage($"Could not find {SearchPattern}", "Error");
                return null;
            }

            int currentCaretOffset = _dataStorage.GetCaretOffset();

            // Сброс при смене паттерна — только в начало (0)
            // Сброс при ручном перемещении курсора — с позиции курсора
            if (SearchPattern != previousSearchPattern)
            {
                lastMatchOffset = 0;
            }
            else if (currentCaretOffset != previousCaretOffset)
            {
                lastMatchOffset = currentCaretOffset;
            }

            previousSearchPattern = SearchPattern;
            previousCaretOffset = currentCaretOffset;

            int index = -1;

            if (SelectedOption == "Down")
            {
                for (int i = 0; i < matches.Count; i++)
                {
                    if (matches[i].Index >= lastMatchOffset)
                    {
                        index = i;
                        break;
                    }
                }

                if (index == -1)
                {
                    if (IsTextFairing)
                    {
                        index = 0;
                    }
                    else
                    {
                        _fileDialog.ShowMessage($"Reached the end of the document", "Inf");
                        return null;
                    }
                }
            }
            else if (SelectedOption == "Up")
            {
                for (int i = matches.Count - 1; i >= 0; i--)
                {
                    if (matches[i].Index < lastMatchOffset)
                    {
                        index = i;
                        break;
                    }
                }

                if (index == -1)
                {
                    if (IsTextFairing)
                    {
                        index = matches.Count - 1;
                    }
                    else
                    {
                        _fileDialog.ShowMessage($"The beginning of the document has been reached.", "Inf");
                        return null;
                    }
                }
            }

            Match currentMatch = matches[index];

            // Обновляем offset после найденного слова
            if (SelectedOption == "Down")
            {
                lastMatchOffset = currentMatch.Index + currentMatch.Length;
            }
            else if (SelectedOption == "Up")
            {
                lastMatchOffset = currentMatch.Index;
            }

            _dataStorage.ResultSearch(currentMatch.Index, currentMatch.Length);
            previousOption = SelectedOption;

            return currentMatch;
        }




        private void ExecuteSearchCommand(object? parameter)
        {
            ReplaceMatch = Search();
            if (SelectedOption == "Up") ReplaceMatch = Search();
        }
        private void ExecuteReplaceCommand(object? parameter)
        {
            var (areaIndex, areaLength) = _dataStorage.GetSelectionCallback();
            var fileText = _dataStorage.GetTextCallback();
            string areaText = fileText.Substring(areaIndex, areaLength);
            if (!areaText.Equals(SearchPattern, StringComparison.OrdinalIgnoreCase))
            {
                ReplaceMatch = Search();
            }
            else
            {
                string resultReplace = _searchService.ReplaceText(fileText, ReplaceMatch.Index, ReplaceMatch.Length, ReplacePattern);
                _dataStorage.PushUpdatedText(resultReplace);
            }
        }
        private void ExecuteReplaceAllCommand(object? parameter)
        {
            var fileText = _dataStorage.GetTextCallback();
            string resultReplaceAll = _searchService.ReplaceAllText(fileText, SearchPattern, ReplacePattern, IsRegisterAware);
            _dataStorage.PushUpdatedText(resultReplaceAll);
        }
        private bool CanExecute(object? parameter) => true;

    }
}
