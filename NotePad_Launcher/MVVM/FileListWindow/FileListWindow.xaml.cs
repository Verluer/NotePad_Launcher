using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Domain.IService;
using Domain.Model;
using Microsoft.Extensions.DependencyInjection;
using NotePad_Launcher.ViewModels.FileListWindow;
using NotePad_Launcher.ViewModels.MainWindow;

namespace NotePad_Launcher
{
    /// <summary>
    /// Логика взаимодействия для FileListWindow.xaml
    /// </summary>
    public partial class FileListWindow : Window
    {
        public FileListWindow()
        {
            InitializeComponent();
            var viewModel = App.ServiceProvider.GetRequiredService<FileListWindowVM>(); ;
            this.DataContext = viewModel;
            viewModel.MaximizeRequested += OnMaximizeRequested;
            viewModel.MinimizeRequested += OnMinimizeRequested;
            viewModel.CloseRequested += OnCloseRequested;
        }

        private void HeadLine_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
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
        private void FileListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FileListView.SelectedItem is FileModel selectedFile)
            {
                // Получаем ссылку на уже открытое основное окно (MainWindow)
                var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

                if (mainWindow != null)
                {
                    // Получаем ViewModel первого окна (MainWindow)
                    var mainWindowVM = mainWindow.DataContext as MainWindowVM;
                    if (mainWindowVM != null)
                    {
                        mainWindowVM.UpdateFileInfo(selectedFile);
                    }
                }

                // Закрываем SecondWindow после передачи данных
                this.Close();
            }
        }

    }
}
