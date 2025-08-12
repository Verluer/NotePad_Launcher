using Domain.IService.IValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Validation
{
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
