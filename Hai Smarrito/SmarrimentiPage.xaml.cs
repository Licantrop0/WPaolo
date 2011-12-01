using System;
using System.Windows.Controls;
using Microsoft.Phone.Controls;

namespace HaiSmarrito
{
    public partial class SmarrimentiPage : PhoneApplicationPage
    {
        public SmarrimentiPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            int id;
            if(int.TryParse(NavigationContext.QueryString["id"], out id))
                SmarrimentiPivot.SelectedIndex = id;

            base.OnNavigatedTo(e);
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