using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
                _dataStorage.PushUpdatedSelectionFile(selectedFile);

                this.Close();
            }
        }

    }
}
