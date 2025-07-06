using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotePad_Launcher.IServiceUI
{
    public interface ILocalizationService : INotifyPropertyChanged
    {
        string this[string key] { get; }
        void LoadLanguage(string langCode);
    }
}
