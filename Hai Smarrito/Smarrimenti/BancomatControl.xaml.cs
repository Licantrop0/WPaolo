using System.Windows;
using System.Windows.Controls;
using NientePanico.Helpers;

namespace NientePanico.Smarrimenti
{
    public partial class BancomatControl : UserControl
    {
        public BancomatControl()
        {
            InitializeComponent();
        }

        private void Italia_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Bancomat Italia", "800822056");
        }

        private void Estero_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Bancomat Estero", "00390245403768");
        }
    }
}
