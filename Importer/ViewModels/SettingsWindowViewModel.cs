﻿using Importer.Utilities;
using PlexFormatter;
using System.ComponentModel;
using System.Configuration;
using System.Windows;
using System.Windows.Input;
using static Importer.Utilities.Constants.AppSettingKeys;

namespace Importer.ViewModels
{
    public class SettingsWindowViewModel : ViewModelBase
    {
        #region ViewModel
        private Window _parentWindow;
        private Configuration _config = null;
        private ICommand _saveToDisk;
        private ICommand _cancel;

        public ICommand SaveToDisk => _saveToDisk != null ? _saveToDisk : _saveToDisk = new RelayCommand(o => IsModified, o => SaveSettingsToDisk());
        public ICommand Close => _cancel != null ? _cancel : _cancel = new RelayCommand(o => TryCloseWindow());

        public SettingsWindowViewModel(Window window)
        {
            _parentWindow = window;
            _config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            PropertyChanged += (o, args) => IsModified = true;
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
            MessageBox.Show("Settings saved successfully!", "Success!", MessageBoxButton.OK);
        }

        public void TryCloseWindow()
        {
            if (IsModified && MessageBox.Show("There are unsaved changes, are you shure you wish to close?", "Save Changes?", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;
            _parentWindow.Close();
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
                OnPropertyChanged(nameof(MovieRoot));
            }
        }

        private string _tvRoot = ConfigurationManager.AppSettings[TV_ROOT] ?? Defaults.PLEX_ROOT_TV;
        public string TVRoot
        {
            get => _tvRoot;
            set
            {
                _tvRoot = value;
                OnPropertyChanged(nameof(TVRoot));
            }
        }

        private string _photoRoot = ConfigurationManager.AppSettings[PHOTO_ROOT] ?? Defaults.PLEX_ROOT_PHOTO;
        public string PhotoRoot
        {
            get => _photoRoot;
            set
            {
                _photoRoot = value;
                OnPropertyChanged(nameof(PhotoRoot));
            }
        }

        private string _musicRoot = ConfigurationManager.AppSettings[MUSIC_ROOT] ?? Defaults.PLEX_ROOT_MOVIE;
        public string MusicRoot
        {
            get => _musicRoot;
            set
            {
                _musicRoot = value;
                OnPropertyChanged(nameof(MusicRoot));
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
                OnPropertyChanged(nameof(RefreshOnImport));
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
                OnPropertyChanged(nameof(DeleteSourceFiles));
            }
        }
        #endregion
    }
}