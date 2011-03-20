using System;
using System.ComponentModel;
using Microsoft.Phone.Controls;
using System.Linq;

namespace FillTheSquare
{
    public partial class RecordsPage : PhoneApplicationPage
    {
        public RecordsPage()
        {
            InitializeComponent();
            FillRecordsList();
        }

        private void FillRecordsList()
        {
            Settings.AddFakeRecords();
            RecordListBox.ItemsSource = Settings.Records.OrderBy(r => r.ElapsedTime);
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}