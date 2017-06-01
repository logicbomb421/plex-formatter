using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Importer.SettingsWindowViewModel.AppSettingKeys;

namespace Importer
{
    public class MainWindowViewModel
    {
        private string _tiDebugIsVisible = null;
        public string ShowDebugPanel
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_tiDebugIsVisible))
                {
                    bool dbg = false;
                    bool.TryParse(ConfigurationManager.AppSettings[SHOW_DEBUG_PANEL], out dbg);
                    _tiDebugIsVisible = dbg ? "Visible" : "Hidden";
                }
                return _tiDebugIsVisible;
            }
        }


        public bool MyProperty { get; set; }
    }
}
