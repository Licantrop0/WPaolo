using System.Windows;
using Microsoft.Phone.Controls;

namespace HaiSmarrito.Carte_Di_Credito
{
    public partial class NazioniPage : PhoneApplicationPage
    {
        private string _cardType;

        public NazioniPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            _cardType = NavigationContext.QueryString["type"];

            switch (_cardType)
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


        private void AmericanSamoa_Click(object sender, RoutedEventArgs e)
        {
            switch (_cardType)
            {
                case "amex":
                    CallHelper.Call("American Express American Samoa", "800 383838383");
                    break;
                case "visa":
                    CallHelper.Call("Visa American Samoa", "800 383838383");
                    break;
                case "mastercard":
                    CallHelper.Call("Mastercard American Samoa", "800 383838383");
                    break;
                case "dinersclub":
                    CallHelper.Call("Diners Club American Samoa", "800 383838383");
                    break;
                default:
                    break;
            }
        }

        private void Argentina_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Australia_Click(object sender, RoutedEventArgs e)
        {

        }


    }
}