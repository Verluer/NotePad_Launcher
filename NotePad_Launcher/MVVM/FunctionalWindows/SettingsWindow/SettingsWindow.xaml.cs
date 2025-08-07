using Domain.IService;
using Microsoft.Extensions.DependencyInjection;
using NotePad_Launcher.MVVM.FunctionalWindows.SearchWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Shell;

namespace NotePad_Launcher.MVVM.FunctionalWindows.SettingsWindow
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            var chrome = new WindowChrome
            {
                CaptionHeight = 0,
                ResizeBorderThickness = new Thickness(6),
                UseAeroCaptionButtons = false
            };

            WindowChrome.SetWindowChrome(this, chrome);

            var viewModel = App.ServiceProvider.GetRequiredService<SettingsWindowVM>(); ;
            this.DataContext = viewModel;
            viewModel.MinimizeRequested += OnMinimizeRequested;
            viewModel.CloseRequested += OnCloseRequested;
        }
        private void HeadLine_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (this.WindowState == WindowState.Maximized)
                {

                    var mousePos = e.GetPosition(this);
                    var screenPos = this.PointToScreen(mousePos);

                    double relativeX = mousePos.X / this.ActualWidth;

                    this.WindowState = WindowState.Normal;

                    this.Left = screenPos.X - relativeX * this.Width;
                    this.Top = screenPos.Y - mousePos.Y;

                    this.DragMove();
                }
                else
                {
                    this.DragMove();
                }
            }
        }
        private void OnCloseRequested()
        {
            this.Close();
        }
        private void OnMinimizeRequested()
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}
