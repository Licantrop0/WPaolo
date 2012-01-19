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
    public partial class BarclayControl : UserControl
    {
        public BarclayControl()
        {
            InitializeComponent();
        }

        private void Italia_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("", "");
        }

        private void Estero_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("", "");
        }
    }
}
