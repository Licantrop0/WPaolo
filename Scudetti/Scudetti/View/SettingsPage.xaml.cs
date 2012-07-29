using System;
using Microsoft.Phone.Controls;
using Scudetti.Sound;

namespace Scudetti.View
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void AboutButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SoundManager.PlayKick();
            NavigationService.Navigate(new Uri("/View/AboutPage.xaml", UriKind.Relative));
        }
    }
}