using System;
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
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CopyToMediaLibraryAppBarButton.IsEnabled = VM.ImagesSelected;
        }

        void TakePictureAppBarButton_Click(object sender, EventArgs e)
        {
            VM.TakePicture.Execute(null);
        }

        void CopyFromMediaLibraryAppBarButton_Click(object sender, EventArgs e)
        {
            VM.CopyFromMediaLibrary.Execute(null);
        }

        void CopyToMediaLibraryAppBarButton_Click(object sender, EventArgs e)
        {
            VM.CopyToMediaLibrary.Execute(null);
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
            { NavigationService.Navigate(new Uri("/AddEditAlbumPage.xaml?Album=" + VM.SelectedAlbum.Model.DirectoryName, UriKind.Relative)); };
            ApplicationBar.MenuItems.Add(EditAlbumAppBarMenuItem);

            var AddAlbumAppBarMenuItem = new ApplicationBarMenuItem();
            AddAlbumAppBarMenuItem.Text = AppResources.AddAlbum;
            AddAlbumAppBarMenuItem.Click += (sender, e) =>
            { NavigationService.Navigate(new Uri("/AddEditAlbumPage.xaml", UriKind.Relative)); };
            ApplicationBar.MenuItems.Add(AddAlbumAppBarMenuItem);
        }

    }
}
