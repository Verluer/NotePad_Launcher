using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IService.IFileSystem
{
    public interface IDirectoryService
    {
        public string ExDirectoryFile(string Folder);
        public void CreateDocumentsDirectory();
        public List<FileModel> GetTextFiles(string DocsPath);
        public List<string> LoadFolderFile(string DocsPath);
        public List<string> FindLocalization();
    }
}
