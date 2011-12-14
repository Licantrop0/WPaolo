using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using HaiSmarrito.Helpers;

namespace HaiSmarrito.Carte_Di_Credito
{
    public partial class VisaControl : UserControl
    {
        public VisaControl()
        {
            InitializeComponent();
        }

        private void Italia_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Visa Italia", "800 383838383");
        }

        private void AreaCaraibica_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Visa Area Caraibica", "800 383838383");
        }
    }
}
