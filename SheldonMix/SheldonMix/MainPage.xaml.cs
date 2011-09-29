﻿using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Media;
using System.Net;
using Microsoft.Phone.Tasks;
using System.IO.IsolatedStorage;
using System.IO;
using Microsoft.Advertising.Mobile.UI;
using System.Device.Location;

namespace SheldonMix
{

    public partial class MainPage : PhoneApplicationPage
    {
        private GeoCoordinateWatcher gcw;

        public MainPage()
        {
            InitializeComponent();
            this.gcw = new GeoCoordinateWatcher();
            this.gcw.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(gcw_PositionChanged);
            this.gcw.Start();
        }

        private void gcw_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            this.gcw.Stop();

            adControl1.Latitude = e.Position.Location.Latitude;
            adControl1.Longitude = e.Position.Location.Longitude;

            gcw.Dispose();
            gcw = null;
        }

        private void AboutAppBarMenu_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

        private void HyperlinkButton_Click_4(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { Uri = new Uri("http://m.youtube.com/watch?v=LYwF2JLCHWg") }.Show();
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { Uri = new Uri("http://m.youtube.com/watch?v=SifGskrY_UY") }.Show();
        }

        private void HyperlinkButton_Click_1(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { Uri = new Uri("http://twitter.com/#!/BigBang_CBS") }.Show();
        }

        private void HyperlinkButton_Click_2(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { Uri = new Uri("http://www.cbs.com/shows/big_bang_theory") }.Show();
        }

        private void HyperlinkButton_Click_3(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { Uri = new Uri("http://www.imdb.com/name/nm1433588/") }.Show();
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

        //Ringtone files must be of type MP3 or WMA.
        //Ringtone files must be less than 40 seconds in length.
        //Ringtone files must not have digital rights management (DRM) protection.
        //Ringtone files must be less than 1 MB in size.
        private void SetRingtone_Click(object sender, RoutedEventArgs e)
        {

            var RingtonePath = "TBBT Ringtone.mp3";
            WPCommon.Helpers.Persistance.SaveFileToIsolatedStorage(RingtonePath);
            var saveRingtoneTask = new SaveRingtoneTask();
            try
            {
                saveRingtoneTask.Source = new Uri("isostore:/" + RingtonePath);
                saveRingtoneTask.DisplayName = "TBBT Intro Song";
                saveRingtoneTask.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}