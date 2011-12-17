using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using IDecide.Localization;
using IDecide.Model;

namespace IDecide.ViewModel
{
    public class ChoiceGroupViewModel : ViewModelBase
    {
        public ChoiceGroup Model { get; private set; }

        public ChoiceGroupViewModel(ChoiceGroup model)
        {
            this.Model = model;
        }

        private RelayCommand _editGroup;
        public RelayCommand EditGroup
        {
            get
            {
                return _editGroup ?? (_editGroup = new RelayCommand(EditGroupAction));
            }
        }
        private void EditGroupAction()
        {
            Messenger.Default.Send<NotificationMessage<ChoiceGroup>>(
                new NotificationMessage<ChoiceGroup>(Model, "Edit"));
        }

        private RelayCommand _removeGroup;
        public RelayCommand RemoveGroup
        {
            get
            {
                return _removeGroup ?? (_removeGroup = new RelayCommand(RemoveGroupAction));
            }
        }
        private void RemoveGroupAction()
        {
            if (MessageBox.Show(string.Format(AppResources.RemoveGroup, Model.Name),
                AppResources.Confirm, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                AppContext.Groups.Remove(this);
                if (AppContext.Groups.Count > 0 && !AppContext.Groups.Any(g => g.Model.IsSelected))
                {
                    AppContext.Groups.First().Model.IsSelected = true;
                }
            }
        }
    }
}
