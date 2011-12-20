using System.Windows;
using Microsoft.Phone.Controls;
using System.Reflection;
using HaiSmarrito.Helpers;
using HaiSmarrito.ViewModel;

namespace HaiSmarrito.Carte_Di_Credito
{
    public partial class NazioniPage : PhoneApplicationPage
    {
        public NazioniPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            var VM = (NazioniViewModel)LayoutRoot.DataContext;
            VM.CardType = NavigationContext.QueryString["type"];
            base.OnNavigatedTo(e);
        }
    }
}