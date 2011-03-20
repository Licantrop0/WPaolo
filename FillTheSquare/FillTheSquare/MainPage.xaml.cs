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

namespace FillTheSquare
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void SquareFiveButton_Click(object sender, RoutedEventArgs e)
        {
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
                NavigationService.Navigate(new Uri("/SquarePage.xaml?size=10", UriKind.Relative));
            }
        }

        private void InstructionsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/InstructionsPage.xaml", UriKind.Relative));
        }

        private void RecordsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/RecordsPage.xaml", UriKind.Relative));
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Settings.AddFakeRecords();
            var id = Settings.Records[0].Id;
            NavigationService.Navigate(new Uri("/CongratulationsPage.xaml?id=" + id, UriKind.Relative));
        }
    }
}