using System.Text;
using Domain.IService;
using Domain.Model;

namespace Service
{
    public class FileService:IFileService
    {
        public OpenFileModel OpenFile(string pathFile)
        {
            return new OpenFileModel
            {
                FileName = Path.GetFileNameWithoutExtension(pathFile),
                FileText = File.ReadAllText(pathFile, Encoding.UTF8)
            };

        }
    }
}
