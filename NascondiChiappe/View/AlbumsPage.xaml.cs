using System;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using NascondiChiappe.Localization;
using NascondiChiappe.Messages;
using NascondiChiappe.ViewModel;

namespace NascondiChiappe
{
    public partial class AlbumsPage : PhoneApplicationPage
    {
        ApplicationBarIconButton CopyToMediaLibraryAppBarButton;

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
            Messenger.Default.Register<CanExecuteOnSelectedPhotosMessage>(
                this, m => CopyToMediaLibraryAppBarButton.IsEnabled = m.CanExecute);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (!AppContext.IsPasswordInserted)
            {
                NavigationService.Navigate(new Uri("/View/PasswordPage.xaml", UriKind.Relative));
                return;
            }
            if (VM.Albums.Count == 0)
            {
                NavigationService.Navigate(new Uri("/View/AddEditAlbumPage.xaml", UriKind.Relative));
                return;
            }

            if (AlbumsPivot.SelectedItem == null)
                AlbumsPivot.SelectedItem = AppContext.PreviousSelectedAlbum;
        }

        private void InitializeApplicationBar()
        {
            var TakePictureAppBarButton = new ApplicationBarIconButton();
            TakePictureAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_camera.png", UriKind.Relative);
            TakePictureAppBarButton.Text = AppResources.TakePhoto;
            TakePictureAppBarButton.Click += (sender, e) => { VM.TakePicture.Execute(null); };
            ApplicationBar.Buttons.Add(TakePictureAppBarButton);

            var CopyFromMediaLibraryAppBarButton = new ApplicationBarIconButton();
            CopyFromMediaLibraryAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_addpicture.png", UriKind.Relative);
            CopyFromMediaLibraryAppBarButton.Text = AppResources.CopyFromMediaLibrary;
            CopyFromMediaLibraryAppBarButton.Click += (sender, e) => { VM.CopyFromMediaLibrary.Execute(null); };
            ApplicationBar.Buttons.Add(CopyFromMediaLibraryAppBarButton);

            CopyToMediaLibraryAppBarButton = new ApplicationBarIconButton();
            CopyToMediaLibraryAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_sendphoto.png", UriKind.Relative);
            CopyToMediaLibraryAppBarButton.Text = AppResources.CopyToMediaLibrary;
            CopyToMediaLibraryAppBarButton.IsEnabled = false;
            CopyToMediaLibraryAppBarButton.Click += (sender, e) => { VM.CopyToMediaLibrary.Execute(null); };
            ApplicationBar.Buttons.Add(CopyToMediaLibraryAppBarButton);

            var EditAlbumAppBarMenuItem = new ApplicationBarMenuItem();
            EditAlbumAppBarMenuItem.Text = AppResources.EditAlbum;
            EditAlbumAppBarMenuItem.Click += (sender, e) =>
            { NavigationService.Navigate(new Uri("/View/AddEditAlbumPage.xaml", UriKind.Relative)); };
            ApplicationBar.MenuItems.Add(EditAlbumAppBarMenuItem);

            var NewAlbumAppBarMenuItem = new ApplicationBarMenuItem();
            NewAlbumAppBarMenuItem.Text = AppResources.NewAlbum;
            NewAlbumAppBarMenuItem.Click += (sender, e) => { VM.NewAlbum.Execute(null); };
            ApplicationBar.MenuItems.Add(NewAlbumAppBarMenuItem);
        }
    }
}
