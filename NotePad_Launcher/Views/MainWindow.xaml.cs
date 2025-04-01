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
using System.ComponentModel;

namespace NotePad_Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowVM _viewModel;
        private bool checkSaveFile = true;


        public MainWindow()
        {
            InitializeComponent();
            var viewModel = new MainWindowVM();
            // Устанавливаем DataContext
            this.DataContext = viewModel;
            FileText.Document = viewModel.FileTextDocument;
            // Подписываемся на события
            viewModel.MaximizeRequested += OnMaximizeRequested;
            viewModel.MinimizeRequested += OnMinimizeRequested;
            viewModel.UpdateWordWrapAction = () =>
            {
                // Обновляем свойство FileText.WordWrap в View
                FileText.WordWrap = viewModel.IsWordWrapEnabled;
            };
            viewModel.OpenFileListWindowRequested += () =>
            {
                var fileListWindow = new FileListWindow();
                fileListWindow.Show();
            };
        }

        private void OnMinimizeRequested()
        {
            this.WindowState = WindowState.Minimized;
        }

        private void OnMaximizeRequested()
        {
            this.WindowState = this.WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        }

        private void HeadLine_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
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