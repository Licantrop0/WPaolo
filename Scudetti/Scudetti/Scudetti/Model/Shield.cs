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
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Scudetti.Model
{
    [DataContract]
    public class Shield : INotifyPropertyChanged
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }
        
        [DataMember]
        public string Image { get; set; }
        
        [DataMember]
        public int Level { get; set; }

        private bool _isValidated = false;
        [DataMember]
        public bool IsValidated
        {
            get { return _isValidated; }
            set
            {
                if (IsValidated == value) return;
                _isValidated = value;
                RaisePropertyChanged("IsValidated");
            }
        }

        public Shield(int id, string name, int level, string image)
        {
            Id = id;
            Name = name;
            Image = image;
            Level = level;
        }

        #region Interface Implementations
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

    }
}
