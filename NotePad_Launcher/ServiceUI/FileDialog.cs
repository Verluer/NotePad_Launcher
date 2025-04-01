using System.Windows;
using Microsoft.Win32;
using NotePad_Launcher.Contracts;

namespace NotePad_Launcher.Services;

public class FileDialog : IFileDialog
{
    public string OpenTextFileDialog(string pathToClose)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Текстовые файлы (*.txt)|*.txt"
        };

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
            Title = "Сохранить файл как",
            Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*",
            DefaultExt = ".txt",
            FileName = "Новый файл"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            return saveFileDialog.FileName;
        }

        return pathToClose;
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