using System;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TwentyTwelve_Organizer
{
    public partial class DemoInfoPage : PhoneApplicationPage
    {
        public DemoInfoPage()
        {
            InitializeComponent();

            if (Settings.LightThemeEnabled)
            {
                var isource = new BitmapImage(new Uri("Images/2012background-white.jpg", UriKind.Relative));
                LayoutRoot.Background = new ImageBrush() { ImageSource = isource };
            }
        }

        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            var detailTask = new MarketplaceDetailTask();
            detailTask.Show();
        }
    }
}