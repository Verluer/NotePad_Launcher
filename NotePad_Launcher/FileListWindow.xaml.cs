using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Domain.IService;
using Domain.Model;

namespace NotePad_Launcher
{
    /// <summary>
    /// Логика взаимодействия для FileListWindow.xaml
    /// </summary>
    public partial class FileListWindow : Window
    {
        private readonly IFileService _fileService;
            
        public FileListWindow(IFileService fileService)
        {
            InitializeComponent();
            _fileService = fileService; // Сохраняем зависимость
            LoadFileList();
        }

        private void HeadLine_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void MinimizeApp_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeApp_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        }
        private void FileListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FileListView.SelectedItem is FileModel selectedFile)
            {
                var model = new FileModel
                {
                    FileName = selectedFile.FileName,
                    FilePath = selectedFile.FilePath,
                    FileText = File.ReadAllText(selectedFile.FilePath),
                };
                // Получаем ссылку на уже открытое окно MainWindow
                var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

                if (mainWindow != null)
                {
                    // Обновляем свойства или вызываем методы в MainWindow
                    mainWindow.UpdateFileInfo(model);
                }
            }
            this.Close();
        }
        private void LoadFileList()
        {
            try
            {
                var files = _fileService.GetTextFiles();
                FileListView.ItemsSource = files; // Привязываем список файлов
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
    }
}
