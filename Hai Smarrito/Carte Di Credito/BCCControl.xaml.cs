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
    public partial class BCCControl : UserControl
    {
        public BCCControl()
        {
            InitializeComponent();
        }

        private void BccItalia_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Bcc Italia", "800086531");
        }

        private void BccEstero_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Bcc Estero", "00390687419901");
        }

        private void VisaItalia_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Visa Italia", "800086530");
        }

        private void VisaEstero_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Visa Estero", "00390647825280");
        }

        private void MastercadItalia_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Mastercard Italia", "800904470");
        }

        private void MastercardEstero_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Mastercard Estero", "00390260843760");
        }
    }
}
