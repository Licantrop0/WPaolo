using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using WPCommon;

namespace FillTheSquare
{
    public partial class CongratulationsPage : PhoneApplicationPage
    {
        Record CurrentRecord;

        public CongratulationsPage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (NavigationContext.QueryString.ContainsKey("id"))
            {
                var RecordId = int.Parse(NavigationContext.QueryString["id"]);
                CurrentRecord = Settings.Records.Where(r => r.Id == RecordId).First();
                this.DataContext = CurrentRecord;
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (!NonLinearNavigationService.Instance.IsRecursiveBackNavigation)
            {
                BouncingPhilStoryboard.Begin();
                Settings.VictorySound.Play();
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (!NonLinearNavigationService.Instance.IsRecursiveBackNavigation)
            {
                CurrentRecord.Name = NameTextBox.Text;
            }
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            if (string.IsNullOrEmpty(NameTextBox.Text))
                MessageBox.Show(AppResources.PleaseInsertName);
            else
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void GoToRecords_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(NameTextBox.Text))
                MessageBox.Show(AppResources.PleaseInsertName);
            else
                NavigationService.Navigate(new Uri("/RecordsPage.xaml", UriKind.Relative));
        }

        private void NameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                this.Focus();

            if (NameTextBox.Text.Length >= 10)
                e.Handled = true;
        }
    }
}