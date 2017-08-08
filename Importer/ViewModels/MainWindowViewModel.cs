using Importer.ViewModels.Tabs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Importer.Utilities.Constants.AppSettingKeys;

namespace Importer.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {

        private MovieTabViewModel _movie;
        public MovieTabViewModel MovieTab => _movie != null ? _movie : _movie = new MovieTabViewModel();

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
    }
}
