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

        private ObservableCollection<AlbumPhoto> _photos;
        public ObservableCollection<AlbumPhoto> Photos
        {
            get
            {
                if (_photos == null)
                {
                    _photos = new ObservableCollection<AlbumPhoto>();
                    foreach (var fileName in isf.GetFileNames(DirectoryName + "\\*"))
                    {
                        using (var file = isf.OpenFile(DirectoryName + "\\" + fileName, FileMode.Open))
                            _photos.Add(new AlbumPhoto(fileName, file));
                    }
                }
                return _photos;
            }
        }


        public Album() { }

        public Album(string name, string directoryName)
        {
            Name = name;
            DirectoryName = directoryName;

            if (!isf.DirectoryExists(DirectoryName))
                isf.CreateDirectory(DirectoryName);
        }

        public void AddPhoto(AlbumPhoto photo)
        {
            Photos.Add(photo);
            var wb = new WriteableBitmap(photo.Photo);
            using (var fs = isf.CreateFile(DirectoryName + "\\" + photo.Name))
            {
                //346ms (average 6 samples)
                wb.SaveJpeg(fs, wb.PixelWidth, wb.PixelHeight, 0, 85);
            }
        }

        public void RemovePhoto(AlbumPhoto photo)
        {
            isf.DeleteFile(DirectoryName + "\\" + photo.Name);
            _photos.Remove(photo);
        }

        public void RemoveDirectoryContent()
        {
            foreach (var fileName in isf.GetFileNames(DirectoryName + "\\*"))
                isf.DeleteFile(DirectoryName + "\\" + fileName);

            isf.DeleteDirectory(DirectoryName);
        }

        //TODO: NON PERFORMANTE, da correggere con lo spostamento effettivo del file
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
                library.SavePicture(photo.Name, photo.GetRotatedPhoto(file));
            }
        }
    }
}
