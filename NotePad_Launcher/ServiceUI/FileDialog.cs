using System.Windows;
using Domain.IService;
using Domain.Model;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using NotePad_Launcher.IServiceUI;

namespace NotePad_Launcher;

public class FileDialog : IFileDialog
{
    private readonly ILocalizationService _localizationService;
    private readonly IConfigService _configService;
    private AppConfigModel Config;

    public FileDialog(ILocalizationService localizationService, IConfigService configService)
    {
        _configService = configService;
        Config = _configService.Load();
        _localizationService = localizationService;
        _localizationService.LoadLanguage(Config.Language);
    }
    public string OpenTextFileDialog(string pathToClose, string extension)
    {
        var openFileDialog = new OpenFileDialog();
        if (extension == "txt")
        {
            openFileDialog.Filter = $"{_localizationService["ClassFileDialogTextFileFilter"]}(*.txt)|*.txt";
        }
        else if (extension == null)
        {
        }

        if (openFileDialog.ShowDialog() == true)
        {
            return openFileDialog.FileName;
        }

        return pathToClose;
    }
    public string SaveFileDialog(string pathToClose)
    {
        var saveFileDialog = new SaveFileDialog
        {
            Title = _localizationService["ClassFileDialogSaveFileTitle"],
            Filter = $"{_localizationService["ClassFileDialogTextFileFilter"]} (*.txt)|*.txt|Все файлы (*.*)|*.*",
            DefaultExt = ".txt",
            FileName = _localizationService["ClassFileDialogSaveFileName"]
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            return saveFileDialog.FileName;
        }

        return null;
    }
    public string FolderFileDialog(string initialPath = null)
    {
        var dialog = new CommonOpenFileDialog
        {
            Title = "Выберите папку",
            IsFolderPicker = true,
            InitialDirectory = initialPath
        };

        if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
        {
            return dialog.FileName;
        }

        return null;
    }
    public void ShowMessage(string message, string title)
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public bool ShowConfirmation(string message)
    {
        return MessageBox.Show(message, "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
    }

    public MessageBoxResult ShowYesNoDialog(string message, string title = "")
    {
        return MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Warning);
    }
}