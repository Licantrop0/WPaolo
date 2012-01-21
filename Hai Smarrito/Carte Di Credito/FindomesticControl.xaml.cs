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
    public partial class FindomesticControl : UserControl
    {
        public FindomesticControl()
        {
            InitializeComponent();
        }

        private void FindomesticItalia_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Findomestic Italia", "800255340");
        }

        private void FindomesticEstero_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Findomestic Estero", "00390553232243");
        }
    }
}
