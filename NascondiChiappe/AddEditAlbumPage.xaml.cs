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
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;

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
                var id = Convert.ToInt32(NavigationContext.QueryString["Album"]);
                CurrentAlbum = Settings.Albums[id];
                LayoutRoot.DataContext = CurrentAlbum;
            }
            else
                AlbumNameTextBox.Focus();
        }

        private void InitializeApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            var SaveAppBarButton = new ApplicationBarIconButton();
            SaveAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_save.png", UriKind.Relative);
            SaveAppBarButton.Text = AppResources.Save;
            SaveAppBarButton.Click += (sender, e) =>
            {
                if (!CheckAlbumName())
                    return;

                if (CurrentAlbum == null)
                    Settings.Albums.Add(new Album(AlbumNameTextBox.Text));
                else
                {
                    Settings.Albums.Remove(CurrentAlbum);
                    Settings.Albums.Add(new Album(AlbumNameTextBox.Text)
                        { Images = (ObservableCollection<BitmapImage>)ImagesListBox.ItemsSource });
                }
                NavigationService.GoBack();
            };

            ApplicationBar.Buttons.Add(SaveAppBarButton);
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
                this.Focus();
        }
    }
}