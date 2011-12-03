using System.Windows;
using System.Windows.Controls;

namespace HaiSmarrito.Carte_Di_Credito
{
    public partial class AmericanExpressControl : UserControl
    {
        public AmericanExpressControl()
        {
            InitializeComponent();
        }

        private void Italia_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("American Express Italia", "800 383838383");
        }
    }
}
