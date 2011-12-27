using System;
using System.Windows.Navigation;
using DeathTimerz.Localization;
using DeathTimerz.ViewModel;
using Microsoft.Advertising.Mobile.UI;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using System.Linq;

namespace DeathTimerz
{
    public partial class DeathTimerzPanorama : PhoneApplicationPage
    {
        ApplicationBarIconButton EditTestAppBarButton;

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
                var ad1 = new AdControl("d4a3587c-e7e3-4663-972a-dd3c4dd7a3a2",
                    "10022419", true) { Height = 80, Width = 480 };

                AdPlaceHolder.Children.Add(ad1);
            }
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            AdPlaceHolder.Children.Clear();
            base.OnNavigatedFrom(e);
        }

        private void MainPanorama_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainPanorama.SelectedIndex == 2) //Death-Test
            {
                ApplicationBar.Buttons.Add(EditTestAppBarButton);
                ApplicationBar.Mode = ApplicationBarMode.Default;
                AdPlaceHolder.Height = 122; //80 + (72px - 30) dell'ApplicationBar
            }
            else
            {
                ApplicationBar.Mode = ApplicationBarMode.Minimized;
                ApplicationBar.Buttons.Remove(EditTestAppBarButton);
                AdPlaceHolder.Height = 80;
            }
        }

        private void InitializeApplicationBar()
        {
            EditTestAppBarButton = new ApplicationBarIconButton();
            EditTestAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_edit.png", UriKind.Relative);
            EditTestAppBarButton.IsEnabled = AppContext.TimeToDeath.HasValue;
            EditTestAppBarButton.Text = AppResources.EditTest;
            EditTestAppBarButton.Click += (sender, e) =>
            { NavigationService.Navigate(new Uri("/View/TestPage.xaml", UriKind.Relative)); };


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
    }

}
