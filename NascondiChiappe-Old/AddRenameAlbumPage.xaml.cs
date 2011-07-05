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

namespace NascondiChiappe
{
    public partial class AddRenameAlbumPage : PhoneApplicationPage
    {
        Album CurrentAlbum;

        public AddRenameAlbumPage()
        {
            InitializeComponent();
            InitializeApplicationBar();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (!AppContext.IsPasswordInserted)
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
                CurrentAlbum = AppContext.Albums.First(a => a.DirectoryName == AlbumId);
                LayoutRoot.DataContext = CurrentAlbum;
            }
            else
                AlbumNameTextBox.Focus();

            if (AppContext.Albums.Count == 0)
            {
                OneAlbumNecessaryTextBlock.Visibility = Visibility.Visible;
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



        void SaveAppBarButton_Click(object sender, EventArgs e)
        {
            if (!CheckAlbumName())
                return;

            if (WPCommon.TrialManagement.IsTrialMode &&
                AppContext.Albums.Count >= 1 &&
                !NavigationContext.QueryString.ContainsKey("Album"))
            {
                //TODO: andare nella pagina trial
                NavigationService.Navigate(new Uri("/DemoPage.xaml", UriKind.Relative));
                return;
            }

            if (CurrentAlbum == null)
                AppContext.Albums.Add(new Album(AlbumNameTextBox.Text, Guid.NewGuid().ToString()));
            else
            {
                AppContext.Albums.Remove(CurrentAlbum);
                AppContext.Albums.Add(new Album(AlbumNameTextBox.Text, CurrentAlbum.DirectoryName));
            }
            NavigationService.GoBack();
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