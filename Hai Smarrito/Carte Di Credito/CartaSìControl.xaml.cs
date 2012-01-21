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
    public partial class CartaSìControl : UserControl
    {
        public CartaSìControl()
        {
            InitializeComponent();
        }

        private void CartasiItalia_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Cartasi Italia", "800151616");
        }

        private void CartasiEstero_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Cartasi Estero", "00390234980020");
        }

        private void CartasiUSA_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Cartasi USA", "18004736896");
        }
    }
}
