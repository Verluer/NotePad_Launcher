using Domain.IService.IFileSystem;
using Domain.IService.IValidation;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Service.FileSystem
{
    public class FileSystemManager : IFileSystemManager
    {
        private readonly IDirectoryService _directoryService;
        private readonly IFileCoreService _fileCoreService;
        private readonly IValidationService _validationService;
        private readonly IFileService _fileService;
        public FileSystemManager(IDirectoryService directoryService, IFileCoreService fileCoreService, IValidationService validationService, IFileService fileService)
        {
            _directoryService = directoryService;
            _fileCoreService = fileCoreService;
            _validationService = validationService;
            _fileService = fileService;
        }
        #region FileService
        public FileModel OpenFile(string pathFile)
        {
            return _fileService.OpenFile(pathFile);
        }
        public FileModel SaveFile(FileModel model, string SaveSetting, string DocsPath)
        {
            return _fileService.SaveFile(model, SaveSetting, DocsPath);
        }
        public FileModel CreateFile(string DocsPath, string FileName)
        {
            return _fileService.CreateFile(DocsPath, FileName);
        }
        public void LogMessage(string message, string DocPath)
        {
            _fileService.LogMessage(message, DocPath);
        }
        #endregion
        #region DirectoryService
        public string ExDirectoryFile(string Folder)
        {
            return _directoryService.ExDirectoryFile(Folder);
        }
        public void CreateDocumentsDirectory()
        {
            _directoryService.CreateDocumentsDirectory();
        }
        public List<FileModel> GetTextFiles(string DocsPath)
        {
           return _directoryService.GetTextFiles(DocsPath);
        }
        public List<string> LoadFolderFile(string DocsPath)
        {
            return _directoryService.LoadFolderFile(DocsPath);
        }
        public List<string> FindLocalization()
        {
            return _directoryService.FindLocalization();
        }
        #endregion
        #region fileCoreService
        public string ExDirectoryApp()
        {
            return _fileCoreService.ExDirectoryApp();
        }

        public string DirectoryGetParent(string path)
        {
            return _fileCoreService.DirectoryGetParent(path);
        }
        public string[] DirectoryGetFiles(string path, string pattern)
        {
            return _fileCoreService.DirectoryGetFiles(path, pattern);
        }
        public void DirectoryCreate(string path)
        {
            _fileCoreService.DirectoryCreate(path);
        }
        public void DirectoryMove(string path, string newPath)
        {
            _fileCoreService.DirectoryMove(path, newPath);
        }
        public void DirectoryDelete(string path, bool recursive)
        {
            _fileCoreService.DirectoryDelete(path, recursive);
        }
        public void WriteAllText(string pathFile, string fileText)
        {
            _fileCoreService.WriteAllText(pathFile, fileText);
        }
        public string ReadAllText(string pathFile)
        {
           return _fileCoreService.ReadAllText(pathFile);
        }
        public void FileDelete(string filePath)
        {
            _fileCoreService.FileDelete(filePath);
        }

        public void FileCreate(string pathFile)
        {
           _fileCoreService.FileCreate(pathFile);
        }
        public void FileMove(string path, string newPath)
        {
            _fileCoreService.FileMove(path, newPath);
        }
        #endregion
    }
}
