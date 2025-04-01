using System.Windows;
using System.Windows.Input;
using Domain.Enum;
using NotePad_Launcher.ViewModels.MainWindow;

namespace NotePad_Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowVM _viewModel;

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
                    encryptionWindow = new EncryptionWindow(method);
                    encryptionWindow.Show();
                    break;
                case EncryptionMethod.Elgamal:
                    encryptionWindow = new EncryptionWindow(method);
                    encryptionWindow.Show();
                    break;
                case EncryptionMethod.Rabina:
                    encryptionWindow = new EncryptionWindow(method);
                    encryptionWindow.Show();
                    break;
                case EncryptionMethod.ECC:
                    encryptionWindow = new EncryptionWindow(method);
                    encryptionWindow.Show();
                    break;
            }
        }
        public string GetFileText()
        {
            return FileText.Document.Text; // Возвращаем актуальное значение TextBox
        }
    }
}