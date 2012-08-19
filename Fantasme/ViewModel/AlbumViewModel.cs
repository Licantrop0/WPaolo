using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using NascondiChiappe.Model;

namespace NascondiChiappe.ViewModel
{
    public class AlbumViewModel : ViewModelBase
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (Name == value)
                    return;
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        public ObservableCollection<Photo> Photos { get; set; }
        public ObservableCollection<Photo> SelectedPhotos { get; set; }

        public AlbumViewModel()
            : this(string.Empty, Enumerable.Empty<Photo>())
        { }

        public AlbumViewModel(IGrouping<string, Photo> album)
            : this(album.Key, album)
        { }

        public AlbumViewModel(string name, IEnumerable<Photo> photos)
        {
            Name = name;
            Photos = new ObservableCollection<Photo>(photos);

            Photos.CollectionChanged += (sender, e) =>
            {
                RaisePropertyChanged("HintVisibility");
            };
            SelectedPhotos = new ObservableCollection<Photo>();
            SelectedPhotos.CollectionChanged += (sender, e) =>
            {
                Messenger.Default.Send<bool>(SelectedPhotos.Count > 0, "SelectedPhotos");
            };
        }


        public Visibility HintVisibility
        {
            get
            {
                return Photos.Count == 0 ?
                    Visibility.Visible :
                    Visibility.Collapsed;
            }
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
