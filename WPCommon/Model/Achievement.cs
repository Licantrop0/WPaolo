using System;
using System.ComponentModel;

namespace WPCommon.CommonModel
{
    public class Achievement : INotifyPropertyChanged
    {
        private Uri _imageUrl;
        public Uri ImageUrl
        {
            get
            {
                if (IsUnlocked)
                    return _imageUrl;
                else
                    return new Uri("/WPCommon;component/Img/lucchetto.png", UriKind.Relative);
            }
            set { _imageUrl = value; }
        }
        public string Name { get; set; }
        public string Description { get; set; }
        private bool _isUnlocked;
        public bool IsUnlocked
        {
            get { return _isUnlocked; }
            set
            {
                if (IsUnlocked == value) return;
                _isUnlocked = value;
                RaisePropertyChanged("ImageUrl");
            }
        }

        public Achievement(string name, string description, Uri imageUrl)
        {
            Name = name;
            Description = description;
            ImageUrl = imageUrl;
            IsUnlocked = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

    }
}
