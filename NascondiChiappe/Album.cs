using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Windows.Media.Imaging;
using ExifLib;

namespace NascondiChiappe
{
    [DataContract]
    public class Album
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string DirectoryName { get; set; }

        IsolatedStorageFile isf { get { return IsolatedStorageFile.GetUserStoreForApplication(); } }

        private ObservableCollection<BitmapImage> _images;
        public ObservableCollection<BitmapImage> Images
        {
            get
            {
                if (_images == null)
                {
                    _images = new ObservableCollection<BitmapImage>();
                    if (isf.DirectoryExists(DirectoryName))
                    {
                        foreach (var fileName in isf.GetFileNames(DirectoryName + "\\*"))
                        {
                            var file = isf.OpenFile(DirectoryName + "\\" + fileName, FileMode.Open);
                            var bitmap = new BitmapImage();
                            bitmap.SetSource(file);
                            _images.Add(bitmap);
                            file.Close();
                        }
                    }
                }
                return _images;
            }
        }

        public Album() { }
        public Album(string name, string directoryName)
        {
            Name = name;
            DirectoryName = directoryName;
        }

        public void RemoveImage(BitmapImage image)
        {
            var index = _images.IndexOf(image);
            var fileName =  isf.GetFileNames(DirectoryName + "\\*")[index];
            isf.DeleteFile(DirectoryName + "\\" + fileName);
            
            Images.Remove(image);
        }

        public void RemoveDirectoryContent()
        {
            foreach (var fileName in isf.GetFileNames(DirectoryName + "\\*"))
            {
                isf.DeleteFile(DirectoryName + "\\" + fileName);
            }
            isf.DeleteDirectory(DirectoryName);
        }

        public void AddImage(Stream image, string fileName)
        {
            var bitmap = new BitmapImage();
            bitmap.SetSource(image);

            WriteableBitmap wbSource = new WriteableBitmap(bitmap);
            WriteableBitmap wbTarget = null;

            image.Position = 0;
            switch (ExifReader.ReadJpeg(image, fileName).Orientation)
            {
                case ExifOrientation.TopRight: //90°
                    wbTarget = new WriteableBitmap(wbSource.PixelHeight, wbSource.PixelWidth);
                    for (int x = 0; x < wbSource.PixelWidth; x++)
                        for (int y = 0; y < wbSource.PixelHeight; y++)
                            wbTarget.Pixels[(wbSource.PixelHeight - y - 1) + x * wbTarget.PixelWidth] = wbSource.Pixels[x + y * wbSource.PixelWidth];
                    break;
                case ExifOrientation.BottomRight: //180°
                    wbTarget = new WriteableBitmap(wbSource.PixelWidth, wbSource.PixelHeight);
                    for (int x = 0; x < wbSource.PixelWidth; x++)
                        for (int y = 0; y < wbSource.PixelHeight; y++)
                            wbTarget.Pixels[(wbSource.PixelWidth - x - 1) + (wbSource.PixelHeight - y - 1) * wbSource.PixelWidth] = wbSource.Pixels[x + y * wbSource.PixelWidth];
                    break;
                case ExifOrientation.BottomLeft: //270°
                    wbTarget = new WriteableBitmap(wbSource.PixelHeight, wbSource.PixelWidth);
                    for (int x = 0; x < wbSource.PixelWidth; x++)
                        for (int y = 0; y < wbSource.PixelHeight; y++)
                            wbTarget.Pixels[y + (wbSource.PixelWidth - x - 1) * wbTarget.PixelWidth] = wbSource.Pixels[x + y * wbSource.PixelWidth];
                    break;
                default: //0°
                    wbTarget = wbSource;
                    break;
            }
            image.Close();

            if (!isf.DirectoryExists(DirectoryName)) isf.CreateDirectory(DirectoryName);
            var fs = isf.CreateFile(DirectoryName + "\\" + Guid.NewGuid());
            wbTarget.SaveJpeg(fs, wbTarget.PixelWidth, wbTarget.PixelHeight, 0, 85);
            fs.Position = 0;
            bitmap.SetSource(fs);
            fs.Close();
             _images.Add(bitmap);
       }
    }
}
