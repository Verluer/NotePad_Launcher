using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IService.ITextUtils
{
    public interface ITextService
    {
        public bool CheckTextChange(string pathFile, string fileText);
    }
}
