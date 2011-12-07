using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace IDecide.Model
{
    [DataContract]
    public class ChoiceGroup : INotifyPropertyChanged
    {
        private string _name;
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

        [DataMember]
        public bool IsDefault { get; set; }

        private bool _isSelected;
        [DataMember]
        public bool IsSelected
        {
            get { return _isSelected; }
            set 
            {
                if(IsSelected == value)
                   return;
                _isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }

        public ChoiceGroup()
        {
            Choices = new ObservableCollection<string>();
        }

        #region INPC Implementation

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        
        #endregion
    }
}