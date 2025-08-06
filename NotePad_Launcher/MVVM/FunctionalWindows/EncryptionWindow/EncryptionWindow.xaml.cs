using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Shell;
using Domain.Enum;
using Domain.IService.IEncryption;
using Domain.Model;
using Microsoft.Extensions.DependencyInjection;
using NotePad_Launcher.MVVM.FunctionalWindows.EncryptionWindow;
using NotePad_Launcher.ViewModels.MainWindow;
using Service.Encryption;
using static ICSharpCode.AvalonEdit.Document.TextDocumentWeakEventManager;

namespace NotePad_Launcher
{
    /// <summary>
    /// Логика взаимодействия для EncryptionWindow.xaml
    /// </summary>
    public partial class EncryptionWindow : Window
    {
        public EncryptionWindow()
        {
            InitializeComponent();

            var chrome = new WindowChrome
            {
                CaptionHeight = 0,
                ResizeBorderThickness = new Thickness(6),
                UseAeroCaptionButtons = false
            };

            WindowChrome.SetWindowChrome(this, chrome);

            var viewModel = App.ServiceProvider.GetRequiredService<EncryptionWindowVM>();
            this.DataContext = viewModel;
            viewModel.MaximizeRequested += OnMaximizeRequested;
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

        private void OnMaximizeRequested()
        {
            this.WindowState = this.WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        }

    }
}
