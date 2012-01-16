using System.Windows;
using Microsoft.Phone.Controls;
using System.Reflection;
using NientePanico.Helpers;
using NientePanico.ViewModel;

namespace NientePanico.Carte_Di_Credito
{
    public partial class NazioniPage : PhoneApplicationPage
    {
        private NazioniViewModel _vM;
        public NazioniViewModel VM
        {
            get
            {
                if (_vM == null)
                    _vM = (NazioniViewModel)LayoutRoot.DataContext;

                return _vM;
            }
        }

        public NazioniPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            VM.CardType = NavigationContext.QueryString["type"];
            base.OnNavigatedTo(e);
        }
    }
}