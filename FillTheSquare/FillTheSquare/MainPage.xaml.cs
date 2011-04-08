﻿using System;
using System.Windows;
using Microsoft.Phone.Controls;
using System.Linq;
using System.Windows.Controls;

namespace FillTheSquare
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void SquareFiveButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.StartSound.Play();
            NavigationService.Navigate(new Uri("/SquarePage.xaml?size=5", UriKind.Relative));
        }

        private void SquareTenButton_Click(object sender, RoutedEventArgs e)
        {
            if (WPCommon.TrialManagement.IsTrialMode)
            {
                NavigationService.Navigate(new Uri("/DemoPage.xaml", UriKind.Relative));
            }
            else
            {
                Settings.StartSound.Play();
                NavigationService.Navigate(new Uri("/SquarePage.xaml?size=10", UriKind.Relative));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Settings.AddFakeRecords();
            var id = Settings.Records.Last().Id;
            NavigationService.Navigate(new Uri("/CongratulationsPage.xaml?id=" + id, UriKind.Relative));
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            var me = (MediaElement)sender;
            me.Stop();
            me.Play();
        }

    }
}