﻿using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Tasks;
using Microsoft.Advertising.Mobile.UI;

namespace WPCommon.Controls
{
    public partial class About : UserControl
    {
        private string _adUnitId = "10022566";
        /// <summary>Set this value to the Ad Unit ID to serve Ads</summary>
        public string AdUnitId
        {
            get { return _adUnitId; }
            set { _adUnitId = value; }
        }

        public About()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var ad1 = new AdControl("21ecdf14-8a3e-49e6-b19a-a94b96b2eb0e", AdUnitId, true)
            {
                Height = 80, Width = 480,
                VerticalAlignment = VerticalAlignment.Bottom
            };

            MainGrid.Children.Add(ad1);
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
                Subject = string.Format("[{0} {1}] {2}",
                    AppNameTextBlock.Text,
                    AppVersionTextBlock.Text,
                    "Feedback"),
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
                    ContentIdentifier = b.Tag.ToString()
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
