using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Windows.Media.Imaging;

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
                            BitmapImage bitmap = new BitmapImage();
                            bitmap.SetSource(file);
                            _images.Add(bitmap);
                        }
                    }
                }
                return _images;
            }
            set { _images = value; }
        }

        public Album() { }
        public Album(string name, string directoryName)
        {
            Name = name;
            DirectoryName = directoryName;
        }

        public void AddImage(Stream image)
        {
            var bitmap = new BitmapImage();
            bitmap.SetSource(image);
            
            _images.Add(bitmap);

            if (!isf.DirectoryExists(DirectoryName))
                isf.CreateDirectory(DirectoryName);

            var wb = new WriteableBitmap(bitmap);
            var FileStream = isf.CreateFile(DirectoryName + "\\" + Guid.NewGuid());
            Extensions.SaveJpeg(wb, FileStream, wb.PixelWidth, wb.PixelHeight, 0, 85);
        }
    }
}
