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
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.ComponentModel;
using System.Collections.Generic;

namespace NascondiChiappe.ViewModel
{
    public class AlbumsViewModel : INotifyPropertyChanged
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

        public ObservableCollection<AlbumViewModel> Albums { get; set; }
        public IList<AlbumPhoto> SelectedPhotos { get; set; }
     
        public bool NoAlbumsPresent { get { return Albums.Count == 0; } }


        private AlbumViewModel _selectedAlbum = null;
        public AlbumViewModel SelectedAlbum
        {
            get { return _selectedAlbum; }
            set
            {
                if (_selectedAlbum == value) return;

                _selectedAlbum = value;
                OnPropertyChanged(this, "SelectedAlbum");
            }
        }

        public AlbumsViewModel()
        {

        }


    }
}
