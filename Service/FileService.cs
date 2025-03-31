using System.Reflection;
using System.Text;
using Domain.IService;
using Domain.Model;

// Пространство имен WPF

namespace Service
{
    public class FileService : IFileService
    {
        private string directoryPath = string.Empty;


        public string ExDirectoryFile()
        {
            string exePath = Assembly.GetExecutingAssembly().Location; //Полный путь к исполняемому файлу
            string exeDirectory = Path.GetDirectoryName(exePath); //Извлечение директории
            string dataFilePath = Path.Combine(exeDirectory, "Documents");
            directoryPath = Path.GetFullPath(dataFilePath); //Директория текстовых файлов
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
            string CreateFileInDirectory = Path.Combine(directoryPath, FileName);
            if (File.Exists(CreateFileInDirectory))
            {
                int i = 1;
                while (File.Exists(CreateFileInDirectory))
                {
                    FileName = $"New Text File({i}).txt";
                    CreateFileInDirectory = Path.Combine(directoryPath, FileName);
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
                newFilePath = Path.Combine(directoryPath, model.FileName + ".txt");
                File.WriteAllText(newFilePath, model.FileText);
            }
            return new FileModel
            {
                FilePath = newFilePath
            };

        }
        public List<FileModel> GetTextFiles()
        {
            if (!Directory.Exists(directoryPath))
                throw new DirectoryNotFoundException($"Директория не найдена: {directoryPath}");

            return Directory.GetFiles(directoryPath, "*.txt")
                .Select(file => new FileModel
                {
                    FileName = Path.GetFileNameWithoutExtension(file),
                    FilePath = file,
                }).ToList();
        }

        public void DeleteFile(string filePath)
        {
            File.Delete(filePath);
        }

        public bool CheckTextChange(string pathFile, string fileText)
        {
            var checkTextFromFile = File.ReadAllText(pathFile, Encoding.UTF8);
            return checkTextFromFile == fileText;
        }

        public void WriteAllText(string pathFile, string fileText)
        {
            File.WriteAllText(pathFile, fileText);
        }

        public string GetFileName(string pathFile)
        {
            return Path.GetFileNameWithoutExtension(pathFile);
        }

        public bool FileExists(string pathFile)
        {
            return File.Exists(pathFile);
        }
    }
}
