using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Threading;
using IDecide.Localization;
using IDecide.Sounds;
using Microsoft.Advertising.Mobile.UI;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ShakeGestures;
using Microsoft.Devices;

namespace IDecide
{
    public partial class MainPage : PhoneApplicationPage
    {
        DispatcherTimer tmr;
        ShakeGesturesHelper Shaker;

        public MainPage()
        {
            InitializeComponent();
            InitializeAppBar();
            InizializeShaker();
            InitializeTimer();
        }

        private void ManGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DecideButton_Click(sender, null);
        }

        private void DecideButton_Click(object sender, RoutedEventArgs e)
        {
            CrowStoryboard.Stop();
            CrowAnimation.Stop();
            SoundManager.StopCrow();
            tmr.Stop();
            tmr.Start();
            AnswerTextBlock.Text = AppContext.GetRandomChoice();
            
            if(AppContext.VibrationEnabled)
                VibrateController.Default.Start(TimeSpan.FromMilliseconds(100));

            if (AppContext.RapidResponse)
            {
                RapidResponseStoryboard.Begin();
            }
            else
            {
                SoundManager.PlayDing();
                CloudAppearStoryboard.Begin();
            }
        }

        private void CrowStoryboard_Completed(object sender, EventArgs e)
        {
            CrowAnimation.Stop();
        }


        #region Inizializations

        private void InitializeAppBar()
        {
            var EditChoicesAppBarButton = new ApplicationBarIconButton();
            EditChoicesAppBarButton.Text = AppResources.EditChoices;
            EditChoicesAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_choices.png", UriKind.Relative);
            EditChoicesAppBarButton.Click += (sender, e) =>
            {
                NavigationService.Navigate(new Uri("/View/ChoicesGroupPage.xaml", UriKind.Relative));
            };
            ApplicationBar.Buttons.Add(EditChoicesAppBarButton);

            var SettingsAppBarButton = new ApplicationBarIconButton();
            SettingsAppBarButton.Text = AppResources.Settings;
            SettingsAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_settings.png", UriKind.Relative);
            SettingsAppBarButton.Click += (sender, e) =>
            {
                NavigationService.Navigate(new Uri("/View/SettingsPage.xaml", UriKind.Relative));
            };
            ApplicationBar.Buttons.Add(SettingsAppBarButton);


            var AboutAppBarMenuItem = new ApplicationBarMenuItem();
            AboutAppBarMenuItem.Text = AppResources.About;
            AboutAppBarMenuItem.Click += (sender, e) =>
            {
                NavigationService.Navigate(new Uri("/View/AboutPage.xaml", UriKind.Relative));
            };
            ApplicationBar.MenuItems.Add(AboutAppBarMenuItem);
        }

        private void InizializeShaker()
        {
            Shaker = ShakeGesturesHelper.Instance;
            Shaker.ShakeGesture += (sender, e) =>
            {
                Dispatcher.BeginInvoke(() => DecideButton_Click(Shaker, null));
                Shaker.Active = false;
                Thread.Sleep(TimeSpan.FromSeconds(5));
                Shaker.Active = true;
            };

            Shaker.MinimumRequiredMovesForShake = 4;
            Shaker.ShakeMagnitudeWithoutGravitationThreshold = 0.3;
        }

        private void InitializeTimer()
        {
            tmr = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(15) };
            tmr.Tick += (sender, e) =>
            {
                if (CloudAppearStoryboard.GetCurrentState() == ClockState.Stopped &&
                    RapidResponseStoryboard.GetCurrentState() == ClockState.Stopped)
                {
                    CrowStoryboard.Begin();
                    CrowAnimation.Begin();
                    SoundManager.PlayCrow();
                }
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            tmr.Start();
            Shaker.Active = true;

            if (AdPlaceHolder.Children.Count == 1) //l'Ad c'è già
                return;

            AdPlaceHolder.Children.Add(new AdControl("572ef47c-fdda-4d58-ba1c-9cfd93c12d43", "10027370", true)
            {
                Height = 80,
                Width = 480,
            });
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            tmr.Stop();
            Shaker.Active = false;
            AdPlaceHolder.Children.Clear();
            base.OnNavigatedFrom(e);
        } 

        #endregion
    }
}