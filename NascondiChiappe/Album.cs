using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.IO;
using System.IO.IsolatedStorage;

namespace NascondiChiappe
{
    [DataContract]
    public class Album
    {
        [DataMember]
        public string Name { get; set; }

        IsolatedStorageFile isf { get { return IsolatedStorageFile.GetUserStoreForApplication(); } }

        private ObservableCollection<BitmapImage> _images;
        public ObservableCollection<BitmapImage> Images
        {
            get
            {
                if (_images == null)
                {
                    _images = new ObservableCollection<BitmapImage>();
                    if (isf.DirectoryExists(Name))
                    {
                        foreach (var fileName in isf.GetFileNames(Name + "\\*"))
                        {
                            var file = isf.OpenFile(Name + "\\" + fileName, FileMode.Open);
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
        public Album(string name)
        {
            Name = name;
        }

        public void AddImage(Stream image)
        {
            var bitmap = new BitmapImage();
            bitmap.SetSource(image);
            
            _images.Add(bitmap);

            if (!isf.DirectoryExists(Name))
                isf.CreateDirectory(Name);

            var wb = new WriteableBitmap(bitmap);
            var FileStream = isf.CreateFile(Name + "\\" + Guid.NewGuid());
            Extensions.SaveJpeg(wb, FileStream, wb.PixelWidth, wb.PixelHeight, 0, 85);
        }
    }
}
