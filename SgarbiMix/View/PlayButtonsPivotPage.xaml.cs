using System;
using System.Threading;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Advertising.Mobile.UI;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using SgarbiMix.ViewModel;
using ShakeGestures;
using WPCommon.Helpers;

namespace SgarbiMix
{
    public partial class PlayButtonsPivotPage : PhoneApplicationPage
    {
        ShakeGesturesHelper Shaker;

        public PlayButtonsPivotPage()
        {
            InitializeComponent();
            InitializeShaker();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Shaker.Active = true;

            if (TrialManagement.IsTrialMode)
                InitializeAd();

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Shaker.Active = false;
            AdPlaceHolder.Children.Clear();
            base.OnNavigatedFrom(e);
        }

        private void InitializeAd()
        {
            if (AdPlaceHolder.Children.Count == 1) //l'Ad c'è già
                return;

            AdPlaceHolder.Children.Add(
                new AdControl("c175f6ba-cb10-4fe3-a1de-a96480a03d3a", "10022581", true)
                { Height = 80, Width = 480 });
        }

        private void InitializeShaker()
        {
            Shaker = ShakeGesturesHelper.Instance;
            Shaker.ShakeGesture += (sender, e) =>
            {
                Shaker.Active = false;
                var snd = AppContext.GetRandomSound();
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    snd.PlayCommand.Execute(null);
                });

                //Questa sleep viene fatta nel thread dell'accelerometro, non blocca la UI
                Thread.Sleep(snd.Duration + TimeSpan.FromMilliseconds(300));
                Shaker.Active = true;
            };

            Shaker.MinimumRequiredMovesForShake = 4;
            Shaker.ShakeMagnitudeWithoutGravitationThreshold = 0.3;
        }

        private void Base1ApplicationBar_Click(object sender, EventArgs e)
        {
            PlayButtonsViewModel.PlayBase("base1");
        }

        private void Base2ApplicationBar_Click(object sender, EventArgs e)
        {
            PlayButtonsViewModel.PlayBase("base2");
        }

        private void Base3ApplicationBar_Click(object sender, EventArgs e)
        {
            PlayButtonsViewModel.PlayBase("base3");
        }

        private void DisclaimerAppBarMenu_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/DisclaimerPage.xaml", UriKind.Relative));
        }

        private void AboutAppBarMenu_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/AboutPage.xaml", UriKind.Relative));
        }

        private void Suggersci_Click(object sender, RoutedEventArgs e)
        {
            new EmailComposeTask()
            {
                Subject = "[SgarbiMix] Suggerimento insulto",
                To = "wpmobile@hotmail.it",
                Body = SuggerimentoTextBox.Text
            }.Show();
        }
    }
}