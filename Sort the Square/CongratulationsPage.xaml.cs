using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using WPCommon;
using Microsoft.Xna.Framework.Audio;
using SortTheSquare.Localization;
using SortTheSquare.Sounds;

namespace SortTheSquare
{
    public partial class CongratulationsPage : PhoneApplicationPage
    {
        Record CurrentRecord;

        public CongratulationsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (!NonLinearNavigationService.Instance.IsRecursiveBackNavigation)
            {
                SoundManager.VictorySound.Play();

                if (NavigationContext.QueryString.ContainsKey("id"))
                {
                    var RecordId = int.Parse(NavigationContext.QueryString["id"]);
                    CurrentRecord = Settings.Records.Where(r => r.Id == RecordId).First();
                    this.DataContext = CurrentRecord;
                }
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (!NonLinearNavigationService.Instance.IsRecursiveBackNavigation)
            {
                CurrentRecord.Name = NameTextBox.Text;
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //Se c'è più di 1 record, inserisce automaticamente il nome del penultimo inserito
            if (Settings.Records.Count > 1)
                NameTextBox.Text = Settings.Records[Settings.Records.Count - 2].Name;
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