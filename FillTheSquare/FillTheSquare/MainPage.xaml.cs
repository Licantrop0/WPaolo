using System;
using System.Windows;
using Microsoft.Phone.Controls;
using System.Linq;
using System.Windows.Controls;
using Microsoft.Xna.Framework.Media;

namespace FillTheSquare
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
            MusicToggleButton.IsChecked = Settings.MusicEnabled;
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

        private void MusicToggleButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.MusicEnabled = !Settings.MusicEnabled;

            if (Settings.MusicEnabled)
            {
                Settings.AskAndPlayMusic();
            }
            else
            {
                MediaPlayer.Stop();
            }
        }
    }
}