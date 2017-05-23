using PlexFormatter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Importer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BackgroundWorker _bwImport = new BackgroundWorker();
        Timer _tmOutputWriter = new Timer();

        object _locker = new object();
        StringBuilder _sbOutput = new StringBuilder();

        public MainWindow()
        {
            InitializeComponent();

            _bwImport.DoWork += bwImport_DoWork;
            _bwImport.RunWorkerCompleted += bwImport_RunWorkerCompleted;
            _tmOutputWriter.Interval = 250;

            _tmOutputWriter.AutoReset = true;
            _tmOutputWriter.Elapsed += new ElapsedEventHandler(tmOutputWriter_Elapsed);
            _tmOutputWriter.Enabled = true;
        }

        #region Output
        private void Out(string message)
        {
            message = $"{DateTime.Now.ToString("HH:mm:ss.fff")} | {message}";
            lock (_locker)
                _sbOutput.AppendLine(message);
        }

        private void tmOutputWriter_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (_locker)
            {
                if (_sbOutput.Length > 0)
                {
                    Dispatcher.Invoke(() =>
                            {
                                txtOutput.Text += _sbOutput.ToString();
                                svOutput.ScrollToEnd();
                            });
                    _sbOutput.Clear(); 
                }
            }
        }
        #endregion

        private void btnChooseFile_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "MP4 Files (*.mp4)|*.mp4|MKV Files (*.mkv)|*.mkv";

            if (dlg.ShowDialog() ?? false)
                txtFile.Text = dlg.FileName;
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            if (!_bwImport.IsBusy)
            {
                btnImport.IsEnabled = false;
                _bwImport.RunWorkerAsync(new { File = txtFile.Text, Title = txtTitle.Text, Year = txtYear.Text });
            }
        }

        private void bwImport_DoWork(object sender, DoWorkEventArgs e)
        {
            dynamic args = e.Argument;
            var formatter = new MovieFormatter(args.File, args.Title, args.Year);

            Out("Validating...");
            var valid = formatter.Validate();
            if (valid.Status == PlexFormatterResult.ResultStatus.Failed)
            {
                Out($"Error validating: {string.Join(", ", valid.Log)}");
                e.Result = false;
                return;
            }

            Out("Formatting...");
            var format = formatter.Format();
            if (format.Status == PlexFormatterResult.ResultStatus.Failed)
            {
                Out($"Error formatting: {string.Join(", ", valid.Log)}");
                e.Result = false;
                return;
            }
            if (format.Log.Count > 0)
                format.Log.ForEach(entry => Out(entry));

            Out("Importing...");
            var import = formatter.Import();
            if (import.Status == PlexFormatterResult.ResultStatus.Failed)
            {
                Out($"Error importing: {string.Join(", ", valid.Log)}");
                e.Result = false;
                return;
            }

            e.Result = true;
        }
        private void bwImport_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //TODO should use mvvm binding for this
            Dispatcher.Invoke(() => btnImport.IsEnabled = true);
            if ((bool)e.Result)
                Out("Successful import!");
            else
                Out("Unsuccessful import.");
        }

        #region Menu Commands
        private void RefreshPlexLibrary_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            new SettingsWindow().Show();
        }
        #endregion
    }
}
