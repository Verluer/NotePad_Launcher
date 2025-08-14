using Domain.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shell;

namespace NotePad_Launcher.MVVM.FunctionalWindows.FindReplaceWindow
{
    /// <summary>
    /// Логика взаимодействия для FindReplaceWindow.xaml
    /// </summary>
    [RegisterService(ServiceLifetime.Transient, asSelf: true)]
    public partial class FindReplaceWindow : Window
    {
        public FindReplaceWindow()
        {
            InitializeComponent();

            var chrome = new WindowChrome
            {
                CaptionHeight = 0,
                ResizeBorderThickness = new Thickness(6),
                UseAeroCaptionButtons = false
            };

            WindowChrome.SetWindowChrome(this, chrome);

            var viewModel = App.ServiceProvider.GetRequiredService<FindReplaceWindowVM>(); ;
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
