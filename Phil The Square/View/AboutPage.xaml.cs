using System;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace FillTheSquare
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            WPMEAbout.AddAdvertising();
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            WPMEAbout.RemoveAdvertising();
            base.OnNavigatedFrom(e);
        }

        private void Youtube_Click(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { Uri = new Uri("http://m.youtube.com/watch?v=6PBK-sg2Zr0") }.Show();
        }
    }
}