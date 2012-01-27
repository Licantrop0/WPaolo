using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Media.Imaging;
using NientePanico.Helpers;
using System.IO;

namespace NientePanico.Model
{
    [DataContract]
    public class CardData : INotifyPropertyChanged
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public DateTime Expire { get; set; }

        [DataMember]
        public string Pin { get; set; }

        public string _frontImageName;
        [DataMember]
        public string FrontImageName
        {
            get { return _frontImageName; }
            set
            {
                _frontImageName = value;
                RaisePropertyChanged("FrontBitmap");
            }
        }

        public string _backImageName;
        [DataMember]
        public string BackImageName
        {
            get { return _backImageName; }
            set
            {
                _backImageName = value;
                RaisePropertyChanged("BackBitmap");
            }
        }

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

        public CardData()
        {
            Expire = DateTime.Today;
        }

        /// <summary>
        /// Cache the image in the private BitmapImage fields
        /// </summary>
        /// <param name="image">The MemoryStream of the Image</param>
        /// <param name="isFront">True if Front image, False if Back image</param>
        public void CacheImage(Stream image, bool isFront)
        {
            if (isFront)
            {
                _frontBitmap = new BitmapImage();
                _frontBitmap.SetSource(image);
            }
            else
            {
                _backBitmap = new BitmapImage();
                _backBitmap.SetSource(image);
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
