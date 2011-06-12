using System.Windows;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Command;
using NascondiChiappe.Helpers;
using System;
using GalaSoft.MvvmLight.Messaging;
using NascondiChiappe.Messages;

namespace NascondiChiappe.ViewModel
{
    public class ImageListViewModel : ViewModelBase
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

        public ImageListViewModel(Album model)
        {
            if (model == null)
                return;

            Model = model;
            Model.Photos.CollectionChanged += (s, e) =>
            {
                RaisePropertyChanged("HintVisibility");
            };
        }

        public Visibility HintVisibility
        {
            get
            {
                if (Model == null)
                    return Visibility.Collapsed;

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
                if (SelectedPhotos == value)
                    return;

                _selectedPhotos = value;

                Messenger.Default.Send<CanExecuteOnSelectedPhotosMessage>(
                    new CanExecuteOnSelectedPhotosMessage(_selectedPhotos.Count > 0));
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
                string.Format("/View/ViewPhotosPage.xaml?Photo={1}",
                Model.DirectoryName, imageIndex),
                UriKind.Relative));
        }
    }
}
