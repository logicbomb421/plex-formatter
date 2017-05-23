using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Configuration;
using static PlexFormatter.Defaults;

namespace Importer
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            txtMovies.Text = ConfigurationManager.AppSettings["MovieRoot"] ?? PLEX_ROOT_MOVIE;
            txtTvShows.Text = ConfigurationManager.AppSettings["TVRoot"] ?? PLEX_ROOT_TV;
            txtPhotos.Text = ConfigurationManager.AppSettings["PhotoRoot"] ?? PLEX_ROOT_PHOTO;
            txtMusic.Text = ConfigurationManager.AppSettings["MusicRoot"] ?? PLEX_ROOT_MUSIC;
            var refresh = PLEX_REFRESH_ON_IMPORT;
            bool.TryParse(ConfigurationManager.AppSettings["RefreshOnImport"], out refresh);
            chkRefreshOnImport.IsChecked = refresh;
        }
    }
}
