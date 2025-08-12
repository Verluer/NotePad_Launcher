using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IService.IValidation
{
    public interface IValidationService
    {
        public bool DirectoryExists(string path);
        public bool FileExists(string pathFile);
    }
}
