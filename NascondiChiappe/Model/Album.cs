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
using GalaSoft.MvvmLight;

namespace NascondiChiappe
{
    [DataContract]
    public class Album : INotifyPropertyChanged
    {
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

        private string _name;
        [DataMember]
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value) return;

                _name = value;
                OnPropertyChanged(this, "Name");
            }
        }

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
                        if (fileName.EndsWith("html")) continue;
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

            //Implementare un ExifWriter o sperare che il metodo SaveJpeg supporti l'orientation un giorno
            var wb = new WriteableBitmap(photo.Bitmap);
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
