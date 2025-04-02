
namespace NotePad_Launcher;

public class StringService : IStringService
{
    public Func<string> GetTextCallback { get; set; }

    public event Action<string> TextUpdated;

    public void PushUpdatedText(string updatedText)
    {
        TextUpdated?.Invoke(updatedText);
    }
}