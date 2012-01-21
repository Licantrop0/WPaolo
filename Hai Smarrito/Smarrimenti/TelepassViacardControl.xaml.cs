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
            CallHelper.Call("Telepass Italia", "840043043");
        }

        private void Estero_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Telepass Estero", "0039064353333");
        }
    }
}
