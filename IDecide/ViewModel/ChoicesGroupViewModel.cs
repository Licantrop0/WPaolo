using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using IDecide.Model;
using NascondiChiappe.Helpers;

namespace IDecide.ViewModel
{
    public class ChoicesGroupViewModel : ViewModelBase
    {
        public INavigationService NavigationService { get; set; }

        public ObservableCollection<ChoiceGroupViewModel> Groups
        {
            get { return IsInDesignMode ? AppContext.GetDefaultChoices() : AppContext.Groups; }
        }

        private RelayCommand _addGroup;
        public RelayCommand AddGroup
        {
            get
            {
                return _addGroup ?? (_addGroup = new RelayCommand(AddGroupAction));
            }
        }
        private void AddGroupAction()
        {
            //Creo il nuovo gruppo, lo seleziono e lo aggiungo alla lista
            var choiceGroup = new ChoiceGroup() { IsSelected = true };
            Groups.Insert(0, new ChoiceGroupViewModel(choiceGroup));

            Messenger.Default.Send<ChoiceGroup>(choiceGroup);

            NavigationService.Navigate(new Uri("/View/AddEditChoicesPage.xaml", UriKind.Relative));
        }
    }
}