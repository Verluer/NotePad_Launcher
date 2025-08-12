using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IService.ISystemApp
{
    public interface IConfigService
    {
        public AppConfigModel Load();
        public void Save(AppConfigModel config);
    }
}
