using System.Windows;

namespace Importer
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        SettingsWindowViewModel _viewModel;

        public SettingsWindow()
        {
            InitializeComponent();
            DataContext = _viewModel = new SettingsWindowViewModel();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveSettingsToDisk();
            MessageBox.Show("Settings saved successfully!", "Success!", MessageBoxButton.OK);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.IsModified && MessageBox.Show("There are unsaved changes, are you shure you wish to close?", "Save Changes?", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;
            Close();
        }
    }
}
