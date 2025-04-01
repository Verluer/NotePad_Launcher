using NotePad_Launcher.Contracts;

namespace NotePad_Launcher.Services;

public class StringService : IStringService
{
    public Func<string> GetTextCallback { get; set; }

    public event Action<string> TextUpdated;

    public void PushUpdatedText(string updatedText)
    {
        TextUpdated?.Invoke(updatedText);
    }
}