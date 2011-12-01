using System;
using System.Windows;
using Microsoft.Phone.Controls;

namespace HaiSmarrito.Carte_Di_Credito
{
    public partial class AmericanExpressPage : PhoneApplicationPage
    {
        public AmericanExpressPage()
        {
            InitializeComponent();
        }

        private void Italia_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("American Express Italia", "800 383838383");
        }

        private void Estero_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Carte Di Credito/NazioniPage.xaml?type=amex", UriKind.Relative));
        }
    }
}