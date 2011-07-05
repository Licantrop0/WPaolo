using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using NascondiChiappe.Localization;
using System.ComponentModel;

namespace NascondiChiappe
{
    public partial class AlbumsPage : PhoneApplicationPage
    {
        ApplicationBarIconButton CopyToMediaLibraryAppBarButton;
        ApplicationBarIconButton DeletePhotosAppBarButton;
        ApplicationBarIconButton CopyFromMediaLibraryAppBarButton;
        ApplicationBarIconButton MovePhotosAppBarButton;

        IList<AlbumPhoto> SelectedPhotos;
        private Album SelectedAlbum;

        private Album SelectedAlbumWrapper
        {
            get
            {
                if (!PhoneApplicationService.Current.State.ContainsKey("selected_album"))
                    PhoneApplicationService.Current.State.Add("selected_album", null);

                var sa = (Album)PhoneApplicationService.Current.State["selected_album"];
                return sa ?? AppContext.Albums.OrderBy(a => a.Name).FirstOrDefault();
            }
            set
            {
                if (SelectedAlbumWrapper == value) return;
                PhoneApplicationService.Current.State["selected_album"] = value;
            }
        }

        public AlbumsPage()
        {
            InitializeComponent();
            InitializeApplicationBar();
            csvAlbums.Source = AppContext.Albums;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (!AppContext.IsPasswordInserted)
            {
                NavigationService.Navigate(new Uri("/PasswordPage.xaml", UriKind.Relative));
                return;
            }
            if (AppContext.Albums.Count == 0)
            {
                NavigationService.Navigate(new Uri("/AddRenameAlbumPage.xaml", UriKind.Relative));
                return;
            }

            AlbumsPivot.ItemsSource = AppContext.Albums;
            SelectedAlbum = SelectedAlbumWrapper;
            //Imposta la lista degli eventuali altri album su cui spostare le foto
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            SelectedAlbumWrapper = SelectedAlbum;
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AlbumsPivot.SelectedIndex == -1) return;

            SelectedAlbum = e.AddedItems[0] as Album;
            SelectedPhotos = null;
            AlbumsListBox.ItemsSource = AppContext.Albums.Where(a => a.DirectoryName != SelectedAlbum.DirectoryName);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedPhotos = ((ListBox)sender).SelectedItems.Cast<AlbumPhoto>().ToList();
            if (SelectedPhotos.Count > 0)
            {
                if (ApplicationBar.Buttons.Count != 2) return;
                ApplicationBar.Buttons.Remove(CopyFromMediaLibraryAppBarButton);
                ApplicationBar.Buttons.Add(CopyToMediaLibraryAppBarButton);
                ApplicationBar.Buttons.Add(DeletePhotosAppBarButton);
                if (AppContext.Albums.Count > 1)
                    ApplicationBar.Buttons.Add(MovePhotosAppBarButton);
            }
            else
            {
                ApplicationBar.Buttons.Remove(CopyToMediaLibraryAppBarButton);
                ApplicationBar.Buttons.Remove(DeletePhotosAppBarButton);
                ApplicationBar.Buttons.Remove(MovePhotosAppBarButton);
                ApplicationBar.Buttons.Add(CopyFromMediaLibraryAppBarButton);
            }
        }

