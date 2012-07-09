using System.Windows.Controls;
using System.Windows.Media.Animation;
using Microsoft.Phone.Controls;
using System.Windows.Media;
using Microsoft.Advertising.Mobile.UI;

namespace Scudetti.View
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (AdPlaceHolder.Children.Count == 1) //l'Ad c'è già
                return;

            AdPlaceHolder.Children.Add(new AdControl("489510bc-7ee7-4d2d-9bf1-9065ff63354d", "10040107", true)
            {
                Height = 80,
                Width = 480,
            });
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            AdPlaceHolder.Children.Clear();
            base.OnNavigatedFrom(e);
        }

    }
}