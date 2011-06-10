using System.Windows;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Command;
using NascondiChiappe.Helpers;
using System;

namespace NascondiChiappe.ViewModel
{
    public class AlbumViewModel : ViewModelBase
    {
        public Album Model { get; private set; }

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


        public AlbumViewModel(Album model)
        {
            Model = model;
            Model.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "Photos")
                    RaisePropertyChanged("HintVisibility");
            };
        }

        public Visibility HintVisibility
        {
            get
            {
                return Model.Photos.Count == 0 ?
                    Visibility.Visible :
                    Visibility.Collapsed;
            }
        }

        IList<AlbumPhoto> _selectedPhotos;
        public IList<AlbumPhoto> SelectedPhotos
        {
            get { return _selectedPhotos; }
            set
            {
                _selectedPhotos = value;
                //CopyToMediaLibrary.RaiseCanExecuteChanged();
            }
        }

        private RelayCommand<int> _showImage;
        public RelayCommand<int> ShowImage
        {
            get { return _showImage ?? (_showImage = new RelayCommand<int>(ShowImageAction)); }
        }

        private void ShowImageAction(int imageIndex)
        {
            NavigationService.Navigate(new Uri(
                string.Format("/View/ViewPhotosPage.xaml?Album={0}&Photo={1}",
                Model.DirectoryName, imageIndex),
                UriKind.Relative));
        }
    }
}
