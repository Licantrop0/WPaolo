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
