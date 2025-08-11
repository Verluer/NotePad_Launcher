using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class AppConfigModel
    {
        public string DocsPath { get; set; } = @"FirstLaunch";
        public string SaveSetting { get; set; } = "SaveNormal";
        public string Language { get; set; } = "en";
        public string SyntaxHighlighting { get; set; } = "C#";
        public bool SyntaxToggle { get; set; } = false;
        public bool WordWrap { get; set; } = false;
    }
}
