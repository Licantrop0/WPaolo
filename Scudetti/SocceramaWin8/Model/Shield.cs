using System.ComponentModel;
using System.Xml.Serialization;
using GalaSoft.MvvmLight;

namespace Scudetti.Model
{
    public class Shield : ObservableObject
    {
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
    }
}
