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
using Microsoft.Phone.Tasks;
using iCub.Helpers;

namespace iCub
{
    public partial class MainPanoramaPage : PhoneApplicationPage
    {
        public MainPanoramaPage()
        {
            InitializeComponent();
        }


        #region Url Click Events

        private void Twitter_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserHelper.Open(new Uri("http://mobile.twitter.com/iCub"));
        }

        private void YouTube_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserHelper.Open(new Uri("http://m.youtube.com/robotcub"));
        }

        private void Facebook_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserHelper.Open(new Uri("http://m.facebook.com/pages/iCub-the-humanoid-robot/10150110507640327"));
        }

        private void OfficialSite_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserHelper.Open(new Uri("http://www.icub.org/"));
        }

        private void IIT_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GiorgioMetta_Click(object sender, RoutedEventArgs e)
        {

        }

        private void iCubSupport_Click(object sender, RoutedEventArgs e)
        {

        }

        private void HackerList_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SerenaIvaldi_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion
    }
}