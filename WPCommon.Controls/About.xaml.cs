using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Tasks;
using System.Threading;

namespace WPCommon.Controls
{
    public partial class About : UserControl
    {
        /// <summary>Set this value to serve Ads for a specific MS AdUnit</summary>
        public string AdUnitId
        {
            get { return AdSwitcher.ADAppId; }
            set { AdSwitcher.ADAppId = value; }
        }

        /// <summary>Set this value to serve Ads for a specific MS Application ID</summary>
        public string ApplicationId
        {
            get { return AdSwitcher.MSAppId; }
            set { AdSwitcher.MSAppId = value; }
        }

        /// <summary>Set this value to serve Ads for a specific AdDuplex App Id</summary>
        public string ADAppId
        {
            get { return AdSwitcher.ADAppId; }
            set { AdSwitcher.ADAppId = value; }
        }

        public About()
        {
            InitializeComponent();

            //Remove Contact us pivot if China
            if (Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "zh")
                LayoutRoot.Items.RemoveAt(1);
        }

        //protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        //{
        //    WPMEAbout.AddAdvertising();
        //    base.OnNavigatedTo(e);
        //}

        //protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        //{
        //    WPMEAbout.RemoveAdvertising();
        //    base.OnNavigatedFrom(e);
        //}

        //Call in OnNavigatedTo
        public void AddAdvertising()
        {
            AdSwitcher.AddAdvertising();
        }

        //Call in OnNavigatedFrom
        public void RemoveAdvertising()
        {
            AdSwitcher.RemoveAdvertising();
        }

        private void Facebook_Click(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { Uri = new Uri("http://m.facebook.com/pages/WP-Mobile-Entertainment/192414040771354") }.Show();
        }

        private void Twitter_Click(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { Uri = new Uri("http://mobile.twitter.com/wp7me") }.Show();
        }

        private void Mail_Click(object sender, RoutedEventArgs e)
        {
            new EmailComposeTask()
            {
                Subject = $"[{AppNameTextBlock.Text} {AppVersionTextBlock.Text}] {"Feedback"}",
                To = "wpmobile@hotmail.it"
            }.Show();
        }

        private void OtherAppsHyperLink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                new MarketplaceSearchTask()
                {
                    ContentType = MarketplaceContentType.Applications,
                    SearchTerms = "WPME"
                }.Show();
            }
            catch (InvalidOperationException)
            { /*do nothing */ }
        }

        private void App_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            try
            {
                new MarketplaceDetailTask()
                {
                    ContentIdentifier = b?.Tag.ToString()
                }.Show();
            }
            catch (InvalidOperationException)
            { /*do nothing */ }
        }

        private void Rate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                new MarketplaceReviewTask().Show();
            }
            catch (InvalidOperationException)
            { /*do nothing */ }
        }
    }
}
