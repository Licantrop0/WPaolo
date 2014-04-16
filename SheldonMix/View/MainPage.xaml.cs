using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Tasks;
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;

namespace SheldonMix.View
{

    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
        }



        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            MainPivot.Margin = new Thickness(0, 0, 0, 80);
            adSwitcher.AddAdvertising();
            adSwitcher.LoadingError = s => { MainPivot.Margin = new Thickness(0); };
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            adSwitcher.RemoveAdvertising();
            base.OnNavigatedFrom(e);
        }

        private void AboutAppBarMenu_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

        private void TwitterCBS_Click(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { Uri = new Uri("http://mobile.twitter.com/BigBang_CBS") }.Show();
        }

        private void FacebookCBS_Click(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { Uri = new Uri("http://m.facebook.com/TheBigBangTheory") }.Show();
        }

        private void WebCBS_Click(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { Uri = new Uri("http://www.cbs.com/shows/big_bang_theory") }.Show();
        }

        private void TwitterSheldon_Click(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { Uri = new Uri("http://mobile.twitter.com/TheRealSheldonC") }.Show();
        }

        private void FacebookSheldon_Click(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { Uri = new Uri("http://m.facebook.com/profile.php?id=23519525029") }.Show();
        }

        private void HyperlinkButton_Click_3(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { Uri = new Uri("http://www.imdb.com/name/nm1433588/") }.Show();
        }

        private void HyperlinkButton_Click_4(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { Uri = new Uri("http://m.youtube.com/watch?v=LYwF2JLCHWg") }.Show();
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { Uri = new Uri("http://m.youtube.com/watch?v=SifGskrY_UY") }.Show();
        }

        private void YoutubeEllen_Click(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { Uri = new Uri("http://m.youtube.com/watch?v=6FFV28XxB-A") }.Show();
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            try
            {
                new MarketplaceReviewTask().Show();
            }
            catch (InvalidOperationException)
            { /*do nothing */ }
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
                Message = "Hey! Sono disponibili nuovi Insulti, vuoi scaricarli?",
                LeftButtonContent = "Altroché!",
                RightButtonContent = "mah... ora no"
            };

            MsgBox.Dismissed += (s1, e1) =>
            {
                if (e1.Result == CustomMessageBoxResult.LeftButton)
                    NavigationService.Navigate(new Uri("/View/UpdatePage.xaml", UriKind.Relative));
            };
            MsgBox.Show();
        }

        //Stack<Uri> BrowserHistory = new Stack<Uri>();
        //private void PhoneApplicationPage_BackKeyPress(object sender, CancelEventArgs e)
        //{
        //    //se non sono nella pagina del web browser o non ho items nella history esco
        //    if (MainPivot.Items.Count == 4 && MainPivot.SelectedIndex != 3) return;
        //    //Pop da saltare perchè appena navigo lo mette nello stack
        //    BrowserHistory.Pop();
        //    if (BrowserHistory.Any())
        //    {
        //        e.Cancel = true;
        //        YouTubeBrowser.Navigate(BrowserHistory.Pop());
        //    }
        //}

        //private void YouTubeBrowser_Navigating(object sender, NavigatingEventArgs e)
        //{
        //    this.Focus();
        //    //LoadingProgress.IsIndeterminate = true;
        //}

        //private void YouTubeBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        //{
        //    BrowserHistory.Push(YouTubeBrowser.Source);
        //    //LoadingProgress.IsIndeterminate = false;
        //}
    }
}