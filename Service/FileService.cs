using System.Reflection;
using System.Text;
using Domain.IService;
using Domain.Model;

// Пространство имен WPF

namespace Service
{
    public class FileService : IFileService
    {
        private readonly IConfigService _configService;
        private AppConfigModel _config;

        public FileService(IConfigService configService)
        {
            _configService = configService;
            _config = _configService.Load();
        }
        public string ExDirectoryFile()
        {
            string exePath = Assembly.GetExecutingAssembly().Location; //Полный путь к исполняемому файлу
            string exeDirectory = Path.GetDirectoryName(exePath); //Извлечение директории
            string dataFilePath = Path.Combine(exeDirectory, "Documents");
            string directoryPath = Path.GetFullPath(dataFilePath); 
            return directoryPath;
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

        public FileModel CreateFile()
        {
            string FileName = "New Text File.txt";
            string CreateFileInDirectory = Path.Combine(_config.DocsPath, FileName);
            if (File.Exists(CreateFileInDirectory))
            {
                int i = 1;
                while (File.Exists(CreateFileInDirectory))
                {
                    FileName = $"New Text File({i}).txt";
                    CreateFileInDirectory = Path.Combine(_config.DocsPath, FileName);
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

        public FileModel SaveFile(FileModel model)
        {
            string newFilePath = null;
            if (model.FilePath != null)
            {
                newFilePath = Path.Combine(Path.GetDirectoryName(model.FilePath), model.FileName + ".txt");
                if (model.FilePath != newFilePath)
                {
                    File.Delete(model.FilePath);
                }
                File.WriteAllText(newFilePath, model.FileText);
            }
            else
            {
                newFilePath = Path.Combine(_config.DocsPath, model.FileName + ".txt");
                if (File.Exists(newFilePath))
                {
                    int i = 1;
                    while (File.Exists(newFilePath))
                    {
                        model.FileName = $"{model.FileName}({i}).txt";
                        newFilePath = Path.Combine(_config.DocsPath, model.FileName);
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
        public List<FileModel> GetTextFiles()
        {
            if (!Directory.Exists(_config.DocsPath))
                throw new DirectoryNotFoundException($"Директория не найдена: {_config.DocsPath}");

            return Directory.GetFiles(_config.DocsPath, "*.txt")
                .Select(file => new FileModel
                {
                    FileName = Path.GetFileNameWithoutExtension(file),
                    FileText = File.ReadAllText(file),
                    FilePath = file,
                }).ToList();
        }

        public void DeleteFile(string filePath)
        {
            File.Delete(filePath);
        }

        public bool CheckTextChange(string pathFile, string fileText)
        {
            if (string.IsNullOrEmpty(pathFile))
                return true;

            var checkTextFromFile = File.ReadAllText(pathFile, Encoding.UTF8);
            return checkTextFromFile == fileText;
        }

        public void WriteAllText(string pathFile, string fileText)
        {
            File.WriteAllText(pathFile, fileText);
        }

        public string GetFileNameWithout(string pathFile)
        {
            return Path.GetFileNameWithoutExtension(pathFile);
        }
        public string GetFileName(string pathFile)
        {
            return Path.GetFileName(pathFile);
        }

        public bool FileExists(string pathFile)
        {
            return File.Exists(pathFile);
        }
    }
}
