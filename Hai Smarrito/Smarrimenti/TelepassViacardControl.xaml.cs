using System.Windows;
using System.Windows.Controls;
using NientePanico.Helpers;

namespace NientePanico.Smarrimenti
{
    public partial class TelepassViacardControl : UserControl
    {
        public TelepassViacardControl()
        {
            InitializeComponent();
        }

        private void Italia_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Telepass Italia", "000000");
        }

        private void Estero_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Telepass Estero", "000000");
        }
    }
}
