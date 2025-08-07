using System.Diagnostics;
using System.Reflection;
using System.Text;
using Domain.IService;
using Domain.Model;

namespace Service
{
    public class FileService : IFileService
    {
        private readonly IConfigService _configService;
        public void LogMessage(string message, string DocsPath)
        {
            string logFilePath = Path.Combine(DocsPath, "log.txt");
            File.AppendAllText(logFilePath, DateTime.Now + ": " + message + Environment.NewLine);
        }
        public string ExDirectoryFile(string Folder)
        {

            var exePath = Assembly.GetExecutingAssembly().Location;
            var exeDirectory = Path.GetDirectoryName(exePath);
            var resourcesDirectory = "";
            var counter = 0;
            while (counter < 5)
            {
                resourcesDirectory = Path.Combine(exeDirectory, Folder);
                if (!Directory.Exists(resourcesDirectory))
                {
                    exeDirectory = Directory.GetParent(exeDirectory).FullName;
                }
                else
                {
                    break;
                }
            }
            return resourcesDirectory;
        }
        public void CreateDocumentsDirectory()
        {
            var exePath = Assembly.GetExecutingAssembly().Location;
            var exeDirectory = Path.GetDirectoryName(exePath);
            var documentsDirect = Path.Combine(exeDirectory, "Documents");
            var subdirectory = Path.Combine(documentsDirect, "Main");
            Directory.CreateDirectory(documentsDirect);
            Directory.CreateDirectory(subdirectory);
        }
        public FileModel OpenFile(string pathFile)
        {
            return new FileModel
            {
                FileName = Path.GetFileNameWithoutExtension(pathFile),
                FileText = File.ReadAllText(pathFile, Encoding.UTF8),
                FilePath = pathFile
            };

        }

        public FileModel CreateFile(string DocsPath, string FileName)
        {
            FileName = $"{FileName}.txt";
            string CreateFileInDirectory = Path.Combine(DocsPath, FileName);
            if (File.Exists(CreateFileInDirectory))
            {
                int i = 1;
                while (File.Exists(CreateFileInDirectory))
                {
                    FileName = $"New Text File({i}).txt";
                    CreateFileInDirectory = Path.Combine(DocsPath, FileName);
                    i++;
                    if (!File.Exists(CreateFileInDirectory))
                    {
                        break;
                    }
                }
            }
            using (FileStream fs = File.Create(CreateFileInDirectory))
            {
                fs.Close();
            }
            return new FileModel
            {
                FileName = Path.GetFileNameWithoutExtension(FileName),
                FilePath = CreateFileInDirectory,
            };
        }

        public FileModel SaveFile(FileModel model, string SaveSetting, string DocsPath)
        {
            string newFilePath = null;
            if (model.FilePath != null)
            {
                if (SaveSetting == "SaveNormal")
                {
                    newFilePath = Path.Combine(Path.GetDirectoryName(model.FilePath), model.FileName + ".txt");
                    if (model.FilePath != newFilePath)
                    {
                        File.Delete(model.FilePath);
                    }
                    File.WriteAllText(newFilePath, model.FileText);
                }
                else if (SaveSetting == "SaveDirectory")
                {
                    newFilePath = Path.Combine(DocsPath, model.FileName + ".txt");
                    if (model.FilePath != newFilePath)
                    {
                        File.Delete(model.FilePath);
                    }
                    File.WriteAllText(newFilePath, model.FileText);
                }

            }   
            else
            {
                newFilePath = Path.Combine(DocsPath, model.FileName + ".txt");
                    if (File.Exists(newFilePath))
                    {
                        int i = 1;
                        while (File.Exists(newFilePath))
                        {
                            model.FileName = $"{model.FileName}({i}).txt";
                            newFilePath = Path.Combine(DocsPath, model.FileName);
                            i++;
                            if (!File.Exists(newFilePath))
                            {
                                break;
                            }
                        }
                    }
                    File.WriteAllText(newFilePath, model.FileText);
                }
                return new FileModel
                {
                    FilePath = newFilePath
                };
        }
        public List<FileModel> GetTextFiles(string DocsPath)
        {
            if (!Directory.Exists(DocsPath))
                throw new DirectoryNotFoundException($"Директория не найдена: {DocsPath}");

            return Directory.GetFiles(DocsPath, "*.txt")
                .Select(file => new FileModel
                {
                    FileName = Path.GetFileNameWithoutExtension(file),
                    FileText = File.ReadAllText(file),
                    FilePath = file,
                }).ToList();
        }
        public List<string> LoadFolderFile(string DocsPath)
        {
            string[] folderPath = Directory.GetDirectories(DocsPath);
            List<string> folderNames = new List<string>();
            foreach (var path in folderPath)
            {
                folderNames.Add(Path.GetFileName(path));
            }
            return folderNames;
        }
        public void DeleteFile(string filePath)
        {
            File.Delete(filePath);
        }

        public bool CheckTextChange(string pathFile, string fileText)
        {
            if (string.IsNullOrEmpty(pathFile) || !File.Exists(pathFile))
                return true;

            var checkTextFromFile = File.ReadAllText(pathFile, Encoding.UTF8);
            return checkTextFromFile == fileText;
        }

        public void WriteAllText(string pathFile, string fileText)
        {
            File.WriteAllText(pathFile, fileText);
        }
        public bool FileExists(string pathFile)
        {
            return File.Exists(pathFile);
        }
        
    }
}
