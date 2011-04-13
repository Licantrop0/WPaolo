using System.ComponentModel;
using System.IO;
using System.Windows;
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
                if (value != _name)
                {
                    _name = value;
                    onPropertyChanged(this, "Name");
                }
            }
        }

        private BitmapImage _image;
        public BitmapImage Image { get { return _image; } }
        public double RotationAngle { get; set; }

        public AlbumPhoto(string name, Stream image)
        {
            Name = name;
            switch (ExifReader.ReadJpeg(image, name).Orientation)
            {
                case ExifOrientation.TopRight:
                    RotationAngle = 90d;
                    break;
                case ExifOrientation.BottomRight:
                    RotationAngle = 180d;
                    break;
                case ExifOrientation.BottomLeft:
                    RotationAngle = 270d;
                    break;
                default:
                    RotationAngle = 0d;
                    break;
            }

            _image = new BitmapImage();
            _image.SetSource(image);
        }

        private WriteableBitmap Rotate(Stream photo)
        {
            WPCommon.ExtensionMethods.StartTrace("Bitmap SetSource (photo)");
            //727ms (average 4 samples)
            var bitmap = new BitmapImage();
            bitmap.SetSource(photo);
            WPCommon.ExtensionMethods.EndTrace();

            WriteableBitmap wbSource = new WriteableBitmap(bitmap);
            WriteableBitmap wbTarget = null;

            photo.Position = 0;
            //1447 (average 3 samples actual rotation)
            switch (ExifReader.ReadJpeg(photo, string.Empty).Orientation)
            {
                case ExifOrientation.TopRight: //90°
                    wbTarget = new WriteableBitmap(wbSource.PixelHeight, wbSource.PixelWidth);
                    for (int x = 0; x < wbSource.PixelWidth; x++)
                        for (int y = 0; y < wbSource.PixelHeight; y++)
                            wbTarget.Pixels[(wbSource.PixelHeight - y - 1) + x * wbTarget.PixelWidth] =
                                wbSource.Pixels[x + y * wbSource.PixelWidth];
                    break;
                case ExifOrientation.BottomRight: //180°
                    wbTarget = new WriteableBitmap(wbSource.PixelWidth, wbSource.PixelHeight);
                    for (int x = 0; x < wbSource.PixelWidth; x++)
                        for (int y = 0; y < wbSource.PixelHeight; y++)
                            wbTarget.Pixels[(wbSource.PixelWidth - x - 1) + (wbSource.PixelHeight - y - 1) * wbSource.PixelWidth] =
                                wbSource.Pixels[x + y * wbSource.PixelWidth];
                    break;
                case ExifOrientation.BottomLeft: //270°
                    wbTarget = new WriteableBitmap(wbSource.PixelHeight, wbSource.PixelWidth);
                    for (int x = 0; x < wbSource.PixelWidth; x++)
                        for (int y = 0; y < wbSource.PixelHeight; y++)
                            wbTarget.Pixels[y + (wbSource.PixelWidth - x - 1) * wbTarget.PixelWidth] =
                                wbSource.Pixels[x + y * wbSource.PixelWidth];
                    break;
                default: //0°
                    wbTarget = wbSource;
                    break;
            }
            return wbTarget;
        }


        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void onPropertyChanged(object sender, string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(sender, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
