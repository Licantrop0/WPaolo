using System;
using System.Windows;
using iCub.Helpers;
using Microsoft.Advertising.Mobile.UI;
using Microsoft.Phone.Controls;

namespace iCub
{
    public partial class MainPanoramaPage : PhoneApplicationPage
    {
        private AdControl AdControl1;

        public MainPanoramaPage()
        {
            InitializeComponent();
            AdControl1 = new AdControl("1f60dbed-6964-46b6-b0ea-b847a6306ce2", "10022720", true)
            {
                Height = 80,
                Width = 480
            };
        }

        //Hack per evitare il crash dell'AdControl quando si naviga
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            AdPlaceholder.Children.Add(AdControl1);
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            AdPlaceholder.Children.Clear();
            base.OnNavigatedFrom(e);
        }

        #region Url Click Events

        private void Twitter_Click(object sender, RoutedEventArgs e)
        {
            LinksHelper.OpenUrl(new Uri("http://mobile.twitter.com/iCub"));
        }

        private void YouTube_Click(object sender, RoutedEventArgs e)
        {
            LinksHelper.OpenUrl(new Uri("http://m.youtube.com/robotcub"));
        }

        private void Facebook_Click(object sender, RoutedEventArgs e)
        {
            LinksHelper.OpenUrl(new Uri("http://m.facebook.com/pages/iCub-the-humanoid-robot/10150110507640327"));
        }

        private void OfficialSite_Click(object sender, RoutedEventArgs e)
        {
            LinksHelper.OpenUrl(new Uri("http://www.icub.org/"));
        }

        #endregion
    }
}