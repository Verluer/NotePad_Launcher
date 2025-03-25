using System.IO.Enumeration;
using System.Reflection;
using System.Text;
using Domain.IService;
using Domain.Model;
using System.Windows; // Пространство имен WPF

namespace Service
{
    public class FileService : IFileService
    {
        private string directoryPath;


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

        public FileModel SaveFile(string pathFile, string fileName, string fileText)
        {
            string newFilePath = string.Empty;
            if (pathFile != null)
            {
                newFilePath = Path.Combine(Path.GetDirectoryName(pathFile), fileName + ".txt");
                if (pathFile != newFilePath)
                {
                    // Удаляем старый файл
                    File.Delete(pathFile);
                }
                File.WriteAllText(newFilePath, fileText);
            }
            else
            {
                newFilePath = Path.Combine(directoryPath, fileName + ".txt");
                File.WriteAllText(newFilePath, fileText);
            }

            return new FileModel
            {
                FilePath = newFilePath
            };

        }
    }
}
