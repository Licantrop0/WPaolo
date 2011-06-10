using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NascondiChiappe.Localization;
using NascondiChiappe.Helpers;
using System;

namespace NascondiChiappe.ViewModel
{
    public class AddEditAlbumViewModel : ViewModelBase
    {
        public IList<AlbumPhoto> SelectedPhotos { get; set; }
        public Album CurrentAlbum { get { return AppContext.CurrentAlbum; } }

        INavigationService _navigationService;
        public INavigationService NavigationService
        {
            get
            {
                if (_navigationService == null)
                    _navigationService = new NavigationService();
                return _navigationService;
            }
        }

        /// <summary>Lista degli eventuali altri album su cui spostare le foto</summary>
        IEnumerable<Album> OtherAlbums
        {
            get
            {
                return AppContext.Albums.Where(a =>
                    a.DirectoryName != CurrentAlbum.DirectoryName);
            }
        }

        public string PageTitle
        {
            get
            {
                return CurrentAlbum == null ?
                    AppResources.AddAlbum : AppResources.EditAlbum;
            }
        }

        private string _albumName;
        public string AlbumName
        {
            get { return CurrentAlbum == null ? _albumName : CurrentAlbum.Name; }
            set
            {
                if (CurrentAlbum == null)
                    _albumName = value;
                else
                    CurrentAlbum.Name = value;
            }
        }

        public Visibility OneAlbumNecessaryVisibility
        {
            get
            {
                return AppContext.Albums.Count == 0 ?
                    Visibility.Visible : Visibility.Collapsed;
            }
        }

        private RelayCommand _deletePhotos;
        public RelayCommand DeletePhotos
        {
            get
            {
                return _deletePhotos ?? (_deletePhotos = new RelayCommand(
                    DeletePhotosAction, () => SelectedPhotos.Count > 0));
            }
        }

        private void DeletePhotosAction()
        {
            if (SelectedPhotos.Count == 0)
            {
                MessageBox.Show(AppResources.SelectPhotos);
                return;
            }

            if (MessageBox.Show(SelectedPhotos.Count == 1 ?
                AppResources.ConfirmPhotoDelete :
                AppResources.ConfirmPhotosDelete,
                AppResources.Confirm, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                for (int i = 0; i < SelectedPhotos.Count; i++)
                    CurrentAlbum.RemovePhoto(SelectedPhotos[i]);
            }
        }

        private RelayCommand _saveAlbum;
        public RelayCommand SaveAlbum
        {
            get
            {
                return _saveAlbum ?? (_saveAlbum = new RelayCommand(SaveAlbumAction));
            }
        }

        private void SaveAlbumAction()
        {
            if (CurrentAlbum == null) //NewAlbum Mode
            {
                if (WPCommon.TrialManagement.IsTrialMode &&
                    AppContext.Albums.Count >= 1) 
                {
                    NavigationService.Navigate(new Uri("/DemoPage.xaml", UriKind.Relative));
                    return;
                }

                AppContext.Albums.Add(new Album(AlbumName, Guid.NewGuid().ToString()));
            }

            NavigationService.GoBack();
        }

        private RelayCommand _deleteAlbum;
        public RelayCommand DeleteAlbum
        {
            get
            {
                return _deleteAlbum ?? (_deleteAlbum = new RelayCommand(DeleteAlbumAction));
            }
        }

        private void DeleteAlbumAction()
        {
            if (MessageBox.Show(string.Format(AppResources.ConfirmAlbumDelete, CurrentAlbum.Name),
                AppResources.Confirm, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                CurrentAlbum.RemoveDirectoryContent();
                AppContext.Albums.Remove(CurrentAlbum);
                NavigationService.GoBack();
            }
        }
    }
}
