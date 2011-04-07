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
                CurrentAlbum = Settings.Albums[AlbumsPivot.SelectedIndex];
                ShowHint();
            }
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentAlbum = e.AddedItems[0] as Album;
            ShowHint();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                CurrentPhoto = null;
            else
                CurrentPhoto = e.AddedItems[0] as BitmapImage;
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
            try
            {
                photoChooserTask.Show();
            }
            catch (InvalidOperationException) { };
        }

        void CopyToMediaLibraryAppBarButton_Click(object sender, EventArgs e)
        {
            if (CurrentPhoto == null)
            {
                MessageBox.Show(AppResources.SelectPhoto);
                return;
            }

            var stream = new MemoryStream();
            var wb = new WriteableBitmap(CurrentPhoto);
            wb.SaveJpeg(stream, wb.PixelWidth, wb.PixelHeight, 0, 85);
            stream.Position = 0;
            new MediaLibrary().SavePicture(Guid.NewGuid().ToString(), stream);
            stream.Close();
            MessageBox.Show(AppResources.PhotoCopied);
        }

        void CaptureTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                var index = e.OriginalFileName.LastIndexOf('\\');
                CurrentAlbum.AddImage(e.ChosenPhoto, e.OriginalFileName.Substring(index + 1));
                ShowHint();
            }
        }

        private void ShowHint()
        {
            if (CurrentAlbum == null)
                return;

            AddPhotosHintTextBlock.Visibility =
                CurrentAlbum.Images.Count == 0 ?
                Visibility.Visible :
                Visibility.Collapsed;
        }


        void ViewPhotoAppBarButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri(
                string.Format("/ViewPhotosPage.xaml?Album={0}&Photo={1}",
                AlbumsPivot.SelectedIndex, CurrentAlbum.Images.IndexOf(CurrentPhoto)),
                UriKind.Relative));
        }


        private void InitializeApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            var ViewPhotoAppBarButton = new ApplicationBarIconButton();
            ViewPhotoAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_viewpic.png", UriKind.Relative);
            ViewPhotoAppBarButton.Text = AppResources.ViewPhoto;
            ViewPhotoAppBarButton.Click += new EventHandler(ViewPhotoAppBarButton_Click);
            ApplicationBar.Buttons.Add(ViewPhotoAppBarButton);

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
        }
    }
}
