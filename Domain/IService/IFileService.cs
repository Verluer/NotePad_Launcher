using Domain.Model;

namespace Domain.IService
{
    public interface IFileService
    {
        public OpenFileModel OpenFile(string pathFile);
    }
}
