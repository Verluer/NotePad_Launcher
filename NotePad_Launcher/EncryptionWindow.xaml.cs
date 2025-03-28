using System.Windows;
using System.Windows.Input;
using Domain.Enum;
using Domain.IService.IEncryption;
using Domain.Model;
using Service.Encryption;
using static ICSharpCode.AvalonEdit.Document.TextDocumentWeakEventManager;

namespace NotePad_Launcher
{
    /// <summary>
    /// Логика взаимодействия для EncryptionWindow.xaml
    /// </summary>
    public partial class EncryptionWindow : Window
    {
        public Action<string> EncryptionResultAction; 
        private string FileText;
        private EncryptionMethod SelectedMethod;
        private readonly IRSAService _rsaService;
        public EncryptionWindow(EncryptionMethod method)
        {
            InitializeComponent();
            SelectedMethod = method;
            _rsaService = new RSAService();
            switch (SelectedMethod)
            {
                case EncryptionMethod.RSA:
                    RSAUI();
                    break;
                case EncryptionMethod.Elgamal:
                    // Действие для метода B
                    break;
                case EncryptionMethod.Rabina:
                    // Действие для метода C
                    break;
                case EncryptionMethod.ECC:
                    // Действие для метода D
                    break;
                default:
                    // Действие, если метод не выбран (None)
                    break;
            }
        }

        private void RSAUI()
        {
            LabelText1.Text = "Enter prime number p:";
            LabelText2.Text = "Enter prime number q:";
            LabelText3.Text = "Enter prime number e or (e,n)";
            CloseKey3.Visibility = Visibility.Collapsed;
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
        private string GetTextFromMainWindow()
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                return mainWindow.GetFileText(); // Получаем актуальное значение из FileText
            }
            return string.Empty;
        }
        private void EncryptionButton_OnClick(object sender, RoutedEventArgs e)
        {
            FileText = GetTextFromMainWindow();
            switch (SelectedMethod)
            {
                case EncryptionMethod.RSA:
                    var model = new EncryptionModel
                    {
                        FileText = FileText,
                        PrimeP = TextValue1.Text,
                        PrimeQ = TextValue2.Text,
                        PrimeE = TextValue3.Text,
                        ModulusN = TextValue4.Text

                    };
                    var result = _rsaService.Encryption(model);
                    LabelCloseKey.Text += $"{result.CloseKeyD},{result.ModulusN}";
                    LabelOpenKey.Text += $"{result.PrimeE},{result.ModulusN}";
                    EncryptionResultAction?.Invoke(result.FileText);
                    break;
                case EncryptionMethod.Elgamal:
                    // Действие для метода B
                    break;
                case EncryptionMethod.Rabina:
                    // Действие для метода C
                    break;
                case EncryptionMethod.ECC:
                    // Действие для метода D
                    break;
                default:
                    // Действие, если метод не выбран (None)
                    break;
            }
        }
        private void DecryptionButton_OnClick(object sender, RoutedEventArgs e)
        {
            FileText = GetTextFromMainWindow();
            switch (SelectedMethod)
            {
                case EncryptionMethod.RSA:
                    var model = new EncryptionModel
                    {
                        FileText = FileText,
                        CloseKeyD = CloseKey1.Text,
                        ModulusN = CloseKey2.Text

                    };
                    var result = _rsaService.Decryption(model);
                    EncryptionResultAction?.Invoke(result.FileText);
                    break;
                case EncryptionMethod.Elgamal:
                    // Действие для метода B
                    break;
                case EncryptionMethod.Rabina:
                    // Действие для метода C
                    break;
                case EncryptionMethod.ECC:
                    // Действие для метода D
                    break;
                default:
                    // Действие, если метод не выбран (None)
                    break;
            }
        }

        private void DigitalSignatureButton_OnClick(object sender, RoutedEventArgs e)
        {
            FileText = GetTextFromMainWindow();
            switch (SelectedMethod)
            {
                case EncryptionMethod.RSA:
                    var model = new EncryptionModel
                    {
                        FileText = FileText,
                        PrimeP = TextValue1.Text,
                        PrimeQ = TextValue2.Text,
                        PrimeE = TextValue3.Text,
                        ModulusN = TextValue4.Text
                    };
                    var result = _rsaService.Signature(model);
                    if (string.IsNullOrEmpty(result.FileText))
                    {
                        switch (model.Signature)
                        {
                            case true:
                                MessageBox.Show("DigitalSignature valid");
                                break;
                            case false:
                                MessageBox.Show("DigitalSignature invalid");
                                break;
                        }
                    }
                    else
                    {
                        EncryptionResultAction?.Invoke(result.FileText);
                    }
                    break;
                case EncryptionMethod.Elgamal:
                    // Действие для метода B
                    break;
                case EncryptionMethod.Rabina:
                    // Действие для метода C
                    break;
                case EncryptionMethod.ECC:
                    // Действие для метода D
                    break;
                default:
                    // Действие, если метод не выбран (None)
                    break;
            }
        }
    }
}
