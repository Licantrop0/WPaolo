using System;
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
            //Settings.AddFakeRecords();
            var id = Settings.Records.Last().Id;
            NavigationService.Navigate(new Uri("/CongratulationsPage.xaml?id=" + id, UriKind.Relative));
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            var me = (MediaElement)sender;
            me.Stop();
            me.Play();
        }


        private void MediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {

        }
    }
}