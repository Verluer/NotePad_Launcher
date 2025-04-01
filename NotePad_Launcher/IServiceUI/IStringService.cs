namespace NotePad_Launcher.Contracts;

public interface IStringService
{
    Func<string> GetTextCallback { get; set; }
    event Action<string> TextUpdated;

    void PushUpdatedText(string updatedText);
}