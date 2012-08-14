using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Tasks;
using NascondiChiappe.Helpers;
using NascondiChiappe.Localization;
using NascondiChiappe.Model;
using System.ComponentModel;

namespace NascondiChiappe.ViewModel
{
    public class AlbumsViewModel : ViewModelBase
    {
        public WPCommon.Helpers.INavigationService NavigationService { get; set; }
        public bool ArePhotosSelected { get { return SelectedAlbum.SelectedPhotos.Count > 0; } }

        private bool _isBusy = false;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (IsBusy == value) return;
                _isBusy = value;
                RaisePropertyChanged("IsBusy");
                //Messaggio per aggiornare l'ApplicationBar nella View
                Messenger.Default.Send<bool>(value, "IsBusy");
            }
        }

        public ObservableCollection<AlbumViewModel> Albums
        { get { return IsInDesignMode ? new ObservableCollection<AlbumViewModel>() : AppContext.Albums; } }

        private AlbumViewModel _selectedAlbum;
        public AlbumViewModel SelectedAlbum
        {
            get { return _selectedAlbum; }
            set
            {
                if (SelectedAlbum == value)
                    return;

                _selectedAlbum = value;
                RaisePropertyChanged("SelectedAlbum");
                RaisePropertyChanged("OtherAlbums");

                //Messaggio per aggiornare l'ApplicationBar nella View
                if (SelectedAlbum != null)
                    Messenger.Default.Send<bool>(ArePhotosSelected, "SelectedPhotos");
            }
        }

        public IEnumerable<AlbumViewModel> OtherAlbums
        {
            get { return Albums.Except(new AlbumViewModel[] { SelectedAlbum }); }
        }

        private AlbumViewModel _moveToAlbum;
        public AlbumViewModel MoveToAlbum
        {
            get { return _moveToAlbum; }
            set
            {
                _moveToAlbum = value;

                if (_moveToAlbum != null)
                    MovePhotos.Execute(_moveToAlbum);
            }
        }

        public AlbumsViewModel(WPCommon.Helpers.INavigationService navigationService)
        {
            NavigationService = navigationService;
            Albums.CollectionChanged += (sender, e) =>
            {
                if (e.NewStartingIndex == -1)
                    SelectedAlbum = Albums.FirstOrDefault();
                else
                    SelectedAlbum = Albums[e.NewStartingIndex];
            };
        }

        #region Commands

        private RelayCommand _newAlbum;
        public RelayCommand NewAlbum
        {
            get { return _newAlbum ?? (_newAlbum = new RelayCommand(NewAlbumAction)); }
        }

        private void NewAlbumAction()
        {
            if (WPCommon.Helpers.TrialManagement.IsTrialMode && Albums.Count > 0)
            {
                NavigationService.Navigate(new Uri("/View/DemoPage.xaml", UriKind.Relative));
                return;
            }

            NavigationService.Navigate(new Uri("/View/AddRenameAlbumPage.xaml", UriKind.Relative));
        }

        private RelayCommand _renameAlbum;
        public RelayCommand RenameAlbum
        {
            get { return _renameAlbum ?? (_renameAlbum = new RelayCommand(RenameAlbumAction)); }
        }

        private void RenameAlbumAction()
        {
            NavigationService.Navigate(new Uri("/View/AddRenameAlbumPage.xaml?id=" + Albums.IndexOf(SelectedAlbum), UriKind.Relative));
        }


        private RelayCommand _copyToMediaLibrary;
        public RelayCommand CopyToMediaLibrary
        {
            get
            {
                return _copyToMediaLibrary ?? (_copyToMediaLibrary =
                    new RelayCommand(CopyToMediaLibraryAction, () => ArePhotosSelected));
            }
        }

        public void CopyToMediaLibraryAction()
        {
            IsBusy = true;
            var bw = new BackgroundWorker();
            bw.DoWork += (sender, e) => PhotoService.CopyToMediaLibrary(SelectedAlbum.SelectedPhotos);
            bw.RunWorkerCompleted += (sender, e) =>
            {
                IsBusy = false;
                MessageBox.Show(SelectedAlbum.SelectedPhotos.Count == 1 ?
                    AppResources.PhotoCopied :
                    AppResources.PhotosCopied);
            };
            bw.RunWorkerAsync();
        }

        private RelayCommand _copyFromMediaLibrary;
        public RelayCommand CopyFromMediaLibrary
        {
            get
            {
                return _copyFromMediaLibrary ?? (_copyFromMediaLibrary = new RelayCommand(CopyFromMediaLibraryAction));
            }
        }

        public void CopyFromMediaLibraryAction()
        {
            if (IsTrialWithCheck())
                return;

            var photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += CaptureTask_Completed;
            try
            {
                photoChooserTask.Show();
            }
            catch (InvalidOperationException) { };
        }

        private RelayCommand _takePicture;
        public RelayCommand TakePicture
        {
            get { return _takePicture ?? (_takePicture = new RelayCommand(TakePictureAction)); }
        }

        private void TakePictureAction()
        {
            if (IsTrialWithCheck())
                return;

            var cameraCaptureTask = new CameraCaptureTask();
            cameraCaptureTask.Completed += CaptureTask_Completed;
            try
            {
                cameraCaptureTask.Show();
            }
            catch (InvalidOperationException) { };
        }

        private RelayCommand _deletePhotos;
        public RelayCommand DeletePhotos
        {
            get
            {
                return _deletePhotos ?? (_deletePhotos = new RelayCommand(
                    DeletePhotosAction, () => ArePhotosSelected));
            }
        }

        private void DeletePhotosAction()
        {
            if (MessageBox.Show(SelectedAlbum.SelectedPhotos.Count == 1 ?
                AppResources.ConfirmPhotoDelete :
                AppResources.ConfirmPhotosDelete,
                AppResources.Confirm, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                SelectedAlbum.RemovePhotos(SelectedAlbum.SelectedPhotos);
            }
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
            if (MessageBox.Show(string.Format(AppResources.ConfirmAlbumDelete, SelectedAlbum.Name),
                AppResources.Confirm, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                SelectedAlbum.RemoveAlbum();
                Albums.Remove(SelectedAlbum);
            }

            if (Albums.Count == 0)
                NewAlbum.Execute(null);
        }

        private RelayCommand<AlbumViewModel> _movePhotos;
        public RelayCommand<AlbumViewModel> MovePhotos
        {
            get
            {
                return _movePhotos ?? (_movePhotos = new RelayCommand<AlbumViewModel>(MovePhotosAction));
            }
        }

        private void MovePhotosAction(AlbumViewModel destination)
        {
            SelectedAlbum.MovePhotos(destination, SelectedAlbum.SelectedPhotos);
        }

        #endregion

        #region PageHelpers

        private void CaptureTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                SelectedAlbum.AddPhoto(e.OriginalFileName, e.ChosenPhoto);
                e.ChosenPhoto.Close();
            }
        }

        private bool IsTrialWithCheck()
        {
            if (WPCommon.Helpers.TrialManagement.IsTrialMode && SelectedAlbum.Photos.Count >= 4)
            {
                NavigationService.Navigate(new Uri("/View/DemoPage.xaml", UriKind.Relative));
                return true;
            }
            return false;
        }

        #endregion

    }
}