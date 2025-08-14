using Domain.Attributes;
using Domain.IService.IFileSystem;
using Domain.IService.ISystemApp;
using Microsoft.Extensions.DependencyInjection;
using NotePad_Launcher.IServiceUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace NotePad_Launcher.ServiceUI
{
    [RegisterService(ServiceLifetime.Singleton, serviceType: typeof(ILocalizationService))]
    public class LocalizationService : ILocalizationService
    {
        private readonly IDirectoryService _directoryService;

        private Dictionary<string, string> _translations = new Dictionary<string, string>();

        public event PropertyChangedEventHandler PropertyChanged;
        public static LocalizationService Instance { get; private set; }
        public LocalizationService(IDirectoryService directoryService)
        {
            _directoryService = directoryService;

            Instance = this;
        }

        public string this[string key]
        {
            get => _translations.TryGetValue(key, out var value) ? value : $"[{key}]";
        }
        
        public void LoadLanguage(string langCode)
        {
            var resourcesDirectory = _directoryService.ExDirectoryFile("Resources\\Locales");
            var path = Path.Combine($@"{resourcesDirectory}", $"{langCode}.json");
            if (!File.Exists(path)) return;

            var json = File.ReadAllText(path);
            _translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

            OnPropertyChanged("");
        }

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
