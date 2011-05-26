﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using NascondiChiappe.Localization;
using NascondiChiappe.ViewModel;

namespace NascondiChiappe
{
    public partial class AlbumsPage : PhoneApplicationPage
    {
        ApplicationBarIconButton CopyToMediaLibraryAppBarButton;
        AlbumsViewModel vm = new AlbumsViewModel();
        public AlbumsPage()
        {
            InitializeComponent();
            InitializeApplicationBar();
            LayoutRoot.DataContext = vm;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (!Settings.IsPasswordInserted)
            {
                NavigationService.Navigate(new Uri("/PasswordPage.xaml", UriKind.Relative));
                return;
            }
            if (vm.Albums.Count == 0)
            {
                NavigationService.Navigate(new Uri("/AddEditAlbumPage.xaml", UriKind.Relative));
                return;
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CopyToMediaLibraryAppBarButton.IsEnabled = vm.SelectedPhotos.Count > 0;
        }

        private void ImageList_DoubleTap(object sender, GestureEventArgs e)
        {
            var photoIndex = vm.SelectedAlbum.Model.Photos.IndexOf((AlbumPhoto)sender);
            NavigationService.Navigate(new Uri(
                string.Format("/ViewPhotosPage.xaml?Album={0}&Photo={1}",
                vm.SelectedAlbum.Model.DirectoryName, photoIndex),
                UriKind.Relative));
        }

        void TakePictureAppBarButton_Click(object sender, EventArgs e)
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

        void CopyFromMediaLibraryAppBarButton_Click(object sender, EventArgs e)
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

        void CaptureTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                var fileName = AlbumPhoto.GetFileNameWithRotation(e.OriginalFileName, e.ChosenPhoto);
                vm.SelectedAlbum.Model.AddPhoto(new AlbumPhoto(fileName, e.ChosenPhoto));
                e.ChosenPhoto.Close();
            }
        }

        private bool IsTrialWithCheck()
        {
            if (WPCommon.TrialManagement.IsTrialMode && vm.SelectedAlbum.Model.Photos.Count >= 4)
            {
                NavigationService.Navigate(new Uri("/DemoPage.xaml", UriKind.Relative));
                return true;
            }
            return false;
        }

        void CopyToMediaLibraryAppBarButton_Click(object sender, EventArgs e)
        {
            foreach (var p in vm.SelectedPhotos)
                vm.SelectedAlbum.Model.CopyToMediaLibrary(p);

            MessageBox.Show(vm.SelectedPhotos.Count == 1 ?
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

        private void InitializeApplicationBar()
        {
            //var ViewPhotoAppBarButton = new ApplicationBarIconButton();
            //ViewPhotoAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_viewpic.png", UriKind.Relative);
            //ViewPhotoAppBarButton.Text = AppResources.ViewPhoto;
            //ViewPhotoAppBarButton.Click += new EventHandler(ViewPhotoAppBarButton_Click);
            //ApplicationBar.Buttons.Add(ViewPhotoAppBarButton);

            var TakePictureAppBarButton = new ApplicationBarIconButton();
            TakePictureAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_camera.png", UriKind.Relative);
            TakePictureAppBarButton.Text = AppResources.TakePhoto;            
            TakePictureAppBarButton.Click += new EventHandler(TakePictureAppBarButton_Click);
            ApplicationBar.Buttons.Add(TakePictureAppBarButton);

            var CopyFromMediaLibraryAppBarButton = new ApplicationBarIconButton();
            CopyFromMediaLibraryAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_addpicture.png", UriKind.Relative);
            CopyFromMediaLibraryAppBarButton.Text = AppResources.CopyFromMediaLibrary;
            CopyFromMediaLibraryAppBarButton.Click += new EventHandler(CopyFromMediaLibraryAppBarButton_Click);
            ApplicationBar.Buttons.Add(CopyFromMediaLibraryAppBarButton);

            CopyToMediaLibraryAppBarButton = new ApplicationBarIconButton();
            CopyToMediaLibraryAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_sendphoto.png", UriKind.Relative);
            CopyToMediaLibraryAppBarButton.Text = AppResources.CopyToMediaLibrary;
            CopyToMediaLibraryAppBarButton.IsEnabled = false;
            CopyToMediaLibraryAppBarButton.Click += new EventHandler(CopyToMediaLibraryAppBarButton_Click);
            ApplicationBar.Buttons.Add(CopyToMediaLibraryAppBarButton);

            var EditAlbumAppBarMenuItem = new ApplicationBarMenuItem();
            EditAlbumAppBarMenuItem.Text = AppResources.EditAlbum;
            EditAlbumAppBarMenuItem.Click += (sender, e) =>
            { NavigationService.Navigate(new Uri("/AddEditAlbumPage.xaml?Album=" + vm.SelectedAlbum.Model.DirectoryName, UriKind.Relative)); };
            ApplicationBar.MenuItems.Add(EditAlbumAppBarMenuItem);

            var AddAlbumAppBarMenuItem = new ApplicationBarMenuItem();
            AddAlbumAppBarMenuItem.Text = AppResources.AddAlbum;
            AddAlbumAppBarMenuItem.Click += (sender, e) =>
            { NavigationService.Navigate(new Uri("/AddEditAlbumPage.xaml", UriKind.Relative)); };
            ApplicationBar.MenuItems.Add(AddAlbumAppBarMenuItem);
        }

        private void GestureListener_DoubleTap(object sender, GestureEventArgs e)
        {

        }


    }
}
