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
            AddFakeRecords();
            RecordListBox.ItemsSource = Settings.Records.OrderBy(r => r.ElapsedTime);
        }

        private static void AddFakeRecords()
        {
            var rnd = new Random();
            for (int i = 0; i < 100; i++)
            {
                Settings.Records.Add(new Record(
                    (i % 2 + 1) * 5,
                    DateTime.Now.AddDays(-rnd.Next(365)).AddHours(-rnd.Next(24)).AddMinutes(-rnd.Next(60)),
                    TimeSpan.FromSeconds(rnd.Next(1000))));
            }
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, CancelEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}