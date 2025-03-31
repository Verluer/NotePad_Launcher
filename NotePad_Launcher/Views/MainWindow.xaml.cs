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
using Microsoft.Extensions.DependencyInjection;
using NotePad_Launcher.Contracts;
using Service.Encryption;
using Application = System.Windows.Application;
using NotePad_Launcher.ViewModels;
using NotePad_Launcher.ViewModels.MainWindow;
using static System.Net.WebRequestMethods;
using System.Windows.Threading;
using NotePad_Launcher.Services;
using FileDialog = Microsoft.Win32.FileDialog;

namespace NotePad_Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string FilePath;
        private readonly IFileService _fileService;
        private readonly MainWindowVM _viewModel;
        private bool checkSaveFile = true;


        public MainWindow(IFileService fileService)
        {
            InitializeComponent();
            _fileService = fileService;
            var viewModel = new MainWindowVM();

            // Устанавливаем DataContext
            this.DataContext = viewModel;

            // Подписываемся на события
            viewModel.MaximizeRequested += OnMaximizeRequested;
            viewModel.MinimizeRequested += OnMinimizeRequested;
            _fileService.ExDirectoryFile();
        }
        public void OnMinimizeRequested()
        {
            this.WindowState = WindowState.Minimized;
        }

        public void OnMaximizeRequested()
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
            FileText.Document.Text = string.Empty;
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
                FileText = FileText.Document.Text,
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
                _fileService.WriteAllText(FilePath, FileText.Document.Text);
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
            FileText.Document.Text = model.FileText;
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
            return FileText.Document.Text; // Возвращаем актуальное значение TextBox
        }
        private void OpenEncryptionWindow(EncryptionMethod method)
        {
            var encryptionWindow = new EncryptionWindow(method);
            encryptionWindow.EncryptionResultAction = (newText) =>
            {
                FileText.Document.Text = newText; // Обновляем TextBox в главном окне
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