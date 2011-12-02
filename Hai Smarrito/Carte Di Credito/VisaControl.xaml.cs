using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace HaiSmarrito.Carte_Di_Credito
{
    public partial class VisaControl : UserControl
    {
        public event EventHandler<NavigationEventArgs> Navigate;

        public VisaControl()
        {
            InitializeComponent();
        }

        private void Italia_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Visa Italia", "800 383838383");
        }

        private void AreaCaraibica_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Visa Area Caraibica", "800 383838383");
        }

        private void AltriPaesi_Click(object sender, RoutedEventArgs e)
        {
            var navigateTo = new NavigationEventArgs(this,
                new Uri("/Carte Di Credito/NazioniPage.xaml?type=visa", UriKind.Relative));
            Navigate.Invoke(this, navigateTo);
        }

    }
}
