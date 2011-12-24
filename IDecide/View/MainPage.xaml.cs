using System;
using System.Threading;
using System.Windows.Navigation;
using System.Windows.Threading;
using IDecide.Localization;
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

        private void InitializeTimer()
        {
            tmr = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(15) };
            tmr.Tick += (sender, e) =>
            {
                CrowStoryboard.Begin();
                CrowAnimation.Begin();
            };

            tmr.Start();
        }

        private void InizializeShaker()
        {
            ShakeGesturesHelper.Instance.ShakeGesture += (sender, e) =>
            {
                Dispatcher.BeginInvoke(() =>
                {
                    CrowStoryboard.Stop();
                    CrowAnimation.Stop();

                    CloudAppearStoryboard.Begin();
                    LampAppearStoryboard.Begin();
                });
                ShakeGesturesHelper.Instance.Active = false;
                Thread.Sleep(TimeSpan.FromSeconds(5));
                ShakeGesturesHelper.Instance.Active = true;
            };

            ShakeGesturesHelper.Instance.MinimumRequiredMovesForShake = 4;
            ShakeGesturesHelper.Instance.ShakeMagnitudeWithoutGravitationThreshold = 0.3;
            ShakeGesturesHelper.Instance.Active = true;
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

        #region Ad Inizialization

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
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
            AdPlaceHolder.Children.Clear();
            base.OnNavigatedFrom(e);
        } 

        #endregion
    }
}