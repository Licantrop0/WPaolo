using DeathTimerz.Localization;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Advertising.Mobile.UI;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Linq;
using System.Windows.Media;

namespace DeathTimerz
{
    public partial class DeathTimerzPanorama : PhoneApplicationPage
    {
        ApplicationBarIconButton EditTestAppBarButton;
        ApplicationBarIconButton PinToStartAppBarButton;

        public DeathTimerzPanorama()
        {
            InitializeComponent();
            InitializeApplicationBar();
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification == "TestUpdated")
                    EditTestAppBarButton.IsEnabled = true;
            });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!AppContext.BirthDay.HasValue)
                MainPanorama.DefaultItem = MainPanorama.Items[3];


            if (AdPlaceHolder.Children.Count == 0)
            {
#if DEBUG
                var ad1 = new AdControl("test_client", "Image480_80", true)
                {
                    Height = 80,
                    Width = 480,
                    
                };
#else
                var ad1 = new AdControl("d4a3587c-e7e3-4663-972a-dd3c4dd7a3a2",
                    "10022419", true) { Height = 80, Width = 480, IsAutoRefreshEnabled= false };
#endif

                ad1.ErrorOccurred += ad1_ErrorOccurred;
                ad1.AdRefreshed += ad1_AdRefreshed;

            }

            base.OnNavigatedTo(e);
        }

        void ad1_AdRefreshed(object sender, EventArgs e)
        {
            AdPlaceHolder.Children.Add((UIElement)sender);
        }

        void ad1_ErrorOccurred(object sender, Microsoft.Advertising.AdErrorEventArgs e)
        {
            //AdPlaceHolder.Background = new SolidColorBrush(Colors.Blue);
            //AdPlaceHolder.Height = 0;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            AdPlaceHolder.Children.Clear();
            base.OnNavigatedFrom(e);
        }

        private void MainPanorama_Loaded(object sender, RoutedEventArgs e)
        {
            MainPanorama_SelectionChanged(sender, null);
        }

        private void MainPanorama_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainPanorama.SelectedIndex == 0 &&
                PinToStartAppBarButton.IsEnabled) //Health Suggestion
            {
                ApplicationBar.Buttons.Add(PinToStartAppBarButton);
                ApplicationBar.Mode = ApplicationBarMode.Default;
                SuggestionGrid.Margin = new Thickness(0, -30, 0, 72);
            }
            else if (MainPanorama.SelectedIndex == 2) //Death-Test
            {
                ApplicationBar.Buttons.Add(EditTestAppBarButton);
                ApplicationBar.Mode = ApplicationBarMode.Default;
            }
            else
            {
                ApplicationBar.Buttons.Remove(EditTestAppBarButton);
                ApplicationBar.Buttons.Remove(PinToStartAppBarButton);
                ApplicationBar.Mode = ApplicationBarMode.Minimized;
                SuggestionGrid.Margin = new Thickness(0, -30, 0, 30);
            }
        }

        private void InitializeApplicationBar()
        {
            EditTestAppBarButton = new ApplicationBarIconButton();
            EditTestAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_edit.png", UriKind.Relative);
            EditTestAppBarButton.IsEnabled = AppContext.TimeToDeath.HasValue;
            EditTestAppBarButton.Text = AppResources.EditTest;
            EditTestAppBarButton.Click += (sender, e) => NavigationService.Navigate(new Uri("/View/TestPage.xaml", UriKind.Relative));

            PinToStartAppBarButton = new ApplicationBarIconButton();
            PinToStartAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_pin.png", UriKind.Relative);
            PinToStartAppBarButton.IsEnabled = ShellTile.ActiveTiles.Count() == 1;
            PinToStartAppBarButton.Text = AppResources.PinToStart;
            PinToStartAppBarButton.Click += (sender, e) => CreateSecondaryTile();

            var SettingsAppBarMenuItem = new ApplicationBarMenuItem();
            SettingsAppBarMenuItem.Text = Sounds.SoundManager.Instance.MusicEnabled ?
                AppResources.DisableMusic : AppResources.EnableMusic;
            SettingsAppBarMenuItem.Click += (sender, e) =>
            {
                Sounds.SoundManager.Instance.MusicEnabled = !Sounds.SoundManager.Instance.MusicEnabled;
                SettingsAppBarMenuItem.Text = Sounds.SoundManager.Instance.MusicEnabled ?
                    AppResources.DisableMusic : AppResources.EnableMusic;
            };
            ApplicationBar.MenuItems.Add(SettingsAppBarMenuItem);

            var AboutAppBarMenuItem = new ApplicationBarMenuItem();
            AboutAppBarMenuItem.Text = AppResources.About;
            AboutAppBarMenuItem.Click += (sender, e) =>
            { NavigationService.Navigate(new Uri("/View/AboutPage.xaml", UriKind.Relative)); };
            ApplicationBar.MenuItems.Add(AboutAppBarMenuItem);

            var DisclaimerAppBarMenuItem = new ApplicationBarMenuItem();
            DisclaimerAppBarMenuItem.Text = AppResources.Disclaimer;
            DisclaimerAppBarMenuItem.Click += (sender, e) =>
            { NavigationService.Navigate(new Uri("/View/DisclaimerPage.xaml", UriKind.Relative)); };
            ApplicationBar.MenuItems.Add(DisclaimerAppBarMenuItem);
        }

        private void CreateSecondaryTile()
        {
            var tileData = new StandardTileData()
            {
                Title = AppResources.AppName,
                BackgroundImage = new Uri("/Background.png", UriKind.Relative),
                BackBackgroundImage = new Uri("isostore:/Shared/ShellContent/LiveTileIcon.jpg"),
            };

            ShellTile.Create(new Uri("/View/DeathTimerzPanorama.xaml", UriKind.Relative), tileData);
        }

    }

}
