using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Media.Imaging;
using NientePanico.Helpers;

namespace NientePanico.ViewModel
{
    [DataContract]
    public class CardDataViewModel : INotifyPropertyChanged
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public DateTime Expire { get; set; }
        [DataMember]
        public string ImageName { get; set; }

        private BitmapImage _bitmap;
        public BitmapImage Bitmap
        {
            get
            {
                if (_bitmap == null)
                    _bitmap = PhotoHelper.GetPhoto(ImageName);

                return _bitmap;
            }
        }

        public bool SetPhoto(Stream stream)
        {
            var _bitmap = new BitmapImage();
            _bitmap.SetSource(stream);
            RaisePropertyChanged("Bitmap");

            //Salvo la foto nell'IS
            ImageName = Guid.NewGuid().ToString();
            return PhotoHelper.SavePhoto(ImageName, _bitmap);
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
