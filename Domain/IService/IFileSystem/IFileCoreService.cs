using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IService.IFileSystem
{
    public interface IFileCoreService
    {
        public string ExDirectoryApp();
        public string DirectoryGetParent(string path);
        public string[] DirectoryGetFiles(string path, string pattern);
        public void DirectoryCreate(string path);
        public void DirectoryMove(string path, string newPath);
        public void DirectoryDelete(string path, bool recursive);

        public void WriteAllText(string pathFile, string fileText);
        public string ReadAllText(string pathFile);
        public void FileDelete(string filePath);
        public void FileCreate(string pathFile);
        public void FileMove(string path, string newPath);
    }
}