        private void ImageList_DoubleTap(object sender, GestureEventArgs e)
        {
            var photoIndex = SelectedAlbum.Photos.IndexOf((AlbumPhoto)sender);
            NavigationService.Navigate(new Uri(
                string.Format("/ViewPhotosPage.xaml?Album={0}&Photo={1}",
                SelectedAlbum.DirectoryName, photoIndex),
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
                var p = new AlbumPhoto(fileName, e.ChosenPhoto);
                if (!SelectedAlbum.AddPhoto(p))
                    MessageBox.Show(AppResources.ErrorSavingPhoto);
                e.ChosenPhoto.Close();
            }
        }

        private bool IsTrialWithCheck()
        {
            if (WPCommon.TrialManagement.IsTrialMode && SelectedAlbum.Photos.Count >= 4)
            {
                NavigationService.Navigate(new Uri("/DemoPage.xaml", UriKind.Relative));
                return true;
            }
            return false;
        }

        void CopyToMediaLibraryAppBarButton_Click(object sender, EventArgs e)
        {
            foreach (var p in SelectedPhotos)
                if (!SelectedAlbum.CopyToMediaLibrary(p))
                {
                    MessageBox.Show(AppResources.ErrorSavingPhoto);
                    return;
                }

            MessageBox.Show(SelectedPhotos.Count == 1 ?
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
            var TakePictureAppBarButton = new ApplicationBarIconButton();
            TakePictureAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_camera.png", UriKind.Relative);
            TakePictureAppBarButton.Text = AppResources.TakePhoto;
            TakePictureAppBarButton.Click += new EventHandler(TakePictureAppBarButton_Click);
            ApplicationBar.Buttons.Add(TakePictureAppBarButton);

            CopyFromMediaLibraryAppBarButton = new ApplicationBarIconButton();
            CopyFromMediaLibraryAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_addpicture.png", UriKind.Relative);
            CopyFromMediaLibraryAppBarButton.Text = AppResources.CopyFromMediaLibrary;
            CopyFromMediaLibraryAppBarButton.Click += new EventHandler(CopyFromMediaLibraryAppBarButton_Click);
            ApplicationBar.Buttons.Add(CopyFromMediaLibraryAppBarButton);

            CopyToMediaLibraryAppBarButton = new ApplicationBarIconButton();
            CopyToMediaLibraryAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_sendphoto.png", UriKind.Relative);
            CopyToMediaLibraryAppBarButton.Text = AppResources.CopyToMediaLibrary;
            CopyToMediaLibraryAppBarButton.Click += new EventHandler(CopyToMediaLibraryAppBarButton_Click);

            DeletePhotosAppBarButton = new ApplicationBarIconButton();
            DeletePhotosAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_cancel.png", UriKind.Relative);
            DeletePhotosAppBarButton.Text = AppResources.DeleteSelectedPhotos;
            DeletePhotosAppBarButton.Click += new EventHandler(DeletePhotosAppBarMenuItem_Click);

            MovePhotosAppBarButton = new ApplicationBarIconButton();
            MovePhotosAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_move.png", UriKind.Relative);
            MovePhotosAppBarButton.Text = AppResources.MoveSelectedPhotos;
            MovePhotosAppBarButton.Click += new EventHandler(MovePhotosAppBarButton_Click);

            var EditAlbumAppBarMenuItem = new ApplicationBarMenuItem();
            EditAlbumAppBarMenuItem.Text = AppResources.EditAlbum;
            EditAlbumAppBarMenuItem.Click += (sender, e) =>
            { NavigationService.Navigate(new Uri("/AddRenameAlbumPage.xaml?Album=" + SelectedAlbum.DirectoryName, UriKind.Relative)); };
            ApplicationBar.MenuItems.Add(EditAlbumAppBarMenuItem);

            var AddAlbumAppBarMenuItem = new ApplicationBarMenuItem();
            AddAlbumAppBarMenuItem.Text = AppResources.AddAlbum;
            AddAlbumAppBarMenuItem.Click += (sender, e) =>
            { NavigationService.Navigate(new Uri("/AddRenameAlbumPage.xaml", UriKind.Relative)); };
            ApplicationBar.MenuItems.Add(AddAlbumAppBarMenuItem);

            var DeleteAlbumAppBarMenuItem = new ApplicationBarMenuItem();
            DeleteAlbumAppBarMenuItem.Text = AppResources.DeleteAlbum;
            DeleteAlbumAppBarMenuItem.Click += new EventHandler(DeleteAlbumAppBarMenuItem_Click);
            ApplicationBar.MenuItems.Add(DeleteAlbumAppBarMenuItem);
        }

        void DeleteAlbumAppBarMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(string.Format(AppResources.ConfirmAlbumDelete, SelectedAlbum.Name),
                AppResources.Confirm, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                SelectedAlbum.RemoveDirectoryContent();
                AppContext.Albums.Remove(SelectedAlbum);
            }
        }

        void DeletePhotosAppBarMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(SelectedPhotos.Count == 1 ?
                AppResources.ConfirmPhotoDelete :
                AppResources.ConfirmPhotosDelete,
                AppResources.Confirm, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                for (int i = 0; i < SelectedPhotos.Count; i++)
                    SelectedAlbum.RemovePhoto(SelectedPhotos[i]);
            }
        }

        void MovePhotosAppBarButton_Click(object sender, EventArgs e)
        {
            PopupBackground.Visibility = Visibility.Visible;
            PopupBorder.Visibility = Visibility.Visible;
        }


        private void AlbumsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            for (int i = 0; i < SelectedPhotos.Count; i++)
            {
                SelectedAlbum.MovePhoto(SelectedPhotos[i], (Album)e.AddedItems[0]);
            }

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

    }
}
