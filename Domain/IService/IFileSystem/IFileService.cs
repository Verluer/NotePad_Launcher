using System.Security.Cryptography.X509Certificates;
using Domain.Model;

namespace Domain.IService.IFileSystem
{
    public interface IFileService
    {
        public FileModel OpenFile(string pathFile);
        public FileModel CreateFile(string DocsPath, string FileName);
        public FileModel SaveFile(FileModel model, string SaveSetting, string DocsPath);
        public void LogMessage(string message, string DocPath);
    }
}
