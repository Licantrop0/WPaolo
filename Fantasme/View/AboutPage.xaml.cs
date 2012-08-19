using System.Reflection;
using Microsoft.Phone.Controls;
using NascondiChiappe.Localization;
using System.Windows.Media;
using System;
using System.Windows.Media.Imaging;
namespace NascondiChiappe.View
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            WPMEAbout.AddAdvertising();
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            WPMEAbout.RemoveAdvertising();
            base.OnNavigatedFrom(e);
        }
    }
}