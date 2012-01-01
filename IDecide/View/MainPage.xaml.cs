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

namespace IDecide
{
    public partial class MainPage : PhoneApplicationPage
    {
        DispatcherTimer tmr;

        public MainPage()
        {
            InitializeComponent();
            CreateAppBar();
            InitializeTimer();
            InizializeShaker();
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
            SoundManager.PlayDing();

            CloudAppearStoryboard.Begin();
            LampAppearStoryboard.Begin();
        }

        private void AppearCloud_Completed(object sender, EventArgs e)
        {
            tmr.Stop();
            tmr.Start();
            AnswerTextBlock.Text = AppContext.GetRandomChoice();
        }

        private void CrowStoryboard_Completed(object sender, EventArgs e)
        {
            CrowAnimation.Stop();
        }

        private void CreateAppBar()
        {
            var EditChoicesAppBarButton = new ApplicationBarIconButton();
            EditChoicesAppBarButton.Text = AppResources.EditChoices;
            EditChoicesAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_settings.png", UriKind.Relative);
            EditChoicesAppBarButton.Click += (sender, e) =>
            {
                NavigationService.Navigate(new Uri("/View/ChoicesGroupPage.xaml", UriKind.Relative));
            };
            ApplicationBar.Buttons.Add(EditChoicesAppBarButton);

            var AboutAppBarMenuItem = new ApplicationBarMenuItem();
            AboutAppBarMenuItem.Text = AppResources.About;
            AboutAppBarMenuItem.Click += (sender, e) =>
            {
                NavigationService.Navigate(new Uri("/View/AboutPage.xaml", UriKind.Relative));
            };
            ApplicationBar.MenuItems.Add(AboutAppBarMenuItem);
        }

        #region Inizializations

        private void InizializeShaker()
        {
            var Shaker = ShakeGesturesHelper.Instance;

            Shaker.ShakeGesture += (sender, e) =>
            {
                Dispatcher.BeginInvoke(() => DecideButton_Click(Shaker, null));
                Shaker.Active = false;
                Thread.Sleep(TimeSpan.FromSeconds(5));
                Shaker.Active = true;
            };

            Shaker.MinimumRequiredMovesForShake = 4;
            Shaker.ShakeMagnitudeWithoutGravitationThreshold = 0.3;
            Shaker.Active = true;
        }

        private void InitializeTimer()
        {
            tmr = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(15) };
            tmr.Tick += (sender, e) =>
            {
                if (CloudAppearStoryboard.GetCurrentState() == ClockState.Stopped)
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
            AdPlaceHolder.Children.Clear();
            base.OnNavigatedFrom(e);
        } 

        #endregion
    }
}