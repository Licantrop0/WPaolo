﻿using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using SheldonMix.Localization;
using System;
using System.Globalization;
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
            InitializeAppBar();
        }

        private void InitializeAppBar()
        {
            var rateUsAppBarButton = new ApplicationBarIconButton()
             {
                 Text = AppResources.RateUs,
                 IconUri = new Uri("/images/smile.png", UriKind.Relative)
             };
            rateUsAppBarButton.Click += (sender, e) =>
            {
                try
                {
                    new MarketplaceReviewTask().Show();
                }
                catch (InvalidOperationException)
                { /*do nothing */ }
            };
            ApplicationBar.Buttons.Add(rateUsAppBarButton);

            var aboutAppBarButton = new ApplicationBarIconButton()
            {
                Text = AppResources.About,
                IconUri = new Uri("/images/i.png", UriKind.Relative)
            };
            aboutAppBarButton.Click += (sender, e) => NavigationService.Navigate(new Uri("/View/AboutPage.xaml", UriKind.Relative));
            ApplicationBar.Buttons.Add(aboutAppBarButton);

            if (CultureInfo.CurrentUICulture.Name == "fr-FR")
            {
                var creditsAppBarButton = new ApplicationBarIconButton()
                {
                    Text = AppResources.Credits,
                    IconUri = new Uri("/images/copyrights.png", UriKind.Relative)
                };
                creditsAppBarButton.Click += (sender, e) => NavigationService.Navigate(new Uri("/View/CreditsPage.xaml", UriKind.Relative));
                ApplicationBar.Buttons.Add(creditsAppBarButton);
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            MainPivot.Margin = new Thickness(0, 0, 0, 80);
            base.OnNavigatedTo(e);
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

        private async void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            using (var isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isf.FileExists(AppContext.XmlPath))
                {
                    if (!DeviceNetworkInformation.IsNetworkAvailable) return;

                    using (var file = isf.OpenFile(AppContext.XmlPath, FileMode.Open))
                    using (var newXml = await AppContext.GetNewXmlAsync())
                    {
                        if (newXml == null) return;
                        if (newXml.Length == file.Length) return;
                    }
                }
                else
                {
                    if (!DeviceNetworkInformation.IsNetworkAvailable)
                    {
                        MessageBox.Show(AppResources.FirstLaunch,
                            AppResources.FirstLaunchTitle,
                            MessageBoxButton.OK);
                        Application.Current.Terminate();
                    }

                    NavigationService.Navigate(new Uri("/View/UpdatePage.xaml", UriKind.Relative));
                    return;
                }
            }

            var msgBox = new CustomMessageBox()
            {
                Message = AppResources.NewSoundsAvailable,
                LeftButtonContent = AppResources.Yes,
                RightButtonContent = AppResources.NotNow
            };

            msgBox.Dismissed += (s1, e1) =>
            {
                if (e1.Result == CustomMessageBoxResult.LeftButton)
                    NavigationService.Navigate(new Uri("/View/UpdatePage.xaml", UriKind.Relative));
            };
            msgBox.Show();
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