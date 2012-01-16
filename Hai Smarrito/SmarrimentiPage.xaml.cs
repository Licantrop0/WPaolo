using System;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using System.Windows.Navigation;
using Microsoft.Advertising.Mobile.UI;

namespace NientePanico
{
    public partial class SmarrimentiPage : PhoneApplicationPage
    {
        public SmarrimentiPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            AdPlaceHolder.Children.Add(new AdControl("test_client", "Image480_80", true)
            {
                Height = 80,
                Width = 480
            });

            if (e.NavigationMode == NavigationMode.Back)
                return;

            int id;
            if (int.TryParse(NavigationContext.QueryString["id"], out id))
                SmarrimentiPivot.SelectedIndex = id;

        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            AdPlaceHolder.Children.Clear();
        }

        private void AppBarInfoButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Carte Di Credito/InfoPage.xaml", UriKind.Relative));
        }

        private void SmarrimentiPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplicationBar.IsVisible = SmarrimentiPivot.SelectedIndex == 0;
        }
    }
}