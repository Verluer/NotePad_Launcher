using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace NotePad_Launcher;

public class WindowService : IWindowService
{
    public void OpenWindow<TWindow>() where TWindow : Window, new()
    {
        var window = App.ServiceProvider.GetRequiredService <TWindow>();
        window.Show();
    }
    public void OpenWindowDialog<TWindow>() where TWindow : Window, new()
    {
        var window = App.ServiceProvider.GetRequiredService<TWindow>();
        window.ShowDialog();
    }
}