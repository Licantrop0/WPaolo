using System;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TwentyTwelve_Organizer.View
{
    public partial class DemoInfoPage : PhoneApplicationPage
    {
        public DemoInfoPage()
        {
            InitializeComponent();
        }

        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            var detailTask = new MarketplaceDetailTask();
            detailTask.Show();
        }
    }
}