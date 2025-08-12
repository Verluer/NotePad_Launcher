using Domain.IService.IFileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Service.FileSystem
{
    public class FileCoreService : IFileCoreService
    {
        public string ExDirectoryApp()
        {
            var exePath = Assembly.GetExecutingAssembly().Location;
            return Path.GetDirectoryName(exePath);
        }
        public string DirectoryGetParent(string path)
        {
            return Directory.GetParent(path).FullName;
        }
        public string[] DirectoryGetFiles(string path, string pattern)
        {
            return Directory.GetFiles(path, pattern);
        }
        public void DirectoryCreate(string path)
        {
            Directory.CreateDirectory(path);
        }
        public void DirectoryMove(string path, string newPath)
        {
            Directory.Move(path, newPath);
        }
        public void DirectoryDelete(string path, bool recursive)
        {
            Directory.Delete(path);
        }
        public void WriteAllText(string pathFile, string fileText)
        {
            File.WriteAllText(pathFile, fileText);
        }
        public string ReadAllText(string pathFile)
        {
            return File.ReadAllText(pathFile, Encoding.UTF8);
        }
        public void FileDelete(string filePath)
        {
            File.Delete(filePath);
        }

        public void FileCreate(string pathFile)
        {
            File.Create(pathFile).Dispose();
        }
        public void FileMove(string path, string newPath)
        {
            File.Move(path, newPath);
        }

    }
}
