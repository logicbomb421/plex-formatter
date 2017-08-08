﻿using System;
using System.Windows.Input;

namespace Importer.Utilities
{
    //minimal implementation of Josh Smith's RelayCommand
    //https://msdn.microsoft.com/en-us/magazine/dd419663.aspx
    public class RelayCommand : ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        /// <param name="execute">Invoked when executing the command.</param>
        public RelayCommand(Action<object> execute) : this(o => true, execute) { }
        /// <param name="execute">Invoked when executing the command.</param>
        /// <param name="canExecute">Invoked to determine if the command can execute.</param>
        public RelayCommand(Predicate<object> canExecute, Action<object> execute)
        {
            _canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute), "Must supply CanExecute predicate.");
            _execute = execute ?? throw new ArgumentNullException(nameof(execute), "Must supply Execute delegate.");
        }

        public bool CanExecute(object parameter) => _canExecute(parameter);
        public void Execute(object parameter) => _execute(parameter);
    }
}
