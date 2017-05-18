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


        public MainWindow()
        {
            InitializeComponent();
            _bwImport.DoWork += bwImport_DoWork;
            _bwImport.RunWorkerCompleted += bwImport_RunWorkerCompleted;
        }

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
                _bwImport.RunWorkerAsync();
        }

        private void bwImport_DoWork(object sender, DoWorkEventArgs e)
        {
            var formatter = new MovieFormatter(txtFile.Text, txtTitle.Text);
            txtOutput.Text = "Validating...\r\n";
            var valid = formatter.Validate();
            if (!valid.IsValid)
            {
                txtOutput.Text += $"Error validating: {string.Join(", ", valid.Log)}";
                return;
            }
            txtOutput.Text = "Formatting...\r\n";
            var format = formatter.Format();
            if (!format.IsValid)
            {
                txtOutput.Text += $"Error formatting: {string.Join(", ", valid.Log)}";
                return;
            }
            txtOutput.Text = "Importing...\r\n";
            var import = formatter.Import();
            if (!import.IsValid)
            {
                txtOutput.Text += $"Error importing: {string.Join(", ", valid.Log)}";
                return;
            }
        }
        private void bwImport_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            txtOutput.Text += "Successful import.";
        }
    }
}
