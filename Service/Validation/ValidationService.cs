using Domain.Attributes;
using Domain.IService.ISystemApp;
using Domain.IService.IValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Validation
{
    [RegisterService(ServiceLifetime.Singleton, serviceType: typeof(IValidationService))]
    public class ValidationService : IValidationService
    {
        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }
        public bool FileExists(string pathFile)
        {
            return File.Exists(pathFile);
        }
    }
}
