using System.Windows;
using System.Windows.Input;
using Domain.Enum;
using Microsoft.Extensions.DependencyInjection;
using NotePad_Launcher.Contracts;
using NotePad_Launcher.ViewModels.MainWindow;

namespace NotePad_Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var viewModel = App.ServiceProvider.GetRequiredService<MainWindowVM>();
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
            viewModel.EncryptedMethodExecuted += OpenEncryptionWindow;
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

        private void OpenEncryptionWindow(EncryptionMethod method)
        {
            EncryptionWindow encryptionWindow;
            switch (method)
            {
                case EncryptionMethod.RSA:
                    encryptionWindow = App.ServiceProvider.GetRequiredService<EncryptionWindow>();
                    encryptionWindow.Show();
                    break;
                case EncryptionMethod.Elgamal:
                    encryptionWindow = App.ServiceProvider.GetRequiredService<EncryptionWindow>();
                    encryptionWindow.Show();
                    break;
                case EncryptionMethod.Rabina:
                    encryptionWindow = App.ServiceProvider.GetRequiredService<EncryptionWindow>();
                    encryptionWindow.Show();
                    break;
                case EncryptionMethod.ECC:
                    encryptionWindow = App.ServiceProvider.GetRequiredService<EncryptionWindow>();
                    encryptionWindow.Show();
                    break;
            }
        }
    }
}