using Domain.IService;
using Domain.Model;
using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Domain.Enum;
using Domain.IService.IEncryption;
using Service.Encryption;
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
        }

        public bool CheckingSaveFile(bool saveFile)
        {
            if (!saveFile)
            {
                MessageBoxResult Ok = MessageBox.Show("Текстовой файл не был сохранен, вы хотите продолжить?", "",
                    MessageBoxButton.YesNo);
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

        private void FileText_TextChanged(object sender, EventArgs e)
        {
            string checkTextFromFile = File.ReadAllText(Path, Encoding.UTF8);
            if (checkTextFromFile == FileText.Text)
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
                FileText.Clear();
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
            FileText.Text = string.Empty;
            Path = nameFile.FilePath;
        }

        private void OpenFileListClick(object sender, RoutedEventArgs e)
        {
            FileListWindow fileListWindow = new FileListWindow(_fileService);
            fileListWindow.Show();
        }

        private void SaveFileClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(FileText.Text);
            var model = new FileModel
            {
                FileText = FileText.Text,
                FileName = FileName.Text,
                FilePath = Path
            };
            if (!string.IsNullOrEmpty(model.FileText.Trim()))
            {
                var saveFile = _fileService.SaveFile(model);
                Path = saveFile.FilePath;
                checkSaveFile = true;
                MessageBox.Show("Текстовой файл успешно сохранен");
                MessageBox.Show("Text: " + FileText.Text);
            }
            else
                MessageBox.Show("Введите текст для текстового файла");

        }

        private void SaveFileDialogClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Сохранить файл как",
                Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*",
                DefaultExt = ".txt",
                FileName = "Новый файл"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                string content = FileText.Text;
                Path = saveFileDialog.FileName;
                // Записываем в файл
                File.WriteAllText(Path, content);
                checkSaveFile = true;
                MessageBox.Show($"Файл сохранен:\n{Path}", "Сохранение", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        public void UpdateFileInfo(FileModel model)
        {
            if (!CheckingSaveFile(checkSaveFile))
            {
                return;
            }

            Path = model.FilePath;
            FileText.Text = model.FileText;
            FileName.Text = model.FileName;
            MessageBox.Show(model.FileName);
        }

        private void DeleteFileClick(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Path))
            {
                MessageBoxResult result = MessageBox.Show(
                    "Вы точно хотите удалить этот текстовой файл??",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    FileText.Clear();
                    _fileService.DeleteFile(Path);
                    FileName.Text = string.Empty;
                    checkSaveFile = true;
                }
            }
        }
        public string GetFileText()
        {
            return FileText.Text; // Возвращаем актуальное значение TextBox
        }

        private void RSAClick(object sender, RoutedEventArgs e)
        {
            string allText = FileText.Text;
            var encryptionWindow = new EncryptionWindow(EncryptionMethod.RSA);
            encryptionWindow.EncryptionResultAction = (newText) =>
            {
                FileText.Text = newText; // Обновляем TextBox в главном окне
            };
            encryptionWindow.Show();
        }

        private void ElgamalClick(object sender, RoutedEventArgs e)
        {
            string allText = FileText.Text;
            var encryptionWindow = new EncryptionWindow(EncryptionMethod.Elgamal);
            encryptionWindow.EncryptionResultAction = (newText) =>
            {
                FileText.Text = newText; // Обновляем TextBox в главном окне
            };
            encryptionWindow.Show();
        }
        private void RabinaClick(object sender, RoutedEventArgs e)
        {
            string allText = FileText.Text;
            var encryptionWindow = new EncryptionWindow(EncryptionMethod.Rabina);
            encryptionWindow.EncryptionResultAction = (newText) =>
            {
                FileText.Text = newText; // Обновляем TextBox в главном окне
            };
            encryptionWindow.Show();
        }

    }
}