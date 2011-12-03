using System.Windows;
using System.Windows.Controls;

namespace HaiSmarrito.Carte_Di_Credito
{
    public partial class MastercardControl : UserControl
    {
        public MastercardControl()
        {
            InitializeComponent();
        }

        private void Italia_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Mastercard Italia", "800 383838383");
        }

        private void AreaCaraibica_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Mastercard Area Caraibica", "800 383838383");
        }
    }
}
