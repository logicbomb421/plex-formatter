using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Importer.Utilities
{
    //minimal + async implementation of Josh Smith's RelayCommand
    //https://msdn.microsoft.com/en-us/magazine/dd419663.aspx
    public class RelayCommand : ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Func<object, Task> _execute;

        public event EventHandler<Exception> OnExecuteError;
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        /// <param name="execute">Invoked when executing the command.</param>
        public RelayCommand(Func<object, Task> execute) : this(o => true, execute, null) { }
        /// <param name="execute">Invoked when executing the command.</param>
        /// <param name="canExecute">Invoked to determine if the command can execute.</param>
        public RelayCommand(Predicate<object> canExecute, Func<object, Task> execute, EventHandler<Exception> onExecuteError)
        {
            _canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute), "Must supply CanExecute predicate.");
            _execute = execute ?? throw new ArgumentNullException(nameof(execute), "Must supply Execute delegate.");
            OnExecuteError += onExecuteError;
        }

        public bool CanExecute(object parameter) => _canExecute(parameter);
        public async void Execute(object parameter)
        {
            try
            {
                await _execute(parameter);
            }
            catch (Exception ex)
            {
                OnExecuteError?.Invoke(this, ex);
            }
        }
    }
}
