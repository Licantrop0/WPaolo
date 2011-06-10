using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Tasks;
using NascondiChiappe.Helpers;
using NascondiChiappe.Localization;

namespace NascondiChiappe.ViewModel
{
    public class AlbumsViewModel : ViewModelBase
    {
        public bool NoAlbumsPresent { get { return Albums.Count == 0; } }
        public bool ImagesSelected { get { return SelectedAlbum.SelectedPhotos.Count > 0; } }

        ObservableCollection<AlbumViewModel> _albums;
        public ObservableCollection<AlbumViewModel> Albums
        {
            get
            {
                //TODO: AIUTTTOO
                //if (_albums == null)
                //{
                    _albums = new ObservableCollection<AlbumViewModel>();
                    foreach (var album in AppContext.Albums)
                        _albums.Add(new AlbumViewModel(album));
                //}
                return _albums;
            }
        }

        private AlbumViewModel _selectedAlbum = null;
        public AlbumViewModel SelectedAlbum
        {
            get { return _selectedAlbum; }
            set
            {
                if (_selectedAlbum == value) return;

                _selectedAlbum = value;
                AppContext.CurrentAlbum = value.Model;
                RaisePropertyChanged("SelectedAlbum");
            }
        }

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

        void CaptureTask_Completed(object sender, PhotoResult e)
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
                NavigationService.Navigate(new Uri("/DemoPage.xaml", UriKind.Relative));
                return true;
            }
            return false;
        }

    }
}
