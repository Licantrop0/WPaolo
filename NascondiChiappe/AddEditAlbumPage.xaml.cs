using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Linq;
using System.Windows.Controls;
using NascondiChiappe.Localization;
using NascondiChiappe.ViewModel;

namespace NascondiChiappe
{
    public partial class AddEditAlbumPage : PhoneApplicationPage
    {

        AddEditAlbumViewModel vm = new AddEditAlbumViewModel();
        ApplicationBarIconButton DeletePhotosAppBarButton = new ApplicationBarIconButton();
        ApplicationBarIconButton MovePhotosAppBarButton = new ApplicationBarIconButton();

        public AddEditAlbumPage()
        {
            InitializeComponent();
            InitializeApplicationBar();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (!Settings.IsPasswordInserted)
            {
                NavigationService.Navigate(new Uri("/PasswordPage.xaml", UriKind.Relative));
                return;
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (NavigationContext.QueryString.ContainsKey("Album")) //EditAlbumMode
            {
                PageTitle.Text = AppResources.EditAlbum;
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

            //TODO: Come faccio a recuperare la lista di tutti gli album che sta nell'AlbumsViewModel dalla view?
            //if (Settings.Albums.Count == 0)
            //{
            //    OneAlbumNecessaryTextBlock.Visibility = Visibility.Visible;
            //}
            //else if (Settings.Albums.Count > 1)
            //{
            //    MovePhotosAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_move.png", UriKind.Relative);
            //    MovePhotosAppBarButton.Text = AppResources.MoveSelectedPhotos;
            //    MovePhotosAppBarButton.IsEnabled = false;
            //    MovePhotosAppBarButton.Click += new EventHandler(MovePhotosAppBarButton_Click);
            //    ApplicationBar.Buttons.Add(MovePhotosAppBarButton);
            //}
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
            if (vm.SelectedPhotos.Count == 0)
            {
                MessageBox.Show(AppResources.SelectPhotos);
                return;
            }
            PopupBackground.Visibility = Visibility.Visible;
            PopupBorder.Visibility = Visibility.Visible;
        }

        void DeletePhotosAppBarMenuItem_Click(object sender, EventArgs e)
        {
            if (vm.SelectedPhotos.Count == 0)
            {
                MessageBox.Show(AppResources.SelectPhotos);
                return;
            }

            if (MessageBox.Show(vm.SelectedPhotos.Count == 1 ?
                AppResources.ConfirmPhotoDelete :
                AppResources.ConfirmPhotosDelete,
                AppResources.Confirm, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                for (int i = 0; i < vm.SelectedPhotos.Count; i++)
                    vm.CurrentAlbum.Model.RemovePhoto(vm.SelectedPhotos[i]);
            }
        }

        void SaveAppBarButton_Click(object sender, EventArgs e)
        {
            if (!CheckAlbumName())
                return;

            //TODO: Come fare ad accedere alla lista totale degli album?!
            //if (WPCommon.TrialManagement.IsTrialMode &&
            //    Settings.Albums.Count >= 1 &&
            //    !NavigationContext.QueryString.ContainsKey("Album"))
            //{
            //    NavigationService.Navigate(new Uri("/DemoPage.xaml", UriKind.Relative));
            //    return;
            //}

            NavigationService.GoBack();
        }

        void DeleteAlbumAppBarMenuItem_Click(object sender, EventArgs e)
        {
            //if (MessageBox.Show(string.Format(AppResources.ConfirmAlbumDelete, CurrentAlbum.Name),
            //    AppResources.Confirm, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            //{
            //    vm.CurrentAlbum.RemoveDirectoryContent();
            //    Settings.Albums.Remove(CurrentAlbum);
            //    NavigationService.GoBack();
            //}
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
            for (int i = 0; i < vm.SelectedPhotos.Count; i++)
            {
                vm.CurrentAlbum.Model.MovePhoto(vm.SelectedPhotos[i], (Album)e.AddedItems[0]);
            }

            PopupBackground.Visibility = Visibility.Collapsed;
            PopupBorder.Visibility = Visibility.Collapsed;
        }

        private void ImagesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NavigationContext.QueryString.ContainsKey("Album")) //EditAlbumMode
            {
                var ArePhotosSelected = vm.SelectedPhotos.Count > 0;

                DeletePhotosAppBarButton.IsEnabled = ArePhotosSelected;
                MovePhotosAppBarButton.IsEnabled = ArePhotosSelected;
            }
        }
    }
}
