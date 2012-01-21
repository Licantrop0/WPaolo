using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using NientePanico.Helpers;

namespace NientePanico.Carte_Di_Credito
{
    public partial class VisaControl : UserControl
    {
        public VisaControl()
        {
            InitializeComponent();
        }

        private void Italia_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Visa Italia", "800819014");
        }

        private void AreaCaraibica_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Visa Area Caraibica", "18008472911");
        }
    }
}
