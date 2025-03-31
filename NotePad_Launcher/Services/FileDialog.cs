using System.Windows;
using Microsoft.Win32;
using NotePad_Launcher.Contracts;

namespace NotePad_Launcher.Services;

public class FileDialog : IFileDialog
{
    public string OpenTextFileDialog(string pathToClose)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Filter = "Текстовые файлы (*.txt)|*.txt"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            return openFileDialog.FileName;
        }

        return pathToClose;
    }

    public void ShowMessage(string message)
    {
        MessageBox.Show(message, "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
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