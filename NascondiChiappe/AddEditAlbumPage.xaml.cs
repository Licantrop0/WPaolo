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
            if (NavigationContext.QueryString.ContainsKey("Album"))
            {
                PageTitle.Text = AppResources.EditAlbum;

                var DeletePicturesAppBarButton = new ApplicationBarIconButton();
                DeletePicturesAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_cancel.png", UriKind.Relative);
                DeletePicturesAppBarButton.Text = AppResources.DeleteSelected;
                DeletePicturesAppBarButton.Click += new EventHandler(DeletePicturesAppBarMenuItem_Click);
                ApplicationBar.Buttons.Add(DeletePicturesAppBarButton);

                var DeleteAlbumAppBarMenuItem = new ApplicationBarMenuItem();
                DeleteAlbumAppBarMenuItem.Text = AppResources.DeleteAlbum;
                DeleteAlbumAppBarMenuItem.Click += new EventHandler(DeleteAlbumAppBarMenuItem_Click);
                ApplicationBar.MenuItems.Add(DeleteAlbumAppBarMenuItem);

                var id = Convert.ToInt32(NavigationContext.QueryString["Album"]);
                CurrentAlbum = Settings.Albums[id];
                LayoutRoot.DataContext = CurrentAlbum;
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
        }

        void DeletePicturesAppBarMenuItem_Click(object sender, EventArgs e)
        {
            foreach (BitmapImage item in ImagesListBox.SelectedItems)
            {
                CurrentAlbum.Images.Remove(item);
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
                Settings.Albums.Add(new Album(AlbumNameTextBox.Text, CurrentAlbum.DirectoryName) { Images = (ObservableCollection<BitmapImage>)ImagesListBox.ItemsSource });
                Settings.Albums.Remove(CurrentAlbum);
            }
            NavigationService.GoBack();
        }

        void DeleteAlbumAppBarMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(string.Format(AppResources.ConfirmDelete, CurrentAlbum.Name),
                AppResources.Confirm, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
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
    }
}