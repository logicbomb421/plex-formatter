using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Importer.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public bool IsModified { get; protected set; }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
