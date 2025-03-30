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
using NotePad_Launcher.Contracts;
using Service.Encryption;
using Application = System.Windows.Application;
using NotePad_Launcher.ViewModels;
using NotePad_Launcher.ViewModels.MainWindow;

namespace NotePad_Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IWindowService
    {
        private string FilePath;
        private readonly IFileService _fileService;
        private bool checkSaveFile = true;

        public MainWindow(IFileService fileService)
        {
            InitializeComponent();
            _fileService = fileService; // Сохраняем зависимость
            var viewModel = new MainWindowVM();
            viewModel.WindowService = this;
            DataContext = viewModel;
        }
        public void Minimize()
        {
            this.WindowState = WindowState.Minimized;
        }

        public void Maximize()
        {
            this.WindowState = this.WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
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

        private void FileText_TextChanged(object sender, EventArgs e)
        {
            checkSaveFile = _fileService.CheckTextChange(FilePath, FileText.Text);
        }

        private void OpenFileClick(object sender, RoutedEventArgs e)
        {
            if (!CheckingSaveFile(checkSaveFile))
            {
                return;
            }

            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                var filePath = openFileDialog.FileName;
                var openFile = _fileService.OpenFile(filePath);
                FilePath = openFile.FilePath;
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
            FilePath = nameFile.FilePath;
        }

        private void OpenFileListClick(object sender, RoutedEventArgs e)
        {
            var fileListWindow = new FileListWindow(_fileService);
            fileListWindow.Show();
        }

        private void SaveFileClick(object sender, RoutedEventArgs e)
        {
            var model = new FileModel
            {
                FileText = FileText.Text,
                FileName = FileName.Text,
                FilePath = FilePath
            };
            if (!string.IsNullOrEmpty(model.FileText.Trim()))
            {
                var saveFile = _fileService.SaveFile(model);
                FilePath = saveFile.FilePath;
                checkSaveFile = true;
                MessageBox.Show("Текстовой файл успешно сохранен");
            }
            else
                MessageBox.Show("Введите текст для текстового файла");

        }

        private void SaveFileDialogClick(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Title = "Сохранить файл как",
                Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*",
                DefaultExt = ".txt",
                FileName = "Новый файл"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                FilePath = saveFileDialog.FileName;
                _fileService.WriteAllText(FilePath, FileText.Text);
                FileName.Text = _fileService.GetFileName(FilePath);
                checkSaveFile = true;
                MessageBox.Show($"Файл сохранен:\n{FilePath}", "Сохранение", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        public void UpdateFileInfo(FileModel model)
        {
            if (!CheckingSaveFile(checkSaveFile))
            {
                return;
            }

            FilePath = model.FilePath;
            FileText.Text = model.FileText;
            FileName.Text = model.FileName;
        }

        private void DeleteFileClick(object sender, RoutedEventArgs e)
        {
            if (_fileService.FileExists(FilePath))
            {
                MessageBoxResult result = MessageBox.Show(
                    "Вы точно хотите удалить этот текстовой файл??",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    FileText.Clear();
                    _fileService.DeleteFile(FilePath);
                    FileName.Text = string.Empty;
                    checkSaveFile = true;
                }
            }
        }

        private void WordWrapClick(object sender, RoutedEventArgs e)
        {
            if (WordWrap.IsChecked)
            {
                FileText.WordWrap = false;
            
            }
            else
            {
                FileText.WordWrap = true;
            }
            WordWrap.IsChecked = !WordWrap.IsChecked;
        }
        public string GetFileText()
        {
            return FileText.Text; // Возвращаем актуальное значение TextBox
        }
        private void OpenEncryptionWindow(EncryptionMethod method)
        {
            var encryptionWindow = new EncryptionWindow(method);
            encryptionWindow.EncryptionResultAction = (newText) =>
            {
                FileText.Text = newText; // Обновляем TextBox в главном окне
            };
            encryptionWindow.Show();
        }
        private void RSAClick(object sender, RoutedEventArgs e)
        {
            OpenEncryptionWindow(EncryptionMethod.RSA);
        }

        private void ElgamalClick(object sender, RoutedEventArgs e)
        {
            OpenEncryptionWindow(EncryptionMethod.Elgamal);
        }
        private void RabinaClick(object sender, RoutedEventArgs e)
        {
            OpenEncryptionWindow(EncryptionMethod.Rabina);
        }
        private void ECCClick(object sender, RoutedEventArgs e)
        {
            OpenEncryptionWindow(EncryptionMethod.ECC);
        }
    }
}