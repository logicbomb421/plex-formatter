using System.Configuration;
using static Importer.Settings.AppSettingKeys;
using PlexFormatter;
using System;

namespace Importer
{
    public static class Settings
    {
        #region Meta
        public static class AppSettingKeys
        {
            public const string MOVIE_ROOT = "MovieRoot";
            public const string TV_ROOT = "TVRoot";
            public const string PHOTO_ROOT = "PhotoRoot";
            public const string MUSIC_ROOT = "MusicRoot";
            public const string REFRESH_ON_IMPORT = "RefreshOnImport";
            public const string DELETE_SOURCE_FILES = "DeleteSourceFiles";
        }

        private static Configuration _config = null;

        public static bool IsModified { get; private set; }

        static Settings()
        {
            _config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        }

        public static void Save()
        {
            if (!IsModified)
                return;
            _config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
        #endregion

        #region Settings
        public static string MovieRoot
        {
            get => ConfigurationManager.AppSettings[MOVIE_ROOT] ?? Defaults.PLEX_ROOT_MOVIE;
            set
            {
                _config.AppSettings.Settings[MOVIE_ROOT].Value = value;
                IsModified = true;
            }
        }
        public static string TVRoot
        {
            get => ConfigurationManager.AppSettings[TV_ROOT] ?? Defaults.PLEX_ROOT_TV;
            set => ConfigurationManager.AppSettings.Set(TV_ROOT, value);
        }
        public static string PhotoRoot
        {
            get => ConfigurationManager.AppSettings[PHOTO_ROOT] ?? Defaults.PLEX_ROOT_PHOTO;
            set => ConfigurationManager.AppSettings.Set(PHOTO_ROOT, value);
        }
        public static string MusicRoot
        {
            get => ConfigurationManager.AppSettings[MUSIC_ROOT] ?? Defaults.PLEX_ROOT_MOVIE;
            set => ConfigurationManager.AppSettings.Set(MUSIC_ROOT, value);
        }

        public static bool RefreshOnImport
        {
            get => bool.TryParse(ConfigurationManager.AppSettings[REFRESH_ON_IMPORT], out bool bb) ? bb : Defaults.PLEX_REFRESH_ON_IMPORT;
            set => ConfigurationManager.AppSettings.Set(REFRESH_ON_IMPORT, value.ToString());
        }
        public static bool DeleteSourceFiles
        {
            get => bool.TryParse(ConfigurationManager.AppSettings[DELETE_SOURCE_FILES], out bool bb) ? bb : Defaults.PLEX_DELETE_SOURCE_FILES;
            set => ConfigurationManager.AppSettings.Set(DELETE_SOURCE_FILES, value.ToString());
        }
        #endregion
    }
}
