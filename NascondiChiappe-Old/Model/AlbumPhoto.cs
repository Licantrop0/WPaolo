using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using ExifLib;

namespace NascondiChiappe
{
    public class AlbumPhoto : INotifyPropertyChanged
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

        public AlbumPhoto(string name, Stream photo)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (photo == null)
                throw new ArgumentNullException("photo");

            switch (name.Last())
            {
                case '0':
                    RotationAngle = 0d;
                    break;
                case '1':
                    RotationAngle = 90d;
                    break;
                case '2':
                    RotationAngle = 180d;
                    break;
                case '3':
                    RotationAngle = 270d;
                    break;
                default:
                    throw new ArgumentException("FileName does not contain the correct rotation info", "name");
            }

            Name = name;
            Bitmap = new BitmapImage();
            Bitmap.SetSource(photo);
        }

        /// <summary>Aggiungo la info sulla rotation nel nome del file</summary>
        /// <param name="originalFileName">Nome del file originale</param>
        /// <param name="photo">MemoryStream che contiene la foto</param>
        /// <returns>nuovo nome del file con Rotation Info</returns>
        /// <remarks>da eliminare dopo aver implementato un ExifWriter</remarks>
        public static string GetFileNameWithRotation(string originalFileName, Stream photo)
        {
            if (string.IsNullOrEmpty(originalFileName))
                throw new ArgumentNullException("originalFileName");

            if (photo == null)
                throw new ArgumentNullException("photo");

            var fileName = Path.GetFileNameWithoutExtension(originalFileName);

            switch (ExifReader.ReadJpeg(photo, fileName).Orientation)
            {
                case ExifOrientation.TopRight:
                    fileName = fileName.Remove(fileName.Length - 1) + '1';
                    break;
                case ExifOrientation.BottomRight:
                    fileName = fileName.Remove(fileName.Length - 1) + '2';
                    break;
                case ExifOrientation.BottomLeft:
                    fileName = fileName.Remove(fileName.Length - 1) + '3';
                    break;
                default:
                    fileName = fileName.Remove(fileName.Length - 1) + '0';
                    break;
            }
            return fileName;
        }

        //TODO: da eliminare dopo aver implementato un ExifWriter
        public Stream GetRotatedPhoto(Stream photo, double rotationAngle)
        {
            //727ms (average 4 samples)
            var bitmap = new BitmapImage();
            bitmap.SetSource(photo);

            WriteableBitmap wbSource = new WriteableBitmap(bitmap);
            WriteableBitmap wbTarget = null;

            switch (Convert.ToInt32(rotationAngle))
            {
                case 90:
                    wbTarget = new WriteableBitmap(wbSource.PixelHeight, wbSource.PixelWidth);
                    for (int x = 0; x < wbSource.PixelWidth; x++)
                        for (int y = 0; y < wbSource.PixelHeight; y++)
                            wbTarget.Pixels[(wbSource.PixelHeight - y - 1) + x * wbTarget.PixelWidth] =
                                wbSource.Pixels[x + y * wbSource.PixelWidth];
                    break;
                case 180:
                    wbTarget = new WriteableBitmap(wbSource.PixelWidth, wbSource.PixelHeight);
                    for (int x = 0; x < wbSource.PixelWidth; x++)
                        for (int y = 0; y < wbSource.PixelHeight; y++)
                            wbTarget.Pixels[(wbSource.PixelWidth - x - 1) + (wbSource.PixelHeight - y - 1) * wbSource.PixelWidth] =
                                wbSource.Pixels[x + y * wbSource.PixelWidth];
                    break;
                case 270:
                    wbTarget = new WriteableBitmap(wbSource.PixelHeight, wbSource.PixelWidth);
                    for (int x = 0; x < wbSource.PixelWidth; x++)
                        for (int y = 0; y < wbSource.PixelHeight; y++)
                            wbTarget.Pixels[y + (wbSource.PixelWidth - x - 1) * wbTarget.PixelWidth] =
                                wbSource.Pixels[x + y * wbSource.PixelWidth];
                    break;
                default: //0
                    photo.Position = 0;
                    return photo;
            }

            var ms = new MemoryStream();
            wbTarget.SaveJpeg(ms, wbSource.PixelWidth, wbSource.PixelHeight, 0, 85);
            ms.Position = 0;
            return ms;
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
