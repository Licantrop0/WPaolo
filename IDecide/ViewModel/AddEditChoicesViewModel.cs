using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using IDecide.Model;
using NascondiChiappe.Helpers;

namespace IDecide.ViewModel
{
    public class AddEditChoicesViewModel : ViewModelBase
    {
        public INavigationService NavigationService { get; set; }

        public bool EditMode { get; private set; }

        private ChoiceGroup _currentChoiceGroup;
        public ChoiceGroup CurrentChoiceGroup
        {
            get { return _currentChoiceGroup; }
            set
            {
                _currentChoiceGroup = value;
                RaisePropertyChanged("CurrentChoiceGroup");
            }
        }

        public AddEditChoicesViewModel()
        {
            Messenger.Default.Register<NotificationMessage<ChoiceGroup>>(
                this, m => ReadMessage(m));
        }

        private void ReadMessage(NotificationMessage<ChoiceGroup> message)
        {
            EditMode = message.Notification == "Edit";
            CurrentChoiceGroup = message.Content;
        }

        private RelayCommand<string> _addChoice;
        public RelayCommand<string> AddChoice
        {
            get { return _addChoice ?? (_addChoice = new RelayCommand<string>(AddChoiceAction)); }
        }
        private void AddChoiceAction(string choice)
        {
           CurrentChoiceGroup.Choices.Insert(0, choice);
        }

        private RelayCommand<string> _deleteChoice;
        public RelayCommand<string> DeleteChoice
        {
            get { return _deleteChoice ?? (_deleteChoice = new RelayCommand<string>(DeleteChoiceAction)); }
        }
        private void DeleteChoiceAction(string choice)
        {
           CurrentChoiceGroup.Choices.Remove(choice);
        }
    }
}
