using System;
using System.Windows.Input;

namespace SheldonMix
{
    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;


        public RelayCommand(
                            Action<object> execute,
                            Predicate<object> canExecute = null)
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
            return _canExecute == null ? true : _canExecute(parameter);
        }


        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
