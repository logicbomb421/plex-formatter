using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Importer.Utilities
{
    /// <summary>A minimal async handler for <see cref="ICommand"/> implementations.</summary>
    public class AsyncCommand : ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;

        /// <summary>Invoked if an exception is thrown when executing the handler.
        /// This is to ensure any errors in the async void Execute call are handled appropriately.</summary>
        public event EventHandler<Exception> OnExecuteError;
        /// <summary>Invoked when the ability for the handler to execute has changed.</summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        /// <summary></summary>
        /// <param name="execute">Invoked when executing the command.</param>
        public AsyncCommand(Action<object> execute) : this(o => true, execute, null) { }
        /// <summary></summary>
        /// <param name="execute">Invoked when executing the command.</param>
        /// <param name="canExecute">Invoked to determine if the command can execute.</param>
        public AsyncCommand(Predicate<object> canExecute, Action<object> execute, EventHandler<Exception> onExecuteError)
        {
            _canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute), "Must supply CanExecute predicate.");
            _execute = execute ?? throw new ArgumentNullException(nameof(execute), "Must supply Execute delegate.");
            OnExecuteError += onExecuteError;
        }

        /// <summary>Determines if the command can execute in the current state.</summary>
        /// <param name="parameter">Any data to pass to the handler.</param>
        /// <returns>True if the handler can execute, else false.</returns>
        public bool CanExecute(object parameter) => _canExecute(parameter);
        
        /// <summary>Executes the handler for the command.</summary>
        /// <param name="parameter">Any data to pass to the handler.</param>
        public async void Execute(object parameter)
        {
            try
            {
                await Task.Factory.StartNew(() => _execute(parameter));
            }
            catch (Exception ex)
            {
                OnExecuteError?.Invoke(this, ex);
            }
        }
    }
}
