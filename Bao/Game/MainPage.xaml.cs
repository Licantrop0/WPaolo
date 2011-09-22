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

namespace BaoGame
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void newGameButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));
        }

        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }

        private void creditsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/CreditsPage.xaml", UriKind.Relative));
        }

        private void tutorialButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/TutorialPage.xaml", UriKind.Relative));
        }

        private void extraButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ExtraPage.xaml", UriKind.Relative));
        }



    }
}