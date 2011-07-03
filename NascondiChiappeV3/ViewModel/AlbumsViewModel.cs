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
using WPCommon;

namespace NascondiChiappe.ViewModel
{
    public class AlbumsViewModel : ViewModelBase
    {
        public INavigationService NavigationService { get; set; }
        public bool ArePhotosSelected { get { return SelectedAlbum.SelectedPhotos.Count > 0; } }

        public ObservableCollection<ImageListViewModel> Albums
        { get { return AppContext.Albums; } }

        private ImageListViewModel _selectedAlbum;
        public ImageListViewModel SelectedAlbum
        {
            get { return _selectedAlbum; }
            set
            {
                if (_selectedAlbum == value)
                    return;

                _selectedAlbum = value;
                RaisePropertyChanged("SelectedAlbum");
                RaisePropertyChanged("OtherAlbums");

                //Messaggio per aggiornare l'ApplicationBar nella View
                Messenger.Default.Send<bool>(ArePhotosSelected, "SelectedPhotos");
            }
        }

        public IEnumerable<ImageListViewModel> OtherAlbums
        {
            get { return Albums.Except(new ImageListViewModel[] { SelectedAlbum }); }
        }

        private ImageListViewModel _moveToAlbum;
        public ImageListViewModel MoveToAlbum
        {
            get { return _moveToAlbum; }
            set
            {
                _moveToAlbum = value;

                if (_moveToAlbum != null)
                    MovePhotos.Execute(_moveToAlbum.Model);
            }
        }

        public AlbumsViewModel()
        {
            Albums.CollectionChanged += (sender, e) =>
            {
                SelectedAlbum = e.NewStartingIndex == -1 ?
                    Albums.FirstOrDefault() :
                    Albums[e.NewStartingIndex];
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
            if (TrialManagement.IsTrialMode && Albums.Count > 0)
            {
                NavigationService.Navigate(new Uri("/View/DemoPage.xaml", UriKind.Relative));
                return;
            }

            Messenger.Default.Send<Album>(new Album(), "AddOrRename");
            NavigationService.Navigate(new Uri("/View/AddRenameAlbumPage.xaml", UriKind.Relative));
        }

        private RelayCommand _renameAlbum;
        public RelayCommand RenameAlbum
        {
            get { return _renameAlbum ?? (_renameAlbum = new RelayCommand(RenameAlbumAction)); }
        }

        private void RenameAlbumAction()
        {
            Messenger.Default.Send<Album>(SelectedAlbum.Model, "AddOrRename");
            NavigationService.Navigate(new Uri("/View/AddRenameAlbumPage.xaml", UriKind.Relative));
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
            foreach (var p in SelectedAlbum.SelectedPhotos)
                SelectedAlbum.Model.CopyToMediaLibrary(p);

            MessageBox.Show(SelectedAlbum.SelectedPhotos.Count == 1 ?
                AppResources.PhotoCopied :
                AppResources.PhotosCopied);

            //TODO: implementare export asincrono con Mango
            //var bw = new BackgroundWorker();
            //bw.DoWork += (sender1, e1) =>
            //{
            //};
            //bw.RunWorkerCompleted += (sender1, e1) =>
            //{
            //};
            //bw.RunWorkerAsync();
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
                //La collection SelectedPhotos cambia durante il ciclo
                //quindi devo scodare sempre dall'inizio
                var n = SelectedAlbum.SelectedPhotos.Count;
                for (int i = 0; i < n; i++)
                    SelectedAlbum.Model.RemovePhoto(SelectedAlbum.SelectedPhotos[0]);
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
            if (MessageBox.Show(string.Format(AppResources.ConfirmAlbumDelete, SelectedAlbum.Model.Name),
                AppResources.Confirm, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                SelectedAlbum.Model.RemoveDirectoryContent();
                Albums.Remove(SelectedAlbum);
            }

            if (Albums.Count == 0)
                NewAlbum.Execute(null);
        }

        private RelayCommand<Album> _movePhotos;
        public RelayCommand<Album> MovePhotos
        {
            get
            {
                return _movePhotos ?? (_movePhotos = new RelayCommand<Album>(MovePhotosAction));
            }
        }

        private void MovePhotosAction(Album destination)
        {
            var n = SelectedAlbum.SelectedPhotos.Count;
            for (int i = 0; i < n; i++)
                SelectedAlbum.Model.MovePhoto(SelectedAlbum.SelectedPhotos[0], destination);
        }

        #endregion

        #region PageHelpers

        private void CaptureTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                var fileName = Photo.GetFileNameWithRotation(e.OriginalFileName, e.ChosenPhoto);
                SelectedAlbum.Model.AddPhoto(new Photo(fileName, e.ChosenPhoto));
                e.ChosenPhoto.Close();
            }
        }

        private bool IsTrialWithCheck()
        {
            if (TrialManagement.IsTrialMode && SelectedAlbum.Model.Photos.Count >= 4)
            {
                NavigationService.Navigate(new Uri("/View/DemoPage.xaml", UriKind.Relative));
                return true;
            }
            return false;
        }

        #endregion

    }
}