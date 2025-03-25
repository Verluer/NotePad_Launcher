using Domain.IService;
using Microsoft.Win32;
using Service;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;
using Application = System.Windows.Application;

namespace NotePad_Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string Path;
        private readonly IFileService _fileService;
        private bool checkSaveFile = true;
        public MainWindow(IFileService fileService)
        {
            InitializeComponent();
            _fileService = fileService; // Сохраняем зависимость
            string testPath = _fileService.ExDirectoryFile();
            if (Directory.Exists(testPath))
            {
                MessageBox.Show("Директория найдена: " + testPath, "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Директория не найдена, создаем...", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                Directory.CreateDirectory(testPath);
                MessageBox.Show("Директория создана: " + testPath, "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        public bool CheckingSaveFile(bool saveFile)
        {
            if (!saveFile)
            {
                MessageBoxResult Ok = MessageBox.Show("Текстовой файл не был сохранен, вы хотите продолжить?", "", MessageBoxButton.YesNo);
                return Ok == MessageBoxResult.Yes;
            }
            return true;
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
            Application.Current.Shutdown();
        }
        private void MinimizeApp_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeApp_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        }
        private void FileText_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Чтение содержимого файла
            string checkTextFromFile = File.ReadAllText(Path, Encoding.UTF8);
            // Сравнение содержимого файла с текстом из RichTextBox
            if (checkTextFromFile == new TextRange(FileText.Document.ContentStart, FileText.Document.ContentEnd).Text)
            {
                checkSaveFile = true;
            }
            else
            {
                checkSaveFile = false;
            }
        }
        private void OpenFileClick(object sender, RoutedEventArgs e)
        {
            if (!CheckingSaveFile(checkSaveFile))
            {
                return;
            }
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                var openFile = _fileService.OpenFile(filePath);
                Path = openFile.FilePath;
                FileName.Text = openFile.FileName;
                FileText.Document.Blocks.Clear();
                FileText.AppendText(openFile.FileText);
            }
        }

        private void CreateFileClick(object sender, RoutedEventArgs e)
        {
            if (!CheckingSaveFile(checkSaveFile))
            {
                return;
            }
            var nameFile = _fileService.CreateFile();
            FileName.Text = nameFile.FileName;
            Path = nameFile.FilePath;
        }
        private void Test(object sender, RoutedEventArgs e)
        {
            EncryptionWindow encryptionWindow = new EncryptionWindow();
            encryptionWindow.Show();
        }
        private void SaveFileClick(object sender, RoutedEventArgs e)
        {
            TextRange textRange = new TextRange(FileText.Document.ContentStart, FileText.Document.ContentEnd);
            string fileText = textRange.Text;
            string fileName = FileName.Text;
            string filePath = Path;
            if (!string.IsNullOrEmpty(fileText.Trim()))
            {
                var testFile = _fileService.SaveFile(filePath, fileName, fileText);
                checkSaveFile = true;
                MessageBox.Show("Текстовой файл успешно сохранен");
            }
            else
                MessageBox.Show("Введите текст для текстового файла");

        }

    }
}