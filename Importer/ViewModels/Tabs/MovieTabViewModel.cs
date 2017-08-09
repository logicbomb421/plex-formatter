using Importer.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PlexFormatter.Formatters;
using System.Configuration;
using static Importer.Utilities.Constants.AppSettingKeys;
using PlexFormatter;

namespace Importer.ViewModels.Tabs
{
    public class MovieTabViewModel : ViewModelBase
    {
        //TODO this entire thing blocks the UI thread. need to implement async/await
        //might need to implement this a level down in RelayCommand

        private bool _isImporting = false;
        private ICommand _chooseFile;
        private ICommand _clear;
        private ICommand _import;

        public ICommand ChooseFile
        {
            get
            {
                if (_chooseFile == null)
                {
                    //TODO test canExecute predicate to make sure it works
                    _chooseFile = new RelayCommand(o => !_isImporting, chooseFile_execute, (sender, ex) => );
                }
                return _chooseFile;
            }
        }
        private Task chooseFile_execute(object param)
        {
            return Task.Factory.StartNew(() =>
            {
                var dlg = new Microsoft.Win32.OpenFileDialog()
                {
                    Filter = "Plex Video Formats|*.mp4;*.mkv;*.avi" //TODO config setting
                };
                if (dlg.ShowDialog() ?? false)
                    Path = dlg.FileName;
            });
        }

        public ICommand Clear
        {
            get
            {
                if (_clear == null)
                {
                    _clear = new RelayCommand(o => IsModified && !_isImporting, o =>
                    {
                        return Task.Factory.StartNew(() =>
                        {
                            Path = string.Empty;
                            Title = string.Empty;
                            Year = null;
                            IsModified = false;
                        });
                    });
                }
                return _clear;
            }
        }

        public ICommand Import
        {
            get
            {
                if (_import == null)
                {
                    _import = new RelayCommand(o => !_isImporting && !string.IsNullOrEmpty(_path) && !string.IsNullOrEmpty(_title), o =>
                    {
                        return Task.Factory.StartNew(() =>
                        {
                            //TODO i dont like the manual control over this bool.. need to figure out the 'MVVM' way of doing this
                            _isImporting = true;
                            var formatter = new MovieFormatter(
                                _path,
                                _title,
                                bool.TryParse(ConfigurationManager.AppSettings[DELETE_SOURCE_FILES], out bool b) ? b : Defaults.PLEX_DELETE_SOURCE_FILES,
                                ConfigurationManager.AppSettings[MOVIE_ROOT] ?? Defaults.PLEX_ROOT_MOVIE,
                                _year,
                                bool.TryParse(ConfigurationManager.AppSettings[USE_EXPERIMENTAL_COPIER], out bool bb) ? bb : Defaults.PLEX_USE_EXPERIMENTAL_COPIER
                            );

                            //Out("Validating...");
                            Output += formatOutput("Validating...");
                            var valid = formatter.Validate();
                            if (valid.Status == PlexFormatterResult.ResultStatus.Failed)
                            {
                                //Out($"Error validating: {string.Join(", ", valid.Log)}");
                                Output += formatOutput($"Error validating: {string.Join(", ", valid.Log)}");
                                _isImporting = false;
                                return;
                            }

                            //Out("Formatting...");
                            Output += formatOutput("Formatting...");
                            var format = formatter.Format();
                            if (format.Status == PlexFormatterResult.ResultStatus.Failed)
                            {
                                //Out($"Error formatting: {string.Join(", ", valid.Log)}");
                                Output += formatOutput($"Error formatting: {string.Join(", ", valid.Log)}");
                                _isImporting = false;
                                return;
                            }
                            if (format.Log.Count > 0) ;
                            format.Log.ForEach(entry => Output += formatOutput(entry));

                            //Out("Importing...");
                            Output += formatOutput("Importing...");
                            var import = formatter.Import();
                            if (import.Status == PlexFormatterResult.ResultStatus.Failed)
                            {
                                //Out($"Error importing: {string.Join(", ", valid.Log)}");
                                Output += formatOutput($"Error importing: {string.Join(", ", valid.Log)}");
                                _isImporting = false;
                                return;
                            }

                            Output += formatOutput("Successful import!");
                            _isImporting = false;
                        });
                    });
                }
                return _import;
            }
        }

        public MovieTabViewModel()
        {
            PropertyChanged += (o, args) => IsModified = true;
        }

        //TODO need to clean up the word wrap somehow
        private string formatOutput(string message, bool newline = true, bool format = true)
             => !format ? message : $"{DateTime.Now.ToString("HH:mm:ss.fff")} | {message}{(newline ? Environment.NewLine : string.Empty)}";

        #region Model Properties
        private string _path = string.Empty;
        public string Path
        {
            get => _path;
            set
            {
                _path = value;
                OnPropertyChanged();
            }
        }

        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        private int? _year = null;
        public int? Year
        {
            get => _year;
            set
            {
                _year = value;
                OnPropertyChanged();
            }
        }

        //TODO this needs to be on the MainWindowViewModel.. fire event from here to update
        private string _output = string.Empty;
        public string Output
        {
            get => _output;
            set
            {
                _output = value;
                OnPropertyChanged();
            }
        }
        #endregion
    }
}