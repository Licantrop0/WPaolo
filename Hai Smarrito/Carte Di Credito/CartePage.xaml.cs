using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace NientePanico.Carte_Di_Credito
{
    public partial class CartePage : PhoneApplicationPage
    {
        public CartePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode == NavigationMode.Back)
                return;

            int id;
            if (int.TryParse(NavigationContext.QueryString["id"], out id))
                CartePivot.SelectedIndex = id;
        }
    }
}