using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Domain.IService.IFileSystem;
using Domain.IService.ISystemApp;
using Domain.IService.IValidation;
using Domain.Model;

namespace Service.FileSystem
{
    public class FileService : IFileService
    {
        private readonly IValidationService _validationService;
        private readonly IFileCoreService _fileCoreService;
        private readonly IDirectoryService _directoryService;
        public FileService(IValidationService validationService, IDirectoryService directoryService, IFileCoreService fileSystemCore)
        {
            _validationService = validationService;
            _fileCoreService = fileSystemCore;
            _directoryService = directoryService;
        }
        public FileModel OpenFile(string pathFile)
        {
            return new FileModel
            {
                FileName = Path.GetFileNameWithoutExtension(pathFile),
                FileText = _fileCoreService.ReadAllText(pathFile),
                FilePath = pathFile
            };

        }
        public FileModel CreateFile(string DocsPath, string FileName)
        {
            FileName = $"{FileName}.txt";
            string CreateFileInDirectory = Path.Combine(DocsPath, FileName);
            if (_validationService.FileExists(CreateFileInDirectory))
            {
                int i = 1;
                while (_validationService.FileExists(CreateFileInDirectory))
                {
                    FileName = $"New Text File({i}).txt";
                    CreateFileInDirectory = Path.Combine(DocsPath, FileName);
                    i++;
                    if (!_validationService.FileExists(CreateFileInDirectory))
                    {
                        break;
                    }
                }
            }

            _fileCoreService.FileCreate(CreateFileInDirectory);

            return new FileModel
            {
                FileName = Path.GetFileNameWithoutExtension(FileName),
                FilePath = CreateFileInDirectory,
            };
        }

        public FileModel SaveFile(FileModel model, string SaveSetting, string DocsPath)
        {
            string newFilePath = null;
            if (model.FilePath != null)
            {
                if (SaveSetting == "SaveNormal")
                {
                    newFilePath = Path.Combine(Path.GetDirectoryName(model.FilePath), model.FileName + ".txt");
                    if (model.FilePath != newFilePath)
                    {
                        _fileCoreService.FileDelete(model.FilePath);
                    }
                    _fileCoreService.WriteAllText(newFilePath, model.FileText);
                }
                else if (SaveSetting == "SaveDirectory")
                {
                    newFilePath = Path.Combine(DocsPath, model.FileName + ".txt");
                    if (model.FilePath != newFilePath)
                    {
                        _fileCoreService.FileDelete(model.FilePath);
                    }
                    _fileCoreService.WriteAllText(newFilePath, model.FileText);
                }

            }
            else
            {
                newFilePath = Path.Combine(DocsPath, model.FileName + ".txt");
                if (_validationService.FileExists(newFilePath))
                {
                    int i = 1;
                    while (_validationService.FileExists(newFilePath))
                    {
                        model.FileName = $"{model.FileName}({i}).txt";
                        newFilePath = Path.Combine(DocsPath, model.FileName);
                        i++;
                        if (!_validationService.FileExists(newFilePath))
                        {
                            break;
                        }
                    }
                }
                _fileCoreService.WriteAllText(newFilePath, model.FileText);
            }
            return new FileModel
            {
                FilePath = newFilePath
            };
        }
        public void LogMessage(string message, string DocPath)
        {
            var folderArr = _directoryService.LoadFolderFile(DocPath);
            var documentsPath = _directoryService.ExDirectoryFile(DocPath);
            var directoryApp = _fileCoreService.ExDirectoryApp();
            string logFilePath = Path.Combine(directoryApp.ToString(), Path.Combine(Path.Combine(documentsPath,folderArr[0]),"log.txt"));
            File.AppendAllText(logFilePath, DateTime.Now + ": " + message + Environment.NewLine);
        }
    }
}
