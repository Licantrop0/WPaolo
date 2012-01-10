using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using IDecide.Localization;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace IDecide.ViewModel
{
    [DataContract]
    public class ChoiceGroupViewModel : ViewModelBase
    {
        private string _name = string.Empty;
        [DataMember]
        public string Name
        {
            get { return _name; }
            set
            {
                if (Name == value)
                    return;
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        [DataMember]
        public ObservableCollection<string> Choices { get; set; }

        private bool _isSelected;
        [DataMember]
        public bool IsSelected
        {
            get { return _isSelected; }
            set 
            {
                if(IsSelected == value) return;
                _isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }

        public ChoiceGroupViewModel()
        {
            Choices = new ObservableCollection<string>();
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
            Messenger.Default.Send<NotificationMessage<ChoiceGroupViewModel>>(
                new NotificationMessage<ChoiceGroupViewModel>(this, "edit"));
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
            string locGroupName = DefaultChoices.ResourceManager.GetString(this.Name) ?? this.Name;
            if (MessageBox.Show(string.Format(AppResources.RemoveGroup, locGroupName),
                AppResources.Confirm, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                AppContext.Groups.Remove(this);

                //se ci sono elementi nella lista e di questi non ce n'è nessuno selezionato, seleziono il primo
                if (AppContext.Groups.Count > 0 && !AppContext.Groups.Any(g => g.IsSelected))
                {
                    AppContext.Groups.First().IsSelected = true;
                }
            }
        }
    }
}