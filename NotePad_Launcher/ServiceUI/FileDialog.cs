using System.Windows;
using Domain.IService;
using Domain.Model;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using NotePad_Launcher.IServiceUI;
using NotePad_Launcher.ServiceUI;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace NotePad_Launcher;

public class FileDialog : IFileDialog
{
    public string OpenTextFileDialog(string pathToClose, string extension)
    {
        var openFileDialog = new OpenFileDialog();
        if (extension == "txt")
        {
            openFileDialog.Filter = $"{LocalizationService.Instance["ClassFileDialogTextFileFilter"]}(*.txt)|*.txt";
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
    public string SaveFileDialog(string pathToClose, string FileName)
    {
        var saveFileDialog = new SaveFileDialog
        {
            Title = LocalizationService.Instance["ClassFileDialogSaveFileTitle"],
            Filter = $"{LocalizationService.Instance["ClassFileDialogTextFileFilter"]} (*.txt)|*.txt|Все файлы (*.*)|*.*",
            DefaultExt = ".txt",
            FileName = FileName
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            return saveFileDialog.FileName;
        }

        return null;
    }
    public string FolderFileDialog(string initialPath = null, Window owner = null)
    {
        var dialog = new CommonOpenFileDialog
        {
            Title = "Выберите папку",
            IsFolderPicker = true,
            InitialDirectory = initialPath
        };


        if (dialog.ShowDialog(owner) == CommonFileDialogResult.Ok)
        {
            return dialog.FileName;
        }

        return initialPath;
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