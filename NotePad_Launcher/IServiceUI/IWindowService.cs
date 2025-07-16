using System.Windows;

namespace NotePad_Launcher;

public interface IWindowService
{
    public void OpenWindow<TWindow>() where TWindow : Window;
    public void OpenWindowDialog<TWindow>() where TWindow : Window;
}