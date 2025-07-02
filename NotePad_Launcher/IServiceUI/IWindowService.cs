using System.Windows;

namespace NotePad_Launcher;

public interface IWindowService
{
    void OpenWindow<TWindow>() where TWindow : Window, new();
    void OpenWindowDialog<TWindow>() where TWindow : Window, new();
}