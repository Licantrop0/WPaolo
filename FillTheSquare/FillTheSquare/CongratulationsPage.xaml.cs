using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using WPCommon;
using Microsoft.Xna.Framework.Audio;
using FillTheSquare.Localization;
using FillTheSquare.Sounds;
using Microsoft.Phone.Tasks;

namespace FillTheSquare
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
                SoundManager.PlayVictory();

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

        private void FacebookShare_Click(object sender, RoutedEventArgs e)
        {
            var fs = new FacebookSharer(
                AppResources.AppName,
                string.Format("Ho finito Phil the Square {0} in {1} secondi!", CurrentRecord.Size, CurrentRecord.ElapsedTime.TotalSeconds),
                new Uri("http://www.facebook.com/permalink.php?story_fbid=223498064336762&id=192414040771354"),
                new Uri("http://image.catalog.zune.net/v3.2/image/8d36721a-b44a-4e4a-bfbc-6d76b11db918?width=200&height=200"));

            var wbt = new WebBrowserTask();
            wbt.URL = "http://www.facebook.com/share.php?u=http%3A%2F%2Ffb-share-control.com%2F%3Ft%3DPhil%2520the%2520Square%26i%3Dhttp%253A%252F%252Fimage.catalog.zune.net%252Fv3.2%252Fimage%252F8d36721a-b44a-4e4a-bfbc-6d76b11db918%253Fwidth%253D200%2526height%253D200%26d%3DHo%2520sbloccato%2520l'obiettivo%2520finisci%2520in%2520modalit%25C3%25A0%2520facile!%26u%3Dhttp%253A%252F%252Fwww.facebook.com%252Fpermalink.php%253Fstory_fbid%253D223498064336762%2526id%253D192414040771354";
            wbt.Show();
        }
    }
}