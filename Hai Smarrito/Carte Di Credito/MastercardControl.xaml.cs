using System.Windows;
using System.Windows.Controls;
using NientePanico.Helpers;

namespace NientePanico.Carte_Di_Credito
{
    public partial class MastercardControl : UserControl
    {
        public MastercardControl()
        {
            InitializeComponent();
        }

        private void Italia_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Mastercard Italia", "800870866");
        }

        private void AreaCaraibica_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Mastercard Area Caraibica", "18003077309");
        }
    }
}
