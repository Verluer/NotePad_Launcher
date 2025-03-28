namespace Domain;

public class Settings
{
    public static char[] UkrainianAlphabet = new char[]
    {
        'А', 'Б', 'В', 'Г', 'Ґ', 'Д', 'Е', 'Є', 'Ж', 'З',
        'И', 'І', 'Ї', 'Й', 'К', 'Л', 'М', 'Н', 'О', 'П',
        'Р', 'С', 'Т', 'У', 'Ф', 'Х', 'Ц', 'Ч', 'Ш', 'Щ',
        'Ь', 'Ю', 'Я', '.', '_', '-', ' ', '#'
    };
    public void LogMessage(string message)
    {
        string logFilePath = "D:\\VIsual Studio\\VS project\\NotePad_Launcher\\NotePad_Launcher\\bin\\Debug\\net8.0-windows\\Documents";
        File.AppendAllText(logFilePath, DateTime.Now + ": " + message + Environment.NewLine);
    }
}