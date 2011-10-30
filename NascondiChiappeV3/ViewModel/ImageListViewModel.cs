using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using NascondiChiappe.Messages;
using NascondiChiappe.Model;

namespace NascondiChiappe.ViewModel
{
    public class ImageListViewModel : ViewModelBase
    {
        public Album Model { get; private set; }

        WPCommon.Helpers.INavigationService _navigationService;
        public WPCommon.Helpers.INavigationService NavigationService
        {
            get
            {
                if (_navigationService == null)
                    _navigationService = new WPCommon.Helpers.NavigationService();
                return _navigationService;
            }
        }

        public ImageListViewModel(Album model)
        {
            Model = model;
            Model.Photos.CollectionChanged += (sender, e) =>
            {
                RaisePropertyChanged("HintVisibility");
            };
            SelectedPhotos = new ObservableCollection<Photo>();
            SelectedPhotos.CollectionChanged += (sender, e) =>
            {
                Messenger.Default.Send<bool>(SelectedPhotos.Count > 0, "SelectedPhotos");
            };            
        }

        public ObservableCollection<Photo> SelectedPhotos { get; set; }

        public Visibility HintVisibility
        {
            get
            {
                return Model.Photos.Count == 0 ?
                    Visibility.Visible :
                    Visibility.Collapsed;
            }
        }

        private RelayCommand<BitmapImage> _showImage;
        public RelayCommand<BitmapImage> ShowImage
        {
            get { return _showImage ?? (_showImage = new RelayCommand<BitmapImage>(ShowImageAction)); }
        }

        private void ShowImageAction(BitmapImage image)
        {
            var CurrentPhoto = Model.Photos.Where(i => i.Bitmap == image).Single();

            Messenger.Default.Send<ViewPhotoMessage>(
                new ViewPhotoMessage(CurrentPhoto, Model.DirectoryName));

            NavigationService.Navigate(new Uri(
                "/View/ViewPhotosPage.xaml", UriKind.Relative));
        }

        private RelayCommand<SelectionChangedEventArgs> _selectionChangedCommand;
        public RelayCommand<SelectionChangedEventArgs> SelectionChangedCommand
        {
            get
            {
                return _selectionChangedCommand ??
                    (_selectionChangedCommand = new RelayCommand<SelectionChangedEventArgs>(SelectionChangedAction));
            }
        }

        private void SelectionChangedAction(SelectionChangedEventArgs e)
        {
            foreach (Photo photo in e.RemovedItems)
                SelectedPhotos.Remove(photo);

            foreach (Photo photo in e.AddedItems)
                SelectedPhotos.Add(photo);
        }
    }
}
