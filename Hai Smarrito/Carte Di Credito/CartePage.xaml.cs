using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace HaiSmarrito.Carte_Di_Credito
{
    public partial class CartePage : PhoneApplicationPage
    {
        public CartePage()
        {
            InitializeComponent();
        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            int id;
            if (int.TryParse(NavigationContext.QueryString["id"], out id))
                CartePivot.SelectedIndex = id;

            base.OnNavigatedTo(e);
        }


        private void Execute_Navigation(object sender, NavigationEventArgs e)
        {
            NavigationService.Navigate(e.Uri);
        }
    }
}