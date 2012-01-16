using Microsoft.Phone.Controls;

namespace NientePanico.Smarrimenti
{
    public partial class PassaportoPage : PhoneApplicationPage
    {
        public PassaportoPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            int id;
            if (int.TryParse(NavigationContext.QueryString["id"], out id))
                PassaportoPivot.SelectedIndex = id;
        }
    }
}