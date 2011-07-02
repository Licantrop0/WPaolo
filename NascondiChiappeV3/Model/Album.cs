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

namespace NascondiChiappe.Model
{
    [DataContract]
    public class Album : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _name;
        [DataMember]
        public string Name
        {
            get { return _name; }
            set
            {
                if (Name == value)
                    return;
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        private string _directoryName;
        [DataMember]
        public string DirectoryName
        {
            get { return _directoryName; }
            set
            {
                if (DirectoryName == value)
                    return;

                _directoryName = value;

                if (!isf.DirectoryExists(DirectoryName))
                    isf.CreateDirectory(DirectoryName);
            }
        }

        IsolatedStorageFile isf { get { return IsolatedStorageFile.GetUserStoreForApplication(); } }

        private ObservableCollection<Photo> _photos;
        public ObservableCollection<Photo> Photos
        {
            get
            {
                if (_photos == null)
                {
                    _photos = new ObservableCollection<Photo>();
                    foreach (var fileName in isf.GetFileNames(DirectoryName + "\\*"))
                    {
                        if (fileName.EndsWith("html")) continue;
                        using (var file = isf.OpenFile(DirectoryName + "\\" + fileName, FileMode.Open))
                            _photos.Add(new Photo(fileName, file));
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
        }

        public bool AddPhoto(Photo photo)
        {
            Photos.Add(photo);
            try
            {
                //Implementare un ExifWriter o sperare che il metodo SaveJpeg supporti l'orientation un giorno
                var wb = new WriteableBitmap(photo.Bitmap);
                using (var fs = isf.CreateFile(DirectoryName + "\\" + photo.Name))
                {
                    //346ms (average 6 samples)
                    wb.SaveJpeg(fs, wb.PixelWidth, wb.PixelHeight, 0, 85);
                }
            }
            catch (IsolatedStorageException)
            {
                Photos.Remove(photo);
                return false;
            }
            return true;
        }

        public void RemovePhoto(Photo photo)
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
        public void MovePhoto(Photo photo, Album album)
        {
            album.AddPhoto(photo);
            RemovePhoto(photo);
        }

        public bool CopyToMediaLibrary(Photo photo)
        {
            try
            {
                using (var file = isf.OpenFile(DirectoryName + "\\" + photo.Name, FileMode.Open, FileAccess.Read))
                {
                    MediaLibrary library = new MediaLibrary();
                    library.SavePicture(photo.Name, photo.GetRotatedPhoto(file, photo.RotationAngle));
                }
            }
            catch (IsolatedStorageException)
            {
                return false;
            }
            return true;
        }
    }
}
