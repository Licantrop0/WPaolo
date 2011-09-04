using System;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;
using DeathTimerz.Localization;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using DeathTimerz.ViewModel;

namespace DeathTimerz
{
    public partial class DeathTimerzPanorama : PhoneApplicationPage
    {
        private MainViewModel _vM;
        public MainViewModel VM
        {
            get
            {
                if (_vM == null)
                    _vM = MainPanorama.DataContext as MainViewModel;
                return _vM;
            }
        }


        public DeathTimerzPanorama()
        {
            InitializeComponent();
            InitializeApplicationBar();
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (Settings.EstimatedDeathAge.HasValue)
                TakeTestButton.Visibility = Visibility.Collapsed;
            else
                TakeTestButton.Visibility = Visibility.Visible;
        }

        private void MainPanorama_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (MainPanorama.SelectedIndex == 2) //Death-Test
                ApplicationBar.Buttons.Add(EditTestAppBarButton);
            else
                ApplicationBar.Buttons.Remove(EditTestAppBarButton);
        }

        ApplicationBarIconButton EditTestAppBarButton;
        private void InitializeApplicationBar()
        {
            var SettingsAppBarMenuItem = new ApplicationBarMenuItem();
            SettingsAppBarMenuItem.Text = Sounds.SoundManager.Instance.MusicEnabled ?
                AppResources.DisableMusic: AppResources.EnableMusic;

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

            EditTestAppBarButton = new ApplicationBarIconButton();
            EditTestAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_edit.png", UriKind.Relative);
            EditTestAppBarButton.Text = AppResources.EditTest;
            EditTestAppBarButton.Click += (sender, e) =>
            { NavigationService.Navigate(new Uri("/View/TestPage.xaml", UriKind.Relative)); };
        }
    }

}
