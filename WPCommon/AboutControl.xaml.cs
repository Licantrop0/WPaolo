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

namespace WPCommon
{
    public partial class AboutControl : UserControl
    {
        public string ApplicationName
        {
            get { return ApplicationNameTextBlock.Text; }
            set { ApplicationNameTextBlock.Text = value; }
        }

        public string GetOtherAppsText
        {
            get { return (string)OtherAppsHyperlink.Content; }
            set { OtherAppsHyperlink.Content = value ?? (string)OtherAppsHyperlink.Content; }
        }

        public string ContactUsText
        {
            get { return (string)ContactUsButton.Content; }
            set { ContactUsButton.Content = value ?? (string)ContactUsButton.Content; }
        }

        public AboutControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            string asmName = Assembly.GetExecutingAssembly().FullName;
            VersionTextBlock.Text = new AssemblyName(asmName).Version.ToString();
        }

        private void OtherAppsHyperlink_Click(object sender, RoutedEventArgs e)
        {
            new MarketplaceSearchTask()
            {
                ContentType = MarketplaceContentType.Applications,
                SearchTerms = "Licantrop0"
            }.Show();
        }

        private void ContactUsButton_Click(object sender, RoutedEventArgs e)
        {
            new EmailComposeTask()
            {
                Subject = string.Format("[{0}] {1}", ApplicationName, "feedback"),
                To = "wpmobile@hotmail.it"
            }.Show();
        }
    }
}
