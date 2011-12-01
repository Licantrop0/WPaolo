using System.Windows;
using Microsoft.Phone.Controls;
using System;

namespace HaiSmarrito.Carte_Di_Credito
{
    public partial class VisaPage : PhoneApplicationPage
    {
        public VisaPage()
        {
            InitializeComponent();
        }

        private void Italia_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Visa Italia", "800 383838383");
        }

        private void AreaCaraibica_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Visa Area Caraibica", "800 383838383");
        }

        private void AltriPaesi_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Carte Di Credito/NazioniPage.xaml?type=visa", UriKind.Relative));
        }
    }
}