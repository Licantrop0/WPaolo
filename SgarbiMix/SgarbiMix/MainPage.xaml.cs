using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using WPCommon;


/* 
    Copyright (c) 2010 WPME
*/


namespace SgarbiMix
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void Insulta_Click(object sender, RoutedEventArgs e)
        {
            if (TrialManagement.IsTrialMode && TrialManagement.AlreadyOpenedToday)
            {
                NavigationService.Navigate(new Uri("/DemoPage.xaml", UriKind.Relative));
            }
            else
            {
                NavigationService.Navigate(new Uri("/PlaySoundPage.xaml", UriKind.Relative));
            }
        }

        private void Disclaimer_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/DisclaimerPage.xaml", UriKind.Relative));
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }
    }
}