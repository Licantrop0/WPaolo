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
using System.Windows.Navigation;

namespace HaiSmarrito.Carte_Di_Credito
{
    public partial class AmericanExpressControl : UserControl
    {
        public event EventHandler<NavigationEventArgs> Navigate;

        public AmericanExpressControl()
        {
            InitializeComponent();
        }

        private void Italia_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("American Express Italia", "800 383838383");
        }

        private void Estero_Click(object sender, RoutedEventArgs e)
        {
            var navigateTo = new NavigationEventArgs(this,
                new Uri("/Carte Di Credito/NazioniPage.xaml?type=amex", UriKind.Relative));
            Navigate.Invoke(this, navigateTo);
        }
    }
}
