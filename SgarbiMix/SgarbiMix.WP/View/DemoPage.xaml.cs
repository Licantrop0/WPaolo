using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System.Windows;

namespace SgarbiMix.WP.View
{
    public partial class DemoPage : PhoneApplicationPage
    {
        public DemoPage()
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