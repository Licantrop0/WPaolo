using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using NascondiChiappe.Helpers;

namespace IDecide.ViewModel
{
    public class AddEditChoicesViewModel : ViewModelBase
    {
        public INavigationService NavigationService { get; set; }

        private ChoiceGroupViewModel _currentChoiceGroup;
        public ChoiceGroupViewModel CurrentChoiceGroup
        {
            get { return _currentChoiceGroup; }
            set { _currentChoiceGroup = value;
            RaisePropertyChanged("CurrentChoiceGroup");
            }
        }

        public AddEditChoicesViewModel()
        {
            Messenger.Default.Register<ChoiceGroupViewModel>(this,
                "AddOrEdit", choiceGroup => CurrentChoiceGroup = choiceGroup);            
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
