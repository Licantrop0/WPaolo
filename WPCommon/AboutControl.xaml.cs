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
using Microsoft.Phone.Tasks;
using System.Reflection;
using System.Windows.Navigation;

namespace WPCommon
{
    public partial class AboutControl : UserControl
    {
        public string ApplicationName
        {
            get { return ApplicationNameTextBlock.Text; }
            set { ApplicationNameTextBlock.Text = value; }
        }

        public Thickness ApplicationNameMargin
        {
            get { return ApplicationNameTextBlock.Margin; }
            set { ApplicationNameTextBlock.Margin = value; }
        }

        //var name = Assembly.GetExecutingAssembly().FullName;
        //WPMEAbout.ApplicationVersion = new AssemblyName(name).Version.ToString(); 
        public string ApplicationVersion
        {
            get { return ApplicationVersionTextBlock.Text; }
            set { ApplicationVersionTextBlock.Text = value; }
        }

        public string GetOtherAppsText
        {
            get { return (string)OtherAppsTextBlock.Text; }
            set { OtherAppsTextBlock.Text = value ?? (string)OtherAppsTextBlock.Text; }
        }

        public Brush BackgroundStackPanel
        {
            get { return MainStackPanel.Background; }
            set { MainStackPanel.Background = value; }
        }

        public double MinFontSize
        {
            get { return ApplicationVersionTextBlock.FontSize; }
            set
            {
                ApplicationVersionTextBlock.FontSize = value;
                OtherAppsTextBlock.FontSize = value * (24 / 19);
                ApplicationNameTextBlock.FontSize = value * (32 / 19);
            }
        }

        public AboutControl()
        {
            InitializeComponent();
        }

        private void Facebook_Click(object sender, MouseButtonEventArgs e)
        {
            new WebBrowserTask() { URL = "http://m.facebook.com/pages/WP-Mobile-Entertainment/192414040771354" }.Show();
        }

        private void Twitter_Click(object sender, MouseButtonEventArgs e)
        {
            new WebBrowserTask() { URL = "http://mobile.twitter.com/wp7me" }.Show();
        }

        private void Mail_Click(object sender, MouseButtonEventArgs e)
        {
            new EmailComposeTask()
            {
                Subject = string.Format("[{0}] {1}", ApplicationName, "feedback"),
                To = "wpmobile@hotmail.it"
            }.Show();
        }

        private void OtherAppsTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            new MarketplaceSearchTask()
            {
                ContentType = MarketplaceContentType.Applications,
                SearchTerms = "Licantrop0"
            }.Show();

        }
    }
}
