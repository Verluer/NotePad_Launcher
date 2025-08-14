using Domain.Attributes;
using Domain.IService.ISystemApp;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace NotePad_Launcher;
[RegisterService(ServiceLifetime.Singleton, serviceType: typeof(IWindowService))]
public class WindowService : IWindowService
{
    private readonly Dictionary<Type, Window> _openWindows = new();

    public void OpenWindow<TWindow>() where TWindow : Window
    {
        var windowType = typeof(TWindow);

        if (_openWindows.TryGetValue(windowType, out var existingWindow))
        {
            if (existingWindow.IsVisible)
            {
                if (existingWindow.WindowState == WindowState.Minimized)
                    existingWindow.WindowState = WindowState.Normal;

                existingWindow.Activate();
                return;
            }
        }

        var window = App.ServiceProvider.GetRequiredService<TWindow>();
        _openWindows[windowType] = window;

        window.Closed += (s, e) => _openWindows.Remove(windowType);

        window.Show();
    }

    public void OpenWindowDialog<TWindow>() where TWindow : Window
    {
        var window = App.ServiceProvider.GetRequiredService<TWindow>();
        window.ShowDialog();
    }
}