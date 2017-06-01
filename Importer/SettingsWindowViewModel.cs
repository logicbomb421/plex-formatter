using PlexFormatter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Importer.Settings.AppSettingKeys;

namespace Importer
{
    public class SettingsWindowViewModel
    {
        #region Meta
        public class AppSettingKeys
        {
            public const string MOVIE_ROOT = "MovieRoot";
            public const string TV_ROOT = "TVRoot";
            public const string PHOTO_ROOT = "PhotoRoot";
            public const string MUSIC_ROOT = "MusicRoot";
            public const string REFRESH_ON_IMPORT = "RefreshOnImport";
            public const string DELETE_SOURCE_FILES = "DeleteSourceFiles";
        }

        private Configuration _config = null;

        public event PropertyChangedEventHandler OnPropertyChanged;
        public bool IsModified { get; private set; }

        public SettingsWindowViewModel()
        {
            _config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        }

        public void SaveSettingsToDisk()
        {
            if (!IsModified)
                return;

            _config.AppSettings.Settings[MOVIE_ROOT].Value = _movieRoot;
            _config.AppSettings.Settings[TV_ROOT].Value = _tvRoot;
            _config.AppSettings.Settings[PHOTO_ROOT].Value = _photoRoot;
            _config.AppSettings.Settings[MUSIC_ROOT].Value = _musicRoot;
            _config.AppSettings.Settings[REFRESH_ON_IMPORT].Value = _refreshOnImport.Value.ToString();
            _config.AppSettings.Settings[DELETE_SOURCE_FILES].Value = _deleteSourceFiles.Value.ToString();

            _config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
            IsModified = false;
        }
        #endregion

        #region Settings
        private string _movieRoot = ConfigurationManager.AppSettings[MOVIE_ROOT] ?? Defaults.PLEX_ROOT_MOVIE;
        public string MovieRoot
        {
            get => _movieRoot;
            set
            {
                _movieRoot = value;
                IsModified = true;
            }
        }

        private string _tvRoot = ConfigurationManager.AppSettings[TV_ROOT] ?? Defaults.PLEX_ROOT_TV;
        public string TVRoot
        {
            get => _tvRoot;
            set
            {
                _tvRoot = value;
                IsModified = true;
            }
        }

        private string _photoRoot = ConfigurationManager.AppSettings[PHOTO_ROOT] ?? Defaults.PLEX_ROOT_PHOTO;
        public string PhotoRoot
        {
            get => _photoRoot;
            set
            {
                _photoRoot = value;
                IsModified = true;
            }
        }

        private string _musicRoot = ConfigurationManager.AppSettings[MUSIC_ROOT] ?? Defaults.PLEX_ROOT_MOVIE;
        public string MusicRoot
        {
            get => _musicRoot;
            set
            {
                _musicRoot = value;
                IsModified = true;
            }
        }

        private bool? _refreshOnImport = null;
        public bool RefreshOnImport
        {
            get
            {
                if (!_refreshOnImport.HasValue)
                    _refreshOnImport = bool.TryParse(ConfigurationManager.AppSettings[REFRESH_ON_IMPORT], out bool bb) ? bb : Defaults.PLEX_REFRESH_ON_IMPORT;
                return _refreshOnImport.Value;
            }
            set
            {
                _refreshOnImport = value;
                IsModified = true;
            }
        }

        private bool? _deleteSourceFiles = null;
        public bool DeleteSourceFiles
        {
            get
            {
                if (!_deleteSourceFiles.HasValue)
                    _deleteSourceFiles = bool.TryParse(ConfigurationManager.AppSettings[DELETE_SOURCE_FILES], out bool bb) ? bb : Defaults.PLEX_DELETE_SOURCE_FILES;
                return _deleteSourceFiles.Value;
            }
            set
            {
                _deleteSourceFiles = value;
                IsModified = true;
            }
        }
        #endregion
    }
}
