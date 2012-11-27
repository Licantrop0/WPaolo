using System.ComponentModel;
using System.Xml.Serialization;

namespace Scudetti.Model
{
    public class Shield : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlAttribute]
        public string Id { get; set; }

        [XmlElement]
        public string[] Names { get; set; }

        [XmlAttribute]
        public string Image { get; set; }

        [XmlAttribute]
        public string Hint { get; set; }

        [XmlAttribute]
        public int Level { get; set; }

        private bool _isValidated = false;
        [XmlAttribute]
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

        public override string ToString()
        {
            return string.Format("{0}, Lv: {1}", Names[0], Level);
        }

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
