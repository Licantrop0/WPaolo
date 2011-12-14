using System;
using System.Windows.Input;

namespace HaiSmarrito.Helpers
{
    public class RelayCommand<T> : ICommand
    {
        public event EventHandler CanExecuteChanged;
        
        readonly Action<T> _execute = null;
        readonly Predicate<T> _canExecute = null;

        public RelayCommand(Action<T> execute, Predicate<T> canExecute= null)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        public void UpdateCanExecuteCommand()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, new EventArgs());
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute((T)parameter);
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }
    }
 

}
