﻿using System;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using NascondiChiappe.Helpers;
using NascondiChiappe.Model;

namespace NascondiChiappe.ViewModel
{
    public class AddRenameAlbumViewModel : ViewModelBase
    {
        public INavigationService NavigationService { get; set; }

        public Visibility OneAlbumNecessary
        {
            get
            {
                return AppContext.Albums.Count == 0 ?
                    Visibility.Visible :
                    Visibility.Collapsed;
            }
        }

        private Album _selectedAlbum = null;
        public Album SelectedAlbum
        {
            get
            {
                return _selectedAlbum;
            }

            set
            {
                if (_selectedAlbum == value)
                    return;

                RaisePropertyChanged("SelectedAlbum");
                _selectedAlbum = value;
            }
        }

        public AddRenameAlbumViewModel()
        {
            Messenger.Default.Register<Album>(this,
                "AddOrRename",
                album => SelectedAlbum = album);
        }

        private RelayCommand _saveAlbum;
        public RelayCommand SaveAlbum
        {
            get { return _saveAlbum ?? (_saveAlbum = new RelayCommand(SaveAlbumAction)); }
        }

        private void SaveAlbumAction()
        {
            if (string.IsNullOrEmpty(SelectedAlbum.DirectoryName))
            {
                SelectedAlbum.DirectoryName = Guid.NewGuid().ToString();
                AppContext.Albums.Add(new ImageListViewModel(SelectedAlbum));
            }
            NavigationService.GoBack();
        }

    }
}
