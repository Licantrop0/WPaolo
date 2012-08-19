using System.ComponentModel;
using System.Runtime.Serialization;

namespace NascondiChiappe.Model
{
    [DataContract]
    public class Photo : INotifyPropertyChanged
    {
        private string _path;
        [DataMember]
        public string Path
        {
            get { return _path; }
            set
            {
                if (Path == value) return;
                _path = value;
                RaisePropertyChanged("Path");
            }
        }

        private string _album;
        [DataMember]
        public string Album
        {
            get { return _album; }
            set
            {
                if (Album == value) return;
                _album = value;
                RaisePropertyChanged("Album");
            }
        }

        //private bool _isSelected;
        //public bool IsSelected
        //{
        //    get { return _isSelected; }
        //    set
        //    {
        //        if (IsSelected == value) return;
        //        _isSelected = value;
        //        RaisePropertyChanged("IsSelected");
        //    }
        //}

        public Photo()
        { }

        public Photo(string path, string directory)
        {
            Path = path;
            Album = directory;
            //IsSelected = false;
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
