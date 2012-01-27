using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Media.Imaging;
using NientePanico.Helpers;

namespace NientePanico.Model
{
    [DataContract]
    public class CardData : INotifyPropertyChanged
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Code { get; set; }

        private DateTime _expire = DateTime.Today;
        [DataMember]
        public DateTime Expire
        {
            get { return _expire; }
            set
            {
                if (Expire == value) return;
                _expire = value;
                RaisePropertyChanged("Expire");
            }
        }

        [DataMember]
        public string Pin { get; set; }
        [DataMember]
        public string FrontImageName { get; set; }
        [DataMember]
        public string BackImageName { get; set; }

        private BitmapImage _frontBitmap;
        public BitmapImage FrontBitmap
        {
            get
            {
                if (_frontBitmap == null)
                    _frontBitmap = PhotoHelper.GetPhoto(FrontImageName);

                return _frontBitmap;
            }
        }

        private BitmapImage _backBitmap;
        public BitmapImage BackBitmap
        {
            get
            {
                if (_backBitmap == null)
                    _backBitmap = PhotoHelper.GetPhoto(BackImageName);

                return _backBitmap;
            }
        }

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
