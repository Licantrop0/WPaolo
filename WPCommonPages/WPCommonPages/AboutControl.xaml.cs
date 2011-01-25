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

namespace WPCommon
{
    public partial class AboutControl : UserControl
    {
        public string AppName { get; set; }
        public string version { get; set; }

        public AboutControl()
        {
            InitializeComponent();
        }

        private void OtherAppsHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            new MarketplaceSearchTask()
            {
                ContentType = MarketplaceContentType.Applications,
                SearchTerms = "Licantrop0"
            }.Show();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ApplicationNameTextBlock.Text = AppName;

            string name = System.Reflection.Assembly.GetExecutingAssembly().FullName;
            var asmName = new System.Reflection.AssemblyName(name);

            VersionTextBlock.Text = asmName.Version.ToString();
        }

        private void ContactUsButton_Click(object sender, RoutedEventArgs e)
        {
            new EmailComposeTask()
            {
                Subject = string.Format("[{0}] {1}", AppName, "feedback"),
                To = "wpmobile@hotmail.it"
            }.Show();
        }
    }
}
