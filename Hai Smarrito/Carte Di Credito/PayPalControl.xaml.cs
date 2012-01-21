using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using NientePanico.Helpers;

namespace NientePanico.Carte_Di_Credito
{
    public partial class PayPalControl : UserControl
    {
        public PayPalControl()
        {
            InitializeComponent();
        }

        private void PayPalItalia_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("PayPal Italia", "800822056");
        }

        private void PayPalEstero_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("PayPal Italia", "00390245403768");
        }

    }
}
