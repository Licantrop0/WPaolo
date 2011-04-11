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

        private ObservableCollection<BitmapImage> _imageCache;
        public ObservableCollection<BitmapImage> Images
        {
            get
            {
                if (_imageCache == null)
                {
                    _imageCache = new ObservableCollection<BitmapImage>();
                    foreach (var fileName in isf.GetFileNames(DirectoryName + "\\*"))
                    {
                        using (var file = isf.OpenFile(DirectoryName + "\\" + fileName, FileMode.Open))
                        {
                            var bitmap = new BitmapImage();
                            bitmap.SetSource(file);
                            _imageCache.Add(bitmap);
                        }
                    }
                }
                return _imageCache;
            }
        }

        public Album()
        {
            //TODO: Controllo per sicurezza, probabilmente inutile (vedere se da rimuovere)
            if (!isf.DirectoryExists(DirectoryName))
                isf.CreateDirectory(DirectoryName);
        }

        public Album(string name, string directoryName)
        {
            Name = name;
            DirectoryName = directoryName;
            if (!isf.DirectoryExists(DirectoryName))
                isf.CreateDirectory(DirectoryName);
        }

        public void RemovePhoto(BitmapImage photo)
        {
            var index = _imageCache.IndexOf(photo);
            var fileName = isf.GetFileNames(DirectoryName + "\\*")[index];
            isf.DeleteFile(DirectoryName + "\\" + fileName);

            _imageCache.Remove(photo);
        }

        public void RemoveDirectoryContent()
        {
            foreach (var fileName in isf.GetFileNames(DirectoryName + "\\*"))
            {
                isf.DeleteFile(DirectoryName + "\\" + fileName);
            }
            isf.DeleteDirectory(DirectoryName);
        }

        //TODO: NON PERFORMANTE!!
        public void AddPhoto(Stream photo, string fileName)
        {
            WPCommon.ExtensionMethods.StartTrace("Rotating Photo...");
            //2126ms (la prima volta), 894ms, 2279ms, 676ms
            var rotatedPhoto = RotatePhoto(photo);
            WPCommon.ExtensionMethods.EndTrace();

            var ms = new MemoryStream();

            WPCommon.ExtensionMethods.StartTrace("SaveJpg, Quality: 100");
            //684ms, 693ms, 738ms, 699ms
            rotatedPhoto.SaveJpeg(ms, rotatedPhoto.PixelWidth, rotatedPhoto.PixelHeight, 0, 100);
            WPCommon.ExtensionMethods.EndTrace();
            
            WPCommon.ExtensionMethods.StartTrace("Bitmap SetSource (stream)");
            //358ms, 345ms, 351ms, 369ms
            var bitmap = new BitmapImage();
            bitmap.SetSource(ms);
            WPCommon.ExtensionMethods.EndTrace();

            AddPhoto(bitmap, fileName);
            ms.Close();
        }

        public void AddPhoto(BitmapImage photo, string fileName)
        {
            _imageCache.Add(photo);

            //35ms, 48ms, 35ms, 42ms
            var wb = new WriteableBitmap(photo);

            var fs = isf.CreateFile(DirectoryName + "\\" + Guid.NewGuid());
            WPCommon.ExtensionMethods.StartTrace("SaveJpg, Quality: 85");
            //325ms, 341ms, 347ms, 363ms
            wb.SaveJpeg(fs, wb.PixelWidth, wb.PixelHeight, 0, 85);
            WPCommon.ExtensionMethods.EndTrace();
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
            return wbTarget;
        }


        //TODO: NON PERFORMANTEEE, da correggere!
        public void MovePhoto(BitmapImage photo, Album album)
        {
            album.AddPhoto(photo, Guid.NewGuid().ToString());
            RemovePhoto(photo);
        }
    }
}
