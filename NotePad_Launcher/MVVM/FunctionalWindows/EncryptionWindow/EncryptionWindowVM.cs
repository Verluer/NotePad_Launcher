using Domain.IService.IEncryption;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Domain.Enum;
using Service.Encryption;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Domain.Model;
using Microsoft.Extensions.DependencyInjection;
using NotePad_Launcher.MVVM.Commands;
using Service;
using NotePad_Launcher.ViewModels;
using Domain.IService.ISystemApp;
using Domain.Attributes;

namespace NotePad_Launcher.MVVM.FunctionalWindows.EncryptionWindow;

[RegisterService(ServiceLifetime.Transient, asSelf: true)]
public class EncryptionWindowVM : INotifyPropertyChanged
{
    private readonly IRSAService _rsaService;
    private readonly IElgamalService _elgamalService;
    private readonly IRabinaService _rabinaService;
    private readonly IECCService _eccService;
    private readonly ILFSRService _lFSRService;
    private readonly IG28147Service _g28147Service;
    private readonly IFileDialog _fileDialog;
    private readonly IServiceFunctions _serviceFunctions;
    public event PropertyChangedEventHandler? PropertyChanged;
    public event Action? MaximizeRequested;
    public event Action? MinimizeRequested;
    public event Action? CloseRequested;

