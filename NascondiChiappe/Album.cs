using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Windows.Media.Imaging;
using ExifLib;
using System.Linq;
using Microsoft.Xna.Framework.Media;
using System.ComponentModel;

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

        private ObservableCollection<AlbumPhoto> _imageCache;
        public ObservableCollection<AlbumPhoto> Images
        {
            get
            {
                if (_imageCache == null)
                {
                    _imageCache = new ObservableCollection<AlbumPhoto>();
                    foreach (var fileName in isf.GetFileNames(DirectoryName + "\\*"))
                    {
                        using (var file = isf.OpenFile(DirectoryName + "\\" + fileName, FileMode.Open))
                            _imageCache.Add(new AlbumPhoto(fileName, file));
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

        public void AddPhoto(AlbumPhoto photo)
        {
            Images.Add(photo);
            var wb = new WriteableBitmap(photo.Image);
            using (var fs = isf.CreateFile(DirectoryName + "\\" + photo.Name))
            {
                //346ms (average 6 samples)
                wb.SaveJpeg(fs, wb.PixelWidth, wb.PixelHeight, 0, 85);
            }
        }

        public void RemovePhoto(AlbumPhoto photo)
        {
            isf.DeleteFile(DirectoryName + "\\" + photo.Name);

            _imageCache.Remove(photo);
        }

        public void RemoveDirectoryContent()
        {
            foreach (var fileName in isf.GetFileNames(DirectoryName + "\\*"))
                isf.DeleteFile(DirectoryName + "\\" + fileName);

            isf.DeleteDirectory(DirectoryName);
        }

        //TODO: NON PERFORMANTEEE, da correggere!
        public void MovePhoto(AlbumPhoto photo, Album album)
        {
            album.AddPhoto(photo);
            RemovePhoto(photo);
        }

        public void CopyToMediaLibrary(AlbumPhoto photo)
        {
            using (var file = isf.OpenFile(DirectoryName + "\\" + photo.Name, FileMode.Open, FileAccess.Read))
            {
                MediaLibrary library = new MediaLibrary();
                library.SavePicture(photo.Name, file);
            }
        }
    }
}
