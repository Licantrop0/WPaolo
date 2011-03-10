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
            NavigationService.Navigate(new Uri("/SquarePage.xaml?size=10", UriKind.Relative));
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

        private void SoundOnButton_Click(object sender, RoutedEventArgs e)
        {
            SoundOffButton.IsEnabled = true;
            SoundOffButton.IsHitTestVisible = true;
            SoundOffButton.Visibility = System.Windows.Visibility.Visible;
            SoundOnButton.IsEnabled = true;
            SoundOnButton.IsHitTestVisible = true;
            SoundOnButton.Visibility = System.Windows.Visibility.Collapsed;

            //qua devo attivare la musica, cosa devo chiamare?
        }

        private void SoundOffButton_Click(object sender, RoutedEventArgs e)
        {
            SoundOnButton.IsEnabled = true;
            SoundOnButton.IsHitTestVisible = true;
            SoundOnButton.Visibility = System.Windows.Visibility.Visible;
            SoundOffButton.IsEnabled = true;
            SoundOffButton.IsHitTestVisible = true;
            SoundOffButton.Visibility = System.Windows.Visibility.Collapsed;

            //qua devo disattivare la musica, cosa devo chiamare?
        }
    }
}