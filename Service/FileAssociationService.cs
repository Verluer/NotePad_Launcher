using Microsoft.Win32;
using System.IO;
using System.Runtime.InteropServices;
using Domain.IService;

namespace Service;

public class FileAssociationService : IFileAssociationService
{
    public void RegisterTxtFileAssociation()
    {
        var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
        var exeName = Path.GetFileName(exePath);
        var directoryPath = Path.GetDirectoryName(exePath);

        var iconFilePath = FindIcon(directoryPath, "app.ico");
        var iconPath = iconFilePath ?? $"\"{exePath}\",0";

        const string extension = ".txt";
        const string fileType = "NotePad_Launcher.txtfile";

        try
        {
            using (var key = Registry.CurrentUser.CreateSubKey($@"Software\Classes\{extension}"))
            {
                key?.SetValue("", fileType);
                key?.SetValue("Content Type", "text/plain");
            }

            using (var classKey = Registry.CurrentUser.CreateSubKey($@"Software\Classes\{fileType}"))
            {
                classKey?.SetValue("", "Text Document");

                using (var iconKey = classKey?.CreateSubKey("DefaultIcon"))
                    iconKey?.SetValue("", iconPath);

                using (var commandKey = classKey?.CreateSubKey(@"shell\open\command"))
                    commandKey?.SetValue("", $"\"{exePath}\" \"%1\"");
            }

            using (var appKey = Registry.CurrentUser.CreateSubKey($@"Software\Classes\Applications\{exeName}"))
            {
                using (var shellKey = appKey?.CreateSubKey(@"shell\open\command"))
                    shellKey?.SetValue("", $"\"{exePath}\" \"%1\"");
            }

            SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
        }
        catch
        {
        }
    }

    public static string FindIcon(string baseDirectory, string iconFileName)
    {
        try
        {
            var iconFilePath = Directory.EnumerateFiles(baseDirectory, iconFileName, SearchOption.AllDirectories).FirstOrDefault();

            return iconFilePath;
        }
        catch
        {
            return null;
        }
    }

    [DllImport("shell32.dll")]
    private static extern void SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);

}