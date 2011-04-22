using System;
using System.ComponentModel;
using Microsoft.Phone.Controls;
using System.Linq;
using System.Windows.Controls;

namespace FillTheSquare
{
    public partial class RecordsPage : PhoneApplicationPage
    {
        public RecordsPage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            //Settings.AddFakeRecords();
            RecordListBox.ItemsSource = Settings.Records.OrderBy(r => r.ElapsedTime);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}