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
            CallHelper.Call("Bancomat Italia", "800 383838383");
        }

        private void Estero_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Bancomat Estero", "800 383838383");
        }
    }
}
