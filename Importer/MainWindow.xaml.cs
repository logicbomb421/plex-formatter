using Importer.ViewModels;
using PlexFormatter;
using PlexFormatter.Formatters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
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
using static Importer.Utilities.Constants.AppSettingKeys;

namespace Importer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Timer _tmOutputWriter = new Timer();
        object _locker = new object();
        StringBuilder _sbOutput = new StringBuilder();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();

            _tmOutputWriter.AutoReset = true;
            _tmOutputWriter.Elapsed += new ElapsedEventHandler(tmOutputWriter_Elapsed);
            _tmOutputWriter.Enabled = true;
        }

        #region Output
        private void Out(string message, bool newline = true, bool format = true)
        {
            //TODO need to clean up the word wrap somehow
            if (format)
                message = $"{DateTime.Now.ToString("HH:mm:ss.fff")} | {message}";

            if (newline)
            {
                lock (_locker)
                    _sbOutput.AppendLine(message);
            }
            else
            {
                lock (_locker)
                    _sbOutput.Append(message);
            }
            
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
                                txtOutput.ScrollToEnd();
                            });
                    _sbOutput.Clear();
                }
            }
        }
        #endregion  

        #region Menu Commands
        private void RefreshPlexLibrary_Click(object sender, RoutedEventArgs e)
        {
            Out("Refreshing Plex library...");
            Task.Factory.StartNew(() =>
            {
                try
                {
                    using (var process = new System.Diagnostics.Process())
                    {
                        process.StartInfo = new System.Diagnostics.ProcessStartInfo(@"C:\Program Files (x86)\Plex\Plex Media Server\Plex Media Scanner.exe")
                        {
                            Arguments = "--scan",
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                            CreateNoWindow = false
                        };
                        process.Start();
                        //TODO isnt working as expected
                        string line = string.Empty;
                        while (!string.IsNullOrEmpty(line = process.StandardOutput.ReadLine()))
                            Out(line);
                    }
                }
                catch (Exception ex)
                {
                    Out($"Unable to refresh library. The error was: {ex.Message}");
                }
            });
        }
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            new SettingsWindow().Show();
        }
        #endregion

        #region Debug
        private void __btnDebug_Click(object sender, RoutedEventArgs e)
        {
            //Task.Factory.StartNew(__writeTextToOutput);
            Task.Factory.StartNew(__writeFilePercentToOutput);
        }
        private void __writeFilePercentToOutput()
        {
            Out("Importing...");
            System.Threading.Thread.Sleep(300);
            for (int i = 0, percent = 0; i <= 20; ++i, percent += 5)
            {
                Out(percent == 100 ? percent.ToString() : $"{percent}...", percent == 100, percent == 0);
                System.Threading.Thread.Sleep(300);
            }
            Out("Successful import!");
        }
        private void __writeTextToOutput()
        {
            var strings = new[] {
                "Validating...",
                $"Error validating: Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                "Formatting...",
                $"Error formatting: Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                "Importing...",
                $"Error importing:  Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                "Successful import!",
                "Unsuccessful import.",
            };

            foreach (var str in strings)
            {
                Out(str);
                System.Threading.Thread.Sleep(300);
            }
        }
        #endregion
    }
}
