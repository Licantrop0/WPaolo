using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using NascondiChiappe.Localization;
using NascondiChiappe.ViewModel;

namespace NascondiChiappe
{
    public partial class AddEditAlbumPage : PhoneApplicationPage
    {
        ApplicationBarIconButton DeletePhotosAppBarButton = new ApplicationBarIconButton();
        ApplicationBarIconButton MovePhotosAppBarButton = new ApplicationBarIconButton();

        private AddEditAlbumViewModel _vM;
        public AddEditAlbumViewModel VM
        {
            get
            {
                if (_vM == null)
                    _vM = LayoutRoot.DataContext as AddEditAlbumViewModel;
                return _vM;
            }
        }

        public AddEditAlbumPage()
        {
            InitializeComponent();
            InitializeApplicationBar();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (!AppContext.IsPasswordInserted)
            {
                NavigationService.Navigate(new Uri("/View/PasswordPage.xaml", UriKind.Relative));
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (NavigationContext.QueryString.ContainsKey("Album")) //EditAlbumMode
            {
                var AlbumId = NavigationContext.QueryString["Album"];

                DeletePhotosAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_cancel.png", UriKind.Relative);
                DeletePhotosAppBarButton.Text = AppResources.DeleteSelectedPhotos;
                DeletePhotosAppBarButton.IsEnabled = false;
                DeletePhotosAppBarButton.Click += new EventHandler(DeletePhotosAppBarMenuItem_Click);
                ApplicationBar.Buttons.Add(DeletePhotosAppBarButton);

                var DeleteAlbumAppBarMenuItem = new ApplicationBarMenuItem();
                DeleteAlbumAppBarMenuItem.Text = AppResources.DeleteAlbum;
                DeleteAlbumAppBarMenuItem.Click += new EventHandler(DeleteAlbumAppBarMenuItem_Click);
                ApplicationBar.MenuItems.Add(DeleteAlbumAppBarMenuItem);
            }
            else
                AlbumNameTextBox.Focus();

            if (AppContext.Albums.Count > 1)
            {
                MovePhotosAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_move.png", UriKind.Relative);
                MovePhotosAppBarButton.Text = AppResources.MoveSelectedPhotos;
                MovePhotosAppBarButton.IsEnabled = false;
                MovePhotosAppBarButton.Click += new EventHandler(MovePhotosAppBarButton_Click);
                ApplicationBar.Buttons.Add(MovePhotosAppBarButton);
            }
        }

        private void InitializeApplicationBar()
        {
            var SaveAppBarButton = new ApplicationBarIconButton();
            SaveAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_save.png", UriKind.Relative);
            SaveAppBarButton.Text = AppResources.Save;
            SaveAppBarButton.Click += new EventHandler(SaveAppBarButton_Click);
            ApplicationBar.Buttons.Add(SaveAppBarButton);
        }

        void MovePhotosAppBarButton_Click(object sender, EventArgs e)
        {
            if (VM.SelectedPhotos.Count == 0)
            {
                MessageBox.Show(AppResources.SelectPhotos);
                return;
            }
            PopupBackground.Visibility = Visibility.Visible;
            PopupBorder.Visibility = Visibility.Visible;
        }

        void DeletePhotosAppBarMenuItem_Click(object sender, EventArgs e)
        {
            VM.DeletePhotos.Execute(null);
        }

        void SaveAppBarButton_Click(object sender, EventArgs e)
        {
            if (!CheckAlbumName())
                return;

            VM.SaveAlbum.Execute(null);
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

        void DeleteAlbumAppBarMenuItem_Click(object sender, EventArgs e)
        {
            VM.DeleteAlbum.Execute(null);
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

        private void AlbumNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SaveAppBarButton_Click(sender, EventArgs.Empty);
        }

        private void AlbumsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            for (int i = 0; i < VM.SelectedPhotos.Count; i++)
            {
                VM.CurrentAlbum.MovePhoto(VM.SelectedPhotos[i], (Album)e.AddedItems[0]);
            }

            PopupBackground.Visibility = Visibility.Collapsed;
            PopupBorder.Visibility = Visibility.Collapsed;
        }

        private void ImagesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NavigationContext.QueryString.ContainsKey("Album")) //EditAlbumMode
            {
                var ArePhotosSelected = VM.SelectedPhotos.Count > 0;

                DeletePhotosAppBarButton.IsEnabled = ArePhotosSelected;
                MovePhotosAppBarButton.IsEnabled = ArePhotosSelected;
            }
        }
    }
}
