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
            CallHelper.Call("PostePayItalia", "800902122");
        }

        private void PostePayEstero_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("PostePayEstero", "00390234980131");
        }

        private void PostamatItalia_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("PostamatItalia", "800652653");
        }

        private void PostamatEstero_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("PostamatItalia", "00390234980132");
        }

        private void BancoPostaItalia_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("BancoPostaItalia", "800207167");
        }

        private void BancoPostaEstero_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("BancoPostaEstero", "00390432744106");
        }
    }
}
