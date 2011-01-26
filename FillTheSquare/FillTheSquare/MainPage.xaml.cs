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
            NavigationService.Navigate(new Uri("/FiveSquarePage.xaml?id=" + 5, UriKind.Relative));
        }

        private void SquareTenButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/FiveSquarePage.xaml?id=" + 10, UriKind.Relative));
        }

        private void InstructionsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Instructions.xaml", UriKind.Relative));
        }

        private void RecordsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Records.xaml", UriKind.Relative));
        }
    }
}