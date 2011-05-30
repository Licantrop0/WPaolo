using System;
using System.Windows;
using Microsoft.Phone.Controls;
using System.Linq;
using System.Windows.Controls;
using Microsoft.Xna.Framework.Media;
using FillTheSquare.Sounds;
using System.Collections.Generic;
using FillTheSquare.ViewModel;

namespace FillTheSquare
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            //dummy variable to access the get property of MusicEnabled
            var a = SettingsViewModel.Instance.MusicEnabled;
        }

        private void SquareFiveButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.CurrentGridSize = 5;
            GoPlay();
        }

        private void SquareTenButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.CurrentGridSize = 10;
            GoPlay();
        }

        private void GoPlay()
        {
            SoundManager.PlayStart();
            Settings.SetGridState(new Stack<GridPoint>());
            Settings.CurrentElapsedTime = TimeSpan.Zero;
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