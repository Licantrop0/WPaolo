using System.Windows;
using System.Windows.Controls;
using NientePanico.Helpers;

namespace NientePanico.Carte_Di_Credito
{
    public partial class AgosControl : UserControl
    {
        public AgosControl()
        {
            InitializeComponent();
        }

        private void Italia_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Agos Italia", "800822056");
        }

        private void Estero_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Agos estero", "00390245403768");
        }
    }
}
