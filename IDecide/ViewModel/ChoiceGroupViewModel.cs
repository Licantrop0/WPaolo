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
            Model = model;
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
                new NotificationMessage<ChoiceGroup>(Model, "edit"));
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
            string locGroupName = DefaultChoices.ResourceManager.GetString(Model.Name) ?? Model.Name;
            if (MessageBox.Show(string.Format(AppResources.RemoveGroup, locGroupName),
                AppResources.Confirm, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                AppContext.Groups.Remove(this);

                //se ci sono elementi nella lista e di questi non ce n'è nessuno selezionato, seleziono il primo
                if (AppContext.Groups.Count > 0 && !AppContext.Groups.Any(g => g.Model.IsSelected))
                {
                    AppContext.Groups.First().Model.IsSelected = true;
                }
            }
        }
    }
}