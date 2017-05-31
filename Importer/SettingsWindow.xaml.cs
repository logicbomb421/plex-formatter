using System.Windows;
using static Importer.Settings;

namespace Importer
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            txtMovies.Text = MovieRoot;
            txtTvShows.Text = TVRoot;
            txtPhotos.Text = PhotoRoot;
            txtMusic.Text = MusicRoot;
            chkRefreshOnImport.IsChecked = RefreshOnImport;
            chkDeleteSource.IsChecked = DeleteSourceFiles;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            MovieRoot = txtMovies.Text;
            TVRoot = txtTvShows.Text;
            PhotoRoot = txtPhotos.Text;
            MusicRoot = txtMusic.Text;
            RefreshOnImport = chkRefreshOnImport.IsChecked.GetValueOrDefault(false);
            DeleteSourceFiles = chkDeleteSource.IsChecked.GetValueOrDefault(false);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
