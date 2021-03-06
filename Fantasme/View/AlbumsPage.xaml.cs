﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Advertising.Mobile.UI;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using NascondiChiappe.Localization;
using NascondiChiappe.Model;
using NascondiChiappe.ViewModel;
using WPCommon.Helpers;

namespace NascondiChiappe.View
{
    public partial class AlbumsPage : PhoneApplicationPage
    {
        private AlbumsViewModel _vM;
        public AlbumsViewModel VM
        {
            get
            {
                if (_vM == null)
                    _vM = LayoutRoot.DataContext as AlbumsViewModel;
                return _vM;
            }
        }

        public AlbumsPage()
        {
            InitializeComponent();
            InitializeApplicationBar();
            InizializeMessages();
        }

        private void InizializeMessages()
        {
            Messenger.Default.Register<bool>(this, "SelectedPhotos",
                AreSelected => UpdateSelectionButtons(AreSelected));

            Messenger.Default.Register<bool>(this, "IsBusy", IsBusy =>
            {
                ApplicationBar.IsVisible = !IsBusy;
                HintFullScreenTextBlock.Visibility = IsBusy ? Visibility.Collapsed : Visibility.Visible;
                PopupBackground.Visibility = IsBusy ? Visibility.Visible : Visibility.Collapsed;
            });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!AppContext.IsPasswordInserted)
            {
                NavigationService.Navigate(new Uri("/View/PasswordPage.xaml", UriKind.Relative));
                return;
            }
            if (VM.Albums.Count == 0)
            {
                VM.NewAlbum.Execute(null);
                return;
            }

            if (!TrialManagement.IsTrialMode)
                return;

            if (AdPlaceHolder.Children.Count == 1) //l'Ad c'è già
                return;

            AdPlaceHolder.Children.Add(new AdControl("4e17209f-3637-44ff-82b0-ed87b368a8fc", "10041859", true)
            {
                Height = 80,
                Width = 480,
            });

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            AdPlaceHolder.Children.Clear();

            if (e.Uri.OriginalString == "/View/PasswordPage.xaml")
                NavigationService.RemoveBackEntry();
        }

        private void UpdateSelectionButtons(bool areSelected)
        {
            if (areSelected)
            {
                if (ApplicationBar.Buttons.Contains(CopyToMediaLibraryAppBarButton)) return;

                ApplicationBar.Buttons.Remove(TakePictureAppBarButton);
                ApplicationBar.Buttons.Remove(CopyFromMediaLibraryAppBarButton);

                ApplicationBar.Buttons.Add(CopyToMediaLibraryAppBarButton);
                ApplicationBar.Buttons.Add(DeletePhotosAppBarButton);
                if (AppContext.Albums.Count > 1)
                    ApplicationBar.Buttons.Add(MovePhotosAppBarButton);
            }
            else
            {
                if (ApplicationBar.Buttons.Contains(TakePictureAppBarButton)) return;

                ApplicationBar.Buttons.Remove(CopyToMediaLibraryAppBarButton);
                ApplicationBar.Buttons.Remove(DeletePhotosAppBarButton);
                ApplicationBar.Buttons.Remove(MovePhotosAppBarButton);

                ApplicationBar.Buttons.Add(TakePictureAppBarButton);
                ApplicationBar.Buttons.Add(CopyFromMediaLibraryAppBarButton);
            }
        }

        private void GestureListener_DoubleTap(object sender, GestureEventArgs e)
        {
            //Fix errore z-order GestureListener
            if (PopupBackground.Visibility == Visibility.Collapsed)
            {
                var CurrentImage = ((Grid)sender).DataContext as Photo;
                var uriString = string.Format("/View/ViewPhotosPage.xaml?album={0}&index={1}",
                    CurrentImage.Album, VM.SelectedAlbum.Photos.IndexOf(CurrentImage));
                NavigationService.Navigate(new Uri(uriString, UriKind.Relative));
            }
        }

        void ShowOtherAlbumsListPopup(object sender, EventArgs e)
        {
            PopupBackground.Visibility = Visibility.Visible;
            PopupBorder.Visibility = Visibility.Visible;
        }

