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
    public partial class IntesaSanPaoloControl : UserControl
    {
        public IntesaSanPaoloControl()
        {
            InitializeComponent();
        }

        private void IntesaItalia_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Intesa Italia", "800902631");
        }

        private void IntesaEstero_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Intesa Estero", "00390289137212");
        }
    }
}
