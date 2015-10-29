using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using SgarbiMix.WP.ViewModel;
using ShakeGestures;
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading;
using System.Windows;
using System.Windows.Navigation;
using WPCommon.Helpers;

namespace SgarbiMix.WP.View
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
            if (AppContext.AllSound == null) return;

            _shaker.Active = true;

            if (!TrialManagement.IsTrialMode) return;

            MainPivot.Margin = new Thickness(0, -30, 0, 80);
            adSwitcher.AddAdvertising();
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

        private void Base4ApplicationBar_Click(object sender, EventArgs e)
        {
            MainViewModel.PlayBase("base4");
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
                        MessageBox.Show("Per il primo avvio ho bisogno della connessione per scaricare i nuovi insulti.",
                            "Connetti e riprova", MessageBoxButton.OK);
                        AppContext.CloseApp();
                    }

                    NavigationService.Navigate(new Uri("/View/UpdatePage.xaml", UriKind.Relative));
                    return;
                }

                if (!DeviceNetworkInformation.IsNetworkAvailable) return;

                using (var file = isf.OpenFile(AppContext.XmlPath, FileMode.Open))
                using (var newXml = await AppContext.GetNewXmlAsync())
                {
                    if (newXml == null) return;
                    if (newXml.Length == file.Length) return;
                }
            }

            var msgBox = new CustomMessageBox()
            {
                Message = "Hey! Sono disponibili nuovi insulti, vuoi scaricarli?",
                LeftButtonContent = "Altroché!",
                RightButtonContent = "mah... ora no"
            };

            msgBox.Dismissed += (s1, e1) =>
            {
                if (e1.Result == CustomMessageBoxResult.LeftButton)
                    NavigationService.Navigate(new Uri("/View/UpdatePage.xaml", UriKind.Relative));
            };
            msgBox.Show();
        }

        public int LastPivotIndex
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("pin_suggestion"))
                    IsolatedStorageSettings.ApplicationSettings["pin_suggestion"] = 0;
                return (int)IsolatedStorageSettings.ApplicationSettings["pin_suggestion"];
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["pin_suggestion"] = value;
            }
        }

        private void MainPivot_Loaded(object sender, RoutedEventArgs e)
        {
            if (MainPivot.Items.Count > 0 && LastPivotIndex < MainPivot.Items.Count - 1)
                MainPivot.SelectedIndex = LastPivotIndex;
            
            MainPivot.SelectionChanged += (s1, e1) => LastPivotIndex = MainPivot.SelectedIndex;
        }
    }
}