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
using Microsoft.Phone.Controls;

namespace HaiSmarrito.Carte_Di_Credito
{
    public partial class MastercardPage : PhoneApplicationPage
    {
        public MastercardPage()
        {
            InitializeComponent();
        }

        private void Italia_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Mastercard Italia", "800 383838383");
        }

        private void AreaCaraibica_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Mastercard Area Caraibica", "800 383838383");
        }

        private void AltriPaesi_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Mastercard Altri Paesi", "800 383838383");
        }
    }
}