using Domain.IService;

namespace Service;

public class ServiceFunctions : IServiceFunctions
{
    public void LogMessage(string message)
    {
        string logFilePath = "D:\\VIsual Studio\\VS project\\NotePad_Launcher\\NotePad_Launcher\\bin\\Debug\\net8.0-windows\\Documents\\log.txt";
        File.AppendAllText(logFilePath, DateTime.Now + ": " + message + Environment.NewLine);
    }
}