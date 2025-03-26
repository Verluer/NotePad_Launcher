using System.Windows;
using System.Windows.Input;
using Domain.Enum;
using Domain.IService.IEncryption;
using Domain.Model;
using Service.Encryption;

namespace NotePad_Launcher
{
    /// <summary>
    /// Логика взаимодействия для EncryptionWindow.xaml
    /// </summary>
    public partial class EncryptionWindow : Window
    {
        private string FileText;
        private EncryptionMethod SelectedMethod;
        private readonly IRSAService _rsaService;
        public EncryptionWindow(string text, EncryptionMethod method)
        {
            InitializeComponent();
            FileText = text;
            SelectedMethod = method;
            _rsaService = new RSAService();
            var model = new EncryptionModel
            {
                FileText = FileText,
                PrimeE = "5",
                PrimeP = "7",
                PrimeQ = "13"

            };
            var result = _rsaService.Encryption(model);
            MessageBox.Show(result.FileText);
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
    }
}
