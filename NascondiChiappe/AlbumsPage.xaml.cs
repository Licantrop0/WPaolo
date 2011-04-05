using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media;
using System.Windows;

namespace NascondiChiappe
{
    public partial class AlbumsPage : PhoneApplicationPage
    {
        Album CurrentAlbum;
        BitmapImage CurrentPhoto;

        public AlbumsPage()
        {
            InitializeComponent();
            InitializeApplicationBar();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (Settings.Albums.Count == 0)
                NavigationService.Navigate(new Uri("/AddEditAlbumPage.xaml", UriKind.Relative));
            else
            {
                AlbumsPivot.ItemsSource = Settings.Albums;
                CurrentAlbum = Settings.Albums[0];
                ShowHint();
            }
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems[0] == null)
                return;
            CurrentAlbum = (Album)e.AddedItems[0];
            ShowHint();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems[0] == null)
                return;
            CurrentPhoto = (BitmapImage)e.AddedItems[0];
            NavigationService.Navigate(new Uri(
                string.Format("/ViewPhotosPage.xaml?Album={0}&Photo={1}",
                AlbumsPivot.SelectedIndex, CurrentAlbum.Images.IndexOf(CurrentPhoto)),
                UriKind.Relative));

        }

        void TakePictureAppBarButton_Click(object sender, EventArgs e)
        {
            var cameraCaptureTask = new CameraCaptureTask();
            cameraCaptureTask.Completed += new EventHandler<PhotoResult>(CaptureTask_Completed);
            try
            {
                cameraCaptureTask.Show();
            }
            catch (InvalidOperationException) { };
        }

        void CopyFromMediaLibraryAppBarButton_Click(object sender, EventArgs e)
        {
            var photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += CaptureTask_Completed;
            photoChooserTask.Show();
        }

        void CopyToMediaLibraryAppBarButton_Click(object sender, EventArgs e)
        {
            if (CurrentPhoto != null)
            {
                var stream = new MemoryStream();
                var wb = new WriteableBitmap(CurrentPhoto);
                wb.SaveJpeg(stream, wb.PixelWidth, wb.PixelHeight, 0, 85);
                stream.Position = 0;
                new MediaLibrary().SavePicture(Guid.NewGuid().ToString(), stream);
                stream.Close();
                MessageBox.Show(AppResources.PhotoCopied);
            }
        }

        void CaptureTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                CurrentAlbum.AddImage(e.ChosenPhoto);
                ShowHint();
            }
        }

        private void ShowHint()
        {
            AddPhotosHintTextBlock.Visibility =
                CurrentAlbum.Images.Count == 0 ?
                Visibility.Visible :
                Visibility.Collapsed;
        }


        void DeleteAlbumAppBarMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(string.Format(AppResources.ConfirmDelete, CurrentAlbum.Name),
                AppResources.Confirm, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                Settings.Albums.Remove(CurrentAlbum);

            if (Settings.Albums.Count == 0)
                NavigationService.Navigate(new Uri("/AddEditAlbumPage.xaml", UriKind.Relative));
        }

        private void InitializeApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

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

            var CopyToMediaLibraryAppBarButton = new ApplicationBarIconButton();
            CopyToMediaLibraryAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_sendphoto.png", UriKind.Relative);
            CopyToMediaLibraryAppBarButton.Text = AppResources.CopyToMediaLibrary;
            CopyToMediaLibraryAppBarButton.Click += new EventHandler(CopyToMediaLibraryAppBarButton_Click);
            ApplicationBar.Buttons.Add(CopyToMediaLibraryAppBarButton);

            var EditAlbumAppBarMenuItem = new ApplicationBarMenuItem();
            EditAlbumAppBarMenuItem.Text = AppResources.EditAlbum;
            EditAlbumAppBarMenuItem.Click += (sender, e) =>
            { NavigationService.Navigate(new Uri("/AddEditAlbumPage.xaml?Album=" + AlbumsPivot.SelectedIndex, UriKind.Relative)); };
            ApplicationBar.MenuItems.Add(EditAlbumAppBarMenuItem);

            var AddAlbumAppBarMenuItem = new ApplicationBarMenuItem();
            AddAlbumAppBarMenuItem.Text = AppResources.AddAlbum;
            AddAlbumAppBarMenuItem.Click += (sender, e) =>
            { NavigationService.Navigate(new Uri("/AddEditAlbumPage.xaml", UriKind.Relative)); };
            ApplicationBar.MenuItems.Add(AddAlbumAppBarMenuItem);

            var ChangePasswordAppBarMenuItem = new ApplicationBarMenuItem();
            ChangePasswordAppBarMenuItem.Text = AppResources.ChangePassword;
            ChangePasswordAppBarMenuItem.Click += (sender, e) =>
            { NavigationService.Navigate(new Uri("/PasswordPage.xaml?ChangePassword=1", UriKind.Relative)); };
            ApplicationBar.MenuItems.Add(ChangePasswordAppBarMenuItem);

            //var DeleteAlbumAppBarMenuItem = new ApplicationBarMenuItem();
            //DeleteAlbumAppBarMenuItem.Text = AppResources.DeleteAlbum;
            //DeleteAlbumAppBarMenuItem.Click += new EventHandler(DeleteAlbumAppBarMenuItem_Click);
            //ApplicationBar.MenuItems.Add(DeleteAlbumAppBarMenuItem);
        }

    }
}