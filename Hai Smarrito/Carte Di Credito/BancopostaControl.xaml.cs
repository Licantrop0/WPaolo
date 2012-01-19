using System.Windows;
using System.Windows.Controls;
using NientePanico.Helpers;

namespace NientePanico.Carte_Di_Credito
{
    public partial class BancopostaControl : UserControl
    {
        public BancopostaControl()
        {
            InitializeComponent();
        }

        private void PostePayItalia_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Bancoposta Italia", "800822056");
        }

        private void PostePayEstero_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PostamatItalia_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PostamatEstero_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BancoPostaItalia_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BancoPostaEstero_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Italia_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
