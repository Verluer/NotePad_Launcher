using NotePad_Launcher.Contracts;

namespace NotePad_Launcher.Services;

public class StringService : IStringService
{
    private string _myFileText;

    public string MyFileText
    {
        get => _myFileText;
        set
        {
            if (_myFileText != value)
            {
                _myFileText = value;
                StringUpdated?.Invoke(_myFileText);
            }
        }
    }
    public event Action<string>? StringUpdated;
}