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
    public partial class CongratulationsPage : PhoneApplicationPage
    {
        public CongratulationsPage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            var RecordId = int.Parse(NavigationContext.QueryString["id"]);
            var CurrentRecord = Settings.Records.Where(r => r.Id == RecordId).First();
            Settings.VictorySound.Play();
            this.DataContext = CurrentRecord;
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void GoToRecords_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/RecordsPage.xaml", UriKind.Relative));
        }
    }
}