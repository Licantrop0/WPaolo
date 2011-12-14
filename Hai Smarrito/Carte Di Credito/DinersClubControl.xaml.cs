using System.Windows.Controls;
using System.Windows;
using HaiSmarrito.Helpers;

namespace HaiSmarrito.Carte_Di_Credito
{
    public partial class DinersClubControl : UserControl
    {
        public DinersClubControl()
        {
            InitializeComponent();
        }

        private void Italia_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Diners Club Italia", "800 383838383");
        }

        private void Estero_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Diners Club Estero", "800 383838383");
        }

    }
}
