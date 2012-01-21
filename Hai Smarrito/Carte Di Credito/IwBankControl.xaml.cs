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
    public partial class IwBankControl : UserControl
    {
        public IwBankControl()
        {
            InitializeComponent();
        }

        private void IwItalia_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("IW Italia", "800991188");
        }

        private void IwEstero_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("IW Italia", "00390274875801");
        }

    }
}
