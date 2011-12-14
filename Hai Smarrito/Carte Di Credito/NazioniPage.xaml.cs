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

            switch (VM.CardType)
            {
                case "amex":
                    this.Title1Textbox.Text = "American Express";
                    break;
                case "visa":
                    this.Title1Textbox.Text = "Visa";
                    break;
                case "mastercard":
                    this.Title1Textbox.Text = "Mastercard";
                    break;
                case "dinersclub":
                    this.Title1Textbox.Text = "Diners Club";
                    break;

                default:
                    break;
            }

            base.OnNavigatedTo(e);
        }
    }
}