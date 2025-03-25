using Domain.Model;

namespace Domain.IService
{
    public interface IFileService
    {
        public FileModel OpenFile(string pathFile);
        string ExDirectoryFile();
        public FileModel CreateFile();
        public FileModel SaveFile(string pathFile, string fileName, string fileText);
    }
}
