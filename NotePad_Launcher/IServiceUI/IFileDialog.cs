using Domain.Model;
using System.Windows;

namespace NotePad_Launcher;

public interface IFileDialog
{
    string OpenTextFileDialog(string pathToClose, string extension);
    string SaveFileDialog(string pathToClose, string FileName);
    string FolderFileDialog(string pathToClose, Window owner);
    void ShowMessage(string message, string title);
    bool ShowConfirmation(string message);
    MessageBoxResult ShowYesNoDialog(string message, string title = "");
    public string InputTextDialog(string title, string message, string inputText);
}