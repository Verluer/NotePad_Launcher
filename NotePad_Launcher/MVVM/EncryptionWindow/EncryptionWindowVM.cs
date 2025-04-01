using Domain.IService.IEncryption;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Domain.Enum;
using Service.Encryption;
using NotePad_Launcher.Contracts;
using System.Windows;
using System.Windows.Forms;

namespace NotePad_Launcher.ViewModels.EncryptionWindow;

public class EncryptionWindowVM : INotifyPropertyChanged
{
    private EncryptionMethod SelectedMethod;
    private readonly IRSAService _rsaService;
    private readonly IElgamalService _elgamalService;
    private readonly IRabinaService _rabinaService;
    private readonly IECCService _eccService;
    public event PropertyChangedEventHandler? PropertyChanged;
    private readonly IStringService _stringService;
    private string _methodName;
    public string MethodName
    {
        get => _methodName;
        set
        {
            _methodName = value;
            OnPropertyChanged(nameof(MethodName));
        }
    }
    private string _textBlockValue1;
    public string TextBlockValue1
    {
        get => _textBlockValue1;
        set
        {
            _textBlockValue1 = value;
            OnPropertyChanged(nameof(TextBlockValue1));
        }
    }
    private string _textBlockValue2;
    public string TextBlockValue2
    {
        get => _textBlockValue2;
        set
        {
            _textBlockValue2 = value;
            OnPropertyChanged(nameof(TextBlockValue2));
        }
    }
    private string _textBlockValue3;
    public string TextBlockValue3
    {
        get => _textBlockValue3;
        set
        {
            _textBlockValue3 = value;
            OnPropertyChanged(nameof(TextBlockValue3));
        }
    }
    private string _textBlockValue4;
    public string TextBlockValue4
    {
        get => _textBlockValue4;
        set
        {
            _textBlockValue4 = value;
            OnPropertyChanged(nameof(TextBlockValue4));
        }
    }
    private string _textBlockValue5;
    public string TextBlockValue5
    {
        get => _textBlockValue5;
        set
        {
            _textBlockValue1 = value;
            OnPropertyChanged(nameof(TextBlockValue5));
        }
    }
    private bool _isElement2Visible = true;

    public bool IsElement2Visible
    {
        get => _isElement2Visible;
        set
        {
            if (_isElement2Visible != value)
            {
                _isElement2Visible = value;
                OnPropertyChanged(nameof(IsElement2Visible)); // Уведомляем об изменении
            }
        }
    }
    private bool _isTextBlock3Visible = true;

    public bool IsTextBlock3Visible
    {
        get => _isTextBlock3Visible;
        set
        {
            if (_isTextBlock3Visible != value)
            {
                _isTextBlock3Visible = value;
                OnPropertyChanged(nameof(IsTextBlock3Visible)); // Уведомляем об изменении
            }
        }
    }
    private bool _isTextBoxValue3Visible = true;

    public bool IsTextBoxValue3Visible
    {
        get => _isTextBoxValue3Visible;
        set
        {
            if (_isTextBoxValue3Visible != value)
            {
                _isTextBoxValue3Visible = value;
                OnPropertyChanged(nameof(IsTextBoxValue3Visible)); // Уведомляем об изменении
            }
        }
    }
    private bool _isTextBoxValue4Visible = true;

    public bool IsTextBoxValue4Visible
    {
        get => _isTextBoxValue4Visible;
        set
        {
            if (_isTextBoxValue4Visible != value)
            {
                _isTextBoxValue4Visible = value;
                OnPropertyChanged(nameof(IsTextBoxValue4Visible)); // Уведомляем об изменении
            }
        }
    }
    private bool _isTextBoxValue5Visible = true;

    public bool IsTextBoxValue5Visible
    {
        get => _isTextBoxValue5Visible;
        set
        {
            if (_isTextBoxValue5Visible != value)
            {
                _isTextBoxValue5Visible = value;
                OnPropertyChanged(nameof(IsTextBoxValue5Visible)); // Уведомляем об изменении
            }
        }
    }
    private bool _isTextBoxCloseKey2Visible = true;

    public bool IsTextBoxCloseKey2Visible
    {
        get => _isTextBoxCloseKey2Visible;
        set
        {
            if (_isTextBoxCloseKey2Visible != value)
            {
                _isTextBoxCloseKey2Visible = value;
                OnPropertyChanged(nameof(IsTextBoxCloseKey2Visible)); // Уведомляем об изменении
            }
        }
    }
    private bool _isTextBoxCloseKey3Visible = true;

    public bool IsTextBoxCloseKey3Visible
    {
        get => _isTextBoxCloseKey3Visible;
        set
        {
            if (_isTextBoxCloseKey3Visible != value)
            {
                _isTextBoxCloseKey3Visible = value;
                OnPropertyChanged(nameof(IsTextBoxCloseKey3Visible)); // Уведомляем об изменении
            }
        }
    }
    public EncryptionWindowVM(EncryptionMethod method)
    {
        SelectedMethod = method;
        _rsaService = new RSAService();
        _elgamalService = new ElgamalService();
        _rabinaService = new RabinaService();
        _eccService = new ECCService();
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
            default:
                // Действие, если метод не выбран (None)
                break;
        }
    }
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
}