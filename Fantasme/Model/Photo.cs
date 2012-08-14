using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Media.Imaging;
using ExifLib;

namespace NascondiChiappe.Model
{
    public class Photo : INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (Name == value) return;

                _name = value;
                OnPropertyChanged(this, "Name");
            }
        }

        public double RotationAngle { get; set; }
        private BitmapImage _bitmap;
        public BitmapImage Bitmap
        {
            get { return _bitmap; }
            set
            {
                if (Bitmap == value) return;

                _bitmap = value;
                OnPropertyChanged(this, "Bitmap");
            }
        }

        public Photo(string name, Stream photo)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (photo == null)
                throw new ArgumentNullException("photo");

            photo.Position = 0;
            switch (ExifReader.ReadJpeg(photo, name).Orientation)
            {
                case ExifOrientation.TopRight:
                    RotationAngle = 90;
                    break;
                case ExifOrientation.BottomRight:
                    RotationAngle = 180;
                    break;
                case ExifOrientation.BottomLeft:
                    RotationAngle = 270;
                    break;
                default:
                    RotationAngle = 0;
                    break;
            }

            Name = name;
            Bitmap = new BitmapImage() { CreateOptions = BitmapCreateOptions.BackgroundCreation };            
            Bitmap.SetSource(photo);
        }

        public Photo(string name, BitmapImage photo)
        {
            Name = name;
            Bitmap = photo;
            RotationAngle = 0;
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(object sender, string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(sender, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
