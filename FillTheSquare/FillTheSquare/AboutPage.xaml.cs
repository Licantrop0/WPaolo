using System.Reflection;
using Microsoft.Phone.Controls;
using FillTheSquare.Localization;
using Microsoft.Phone.Tasks;

namespace FillTheSquare
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();
            var name = Assembly.GetExecutingAssembly().FullName;
            WPMEAbout.ApplicationVersion = new AssemblyName(name).Version.ToString();
            WPMEAbout.GetOtherAppsText = AppResources.GetOtherApps;
        }

        private void Youtube_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            new WebBrowserTask() { URL = "http://www.youtube.com/watch?v=6PBK-sg2Zr0" }.Show();
        }
    }
}