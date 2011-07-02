using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using NascondiChiappe.Localization;
using NascondiChiappe.ViewModel;
using NascondiChiappe.Model;
using GalaSoft.MvvmLight.Messaging;

namespace NascondiChiappe.View
{
    public partial class AlbumsPage : PhoneApplicationPage
    {
        ApplicationBarIconButton CopyToMediaLibraryAppBarButton;
        ApplicationBarIconButton DeletePhotosAppBarButton;
        ApplicationBarIconButton CopyFromMediaLibraryAppBarButton;
        ApplicationBarIconButton MovePhotosAppBarButton;

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

            Messenger.Default.Register<bool>(this, "SelectedPhotos",
                AreSelected => UpdateSelectionButtons(AreSelected));
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
                VM.NewAlbum.Execute(null);
                return;
            }
        }


        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void UpdateSelectionButtons(bool areSelected)
        {
            if (areSelected)
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
                if (ApplicationBar.Buttons.Count == 2) return;

                ApplicationBar.Buttons.Remove(CopyToMediaLibraryAppBarButton);
                ApplicationBar.Buttons.Remove(DeletePhotosAppBarButton);
                ApplicationBar.Buttons.Remove(MovePhotosAppBarButton);
                ApplicationBar.Buttons.Add(CopyFromMediaLibraryAppBarButton);
            }
        }

        private void GestureListener_DoubleTap(object sender, GestureEventArgs e)
        {
            var CurrentImage = sender as Image;
            VM.SelectedAlbum.ShowImage.Execute(CurrentImage.Source);
        }


        void ShowOtherAlbumsListPopup(object sender, EventArgs e)
        {

        }

        private void InitializeApplicationBar()
        {
            var TakePictureAppBarButton = new ApplicationBarIconButton();
            TakePictureAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_camera.png", UriKind.Relative);
            TakePictureAppBarButton.Text = AppResources.TakePhoto;
            TakePictureAppBarButton.Click += (sender, e) => { VM.TakePicture.Execute(null); };
            ApplicationBar.Buttons.Add(TakePictureAppBarButton);

            CopyFromMediaLibraryAppBarButton = new ApplicationBarIconButton();
            CopyFromMediaLibraryAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_addpicture.png", UriKind.Relative);
            CopyFromMediaLibraryAppBarButton.Text = AppResources.CopyFromMediaLibrary;
            CopyFromMediaLibraryAppBarButton.Click += (sender, e) => { VM.CopyFromMediaLibrary.Execute(null); };
            ApplicationBar.Buttons.Add(CopyFromMediaLibraryAppBarButton);

            CopyToMediaLibraryAppBarButton = new ApplicationBarIconButton();
            CopyToMediaLibraryAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_sendphoto.png", UriKind.Relative);
            CopyToMediaLibraryAppBarButton.Text = AppResources.CopyToMediaLibrary;
            CopyToMediaLibraryAppBarButton.Click += (sender, e) => { VM.CopyToMediaLibrary.Execute(null); };

            DeletePhotosAppBarButton = new ApplicationBarIconButton();
            DeletePhotosAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_cancel.png", UriKind.Relative);
            DeletePhotosAppBarButton.Text = AppResources.DeleteSelectedPhotos;
            DeletePhotosAppBarButton.Click += (sender1, e1) => { VM.DeletePhotos.Execute(null); };

            MovePhotosAppBarButton = new ApplicationBarIconButton();
            MovePhotosAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_move.png", UriKind.Relative);
            MovePhotosAppBarButton.Text = AppResources.MoveSelectedPhotos;
            MovePhotosAppBarButton.Click += new EventHandler(ShowOtherAlbumsListPopup);

            var RenameAlbumAppBarMenuItem = new ApplicationBarMenuItem();
            RenameAlbumAppBarMenuItem.Text = AppResources.EditAlbum;
            RenameAlbumAppBarMenuItem.Click += (sender, e) => { VM.RenameAlbum.Execute(null); };
            ApplicationBar.MenuItems.Add(RenameAlbumAppBarMenuItem);

            var AddAlbumAppBarMenuItem = new ApplicationBarMenuItem();
            AddAlbumAppBarMenuItem.Text = AppResources.AddAlbum;
            AddAlbumAppBarMenuItem.Click += (sender, e) => { VM.NewAlbum.Execute(null); };
            ApplicationBar.MenuItems.Add(AddAlbumAppBarMenuItem);

            var DeleteAlbumAppBarMenuItem = new ApplicationBarMenuItem();
            DeleteAlbumAppBarMenuItem.Text = AppResources.DeleteAlbum;
            DeleteAlbumAppBarMenuItem.Click += (sender, e) => { VM.DeleteAlbum.Execute(null); };
            ApplicationBar.MenuItems.Add(DeleteAlbumAppBarMenuItem);
        }

    }
}