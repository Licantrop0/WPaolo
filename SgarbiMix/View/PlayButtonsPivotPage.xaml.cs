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
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace SgarbiMix
{
    public partial class PlayButtonsPivotPage : PhoneApplicationPage
    {
        ShakeGesturesHelper _shaker;

        public PlayButtonsPivotPage()
        {
            InitializeComponent();
            InitializeShaker();
            adSwitcher.LoadingError = s => MainPivot.Margin = new Thickness(0, -30, 0, 0);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _shaker.Active = true;
            if (TrialManagement.IsTrialMode)
            {
                MainPivot.Margin = new Thickness(0, -30, 0, 80);
                adSwitcher.AddAdvertising();
            }
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _shaker.Active = false;
            adSwitcher.RemoveAdvertising();
            base.OnNavigatedFrom(e);
        }

        private void InitializeShaker()
        {
            _shaker = ShakeGesturesHelper.Instance;
            _shaker.ShakeGesture += (sender, e) =>
            {
                _shaker.Active = false;
                var snd = AppContext.GetRandomSound();
                Deployment.Current.Dispatcher.BeginInvoke(() => snd.PlayCommand.Execute(null));

                //Questa sleep viene fatta nel thread dell'accelerometro, non blocca la UI
                Thread.Sleep(snd.Duration + TimeSpan.FromMilliseconds(300));
                _shaker.Active = true;
            };
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Song song = Song.FromUri("capra", new Uri("Sounds\\Capra!.mp3", UriKind.Relative));
            MediaPlayer.Play(song);
        }
    }
}