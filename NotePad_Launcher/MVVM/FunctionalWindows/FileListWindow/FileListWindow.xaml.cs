using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shell;
using Domain.IService;
using Domain.Model;
using Microsoft.Extensions.DependencyInjection;
using NotePad_Launcher.MVVM.FunctionalWindows.FileListWindow;
using NotePad_Launcher.ViewModels.MainWindow;

namespace NotePad_Launcher
{
    /// <summary>
    /// Логика взаимодействия для FileListWindow.xaml
    /// </summary>
    public partial class FileListWindow : Window
    {
        private readonly IDataStorage _dataStorage;

        public FileListWindow(IDataStorage dataStorage)
        {
            InitializeComponent();

            var chrome = new WindowChrome
            {
                CaptionHeight = 0,
                ResizeBorderThickness = new Thickness(6),
                UseAeroCaptionButtons = false
            };

            WindowChrome.SetWindowChrome(this, chrome);

            var viewModel = App.ServiceProvider.GetRequiredService<FileListWindowVM>(); ;
            this.DataContext = viewModel;
            viewModel.MaximizeRequested += OnMaximizeRequested;
            viewModel.MinimizeRequested += OnMinimizeRequested;
            viewModel.CloseRequested += OnCloseRequested;
            _dataStorage = dataStorage;
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
        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item && item.DataContext is FileModel selectedFile)
            {
                if ((Keyboard.Modifiers & ModifierKeys.Control) == 0 && (Keyboard.Modifiers & ModifierKeys.Shift) == 0)
                {
                    _dataStorage.PushUpdatedSelectionFile(selectedFile, false);
                    this.Close();
                }
            }
        }

    }
}
