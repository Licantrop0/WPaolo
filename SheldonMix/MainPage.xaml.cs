using System;
using System.Device.Location;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using Microsoft.Advertising.Mobile.UI;

namespace SheldonMix
{

    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            MainPivot.Margin = new Thickness(0, 0, 0, 80); 
            if (AdPlaceHolder.Children.Count == 1) //l'Ad c'è già
                return;

            var ad = new AdControl("04e2ad45-3752-4d8c-867c-b1eb0cf4a3e1", "10022426", true) { Width = 480, Height = 80 };
            ad.ErrorOccurred += ad_ErrorOccurred;            
            AdPlaceHolder.Children.Add(ad);

            base.OnNavigatedTo(e);
        }

        private void ad_ErrorOccurred(object sender, Microsoft.Advertising.AdErrorEventArgs e)
        {
            MainPivot.Margin = new Thickness(0);
            AdPlaceHolder.Children.Clear();
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            AdPlaceHolder.Children.Clear();
            base.OnNavigatedFrom(e);
        }

        private void AboutAppBarMenu_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

        private void HyperlinkButton_Click_4(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { Uri = new Uri("http://m.youtube.com/watch?v=LYwF2JLCHWg") }.Show();
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { Uri = new Uri("http://m.youtube.com/watch?v=SifGskrY_UY") }.Show();
        }

        private void TwitterCBS_Click(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { Uri = new Uri("http://mobile.twitter.com/BigBang_CBS") }.Show();
        }

        private void FacebookCBS_Click(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { Uri = new Uri("http://m.facebook.com/TheBigBangTheory") }.Show();
        }
        
        private void TwitterSheldon_Click(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { Uri = new Uri("http://mobile.twitter.com/TheRealSheldonC") }.Show();
        }
        
        private void FacebookSheldon_Click(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { Uri = new Uri("http://m.facebook.com/profile.php?id=23519525029") }.Show();
        }

        private void HyperlinkButton_Click_2(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { Uri = new Uri("http://www.cbs.com/shows/big_bang_theory") }.Show();
        }

        private void HyperlinkButton_Click_3(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { Uri = new Uri("http://www.imdb.com/name/nm1433588/") }.Show();
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            try
            {
                new MarketplaceReviewTask().Show();
            }
            catch (InvalidOperationException)
            { /*do nothing */ }
        }

    }
}