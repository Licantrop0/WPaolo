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
using System.IO;
using System.Xml.Linq;
using System.Xml;
using System.Collections.ObjectModel;
using WPCommon;
using System.Globalization;

namespace ShowImages
{
    public partial class SearchPage : PhoneApplicationPage
    {
        private const string AppId = "013AEAA7B7C72A496033289E7F35191F03DEE3CB";
        private const string IMAGE_NS = "http://schemas.microsoft.com/LiveSearch/2008/04/XML/multimedia";
        private const string _baseURI = "http://api.search.live.net/xml.aspx?Appid={0}&sources=Image&query={1}&Market={2}&Version=2.0&Adult=Off";

        public SearchPage()
        {
            InitializeComponent();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            WebClient wc = new WebClient();
            wc.OpenReadCompleted += new
            OpenReadCompletedEventHandler(wc_OpenReadCompleted);
            // Image-specific request fields (optional)
            var newUri = String.Format(_baseURI, AppId, ImageUrlTextBox.Text, CultureInfo.CurrentUICulture.Name)
                + "&Image.Count=20"
                + "&Image.Offset=0";

            wc.OpenReadAsync(new Uri(newUri));
        }

        void wc_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            Stream streamResult = e.Result;
            XDocument xd = XDocument.Load((XmlReader.Create(streamResult)));

            var nodes = xd.Descendants(XName.Get("Results", IMAGE_NS)).Nodes();
            Settings.CurrentImageList.AddRange(
                nodes.Select(n =>  new SelectionableImage(
                    ((XElement)n).Element(XName.Get("MediaUrl", IMAGE_NS)).Value,
                    ((XElement)n).Element(XName.Get("Title", IMAGE_NS)).Value)));

            NavigationService.Navigate(new Uri("/ImageListPage.xaml", UriKind.Relative));
        }
    }
}