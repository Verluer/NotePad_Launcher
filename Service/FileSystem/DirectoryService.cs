using Domain.Attributes;
using Domain.IService.IFileSystem;
using Domain.IService.ISystemApp;
using Domain.IService.IValidation;
using Domain.Model;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Service.FileSystem
{
    [RegisterService(ServiceLifetime.Singleton, serviceType: typeof(IDirectoryService))]
    public class DirectoryService : IDirectoryService
    {
        private readonly IValidationService _validationService;
        private readonly IFileCoreService _fileCoreService;
        public DirectoryService(IValidationService validationService, IFileCoreService fileSystemCore)
        {
            _validationService = validationService;
            _fileCoreService = fileSystemCore;
        }
        public string ExDirectoryFile(string Folder)
        {
            var exeDirectory = _fileCoreService.ExDirectoryApp();
            var resourcesDirectory = "";
            var counter = 0;
            while (counter < 5)
            {
                resourcesDirectory = Path.Combine(exeDirectory, Folder);
                if (!_validationService.DirectoryExists(resourcesDirectory))
                {
                    exeDirectory = _fileCoreService.DirectoryGetParent(exeDirectory);
                }
                else
                {
                    break;
                }
            }
            return resourcesDirectory;
        }
        public void CreateDocumentsDirectory()
        {
            var exeDirectory = _fileCoreService.ExDirectoryApp();
            var documentsDirect = Path.Combine(exeDirectory, "Documents");
            var subdirectory = Path.Combine(documentsDirect, "Main");
            _fileCoreService.DirectoryCreate(documentsDirect);
            _fileCoreService.DirectoryCreate(subdirectory);
        }
        public List<FileModel> GetTextFiles(string DocsPath)
        {
            if (!_validationService.DirectoryExists(DocsPath))
                throw new DirectoryNotFoundException($"Директория не найдена: {DocsPath}");

            return Directory.GetFiles(DocsPath, "*.txt")
                    .Select(file => new FileModel
                        {
                            FileName = Path.GetFileNameWithoutExtension(file),
                            FileText = _fileCoreService.ReadAllText(file),
                            FilePath = file,
                        }).ToList();
        }
        public List<string> LoadFolderFile(string DocsPath)
        {
            return Directory.GetDirectories(DocsPath)
                .Select(path => Path.GetFileName(path))
                .ToList();
        }
        public List<string> FindLocalization()
        {
            var exeDirectory = _fileCoreService.ExDirectoryApp();
            var folderLoc = Path.Combine("Resources", "Locales");
            var directoryLoc = Path.Combine(exeDirectory, folderLoc);
            string[] tempLocFiles = _fileCoreService.DirectoryGetFiles(directoryLoc, "*.json");
            List<string> locNames = new List<string>();
            foreach (var locName in tempLocFiles)
            {
                locNames.Add(Path.GetFileNameWithoutExtension(locName));
            }
            return locNames;
        }
    }
}
