using System.Windows;
using System.Windows.Controls;
using Domain.Attributes;
using Domain.IService;
using Domain.IService.ISystemApp;
using Domain.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using NotePad_Launcher.IServiceUI;
using NotePad_Launcher.MVVM.DialogWindows.InputTextDialog;
using NotePad_Launcher.ServiceUI;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace NotePad_Launcher;

[RegisterService(ServiceLifetime.Singleton, serviceType: typeof(IFileDialog))]
public class FileDialog : IFileDialog
{
    private readonly ILocalizationService _localizationService;

    public FileDialog(ILocalizationService someDependency)
    {
        _localizationService = someDependency;
    }
    public string OpenTextFileDialog(string pathToClose, string extension)
    {

        var openFileDialog = new OpenFileDialog();
        if (extension == "txt")
        {
            openFileDialog.Filter = $"{_localizationService["ClassFileDialogTextFileFilter"]}(*.txt)|*.txt";
        }
        if (extension == "pem")
        {
            openFileDialog.Filter = $"PemFile:(*.pem)|*.pem";
        }
        if (extension == "pfx")
        {
            openFileDialog.Filter = $"CertFile:(*.pfx)|*.pfx";
        }
        if (extension == "json")
        {
            openFileDialog.Filter = $"JsonFile:(*.json)|*.json";
        }
        else if (extension == null)
        {
        }

        if (openFileDialog.ShowDialog() == true)
        {
            return openFileDialog.FileName;
        }

        return null;
    }
    public string SaveFileDialog(string pathToClose, string FileName)
    {
        var saveFileDialog = new SaveFileDialog
        {
            Title = _localizationService["ClassFileDialogSaveFileTitle"],
            Filter = $"{_localizationService["ClassFileDialogTextFileFilter"]} (*.txt)|*.txt|Все файлы (*.*)|*.*",
            DefaultExt = ".txt",
            FileName = FileName
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
        var owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);

        if (dialog.ShowDialog(owner) == CommonFileDialogResult.Ok)
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

    public string InputTextDialog(string title, string message, string inputText, bool isTextBox, bool isComboBox)
    {
        var factory = App.ServiceProvider.GetRequiredService<Func<string, string, string, bool, bool, InputTextDialogVM>>();
        var vm = factory(title, message, inputText, isTextBox, isComboBox);

        var dialog = new InputTextDialog { DataContext = vm };
        try
        {
            var result = dialog.ShowDialog();
            if (result == true)
                return vm.Result;
            else
                return null;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
            throw;
        }

    }
}