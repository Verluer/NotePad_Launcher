using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Domain.Attributes;
using Domain.IService.IFileSystem;
using Domain.IService.ISystemApp;
using Domain.IService.IValidation;
using Domain.Model;
using Microsoft.Extensions.DependencyInjection;
using Service.SystemApp;

namespace Service.FileSystem
{
    [RegisterService(ServiceLifetime.Singleton, serviceType: typeof(IFileService))]
    public class FileService : IFileService
    {
        private readonly IValidationService _validationService;
        private readonly IFileCoreService _fileCoreService;
        private readonly IDirectoryService _directoryService;
        private readonly IConfigService _configService;
        public FileService(IValidationService validationService, IDirectoryService directoryService, IFileCoreService fileSystemCore, IConfigService configService)
        {
            _validationService = validationService;
            _fileCoreService = fileSystemCore;
            _directoryService = directoryService;
            _configService = configService;
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
            string CreateFileInDirectory = Path.Combine(DocsPath, FileName + ".txt");
            if (_validationService.FileExists(CreateFileInDirectory))
            {
                int i = 1;
                while (_validationService.FileExists(CreateFileInDirectory))
                {
                    string tempFileName = $"{FileName}({i}).txt";
                    CreateFileInDirectory = Path.Combine(DocsPath, tempFileName);
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
                        if (_validationService.FileExists(newFilePath))
                        {
                            newFilePath = GetUniqueFilePath(newFilePath, model.FileName, model.FilePath);
                        }
                        _fileCoreService.FileDelete(model.FilePath);
                    }
                }
                else if (SaveSetting == "SaveDirectory")
                {
                    newFilePath = Path.Combine(DocsPath, model.FileName + ".txt");
                    if (model.FilePath != newFilePath)
                    {
                        if (_validationService.FileExists(newFilePath))
                        {
                            newFilePath = GetUniqueFilePath(newFilePath, model.FileName, model.FilePath);
                        }
                        _fileCoreService.FileDelete(model.FilePath);
                    }      
                }
               _fileCoreService.WriteAllText(newFilePath, model.FileText);

            }
            else
            {
                newFilePath = Path.Combine(DocsPath, model.FileName + ".txt");
                if (_validationService.FileExists(newFilePath))
                newFilePath = GetUniqueFilePath(newFilePath, model.FileName, DocsPath);
                _fileCoreService.WriteAllText(newFilePath, model.FileText);
            }
            return new FileModel
            {
                FilePath = newFilePath,
                FileName = Path.GetFileNameWithoutExtension(newFilePath)
            };
        }
        private string GetUniqueFilePath(string fileName, string newFilePath, string filePath)
        {
            int i = 1;
            while (_validationService.FileExists(newFilePath))
            {
                fileName = Path.GetFileNameWithoutExtension(fileName);
                string tempFileName = $"{fileName}({i}).txt";
                if (Path.GetExtension(filePath).Equals(".txt", StringComparison.OrdinalIgnoreCase))
                {
                    newFilePath = Path.Combine(Path.GetDirectoryName(filePath), tempFileName);
                }
                else
                {
                    newFilePath = Path.Combine(filePath, tempFileName);
                }
                i++;
                if (!_validationService.FileExists(newFilePath))
                {
                    break;
                }
            }
            return newFilePath;
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
