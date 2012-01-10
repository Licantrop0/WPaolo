using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using NascondiChiappe.Helpers;
using IDecide.Localization;
using System.Linq;

namespace IDecide.ViewModel
{
    public class AddEditChoicesViewModel : ViewModelBase
    {
        private bool IsEditMode;

        private ChoiceGroupViewModel _currentChoiceGroup;
        public ChoiceGroupViewModel CurrentChoiceGroup
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
            Messenger.Default
                .Register<NotificationMessage<ChoiceGroupViewModel>>(this, m =>
                {
                    CurrentChoiceGroup = m.Content;
                    IsEditMode = m.Notification == "edit";
                });
        }

        public string LocGroupName
        {
            get
            {
                return DefaultChoices.ResourceManager.GetString(
                    CurrentChoiceGroup.Name) ?? CurrentChoiceGroup.Name;
            }
            set
            {
                if (value == LocGroupName) return;
                CurrentChoiceGroup.Name = value;
            }
        }

        private string _choice;
        public string Choice
        {
            get { return _choice; }
            set
            {
                _choice = value;
                RaisePropertyChanged("Choice");
            }
        }

        private RelayCommand _addChoice;
        public RelayCommand AddChoice
        {
            get { return _addChoice ?? (_addChoice = new RelayCommand(AddChoiceAction)); }
        }
        private void AddChoiceAction()
        {
            CurrentChoiceGroup.Choices.Insert(0, Choice);
            Choice = string.Empty;
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

        public void SaveGroup()
        {
            //Se è un nuovo gruppo e sono state inserite scelte, allora lo aggiunge alla lista
            if (!IsEditMode && CurrentChoiceGroup.Choices.Any())
            {
                AppContext.Groups.Insert(0, CurrentChoiceGroup);
            }
        }
    }
}
