using Microsoft.Phone.Controls;
using SgarbiMix.WP7.ViewModel;
using ShakeGestures;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Navigation;
using WPCommon.Helpers;

namespace SgarbiMix.WP7.View
{
    public partial class MainPage : PhoneApplicationPage
    {
        ShakeGesturesHelper _shaker;
        public MainPage()
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
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _shaker.Active = false;
            adSwitcher.RemoveAdvertising();
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
            MainViewModel.PlayBase("base1");
        }

        private void Base2ApplicationBar_Click(object sender, EventArgs e)
        {
            MainViewModel.PlayBase("base2");
        }

        private void Base3ApplicationBar_Click(object sender, EventArgs e)
        {
            MainViewModel.PlayBase("base3");
        }

        private void DisclaimerAppBarMenu_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/DisclaimerPage.xaml", UriKind.Relative));
        }

        private void SuggestionAppBarMenu_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/SuggestionPage.xaml", UriKind.Relative));
        }

        private void AboutAppBarMenu_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/AboutPage.xaml", UriKind.Relative));
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (AppContext.AllSound == null)
                NavigationService.Navigate(new Uri("/View/UpdatePage.xaml", UriKind.Relative));

        }
    }
}