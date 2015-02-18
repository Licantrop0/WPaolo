﻿using System.ComponentModel;
using System.Xml.Serialization;

namespace Soccerama.Model
{
    public class Shield : INotifyPropertyChanged
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
