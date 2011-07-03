using System.Reflection;
using Microsoft.Phone.Controls;
using NascondiChiappe.Localization;

namespace NascondiChiappe.View
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();

            WPMEAbout.ApplicationName = AppResources.AppName;

            var name = Assembly.GetExecutingAssembly().FullName;
            WPMEAbout.ApplicationVersion = new AssemblyName(name).Version.ToString(); 
        }
    }
}