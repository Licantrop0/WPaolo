using System.Windows;
using System.Windows.Controls;
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

        private void MastercardItalia_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Mastercard Italia", "800904470");
        }

        private void MastercardEstero_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Mastercard Estero", "00390260843760");
        }
    }
}
