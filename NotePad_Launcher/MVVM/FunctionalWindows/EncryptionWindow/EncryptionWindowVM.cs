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
    private readonly IAESService _aESService;
    private readonly IKEK_SSK _kEK_SSKService;
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
    private ICommand? _generateCommand;
    private ICommand? _getPath1Command;
    private ICommand? _getPath2Command;
    private ICommand? _getPath3Command;
    private ICommand? _getPath4Command;
    private ICommand? _getPath5Command;
    private ICommand? _getPath6Command;

    private string _methodName;
    private string pathPrivateKeyEnRSA;
    private string pathPublicKeyEnRSA;
    private string pathPrivateKeySigRSA;
    private string pathPublicKeySigRSA;
    private string pathMetaData;

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
        _aESService = new AESService();
        _kEK_SSKService = new KEK_SSK();
      
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
            case EncryptionMethod.AES:
                AESUI();
                break;
            case EncryptionMethod.KEK_SSK:
                KEK_SSK();
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
    private string _textBlockValue6;
    public string TextBlockValue6
    {
        get => _textBlockValue6;
        set => SetField(ref _textBlockValue6, value);
    }
    private string _textBlockValue7;
    public string TextBlockValue7
    {
        get => _textBlockValue7;
        set => SetField(ref _textBlockValue7, value);
    }
    private string _textBlockValue8;
    public string TextBlockValue8
    {
        get => _textBlockValue8;
        set => SetField(ref _textBlockValue8, value);
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
    private bool _isComboBoxVisible = false;
    public bool IsComboBoxVisible
    {
        get => _isComboBoxVisible;
        set => SetField(ref _isComboBoxVisible, value);
    }
    private bool _isButtonDigitalVisible = true;
    public bool IsButtonDigitalVisible
    {
        get => _isButtonDigitalVisible;
        set => SetField(ref _isButtonDigitalVisible, value);
    }
    private bool _isButtonValue3Visible = false;
    public bool IsButtonValue3Visible
    {
        get => _isButtonValue3Visible;
        set => SetField(ref _isButtonValue3Visible, value);
    }
    private bool _isElement4Visible = false;
    public bool IsElement4Visible
    {
        get => _isElement4Visible;
        set => SetField(ref _isElement4Visible, value);
    }
    private bool _isElement5Visible = false;
    public bool IsElement5Visible
    {
        get => _isElement5Visible;
        set => SetField(ref _isElement5Visible, value);
    }
    private bool _isElement6Visible = false;
    public bool IsElement6Visible
    {
        get => _isElement6Visible;
        set => SetField(ref _isElement6Visible, value);
    }
    private bool _isButtonGenerateVisible = false;
    public bool IsButtonGenerateVisible
    {
        get => _isButtonGenerateVisible;
        set => SetField(ref _isButtonGenerateVisible, value);
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
        IsComboBoxVisible = true;
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
    private void AESUI()
    {
        MethodName = "AES Encryption";
        IsButtonDigitalVisible = false;
        TextBlockValue1 = "Enter password (optional)";
        TextBlockValue2 = "Enter a name for the encryption files";
        TextBlockValue3 = "Select where to save encrypted files";
        TextBlockValue4 = "Select a private key (rec) for decryption (.pem)";
        TextBlockValue5 = "Select a public key (sig) for decryption (.pem)";
        TextBlockValue6 = "Select a MetaData (.json)";
        TextBlockValue7 = "Select a public key (rec) for encryption (.pem)";
        TextBlockValue8 = "Select a private key (sig) for encryption (.pem)";
        IsButtonValue3Visible = true;
        IsElement4Visible = true;
        IsElement5Visible = true;
        IsElement6Visible = true;
        IsButtonGenerateVisible = true;
        IsTextBoxValue4Visible = false;
        IsTextBoxValue5Visible = false;
        IsTextBlockCloseKeyVisible = false;
        IsTextBoxCloseKey1Visible = false;
        IsTextBoxCloseKey2Visible= false;
        IsTextBoxCloseKey3Visible = false;
    }
    private void KEK_SSK()
    {
        MethodName = "KEK-SSK Encryption";
        TextBlockValue1 = "Enter master Key (HEX):";
        IsElement2Visible = false;
        IsTextBlock3Visible = false;
        IsTextBoxValue3Visible = false;
        IsTextBoxValue4Visible = false;
        IsTextBoxCloseKey3Visible = false;
        IsTextBoxCloseKey2Visible = false;
        IsTextBoxValue5Visible = false;
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
    public ICommand GenerateCommand => _generateCommand ??= new OtherRelayCommands(ExecuteGenerateCommand, CanExecute);
    public ICommand GetPath1Command => _getPath1Command ??= new OtherRelayCommands(ExecuteGetPath1Command, CanExecute);
    public ICommand GetPath2Command => _getPath2Command ??= new OtherRelayCommands(ExecuteGetPath2Command, CanExecute);
    public ICommand GetPath3Command => _getPath3Command ??= new OtherRelayCommands(ExecuteGetPath3Command, CanExecute);
    public ICommand GetPath4Command => _getPath4Command ??= new OtherRelayCommands(ExecuteGetPath4Command, CanExecute);
    public ICommand GetPath5Command => _getPath5Command ??= new OtherRelayCommands(ExecuteGetPath5Command, CanExecute);
    public ICommand GetPath6Command => _getPath6Command ??= new OtherRelayCommands(ExecuteGetPath6Command, CanExecute);
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
    private void ExecuteGetPath1Command(object? parameter)
    {
        var owner = System.Windows.Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
        TextBoxValue3 = _fileDialog.FolderFileDialog(App.Config.DocsPath);
    }
    private void ExecuteGetPath2Command(object? parameter)
    {
        var owner = System.Windows.Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
        pathPrivateKeyEnRSA = _fileDialog.OpenTextFileDialog(App.Config.DocsPath, "pem");
        if (pathPrivateKeyEnRSA != null)
        {
            TextBlockValue4 = "Private key (rec) selected";
        }
    }
    private void ExecuteGetPath3Command(object? parameter)
    {
        var owner = System.Windows.Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
        pathPublicKeySigRSA = _fileDialog.OpenTextFileDialog(App.Config.DocsPath, "pem");
        if (pathPublicKeySigRSA != null)
        {
            TextBlockValue5 = "Publick key (sig) selected";
        }
    }
    private void ExecuteGetPath4Command(object? parameter)
    {
        var owner = System.Windows.Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
        pathMetaData = _fileDialog.OpenTextFileDialog(App.Config.DocsPath, "json");
        if (pathMetaData != null)
        {
            TextBlockValue6 = "MetaData selected";
        }
    }
    private void ExecuteGetPath5Command(object? parameter)
    {
        var owner = System.Windows.Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
        pathPublicKeyEnRSA = _fileDialog.OpenTextFileDialog(App.Config.DocsPath, "pem");
        if (pathPublicKeyEnRSA != null)
        {
            TextBlockValue7 = "Publick key (rec) selected";
        }
    }
    private void ExecuteGetPath6Command(object? parameter)
    {
        var owner = System.Windows.Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
        pathPrivateKeySigRSA = _fileDialog.OpenTextFileDialog(App.Config.DocsPath, "pem");
        if (pathPrivateKeySigRSA != null)
        {
            TextBlockValue8 = "Private key (sig) selected";
        }
    }
    private void ExecuteGenerateCommand(object? parameter)
    {
        if (TextBoxValue2 != null && TextBoxValue3 != null)
        {
            using var recipientRsa = _rsaService.CreateRsaKeyPair(2048, TextBoxValue3, "rec", TextBoxValue2); // RSA отримувача
            using var signerRsa = _rsaService.CreateRsaKeyPair(2048, TextBoxValue3, "sig", TextBoxValue2); // RSA підписанта
        }
        else _fileDialog.ShowMessage("Enter name and where save", "Error");
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
                TextBoxCloseKey1 = result.CloseKeyD;
                TextBoxCloseKey2 = result.ModulusN;
                TextBoxValue4 = result.ModulusN;
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
            case EncryptionMethod.AES:
                model = new EncryptionModel
                {
                    FileText = FileText,
                    PrimeP = TextBoxValue1, //password
                    PrimeQ = TextBoxValue2, //name-base
                    PrimeE = TextBoxValue3, //path
                };
                result = _aESService.Encryption(model, pathPublicKeyEnRSA, pathPrivateKeySigRSA);
                _aESService.SaveEncryptedBundle(TextBoxValue3, TextBoxValue2, result);
                _dataStorage.PushUpdatedText(result.FileText);
                break;
            case EncryptionMethod.KEK_SSK:
                model = new EncryptionModel
                {
                    FileText = FileText,
                    PrimeP = TextBoxValue1, //Master Key
                };
                result = _kEK_SSKService.Encryption(model);
                TextBoxCloseKey1 = result.CloseKeyD;
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
                    TextBlockCloseKey = $"Close key: {result.CloseKeyD},{result.ModulusN}";
                    TextBlockOpenKey = $"Open key: {result.PrimeE},{result.ModulusN}";
                    TextBlockValue4 = result.ModulusN;
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
            case EncryptionMethod.AES:
                model = new EncryptionModel
                {
                    FileText = FileText,
                    PrimeP = TextBoxValue1, //password
                    PrimeQ = TextBoxValue2, //name-base
                    PrimeE = TextBoxValue3, //path
                    Metadata = _aESService.LoadEncryptedBundle(pathMetaData),
                };
                result = _aESService.Decryption(model, pathPrivateKeyEnRSA, pathPublicKeySigRSA);
                _dataStorage.PushUpdatedText(result.FileText);
                break;
            case EncryptionMethod.KEK_SSK:
                model = new EncryptionModel
                {
                    FileText = FileText,
                    CloseKeyD = TextBoxCloseKey1, //SSK
                    PrimeP = TextBoxValue1,
                };
                result = _kEK_SSKService.Decryption(model);
                _dataStorage.PushUpdatedText(result.FileText);
                break;
            default:
                break;
        }
    }
    private bool CanExecute(object? parameter) => true;

}