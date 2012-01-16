using Microsoft.Phone.Controls;

namespace NientePanico
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