    private ICommand? _closeCommand;
    private ICommand? _minimizeCommand;
    private ICommand? _maximizeCommand;
    private ICommand? _encryptionCommand;
    private ICommand? _digitalSignatureCommand;
    private ICommand? _decryptionCommand;
    private string _methodName;
    private EncryptionMethod _selectedMethod;
    public EncryptionMethod SelectedMethod { get; }
    private readonly IDataStorage _dataStorage;
    public EncryptionWindowVM(IDataStorage dataStorage, IFileDialog fileDialog)
    {
        _dataStorage = dataStorage;
        SelectedMethod = _dataStorage.CurrentMethod;
        _rsaService = new RSAService();
        _elgamalService = new ElgamalService();
        _rabinaService = new RabinaService();
        _eccService = new ECCService();
        _lFSRService = new LFSRService();
        _g28147Service = new G28147Service();
        _fileDialog = fileDialog;
        TextBlockCloseKey1 = "Enter Close key";
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
                ECCUI();
                break;
            case EncryptionMethod.LFSR:
                LFSRUI();
                break;
            case EncryptionMethod.G28147:
                G28147UI();
                break;
            default:
                break;
        }
    }
    #region TextBlock
    public string MethodName
    {
        get => _methodName;
        set => SetField(ref _methodName, value);
    }
    private string _textBlockValue1;
    public string TextBlockValue1
    {
        get => _textBlockValue1;
        set => SetField(ref _textBlockValue1, value);
    }
    private string _textBlockValue2;
    public string TextBlockValue2
    {
        get => _textBlockValue2;
        set => SetField(ref _textBlockValue2, value);
    }
    private string _textBlockValue3;
    public string TextBlockValue3
    {
        get => _textBlockValue3;
        set => SetField(ref _textBlockValue3, value);
    }
    private string _textBlockValue4;
    public string TextBlockValue4
    {
        get => _textBlockValue4;
        set => SetField(ref _textBlockValue4, value);
    }
    private string _textBlockValue5;
    public string TextBlockValue5
    {
        get => _textBlockValue5;
        set => SetField(ref _textBlockValue5, value);
    }
    private string _textBlockCloseKey;
    public string TextBlockCloseKey
    {
        get => _textBlockCloseKey;
        set => SetField(ref _textBlockCloseKey, value);
    }
    private string _textBlockOpenKey;
    public string TextBlockOpenKey
    {
        get => _textBlockOpenKey;
        set => SetField(ref _textBlockOpenKey, value);
    }
    private string _textBlockCloseKey1;
    public string TextBlockCloseKey1
    {
        get => _textBlockCloseKey1;
        set => SetField(ref _textBlockCloseKey1, value);
    }
    #endregion
    #region TextBox

    private string _textBoxValue1;
    public string TextBoxValue1
    {
        get => _textBoxValue1;
        set
        {
            _textBoxValue1 = value;
            OnPropertyChanged(nameof(TextBoxValue1));
        }
    }
    private string _textBoxValue2;
    public string TextBoxValue2
    {
        get => _textBoxValue2;
        set
        {
            _textBoxValue2 = value;
            OnPropertyChanged(nameof(TextBoxValue2));
        }
    }
    private string _textBoxValue3;
    public string TextBoxValue3
    {
        get => _textBoxValue3;
        set
        {
            _textBoxValue3 = value;
            OnPropertyChanged(nameof(TextBoxValue3));
        }
    }
    private string _textBoxValue4;
    public string TextBoxValue4
    {
        get => _textBoxValue4;
        set
        {
            _textBoxValue4 = value;
            OnPropertyChanged(nameof(TextBoxValue4));
        }
    }
    private string _textBoxValue5;
    public string TextBoxValue5
    {
        get => _textBoxValue5;
        set
        {
            _textBoxValue5 = value;
            OnPropertyChanged(nameof(TextBoxValue5));
        }
    }
    private string _textBoxCloseKey1;
    public string TextBoxCloseKey1
    {
        get => _textBoxCloseKey1;
        set
        {
            _textBoxCloseKey1 = value;
            OnPropertyChanged(nameof(TextBoxCloseKey1));
        }
    }
    private string _textBoxCloseKey2;
    public string TextBoxCloseKey2
    {
        get => _textBoxCloseKey2;
        set
        {
            _textBoxCloseKey2 = value;
            OnPropertyChanged(nameof(TextBoxCloseKey2));
        }
    }
    private string _textBoxCloseKey3;
    public string TextBoxCloseKey3
    {
        get => _textBoxCloseKey3;
        set
        {
            _textBoxCloseKey3 = value;
            OnPropertyChanged(nameof(TextBoxCloseKey3));
        }
    }
    #endregion
    #region BoolElement
    private bool _isElement1Visible = true;

    public bool IsElement1Visible
    {
        get => _isElement1Visible;
        set => SetField(ref _isElement1Visible, value);
    }

    private bool _isElement2Visible = true;

    public bool IsElement2Visible
    {
        get => _isElement2Visible;
        set => SetField(ref _isElement2Visible, value);
    }
    private bool _isTextBlock3Visible = true;

    public bool IsTextBlock3Visible
    {
        get => _isTextBlock3Visible;
        set => SetField(ref _isTextBlock3Visible, value);
    }
    private bool _isTextBlockCloseKeyVisible = true;

    public bool IsTextBlockCloseKeyVisible
    {
        get => _isTextBlockCloseKeyVisible;
        set => SetField(ref _isTextBlockCloseKeyVisible, value);
    }
    private bool _isTextBoxValue3Visible = true;

    public bool IsTextBoxValue3Visible
    {
        get => _isTextBoxValue3Visible;
        set => SetField(ref _isTextBoxValue3Visible, value);
    }
    private bool _isTextBoxValue4Visible = true;

    public bool IsTextBoxValue4Visible
    {
        get => _isTextBoxValue4Visible;
        set => SetField(ref _isTextBoxValue4Visible, value);
    }
    private bool _isTextBoxValue5Visible = true;

    public bool IsTextBoxValue5Visible
    {
        get => _isTextBoxValue5Visible;
        set => SetField(ref _isTextBoxValue5Visible, value);
    }
    private bool _isTextBoxCloseKey1Visible = true;

    public bool IsTextBoxCloseKey1Visible
    {
        get => _isTextBoxCloseKey1Visible;
        set => SetField(ref _isTextBoxCloseKey1Visible, value);
    }
    private bool _isTextBoxCloseKey2Visible = true;

    public bool IsTextBoxCloseKey2Visible
    {
        get => _isTextBoxCloseKey2Visible;
        set => SetField(ref _isTextBoxCloseKey2Visible, value);
    }
    private bool _isTextBoxCloseKey3Visible = true;

    public bool IsTextBoxCloseKey3Visible
    {
        get => _isTextBoxCloseKey3Visible;
        set => SetField(ref _isTextBoxCloseKey3Visible, value);
    }
    private bool _isButtonDigitalVisible = true;
    public bool IsButtonDigitalVisible
    {
        get => _isButtonDigitalVisible;
        set => SetField(ref _isButtonDigitalVisible, value);
    }
    #endregion
    #region ComboBox
    private string _selectedMode;
    public string SelectedMode
    {
        get => _selectedMode;
        set => SetField(ref _selectedMode, value);
    }

    public List<string> Mode { get; set; }
    #endregion
    private void RSAUI()
    {
        MethodName = "RSA Encryption";
        TextBlockValue1 = "Enter prime number p:";
        TextBlockValue2 = "Enter prime number q:";
        TextBlockValue3 = "Enter prime number e or (e,n)";
        IsTextBoxCloseKey3Visible = false;
        IsTextBoxValue5Visible = false;
    }

    private void ElgamalUI()
    {
        MethodName = "Elgamal Encryption";
        TextBlockValue1 = "Enter prime number p:";
        TextBlockValue2 = "Enter primitive root g:";
        TextBlockValue3 = "Enter open key (y, g, p)";
    }
    private void RabinaUI()
    {
        MethodName = "Rabina Encryption";
        TextBlockValue1 = "Enter prime number p:";
        TextBlockValue2 = "Enter prime number q:";
        TextBlockValue3 = "?Enter open key n";
        IsTextBoxValue4Visible = false;
        IsTextBoxValue5Visible = false;
        IsTextBoxCloseKey3Visible = false;
    }
    private void ECCUI()
    {
        MethodName = "ECC Encryption";
        TextBlockValue1 = "Enter close key d:";
        TextBlockValue3 = "?Enter open key";
        IsElement2Visible = false;
        IsTextBoxValue4Visible = false;
        IsTextBoxValue5Visible = false;
        IsTextBoxCloseKey2Visible = false;
        IsTextBoxCloseKey3Visible = false;
    }
    private void LFSRUI()
    {
        MethodName = "LFSR Encryption";
        IsButtonDigitalVisible = false;
        TextBlockValue1 = "Enter the length of the register (1-64):";
        TextBlockValue2 = "Enter seed (hex):";
        TextBlockValue3 = "Enter tapMask (hex):";
        IsTextBlockCloseKeyVisible = false;
        IsTextBoxValue4Visible = false;
        IsTextBoxValue5Visible = false;
        IsTextBoxCloseKey1Visible = false;
        IsTextBoxCloseKey2Visible = false;
        IsTextBoxCloseKey3Visible = false;
    }
    private void G28147UI()
    {
        MethodName = "G28147 Encryption";
        IsButtonDigitalVisible = false;
        TextBlockValue1 = "Enter the encryption key (32 bytes in hex):";   // ключ
        TextBlockValue2 = "Enter the initialization vector (8 bytes in hex):"; // IV
        Mode = new List<string> { "ECB", "GAMMA", "CFB" };
        SelectedMode = Mode[0];
        IsTextBlock3Visible = false;
        IsTextBlockCloseKeyVisible = false;
        IsTextBoxValue3Visible = false;
        IsTextBoxValue4Visible = false;
        IsTextBoxValue5Visible = false;
        IsTextBoxCloseKey1Visible = false;
        IsTextBoxCloseKey2Visible = false;
        IsTextBoxCloseKey3Visible = false;
    }
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
    public ICommand CloseCommand => _closeCommand ??= new OtherRelayCommands(ExecuteCloseCommand, CanExecute);
    public ICommand MinimizeCommand => _minimizeCommand ??= new OtherRelayCommands(ExecuteMinimizeCommand, CanExecute);
    public ICommand MaximizeCommand => _maximizeCommand ??= new OtherRelayCommands(ExecuteMaximizeCommand, CanExecute);
    public ICommand EncryptionCommand => _encryptionCommand ??= new OtherRelayCommands(ExecuteEncryptionCommand, CanExecute);
    public ICommand DigitalSignatureCommand => _digitalSignatureCommand ??= new OtherRelayCommands(ExecuteDigitalSignatureCommand, CanExecute);
    public ICommand DecryptionCommand => _decryptionCommand ??= new OtherRelayCommands(ExecuteDecryptionCommand, CanExecute);
    private void ExecuteCloseCommand(object? parameter)
    {
        CloseRequested?.Invoke();
    }
    private void ExecuteMinimizeCommand(object? parameter)
    {
        MinimizeRequested?.Invoke();
    }
    private void ExecuteMaximizeCommand(object? parameter)
    {
        MaximizeRequested?.Invoke();
    }
    private void ExecuteEncryptionCommand(object? parameter)
    {
        var FileText = _dataStorage.GetTextCallback();
        EncryptionModel model;
        EncryptionModel result;
        switch (SelectedMethod)
        {
            case EncryptionMethod.RSA:
                model = new EncryptionModel
                {
                    FileText = FileText,
                    PrimeP = TextBoxValue1,
                    PrimeQ = TextBoxValue2,
                    PrimeE = TextBoxValue3,
                    ModulusN = TextBoxValue4

                };
                result = _rsaService.Encryption(model);
                TextBlockCloseKey = $"Close key: {result.CloseKeyD},{result.ModulusN}";
                TextBlockOpenKey = $"Open key: {result.PrimeE},{result.ModulusN}";
                _dataStorage.PushUpdatedText(result.FileText);
                break;
            case EncryptionMethod.Elgamal:
                if (string.IsNullOrEmpty(TextBoxValue1) && string.IsNullOrEmpty(TextBoxValue2))
                {
                    model = new EncryptionModel
                    {
                        FileText = FileText,
                        PrimeP = TextBoxValue5,
                        PrimeQ = TextBoxValue4,
                        PrimeE = TextBoxValue3,
                    };
                }
                else
                {
                    model = new EncryptionModel
                    {
                        FileText = FileText,
                        PrimeP = TextBoxValue1,
                        PrimeQ = TextBoxValue2,
                        PrimeE = TextBoxValue3,
                    };
                }
                result = _elgamalService.Encryption(model);
                TextBlockCloseKey = $"{result.CloseKeyD},{result.PrimeP},{result.PrimeQ}";
                TextBlockOpenKey = $"{result.PrimeE},{result.PrimeQ},{result.PrimeP}";
                _dataStorage.PushUpdatedText(result.FileText);
                break;
            case EncryptionMethod.Rabina:
                model = new EncryptionModel
                {
                    FileText = FileText,
                    PrimeP = TextBoxValue1,
                    PrimeQ = TextBoxValue2,
                    ModulusN = TextBoxValue3

                };
                result = _rabinaService.Encryption(model);
                if (result.Signature == false)
                {
                    _fileDialog.ShowMessage("Одно из чисел не удовлетворяет условия 3 mod4, введите другое", "Warning");
                    return;
                }

                TextBlockCloseKey = $"Close key: {result.PrimeP},{result.PrimeQ}";
                TextBlockOpenKey = $"Open key: {result.ModulusN}";
                _dataStorage.PushUpdatedText(result.FileText);
                break;
            case EncryptionMethod.ECC:
                model = new EncryptionModel
                {
                    FileText = FileText,
                    CloseKeyD = TextBoxValue1,
                    PrimeE = TextBoxValue3,
                };
                result = _eccService.Encryption(model);
                TextBoxValue3 = result.PrimeE;
                _dataStorage.PushUpdatedText(result.FileText);
                break;
            case EncryptionMethod.LFSR:
                model = new EncryptionModel
                {
                    FileText = FileText,
                    PrimeP = TextBoxValue1, //Length of the register
                    PrimeQ = TextBoxValue2, //Seed
                    PrimeE = TextBoxValue3, //tapMask
                };
                result = _lFSRService.Encryption(model);
                _dataStorage.PushUpdatedText(result.FileText);
                break;
            case EncryptionMethod.G28147:
                model = new EncryptionModel
                {
                    FileText = FileText,
                    PrimeP = TextBoxValue1, //Key HEX 64
                    PrimeQ = TextBoxValue2, //IV
                    PrimeE = SelectedMode, //Mode
                };
                result = _g28147Service.Encryption(model);
                _dataStorage.PushUpdatedText(result.FileText);
                break;
            default:
                break;
        }
    }
    private void ExecuteDigitalSignatureCommand(object? parameter)
    {
        var FileText = _dataStorage.GetTextCallback();
        EncryptionModel model;
        EncryptionModel result;
        switch (SelectedMethod)
        {
            case EncryptionMethod.RSA:
                model = new EncryptionModel
                {
                    FileText = FileText,
                    PrimeP = TextBoxValue1,
                    PrimeQ = TextBoxValue2,
                    PrimeE = TextBoxValue3,
                    ModulusN = TextBoxValue4
                };
                result = _rsaService.Signature(model);
                if (string.IsNullOrEmpty(result.FileText))
                {
                    switch (result.Signature)
                    {
                        case true:
                            _fileDialog.ShowMessage("DigitalSignature valid", "Result Signature");
                            break;
                        case false:
                            _fileDialog.ShowMessage("DigitalSignature invalid", "Result Signature");
                            break;
                    }
                }
                else
                {
                    _dataStorage.PushUpdatedText(result.FileText);
                }
                break;
            case EncryptionMethod.Elgamal:
                break;
            case EncryptionMethod.Rabina:
                break;
            case EncryptionMethod.ECC:
                model = new EncryptionModel
                {
                    FileText = FileText,
                    PrimeE = TextBoxValue3,
                    CloseKeyD = TextBoxValue1
                };
                result = _eccService.Signature(model);
                TextBoxValue3 = result.PrimeE;
                if (string.IsNullOrEmpty(result.FileText))
                {
                    switch (result.Signature)
                    {
                        case true:
                            _fileDialog.ShowMessage("DigitalSignature valid", "Result Signature");
                            break;
                        case false:
                            _fileDialog.ShowMessage("DigitalSignature invalid", "Result Signature");
                            break;
                    }
                }
                else
                {
                    _dataStorage.PushUpdatedText(result.FileText);
                }
                break;
            default:
                break;
        }
    }
    private void ExecuteDecryptionCommand(object? parameter)
    {
        var FileText = _dataStorage.GetTextCallback();
        EncryptionModel model;
        EncryptionModel result;
        switch (SelectedMethod)
        {
            case EncryptionMethod.RSA:
                model = new EncryptionModel
                {
                    FileText = FileText,
                    CloseKeyD = TextBoxCloseKey1,
                    ModulusN = TextBoxCloseKey2

                };
                result = _rsaService.Decryption(model);
                _dataStorage.PushUpdatedText(result.FileText);
                break;
            case EncryptionMethod.Elgamal:
                model = new EncryptionModel
                {
                    FileText = FileText,
                    CloseKeyD = TextBoxCloseKey1,
                    PrimeP = TextBoxCloseKey2,
                    PrimeQ = TextBoxCloseKey3

                };
                result = _elgamalService.Decryption(model);
                _dataStorage.PushUpdatedText(result.FileText);
                break;
            case EncryptionMethod.Rabina:
                model = new EncryptionModel
                {
                    FileText = FileText,
                    PrimeP = TextBoxCloseKey1,
                    PrimeQ = TextBoxCloseKey2

                };
                result = _rabinaService.Decryption(model);
                _dataStorage.PushUpdatedText(result.FileText);
                break;
            case EncryptionMethod.ECC:
                model = new EncryptionModel
                {
                    FileText = FileText,
                    CloseKeyD = TextBoxCloseKey1

                };
                result = _eccService.Decryption(model);
                _dataStorage.PushUpdatedText(result.FileText);
                break;
            case EncryptionMethod.LFSR:
                model = new EncryptionModel
                {
                    FileText = FileText,
                    PrimeP = TextBoxValue1, //Length of the register
                    PrimeQ = TextBoxValue2, //Seed
                    PrimeE = TextBoxValue3, //tapMask
                };
                result = _lFSRService.Decryption(model);
                _dataStorage.PushUpdatedText(result.FileText);
                break;
            case EncryptionMethod.G28147:
                model = new EncryptionModel
                {
                    FileText = FileText,
                    PrimeP = TextBoxValue1, //Key HEX 64
                    PrimeQ = TextBoxValue2, //IV
                    PrimeE = SelectedMode, //Mode
                };
                result = _g28147Service.Decryption(model);
                _dataStorage.PushUpdatedText(result.FileText);
                break;
            default:
                break;
        }
    }
    private bool CanExecute(object? parameter) => true;

}