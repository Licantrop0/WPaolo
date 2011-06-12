using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using NascondiChiappe.ViewModel;

namespace NascondiChiappe
{
    public partial class ImageList : UserControl
    {
        private ImageListViewModel _vM;
        public ImageListViewModel VM
        {
            get
            {
                if (_vM == null)
                    _vM = DataContext as ImageListViewModel;
                return _vM;
            }
        }

        public ImageList()
        {
            InitializeComponent();
        }

        private void GestureListener_DoubleTap(object sender, GestureEventArgs e)
        {
            //TODO: gestire meglio CurrentPhoto
            var CurrentImage = sender as Image;
            var CurrentPhoto = AppContext.CurrentAlbum.Photos.Where(i=> i.Bitmap == CurrentImage.Source).Single();
            var ImageIndex = AppContext.CurrentAlbum.Photos.IndexOf(CurrentPhoto);
            VM.ShowImage.Execute(ImageIndex);
        }

        private void ImagesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VM.SelectedPhotos = ImagesListBox.SelectedItems.Cast<AlbumPhoto>().ToList();
        }
    }
}
