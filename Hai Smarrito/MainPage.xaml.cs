using Microsoft.Phone.Controls;
using System.Windows.Controls;
using System;
using Microsoft.Advertising.Mobile.UI;
using WPCommon.Helpers;

namespace NientePanico
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (TrialManagement.IsTrialMode)
            {
                AdPlaceHolder.Children.Add(new AdControl("test_client", "Image480_80", true) { Height = 80, Width = 480 });
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            AdPlaceHolder.Children.Clear();
        }
    }
}
