using PlexFormatter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        BackgroundWorker _bwOutputWriter = new BackgroundWorker();

        object _locker = new object();
        StringBuilder _sbOutput = new StringBuilder();

        public MainWindow()
        {
            InitializeComponent();
            _bwImport.DoWork += bwImport_DoWork;
            _bwImport.RunWorkerCompleted += bwImport_RunWorkerCompleted;
            _bwOutputWriter.DoWork += bwOutputWriter_DoWork;
            _bwOutputWriter.RunWorkerCompleted += bwOutputWriter_RumWorkerCompleted;
        }

        #region Output
        private void Out(string message)
        {
            message = $"{DateTime.Now.ToString("HH:mm:ss.fff")} | {message}";
            lock (_locker)
                _sbOutput.AppendLine(message);
        }

        private void bwOutputWriter_DoWork(object sender, DoWorkEventArgs e)
        {
            lock (_locker)
            {
                txtOutput.Text += _sbOutput.ToString();
                _sbOutput.Clear();
            }
        }
        private void bwOutputWriter_RumWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }
        #endregion

        private void btnChooseFile_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            //dlg.DefaultExt = ".png";
            dlg.Filter = "MP4 Files (*.mp4)|*.mp4|MKV Files (*.mkv)|*.mkv";
            
            if (dlg.ShowDialog() ?? false)
                txtFile.Text = dlg.FileName;
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            if (!_bwImport.IsBusy)
                _bwImport.RunWorkerAsync (new { File = txtFile.Text, Title = txtTitle.Text, Year = txtYear.Text});
        }

        private void bwImport_DoWork(object sender, DoWorkEventArgs e)
        {
            var formatter = new MovieFormatter(txtFile.Text, txtTitle.Text, txtYear.Text);
            Out("Validating...");
            var valid = formatter.Validate();
            if (!valid.IsValid)
            {
                Out($"Error validating: {string.Join(", ", valid.Log)}");
                e.Result = false;
                return;
            }
            Out("Formatting...");
            var format = formatter.Format();
            if (!format.IsValid)
            {
                Out($"Error formatting: {string.Join(", ", valid.Log)}");
                e.Result = false;
                return;
            }
            Out("Importing...");
            var import = formatter.Import();
            if (!import.IsValid)
            {
                Out($"Error importing: {string.Join(", ", valid.Log)}");
                e.Result = false;
                return;
            }
            e.Result = true;
        }
        private void bwImport_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((bool)e.Result)
                Out("Successful import.");
            else
                Out("Unsuccessful import.");
        }
    }
}
