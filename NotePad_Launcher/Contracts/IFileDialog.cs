using System.Windows;

namespace NotePad_Launcher.Contracts;

public interface IFileDialog
{
    string OpenTextFileDialog(string pathToClose);
    string SaveFileDialog(string pathToClose);
    void ShowMessage(string message, string title);
    bool ShowConfirmation(string message);
    MessageBoxResult ShowYesNoDialog(string message, string title = "");
}