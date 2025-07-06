using NotePad_Launcher.IServiceUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NotePad_Launcher.ServiceUI
{
    public class LocalizationService : ILocalizationService
    {
        private Dictionary<string, string> _translations = new Dictionary<string, string>();

        public event PropertyChangedEventHandler PropertyChanged;

        public static LocalizationService Instance { get; } = new LocalizationService();

        public string this[string key]
        {
            get => _translations.TryGetValue(key, out var value) ? value : $"[{key}]";
        }

        public void LoadLanguage(string langCode)
        {
            var path = Path.Combine(@"D:\VIsual Studio\VS project\NotePad_Launcher\NotePad_Launcher\Resources\Locales", $"{langCode}.json");
            if (!File.Exists(path)) return;

            var json = File.ReadAllText(path);
            _translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

            OnPropertyChanged("");
        }

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
