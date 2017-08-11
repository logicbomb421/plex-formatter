using Importer.Utilities;
using System;
using System.Windows.Input;
using PlexFormatter.Formatters;
using System.Configuration;
using static Importer.Utilities.Constants.AppSettingKeys;
using PlexFormatter;
using Microsoft.Win32;

namespace Importer.ViewModels.Tabs
{
    public class MovieTabViewModel : ViewModelBase
    {
        private bool _isImporting = false;

        #region ICommand ChooseFile
        public ICommand ChooseFile { get; private set; }
        private bool chooseFile_canExecute(object param) => !_isImporting;
        private void chooseFile_execute(object param)
        {
            var dlg = new OpenFileDialog()
            {
                Filter = "Plex Video Formats|*.mp4;*.mkv;*.avi", //TODO config setting
                CheckFileExists = true,
                CheckPathExists = true,
                InitialDirectory = Environment.GetEnvironmentVariable("HOMEDRIVE") + Environment.GetEnvironmentVariable("HOMEPATH"),
            };
            if (dlg.ShowDialog() ?? false)
                Path = dlg.FileName;
        }
        private void chooseFile_onExecuteError(object sender, Exception ex)
            => Out($"An error occurred while attempting to open the file dialog. The error was: {ex.Message}");
        #endregion

        #region ICommand Clear
        public ICommand Clear { get; private set; }
        private bool clear_canExecute(object param) => IsModified;
        private void clear_execute(object param)
        {
            Path = string.Empty;
            Title = string.Empty;
            Year = null;
            IsModified = false;
        }
        private void clear_onExecuteError(object sender, Exception ex)
           => Out($"An error occurred while attempting to clear all input. The error was: {ex.Message}");
        #endregion

        #region ICommand Import
        public ICommand Import { get; private set; }
        private bool import_canExecute(object param) => !_isImporting;
        private void import_execute(object param)
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

            Out("Validating...");
            var valid = formatter.Validate();
            if (valid.Status == ResultStatus.Failed)
            {
                Out($"Error validating: {string.Join(", ", valid.Log)}");
                _isImporting = false;
                return;
            }

            Out("Formatting...");
            var format = formatter.Format();
            if (format.Status == ResultStatus.Failed)
            {
                Out($"Error formatting: {string.Join(", ", valid.Log)}");
                _isImporting = false;
                return;
            }
            if (format.Log.Count > 0) ;
            format.Log.ForEach(entry => Out(entry));

            Out("Importing...");
            var import = formatter.Import();
            if (import.Status == ResultStatus.Failed)
            {
                Out($"Error importing: {string.Join(", ", valid.Log)}");
                _isImporting = false;
                return;
            }

            Out("Successful import!");
            _isImporting = false;
        }
        private void import_onExecuteError(object sender, Exception ex)
            => Out($"An error occurred while importing. The error was: {ex.Message}");
        #endregion

        public MovieTabViewModel()
        {
            PropertyChanged += (o, args) => IsModified = true;
            ChooseFile = new AsyncCommand(chooseFile_canExecute, chooseFile_execute, chooseFile_onExecuteError);//TODO test canExecute predicate to make sure it works
            Clear = new AsyncCommand(clear_canExecute, clear_execute, clear_onExecuteError);
            Import = new AsyncCommand(import_canExecute, import_execute, import_onExecuteError);
        }

        //TODO need to clean up the word wrap somehow
        private void Out(string message, bool newline = true, bool format = true)
             => Output += !format ? message : $"{DateTime.Now.ToString("HH:mm:ss.fff")} | {message}{(newline ? Environment.NewLine : string.Empty)}";

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