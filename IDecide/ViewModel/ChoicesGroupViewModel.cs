﻿using System.Collections.ObjectModel;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using IDecide.Localization;
using NascondiChiappe.Helpers;
using System;
using System.Linq;

namespace IDecide.ViewModel
{
    public class ChoicesGroupViewModel : ViewModelBase
    {
        public INavigationService NavigationService { get; set; }

        public ObservableCollection<ChoiceGroupViewModel> Groups
        {
            get { return AppContext.Groups; }
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
            var choiceGroup = new ChoiceGroupViewModel(new Model.ChoiceGroup() { IsSelected = true });
            AppContext.Groups.Single(g => g.Model.IsSelected).Model.IsSelected = false;
            AppContext.Groups.Add(choiceGroup);
            Messenger.Default.Send<NotificationMessage<ChoiceGroupViewModel>>(
                new NotificationMessage<ChoiceGroupViewModel>(choiceGroup, "Add"));
            NavigationService.Navigate(new Uri("/View/AddEditChoicesPage.xaml", UriKind.Relative));
        }

        private RelayCommand<ChoiceGroupViewModel> _editGroup;
        public RelayCommand<ChoiceGroupViewModel> EditGroup
        {
            get
            {
                return _editGroup ?? (_editGroup = new RelayCommand<ChoiceGroupViewModel>(EditGroupAction));
            }
        }
        private void EditGroupAction(ChoiceGroupViewModel group)
        {
            Messenger.Default.Send<NotificationMessage<ChoiceGroupViewModel>>(
                new NotificationMessage<ChoiceGroupViewModel>(group, "Edit"));
            NavigationService.Navigate(new Uri("/View/AddEditChoicesPage.xaml", UriKind.Relative));
        }

        private RelayCommand<ChoiceGroupViewModel> _deleteGroup;
        public RelayCommand<ChoiceGroupViewModel> DeleteGroup
        {
            get
            {
                return _deleteGroup ?? (_deleteGroup = new RelayCommand<ChoiceGroupViewModel>(DeleteGroupAction));
            }
        }
        private void DeleteGroupAction(ChoiceGroupViewModel group)
        {
            if (MessageBox.Show(string.Format(AppResources.Cancel, group.Model.Name),
                AppResources.Confirm, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (group.Model.IsSelected)
                    Groups.First(g => g.Model.IsDefault).Model.IsSelected = true;

                Groups.Remove(group);
            }
        }

    }
}
