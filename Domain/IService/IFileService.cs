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
    }
}
