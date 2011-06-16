using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Tasks;
using NascondiChiappe.Helpers;
using NascondiChiappe.Localization;
using NascondiChiappe.Messages;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Data;

namespace NascondiChiappe.ViewModel
{
    public class AlbumsViewModel : ViewModelBase
    {
        INavigationService _navigationService;
        private INavigationService NavigationService
        {
            get
            {
                if (_navigationService == null)
                    _navigationService = new NavigationService();
                return _navigationService;
            }
        }

        public AlbumsViewModel()
        {
            Messenger.Default.Register<AddAlbumMessage>(this, m => AddAlbum(m.AddedAlbum));
            //TODO trovare un metodo più furbo che far ricaricare sempre tutta la lista
            Messenger.Default.Register<RefreshAlbumsMessage>(this, m => _albums = null);
        }

        #region Public Properties

        public bool NoAlbumsPresent { get { return Albums.Count == 0; } }
        public bool ImagesSelected { get { return SelectedAlbum.SelectedPhotos.Count > 0; } }

        private ObservableCollection<ImageListViewModel> _albums;
        public ObservableCollection<ImageListViewModel> Albums
        {
            get
            {
                if (_albums == null)
                {
                    _albums = new ObservableCollection<ImageListViewModel>();
                    foreach (var album in AppContext.Albums)
                        _albums.Add(new ImageListViewModel(album));
                }
                SelectedAlbum = _albums.FirstOrDefault();
                return _albums;
            }
        }

        private ImageListViewModel _selectedAlbum = null;
        public ImageListViewModel SelectedAlbum
        {
            get { return _selectedAlbum; }
            set
            {
                if (_selectedAlbum == value) return;

                _selectedAlbum = value;
                AppContext.CurrentAlbum = value == null ? null : value.Model;
                //Bug nel controllo pivot
                RaisePropertyChanged("SelectedAlbum");
            }
        }
        #endregion

        #region Commands

        private RelayCommand _newAlbum;
        public RelayCommand NewAlbum
        {
            get { return _newAlbum ?? (_newAlbum = new RelayCommand(NewAlbumAction)); }
        }

        private void NewAlbumAction()
        {
            AppContext.PreviousSelectedAlbum = SelectedAlbum.Model;
            AppContext.CurrentAlbum = null;
            NavigationService.Navigate(new Uri("/View/AddEditAlbumPage.xaml", UriKind.Relative));

        }

        private RelayCommand _copyToMediaLibrary;
        public RelayCommand CopyToMediaLibrary
        {
            get
            {
                return _copyToMediaLibrary ?? (_copyToMediaLibrary =
                    new RelayCommand(CopyToMediaLibraryAction, () => ImagesSelected));
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

        #endregion

        #region PageHelpers

        private void AddAlbum(Album addedAlbum)
        {
            var ilvm = new ImageListViewModel(addedAlbum);
            Albums.Add(ilvm);
            SelectedAlbum = ilvm;
        }

        private void CaptureTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                var fileName = AlbumPhoto.GetFileNameWithRotation(e.OriginalFileName, e.ChosenPhoto);
                SelectedAlbum.Model.AddPhoto(new AlbumPhoto(fileName, e.ChosenPhoto));
                e.ChosenPhoto.Close();
            }
        }

        private bool IsTrialWithCheck()
        {
            if (WPCommon.TrialManagement.IsTrialMode && SelectedAlbum.Model.Photos.Count >= 4)
            {
                NavigationService.Navigate(new Uri("/View/DemoPage.xaml", UriKind.Relative));
                return true;
            }
            return false;
        }

        #endregion

    }
}
