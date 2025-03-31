using Microsoft.Win32;
using NotePad_Launcher.Contracts;

namespace NotePad_Launcher.Services;

public class FileDialog : IFileDialog
{
    public string OpenTextFileDialog()
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Filter = "Текстовые файлы (*.txt)|*.txt"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            return openFileDialog.FileName;
        }

        return null;
    }
}