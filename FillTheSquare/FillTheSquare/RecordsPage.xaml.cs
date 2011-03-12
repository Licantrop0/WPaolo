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
    public partial class RecordsPage : PhoneApplicationPage
    {
        public RecordsPage()
        {
            InitializeComponent();
            FillRecordsList();

            BackKeyPress += new EventHandler<System.ComponentModel.CancelEventArgs>(BackPage_Click);
        }

        private void FillRecordsList()
        {
            RecordListBox.ItemsSource = Settings.RecordsList;
        }

        private void BackPage_Click(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}