        private void AlbumsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopupBackground.Visibility = Visibility.Collapsed;
            PopupBorder.Visibility = Visibility.Collapsed;
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, CancelEventArgs e)
        {
            if (PopupBorder.Visibility == Visibility.Visible)
            {
                PopupBackground.Visibility = Visibility.Collapsed;
                PopupBorder.Visibility = Visibility.Collapsed;
                e.Cancel = true;
            }
        }

        //Not sure about usability
        //private void PopupBackground_Tap(object sender, GestureEventArgs e)
        //{
        //    if (PopupBorder.Visibility == Visibility.Visible)
        //    {
        //        PopupBackground.Visibility = Visibility.Collapsed;
        //        PopupBorder.Visibility = Visibility.Collapsed;
        //    }   
        //}

        ApplicationBarIconButton TakePictureAppBarButton;
        ApplicationBarIconButton CopyFromMediaLibraryAppBarButton;
        ApplicationBarIconButton CopyToMediaLibraryAppBarButton;
        ApplicationBarIconButton DeletePhotosAppBarButton;
        ApplicationBarIconButton MovePhotosAppBarButton;

        private void InitializeApplicationBar()
        {
            TakePictureAppBarButton = new ApplicationBarIconButton();
            TakePictureAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_camera.png", UriKind.Relative);
            TakePictureAppBarButton.Text = AppResources.TakePhoto;
            TakePictureAppBarButton.Click += (sender, e) => VM.TakePicture.Execute(null);

            CopyFromMediaLibraryAppBarButton = new ApplicationBarIconButton();
            CopyFromMediaLibraryAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_addpicture.png", UriKind.Relative);
            CopyFromMediaLibraryAppBarButton.Text = AppResources.CopyFromMediaLibrary;
            CopyFromMediaLibraryAppBarButton.Click += (sender, e) => VM.CopyFromMediaLibrary.Execute(null);

            CopyToMediaLibraryAppBarButton = new ApplicationBarIconButton();
            CopyToMediaLibraryAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_sendphoto.png", UriKind.Relative);
            CopyToMediaLibraryAppBarButton.Text = AppResources.CopyToMediaLibrary;
            CopyToMediaLibraryAppBarButton.Click += (sender, e) => VM.CopyToMediaLibrary.Execute(null);

            DeletePhotosAppBarButton = new ApplicationBarIconButton();
            DeletePhotosAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_cancel.png", UriKind.Relative);
            DeletePhotosAppBarButton.Text = AppResources.DeleteSelectedPhotos;
            DeletePhotosAppBarButton.Click += (sender1, e1) => VM.DeletePhotos.Execute(null);

            MovePhotosAppBarButton = new ApplicationBarIconButton();
            MovePhotosAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_move.png", UriKind.Relative);
            MovePhotosAppBarButton.Text = AppResources.MoveSelectedPhotos;
            MovePhotosAppBarButton.Click += new EventHandler(ShowOtherAlbumsListPopup);

            var AddAlbumAppBarMenuItem = new ApplicationBarMenuItem();
            AddAlbumAppBarMenuItem.Text = AppResources.AddAlbum;
            AddAlbumAppBarMenuItem.Click += (sender, e) => VM.NewAlbum.Execute(null);
            ApplicationBar.MenuItems.Add(AddAlbumAppBarMenuItem);

            var RenameAlbumAppBarMenuItem = new ApplicationBarMenuItem();
            RenameAlbumAppBarMenuItem.Text = AppResources.RenameAlbum;
            RenameAlbumAppBarMenuItem.Click += (sender, e) => VM.RenameAlbum.Execute(null);
            ApplicationBar.MenuItems.Add(RenameAlbumAppBarMenuItem);

            var DeleteAlbumAppBarMenuItem = new ApplicationBarMenuItem();
            DeleteAlbumAppBarMenuItem.Text = AppResources.DeleteAlbum;
            DeleteAlbumAppBarMenuItem.Click += (sender, e) => VM.DeleteAlbum.Execute(null);
            ApplicationBar.MenuItems.Add(DeleteAlbumAppBarMenuItem);

            var AboutAppBarMenuItem = new ApplicationBarMenuItem();
            AboutAppBarMenuItem.Text = AppResources.About;
            AboutAppBarMenuItem.Click += (sender, e) =>
                NavigationService.Navigate(new Uri("/View/AboutPage.xaml", UriKind.Relative));
            ApplicationBar.MenuItems.Add(AboutAppBarMenuItem);
        }
    }
}