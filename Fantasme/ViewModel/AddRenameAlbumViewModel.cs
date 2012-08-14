using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NascondiChiappe.Localization;

namespace NascondiChiappe.ViewModel
{
    public class AddRenameAlbumViewModel : ViewModelBase
    {
        private bool _isNewAlbumMode;

        public Visibility OneAlbumNecessary
        {
            get
            {
                return AppContext.Albums.Count == 0 ?
                    Visibility.Visible :
                    Visibility.Collapsed;
            }
        }

        public string Title
        {
            get
            {
                return _isNewAlbumMode ?
                    AppResources.AddAlbum :
                    AppResources.RenameAlbum;
            }
        }


        public AlbumViewModel SelectedAlbum { get; set; }
        public AddRenameAlbumViewModel(AlbumViewModel album)
        {
            SelectedAlbum = album;
            _isNewAlbumMode = string.IsNullOrEmpty(album.Name);
        }

        private RelayCommand _saveAlbum;
        public RelayCommand SaveAlbum
        {
            get { return _saveAlbum ?? (_saveAlbum = new RelayCommand(SaveAlbumAction)); }
        }

        private void SaveAlbumAction()
        {
            if (_isNewAlbumMode)
            {
                AppContext.Albums.Add(SelectedAlbum);
            }
            else
            {
                foreach (var photo in SelectedAlbum.Photos)
                {
                    photo.Album = SelectedAlbum.Name;
                }
            }
        }

    }
}
