using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using SgarbiMix.WP7.ViewModel;
using ShakeGestures;
using System;
using System.IO;
using System.IO.IsolatedStorage;
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

        private async void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            using (var isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!isf.FileExists(AppContext.XmlPath))
                {
                    if (!DeviceNetworkInformation.IsNetworkAvailable)
                    {
                        MessageBox.Show("Per il primo avvio è necessario connettersi a internet per scaricare i nuovi insulti.",
                            "Connetti e riprova", MessageBoxButton.OK);
                        AppContext.CloseApp();
                    }

                    NavigationService.Navigate(new Uri("/View/UpdatePage.xaml", UriKind.Relative));
                    return;
                }
                else
                {
                    if (!DeviceNetworkInformation.IsNetworkAvailable) return;

                    using (var file = isf.OpenFile(AppContext.XmlPath, FileMode.Open))
                    using (var NewXml = await AppContext.GetNewXmlAsync())
                    {
                        if (NewXml == null) return;
                        if (NewXml.Length == file.Length) return;
                    }
                }
            }
            var MsgBox = new CustomMessageBox()
            {
                Message = "Sono disponibili nuovi insulti, vuoi scaricarli?",
                LeftButtonContent = "Altroché!",
                RightButtonContent = "Ma sei scemo?"
            };

            MsgBox.Dismissed += (s1, e1) =>
            {
                if (e1.Result == CustomMessageBoxResult.LeftButton)
                    NavigationService.Navigate(new Uri("/View/UpdatePage.xaml", UriKind.Relative));
            };
            MsgBox.Show();
        }
    }
}