using System.Security.Cryptography.X509Certificates;
using Domain.Model;

namespace Domain.IService
{
    public interface IFileService
    {
        public FileModel OpenFile(string pathFile);
        public string ExDirectoryFile();
        public FileModel CreateFile();
        public FileModel SaveFile(FileModel model);
        public List<FileModel> GetTextFiles();
        public void DeleteFile(string filePath);
        public bool CheckTextChange(string pathFile, string fileText);
        public void WriteAllText(string pathFile, string fileText);
        public string GetFileName(string pathFile);
        public bool FileExists(string pathFile);
    }
}
