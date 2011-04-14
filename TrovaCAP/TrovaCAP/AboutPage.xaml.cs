using System.Reflection;
using Microsoft.Phone.Controls;

namespace TrovaCAP
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();
            WPMEAbout.ApplicationName = AppResources.ApplicationName;
            WPMEAbout.GetOtherAppsText = AppResources.GetOtherApps;
            var name = Assembly.GetExecutingAssembly().FullName;
            WPMEAbout.ApplicationVersion = new AssemblyName(name).Version.ToString(); 
        }
    }
}