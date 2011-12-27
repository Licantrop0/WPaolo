using System;
using System.Windows;
using Microsoft.Phone.Controls;
using System.Linq;
using System.Windows.Controls;
using SortTheSquare.Sounds;

namespace SortTheSquare
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
            MusicToggleButton.DataContext = SoundManager.Instance;
        }

        private void SquareEasyButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.CurrentGridSize = 2;
            GoPlay();
        }

        private void SquareNormalButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.CurrentGridSize = 4;
            GoPlay();
        }

        private void SquareHardButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.CurrentGridSize = 6;
            GoPlay();
        }

        private void GoPlay()
        {
            SoundManager.StartSound.Play();
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