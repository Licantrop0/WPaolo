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
        public bool ButtonEnabled { get { return !Model.IsDefault; } }

        public Visibility CanDeleteVisibility
        {
            get
            {
                return Model.IsDefault ?
                    Visibility.Collapsed :
                    Visibility.Visible;
            }
        }

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
            if (MessageBox.Show(string.Format(AppResources.Cancel, Model.Name),
                AppResources.Confirm, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (Model.IsSelected)
                    AppContext.Groups.First(g => g.Model.IsDefault).Model.IsSelected = true;

                AppContext.Groups.Remove(this);
            }
        }
    }
}
