using DeathTimerz.Localization;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

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
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!AppContext.BirthDay.HasValue)
                MainPanorama.DefaultItem = MainPanorama.Items[2];

            AdSwitcher.AddAdvertising();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            AdSwitcher.RemoveAdvertising();
        }

        private void MainPanorama_Loaded(object sender, RoutedEventArgs e)
        {
            if (MainPanorama.SelectedIndex != 0) return; //Health Suggestion
            if (ShellTile.ActiveTiles.Count() == 1 && !ExtensionMethods.IsLowMemDevice())
            {
                ApplicationBar.Mode = ApplicationBarMode.Default;
                ApplicationBar.Buttons.Add(PinToStartAppBarButton);
                SuggestionPanoramaItem.Margin = new Thickness(0, -60, 0, ApplicationBar.DefaultSize);
            }
        }

        private void MainPanorama_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplicationBar.Buttons.Remove(EditTestAppBarButton);
            ApplicationBar.Buttons.Remove(PinToStartAppBarButton);
            TakeTestButton.IsHitTestVisible = false;

            switch (MainPanorama.SelectedIndex)
            {
                case 0: //Health Suggestion
                    if (ShellTile.ActiveTiles.Count() == 1 && !ExtensionMethods.IsLowMemDevice())
                        ApplicationBar.Buttons.Add(PinToStartAppBarButton);
                    break;
                case 1: //Death-Test
                    if (AppContext.DeathDeviation.HasValue)
                        ApplicationBar.Buttons.Add(EditTestAppBarButton);
                    else
                        TakeTestButton.IsHitTestVisible = true;
                    break;
            }

            if (ApplicationBar.Buttons.Count == 0)
            {
                ApplicationBar.Mode = ApplicationBarMode.Minimized;
                SuggestionPanoramaItem.Margin = new Thickness(0, -60, 0, ApplicationBar.MiniSize);
                TimeToDeathPanoramaItem.Margin = new Thickness(0, -60, 0, ApplicationBar.MiniSize);
            }
            else
            {
                ApplicationBar.Mode = ApplicationBarMode.Default;
                SuggestionPanoramaItem.Margin = new Thickness(0, -60, 0, ApplicationBar.DefaultSize);
                TimeToDeathPanoramaItem.Margin = new Thickness(0, -60, 0, ApplicationBar.DefaultSize);
            }

        }

        private void InitializeApplicationBar()
        {
            EditTestAppBarButton = new ApplicationBarIconButton();
            EditTestAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_edit.png", UriKind.Relative);
            EditTestAppBarButton.Text = AppResources.EditTest;
            EditTestAppBarButton.Click += (sender, e) => NavigationService.Navigate(new Uri("/View/TestPage.xaml", UriKind.Relative));

            PinToStartAppBarButton = new ApplicationBarIconButton();
            PinToStartAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_pin.png", UriKind.Relative);
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
            UpdateHealthAdvicesTask.ScheduledAgent.UpdateTileData();
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
