using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
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
            var choiceGroup = new ChoiceGroupViewModel() { IsSelected = true };
            Messenger.Default.Send<NotificationMessage<ChoiceGroupViewModel>>(
                new NotificationMessage<ChoiceGroupViewModel>(choiceGroup, "add"));
            NavigationService.Navigate(new Uri("/View/AddEditChoicesPage.xaml", UriKind.Relative));
        }
    }
}