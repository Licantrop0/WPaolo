using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Windows.Media.Imaging;
using ExifLib;
using System.Linq;

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

        public void RemovePhoto(BitmapImage photo)
        {
            var index = _images.IndexOf(photo);
            var fileName = isf.GetFileNames(DirectoryName + "\\*")[index];
            isf.DeleteFile(DirectoryName + "\\" + fileName);

            Images.Remove(photo);
        }

        public void RemoveDirectoryContent()
        {
            foreach (var fileName in isf.GetFileNames(DirectoryName + "\\*"))
            {
                isf.DeleteFile(DirectoryName + "\\" + fileName);
            }
            isf.DeleteDirectory(DirectoryName);
        }

        public void AddPhoto(Stream photo, string fileName)
        {
            var rotatedPhoto = RotatePhoto(photo);
            var ms = new MemoryStream();
            rotatedPhoto.SaveJpeg(ms, rotatedPhoto.PixelWidth, rotatedPhoto.PixelHeight, 0, 100);
            var bitmap = new BitmapImage();
            bitmap.SetSource(ms);
            AddPhoto(bitmap, fileName);
            ms.Close();
        }

        public void AddPhoto(BitmapImage photo, string fileName)
        {
            _images.Add(photo);

            var wb = new WriteableBitmap(photo);
            if (!isf.DirectoryExists(DirectoryName)) isf.CreateDirectory(DirectoryName);
            var fs = isf.CreateFile(DirectoryName + "\\" + Guid.NewGuid());
            wb.SaveJpeg(fs, wb.PixelWidth, wb.PixelHeight, 0, 85);
        }

        private WriteableBitmap RotatePhoto(Stream photo)
        {
            var bitmap = new BitmapImage();
            bitmap.SetSource(photo);

            WriteableBitmap wbSource = new WriteableBitmap(bitmap);
            WriteableBitmap wbTarget = null;

            photo.Position = 0;
            switch (ExifReader.ReadJpeg(photo, string.Empty).Orientation)
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
            photo.Close();
            return wbTarget;
        }

        public void MovePhoto(BitmapImage photo, Album album)
        {
            album.AddPhoto(photo, Guid.NewGuid().ToString());
            RemovePhoto(photo);
        }
    }
}
