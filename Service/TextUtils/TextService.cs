using Domain.Attributes;
using Domain.IService.IFileSystem;
using Domain.IService.ISystemApp;
using Domain.IService.ITextUtils;
using Domain.IService.IValidation;
using Microsoft.Extensions.DependencyInjection;
using Service.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.TextUtils
{

    [RegisterService(ServiceLifetime.Singleton, serviceType: typeof(ITextService))]
    public class TextService : ITextService
    {
        private readonly IFileCoreService _fileCoreService;
        private readonly IValidationService _validationService;
        public TextService(IFileCoreService fileSystemCore, IValidationService validationService)
        {
            _validationService = validationService;
            _fileCoreService = fileSystemCore;
        }
        public bool CheckTextChange(string pathFile, string fileText)
        {
            if (string.IsNullOrEmpty(pathFile) || !_validationService.FileExists(pathFile))
                return true;

            var checkTextFromFile = _fileCoreService.ReadAllText(pathFile);
            return checkTextFromFile == fileText;
        }
    }
}
