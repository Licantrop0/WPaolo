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
using Microsoft.Phone.Scheduler;

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

            if (e.NavigationMode == NavigationMode.New)
            {
                AdSwitcher.AddAdvertising();
            }
       }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            AdSwitcher.RemoveAdvertising();
        }

        private void MainPanorama_Loaded(object sender, RoutedEventArgs e)
        {
            if (MainPanorama.SelectedIndex == 0 && ShellTile.ActiveTiles.Count() == 1) //Health Suggestion
            {
                ApplicationBar.Buttons.Add(PinToStartAppBarButton);
                ApplicationBar.Mode = ApplicationBarMode.Default;
                SuggestionGrid.Margin = new Thickness(0, -30, 0, 72);
            }
            else
            {
                SuggestionGrid.Margin = new Thickness(0, -30, 0, 30);
            }
        }

        private void MainPanorama_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainPanorama.SelectedIndex == 0 &&
                ShellTile.ActiveTiles.Count() == 1) //Health Suggestion
            {
                ApplicationBar.Buttons.Add(PinToStartAppBarButton);
                ApplicationBar.Mode = ApplicationBarMode.Default;
                SuggestionGrid.Margin = new Thickness(0, -30, 0, 72);
            }
            else if (MainPanorama.SelectedIndex == 2 &&
                AppContext.TimeToDeath.HasValue) //Death-Test
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

            if (ShellTile.ActiveTiles.Count() == 1)
            {
                PinToStartAppBarButton = new ApplicationBarIconButton();
                PinToStartAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_pin.png", UriKind.Relative);
                PinToStartAppBarButton.Text = AppResources.PinToStart;
                PinToStartAppBarButton.Click += (sender, e) => CreateSecondaryTile();
            }
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
            UpdateHealthAdvicesTask.ScheduledAgent.WriteFile();
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
