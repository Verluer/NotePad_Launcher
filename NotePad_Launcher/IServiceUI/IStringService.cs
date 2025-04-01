namespace NotePad_Launcher.Contracts;

public interface IStringService
{
    string MyFileText { get; set; }
    event Action<string> StringUpdated;
}