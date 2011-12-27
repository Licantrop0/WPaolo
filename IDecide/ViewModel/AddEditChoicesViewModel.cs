using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using IDecide.Model;
using NascondiChiappe.Helpers;
using IDecide.Localization;

namespace IDecide.ViewModel
{
    public class AddEditChoicesViewModel : ViewModelBase
    {
        public INavigationService NavigationService { get; set; }

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
            Messenger.Default.Register<ChoiceGroup>(
                this, cg => CurrentChoiceGroup = cg);
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
            set {
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
    }
}
