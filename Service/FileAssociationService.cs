using Microsoft.Win32;
using System.IO;
using System.Runtime.InteropServices;
using Domain.IService;

namespace Service;

public class FileAssociationService : IFileAssociationService
{
    public void RegisterTxtFileAssociation()
    {
        var exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        var directoryPath = Path.GetDirectoryName(exePath);

        var iconFilePath = FindIcon(directoryPath, "app.ico");

        var iconPath = iconFilePath ?? $"\"{exePath}\",0";

        const string extension = ".txt";
        const string fileType = "NotePad_Launcher.txtfile";
        var existingFileType = Registry.GetValue($"HKEY_CLASSES_ROOT\\{extension}", "", null);
        if (existingFileType == null || !existingFileType.ToString().Equals(fileType))
        {
            try
            {
                using (var key = Registry.ClassesRoot.CreateSubKey(extension))
                {
                    if (key == null) return;
                    key.SetValue("", fileType);
                    key.SetValue("Content Type", "text/plain");
                }

                using (var classKey = Registry.ClassesRoot.CreateSubKey(fileType))
                {
                    if (classKey == null) return;
                    classKey.SetValue("", "Text Document for NotePad_Launcher");

                    using (var iconKey = classKey.CreateSubKey("DefaultIcon"))
                    {
                        if (iconKey == null) return;
                        iconKey.SetValue("", iconPath);
                    }

                    using (var commandKey = classKey.CreateSubKey(@"shell\open\command"))
                    {
                        if (commandKey == null) return;
                        commandKey.SetValue("", $"\"{exePath}\" \"%1\"");
                    }
                }

                SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
            }
            catch
            {
                //
            }
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