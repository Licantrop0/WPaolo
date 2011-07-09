using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Tasks;
using WPCommon.ViewModel;

namespace WPCommon
{
    public partial class AboutControl : UserControl
    {
        public AboutControl()
        {
            InitializeComponent();
        }

        private AboutViewModel _vM;
        public AboutViewModel VM
        {
            get
            {
                if (_vM == null)
                    _vM = LayoutRoot.DataContext as AboutViewModel;
                return _vM;
            }
            set
            {
                LayoutRoot.DataContext = value;
            }
        }

        private void Facebook_Click(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { URL = "http://m.facebook.com/pages/WP-Mobile-Entertainment/192414040771354" }.Show();
        }

        private void Twitter_Click(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { URL = "http://mobile.twitter.com/wp7me" }.Show();
        }

        private void Mail_Click(object sender, RoutedEventArgs e)
        {
            new EmailComposeTask()
            {
                Subject = string.Format("[{0}] {1}", VM.AppName, "feedback"),
                To = "wpmobile@hotmail.it"
            }.Show();
        }

        private void OtherAppsHyperLink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                new MarketplaceSearchTask()
                 {
                     ContentType = MarketplaceContentType.Applications,
                     SearchTerms = "WPME"
                 }.Show();
            }
            catch (InvalidOperationException)
            { /*do nothing */ }
        }

        private void App_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            try
            {
                new MarketplaceDetailTask()
                {
                    ContentIdentifier = b.Tag.ToString()
                }.Show();
            }
            catch (InvalidOperationException)
            { /*do nothing */ }
        }

        private void Rate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                new MarketplaceReviewTask().Show();
            }
            catch (InvalidOperationException)
            { /*do nothing */ }
        }
    }
}