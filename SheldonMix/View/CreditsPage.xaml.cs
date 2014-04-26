using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System;
using System.Windows;

namespace SheldonMix.View
{
    public partial class CreditsPage : PhoneApplicationPage
    {
        public CreditsPage()
        {
            InitializeComponent();
        }

        private void Facebook_Click(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { Uri = new Uri("https://m.facebook.com/pages/Team-Manga/709494562407755") }.Show();
        }

        private void Twitter_Click(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { Uri = new Uri("http://mobile.twitter.com/Dokuta_Fu") }.Show();
        }
    }
}