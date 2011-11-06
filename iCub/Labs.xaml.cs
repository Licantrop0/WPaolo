using System;
using System.Windows;
using iCub.Helpers;
using Microsoft.Phone.Controls;

namespace iCub
{
    public partial class Labs : PhoneApplicationPage
    {
        public Labs()
        {
            InitializeComponent();
        }

        private void ScuolaSuperioreSantAnna_Click(object sender, RoutedEventArgs e)
        {
            LinksHelper.OpenUrl(new Uri("http://www.sssup.it/"));
        }

        private void IIT_Click(object sender, RoutedEventArgs e)
        {
            LinksHelper.OpenUrl(new Uri("http://www.iit.it/"));
        }

        private void ConsiglioNazionaleRicerche_Click(object sender, RoutedEventArgs e)
        {
            LinksHelper.OpenUrl(new Uri("http://www.cnr.it/"));
        }

        private void UMPC_Click(object sender, RoutedEventArgs e)
        {
            LinksHelper.OpenUrl(new Uri("http://www.upmc.fr/"));
        }

        private void INSERM_Click(object sender, RoutedEventArgs e)
        {
            LinksHelper.OpenUrl(new Uri("http://www.inserm.fr/"));
        }

        private void ImperialCollegeLondon_Click(object sender, RoutedEventArgs e)
        {
            LinksHelper.OpenUrl(new Uri("http://www3.imperial.ac.uk/"));
        }

        private void AberystwythUniversity_Click(object sender, RoutedEventArgs e)
        {
            LinksHelper.OpenUrl(new Uri("http://www.aber.ac.uk/en/"));
        }

    }
}