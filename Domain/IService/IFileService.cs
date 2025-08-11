using System.Security.Cryptography.X509Certificates;
using Domain.Model;

namespace Domain.IService
{
    public interface IFileService
    {
        public FileModel OpenFile(string pathFile);
        public string ExDirectoryFile(string Folder);
        public void CreateDocumentsDirectory();
        public FileModel CreateFile(string DocsPath, string FileName);
        public FileModel SaveFile(FileModel model, string SaveSetting, string DocsPath);
        public List<FileModel> GetTextFiles(string DocsPath);
        public List<string> LoadFolderFile(string DocsPath);
        public List<string> FindLocalization();
        public void DeleteFile(string filePath);
        public bool CheckTextChange(string pathFile, string fileText);
        public void WriteAllText(string pathFile, string fileText);
        public bool FileExists(string pathFile);
    }
}
