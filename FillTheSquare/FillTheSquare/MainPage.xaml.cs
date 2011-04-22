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
            Settings.CurrentGridSize = 5;
            NavigationService.Navigate(new Uri("/SquarePage.xaml", UriKind.Relative));
        }

        private void SquareTenButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.StartSound.Play();
            Settings.CurrentGridSize = 10;
            NavigationService.Navigate(new Uri("/SquarePage.xaml", UriKind.Relative));
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Settings.AddFakeRecords();
            var id = Settings.Records.Last().Id;
            NavigationService.Navigate(new Uri("/CongratulationsPage.xaml?id=" + id, UriKind.Relative));
        }
    }
}