﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Linq;
using System.Windows.Controls;

namespace NascondiChiappe
{
    public partial class AddEditAlbumPage : PhoneApplicationPage
    {
        Album CurrentAlbum;

        public AddEditAlbumPage()
        {
            InitializeComponent();
            InitializeApplicationBar();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (NavigationContext.QueryString.ContainsKey("Album")) //EditAlbumMode
            {
                PageTitle.Text = AppResources.EditAlbum;
                var id = Convert.ToInt32(NavigationContext.QueryString["Album"]);
                CurrentAlbum = Settings.Albums[id];
                LayoutRoot.DataContext = CurrentAlbum;
                AlbumsListBox.ItemsSource = Settings.Albums;

                var DeletePhotosAppBarButton = new ApplicationBarIconButton();
                DeletePhotosAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_cancel.png", UriKind.Relative);
                DeletePhotosAppBarButton.Text = AppResources.DeleteSelectedPhotos;
                DeletePhotosAppBarButton.Click += new EventHandler(DeletePhotosAppBarMenuItem_Click);
                ApplicationBar.Buttons.Add(DeletePhotosAppBarButton);

                var DeleteAlbumAppBarMenuItem = new ApplicationBarMenuItem();
                DeleteAlbumAppBarMenuItem.Text = AppResources.DeleteAlbum;
                DeleteAlbumAppBarMenuItem.Click += new EventHandler(DeleteAlbumAppBarMenuItem_Click);
                ApplicationBar.MenuItems.Add(DeleteAlbumAppBarMenuItem);
            }
            else
            {
                if (Settings.Albums.Count == 0)
                    OneAlbumNecessaryTextBlock.Visibility = Visibility.Visible;

                AlbumNameTextBox.Focus();
            }
        }

        private void InitializeApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            var SaveAppBarButton = new ApplicationBarIconButton();
            SaveAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_save.png", UriKind.Relative);
            SaveAppBarButton.Text = AppResources.Save;
            SaveAppBarButton.Click += new EventHandler(SaveAppBarButton_Click);
            ApplicationBar.Buttons.Add(SaveAppBarButton);

            if (Settings.Albums.Count > 1)
            {
                var MovePhotosAppBarButton = new ApplicationBarIconButton();
                MovePhotosAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_move.png", UriKind.Relative);
                MovePhotosAppBarButton.Text = AppResources.MoveSelectedPhotos;
                MovePhotosAppBarButton.Click += new EventHandler(MovePhotosAppBarButton_Click);
                ApplicationBar.Buttons.Add(MovePhotosAppBarButton);
            }
        }

        void MovePhotosAppBarButton_Click(object sender, EventArgs e)
        {
            if (ImagesListBox.SelectedItems.Count == 0)
            {
                MessageBox.Show(AppResources.SelectPhotos);
                return;
            }
            PopupBorder.Visibility = Visibility.Visible;
        }

        void DeletePhotosAppBarMenuItem_Click(object sender, EventArgs e)
        {
            if (ImagesListBox.SelectedItems.Count == 0)
            {
                MessageBox.Show(AppResources.SelectPhotos);
                return;
            }
            if (MessageBox.Show(AppResources.ConfirmPhotoDelete,
                AppResources.Confirm, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                foreach (BitmapImage item in ImagesListBox.SelectedItems)
                    CurrentAlbum.RemovePhoto(item);
            }

        }

        void SaveAppBarButton_Click(object sender, EventArgs e)
        {
            if (!CheckAlbumName())
                return;

            if (CurrentAlbum == null)
                Settings.Albums.Add(new Album(AlbumNameTextBox.Text, Guid.NewGuid().ToString()));
            else
            {
                var index = Settings.Albums.IndexOf(CurrentAlbum);
                Settings.Albums.Remove(CurrentAlbum);
                Settings.Albums.Insert(index, new Album(AlbumNameTextBox.Text, CurrentAlbum.DirectoryName));
            }
            NavigationService.Navigate(new Uri("/AlbumsPage.xaml", UriKind.Relative));
        }

        void DeleteAlbumAppBarMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(string.Format(AppResources.ConfirmAlbumDelete, CurrentAlbum.Name),
                AppResources.Confirm, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                CurrentAlbum.RemoveDirectoryContent();
                Settings.Albums.Remove(CurrentAlbum);
                NavigationService.GoBack();
            }
        }


        private void PhoneApplicationPage_BackKeyPress(object sender, CancelEventArgs e)
        {
            e.Cancel = !CheckAlbumName();
        }

        private bool CheckAlbumName()
        {
            if (string.IsNullOrEmpty(AlbumNameTextBox.Text))
            {
                MessageBox.Show(AppResources.AlbumNameRequired);
                AlbumNameTextBox.Focus();
                return false;
            }
            return true;
        }

        private void AlbumNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SaveAppBarButton_Click(sender, EventArgs.Empty);
        }

        private void AlbumsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (BitmapImage item in ImagesListBox.SelectedItems)
                CurrentAlbum.MovePhoto(item, (Album)e.AddedItems[0]);

            
            PopupBorder.Visibility = Visibility.Collapsed;
        }
    }
}