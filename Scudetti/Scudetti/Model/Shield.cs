using System.ComponentModel;
using System.Xml.Serialization;

namespace Scudetti.Model
{
    public class Shield : INotifyPropertyChanged
    {
        [XmlAttribute]
        public int Id { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string Image { get; set; }

        [XmlAttribute]
        public int Level { get; set; }

        private bool _isValidated = false;
        [XmlAttribute]
        public bool IsUnlocked
        {
            get { return _isValidated; }
            set
            {
                if (IsUnlocked == value) return;
                _isValidated = value;
                RaisePropertyChanged("IsValidated");
            }
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
