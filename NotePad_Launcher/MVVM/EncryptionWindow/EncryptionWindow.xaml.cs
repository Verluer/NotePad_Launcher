using System.Windows;
using System.Windows.Input;
using Domain.Enum;
using Domain.IService.IEncryption;
using Domain.Model;
using NotePad_Launcher.ViewModels.EncryptionWindow;
using NotePad_Launcher.ViewModels.MainWindow;
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
        private readonly IECCService _eccService;
        public EncryptionWindow(EncryptionMethod method)
        {
            InitializeComponent();
            var viewModel = new EncryptionWindowVM(method);
            this.DataContext = viewModel;
            _rsaService = new RSAService();
            _elgamalService = new ElgamalService();
            _rabinaService = new RabinaService();
            _eccService = new ECCService();
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
                    model = new EncryptionModel
                    {
                        FileText = FileText,
                        CloseKeyD = TextValue1.Text,
                        PrimeE = TextValue3.Text,
                    };
                    result = _eccService.Encryption(model);
                    TextValue3.Text = result.PrimeE;
                    EncryptionResultAction?.Invoke(result.FileText);
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
                    model = new EncryptionModel
                    {
                        FileText = FileText,
                        CloseKeyD = CloseKey1.Text
                        
                    };
                    result = _eccService.Decryption(model);
                    EncryptionResultAction?.Invoke(result.FileText);
                    break;
                default:
                    // Действие, если метод не выбран (None)
                    break;
            }
        }

        private void DigitalSignatureButton_OnClick(object sender, RoutedEventArgs e)
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
                        PrimeP = TextValue1.Text,
                        PrimeQ = TextValue2.Text,
                        PrimeE = TextValue3.Text,
                        ModulusN = TextValue4.Text
                    };
                    result = _rsaService.Signature(model);
                    if (string.IsNullOrEmpty(result.FileText))
                    {
                        switch (result.Signature)
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
                    model = new EncryptionModel
                    {
                        FileText = FileText,
                        PrimeE = TextValue3.Text,
                        CloseKeyD = TextValue1.Text
                    };
                    result = _eccService.Signature(model);
                    TextValue3.Text = result.PrimeE;
                    if (string.IsNullOrEmpty(result.FileText))
                    {
                        switch (result.Signature)
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
                default:
                    // Действие, если метод не выбран (None)
                    break;
            }
        }
    }
}
