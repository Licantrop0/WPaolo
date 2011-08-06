using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.ComponentModel;

namespace IDecide.Model
{
    [DataContract]
    public class ChoiceGroup : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

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
        public IEnumerable<string> Choices { get; set; }
        
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
        [DataMember]
        public bool IsDefault { get; set; }

        public ChoiceGroup()
        {
            this.IsSelected = false;
            this.IsDefault = false;
            Choices = new List<string>();
        }
    }
}
