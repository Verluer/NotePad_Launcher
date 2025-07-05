using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Domain.IService;
using Domain.Model;
using Domain.IService;

namespace Service
{
    public class ConfigService : IConfigService
    {
        private static readonly string FolderPath =
           Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NotePadLauncher");

        private static readonly string ConfigFile = Path.Combine(FolderPath, "appconfig.json");

        public AppConfigModel Load()
        {
            if (!File.Exists(ConfigFile))
                return new AppConfigModel(); // значения по умолчанию

            string json = File.ReadAllText(ConfigFile);
            return JsonSerializer.Deserialize<AppConfigModel>(json) ?? new AppConfigModel();
        }

        public void Save(AppConfigModel config)
        {
            if (!Directory.Exists(FolderPath))
                Directory.CreateDirectory(FolderPath);

            string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigFile, json);
        }
    }
}
