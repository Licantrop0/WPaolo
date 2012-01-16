using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using NientePanico.ViewModel;

namespace NientePanico.Carte_Di_Credito
{
    public partial class NazioniPage : PhoneApplicationPage
    {
        public NazioniPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var VM = (NazioniViewModel)LayoutRoot.DataContext;
            VM.CardType = NavigationContext.QueryString["type"];
        }
    }
}