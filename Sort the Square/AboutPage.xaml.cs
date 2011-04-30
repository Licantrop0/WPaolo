using System.Reflection;
using Microsoft.Phone.Controls;
using SortTheSquare.Localization;

namespace SortTheSquare
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
    }
}