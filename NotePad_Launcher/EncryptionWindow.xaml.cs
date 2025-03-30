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
        private readonly IElgamalService _elgamalService;
        private readonly IRabinaService _rabinaService;
        public EncryptionWindow(EncryptionMethod method)
        {
            InitializeComponent();
            SelectedMethod = method;
            _rsaService = new RSAService();
            _elgamalService = new ElgamalService();
            _rabinaService = new RabinaService();
            switch (SelectedMethod)
            {
                case EncryptionMethod.RSA:
                    RSAUI();
                    break;
                case EncryptionMethod.Elgamal:
                    ElgamalUI();
                    break;
                case EncryptionMethod.Rabina:
                    RabinaUI();
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
            TextValue5.Visibility = Visibility.Collapsed;
        }

        private void ElgamalUI()
        {
            LabelText1.Text = "Enter prime number p:";
            LabelText2.Text = "Enter primitive root g:";
            LabelText3.Text = "Enter open key (y, g, p)";
        }
        private void RabinaUI()
        {
            LabelText1.Text = "Enter prime number p:";
            LabelText2.Text = "Enter prime number q:";
            LabelText3.Text = "?Enter open key n";
            CloseKey3.Visibility = Visibility.Collapsed;
            TextValue5.Visibility = Visibility.Collapsed;
            TextValue4.Visibility = Visibility.Collapsed;
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
            EncryptionModel model;
            EncryptionModel result;
            FileText = GetTextFromMainWindow();
            switch (SelectedMethod)
            {
                case EncryptionMethod.RSA:
                    model = new EncryptionModel
                    {
                        FileText = FileText,
                        PrimeP = TextValue1.Text,
                        PrimeQ = TextValue2.Text,
                        PrimeE = TextValue3.Text,
                        ModulusN = TextValue4.Text

                    };
                    result = _rsaService.Encryption(model);
                    LabelCloseKey.Text += $"{result.CloseKeyD},{result.ModulusN}";
                    LabelOpenKey.Text += $"{result.PrimeE},{result.ModulusN}";
                    EncryptionResultAction?.Invoke(result.FileText);
                    break;
                case EncryptionMethod.Elgamal:
                    if (string.IsNullOrEmpty(TextValue1.Text) && string.IsNullOrEmpty(TextValue2.Text))
                    {
                        model = new EncryptionModel
                        {
                            FileText = FileText,
                            PrimeP = TextValue5.Text,
                            PrimeQ = TextValue4.Text,
                            PrimeE = TextValue3.Text,
                        };
                    }
                    else
                    {
                        model = new EncryptionModel
                        {
                            FileText = FileText,
                            PrimeP = TextValue1.Text,
                            PrimeQ = TextValue2.Text,
                            PrimeE = TextValue3.Text,
                        };
                    }
                    result = _elgamalService.Encryption(model);
                    LabelCloseKey.Text += $"{result.CloseKeyD},{result.PrimeP},{result.PrimeQ}";
                    LabelOpenKey.Text += $"{result.PrimeE},{result.PrimeQ},{result.PrimeP}";
                    EncryptionResultAction?.Invoke(result.FileText);
                    break;
                case EncryptionMethod.Rabina:
                    model = new EncryptionModel
                    {
                        FileText = FileText,
                        PrimeP = TextValue1.Text,
                        PrimeQ = TextValue2.Text,
                        ModulusN = TextValue3.Text

                    };
                    result = _rabinaService.Encryption(model);
                    if (result.Signature == false)
                    {
                        MessageBox.Show("Одно из чисел не удовлетворяет условия 3 mod4, введите другое");
                        return;
                    }

                    LabelCloseKey.Text += $"{result.PrimeP},{result.PrimeQ}";
                    LabelOpenKey.Text += $"{result.ModulusN}";
                    EncryptionResultAction?.Invoke(result.FileText);
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
            EncryptionModel model;
            EncryptionModel result;
            switch (SelectedMethod)
            {
                case EncryptionMethod.RSA:
                    model = new EncryptionModel
                    {
                        FileText = FileText,
                        CloseKeyD = CloseKey1.Text,
                        ModulusN = CloseKey2.Text

                    };
                    result = _rsaService.Decryption(model);
                    EncryptionResultAction?.Invoke(result.FileText);
                    break;
                case EncryptionMethod.Elgamal:
                    model = new EncryptionModel
                    {
                        FileText = FileText,
                        CloseKeyD = CloseKey1.Text,
                        PrimeP = CloseKey2.Text,
                        PrimeQ = CloseKey3.Text

                    };
                    result = _elgamalService.Decryption(model);
                    EncryptionResultAction?.Invoke(result.FileText);
                    break;
                case EncryptionMethod.Rabina:
                    model = new EncryptionModel
                    {
                        FileText = FileText,
                        PrimeP = CloseKey1.Text,
                        PrimeQ = CloseKey2.Text

                    };
                    result = _rabinaService.Decryption(model);
                    EncryptionResultAction?.Invoke(result.FileText);
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
