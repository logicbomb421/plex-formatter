using System.ComponentModel;

namespace Importer.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public bool IsModified { get; protected set; }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
