using System.Windows;

namespace NotePad_Launcher.Contracts;

public interface IFileDialog
{
    string OpenTextFileDialog(string pathToClose);
    void ShowMessage(string message);
    bool ShowConfirmation(string message);
    MessageBoxResult ShowYesNoDialog(string message, string title = "");
}