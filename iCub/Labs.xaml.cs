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

        private void Urbana_Click(object sender, RoutedEventArgs e)
        {
            LinksHelper.OpenUrl(new Uri("http://www.aber.ac.uk/en/"));
        }

        private void Lisbona_Click(object sender, RoutedEventArgs e)
        {
            LinksHelper.OpenUrl(new Uri("http://www.ist.utl.pt/"));
        }

        private void Plymouth_Click(object sender, RoutedEventArgs e)
        {
            LinksHelper.OpenUrl(new Uri("http://www.plymouth.ac.uk/"));
        }

        private void Hertfordshire_Click(object sender, RoutedEventArgs e)
        {
            LinksHelper.OpenUrl(new Uri("http://www.herts.ac.uk/home-page.cfm"));
        }

        private void Turkey_Click(object sender, RoutedEventArgs e)
        {
            LinksHelper.OpenUrl(new Uri("http://www.metu.edu.tr/"));
        }

        private void Inserm_Click(object sender, RoutedEventArgs e)
        {
            LinksHelper.OpenUrl(new Uri("http://www.inserm.fr/"));
        }

        private void Upf_Click(object sender, RoutedEventArgs e)
        {
            LinksHelper.OpenUrl(new Uri("http://www.upf.edu/"));
        }

        private void Lasa_Click(object sender, RoutedEventArgs e)
        {
            LinksHelper.OpenUrl(new Uri("http://lasa.epfl.ch/"));
        }

        private void Idsia_Click(object sender, RoutedEventArgs e)
        {
            LinksHelper.OpenUrl(new Uri("http://www.idsia.ch/"));
        }
        
        private void Fias_Click(object sender, RoutedEventArgs e)
        {
            LinksHelper.OpenUrl(new Uri("http://fias.uni-frankfurt.de/"));
        }
        
        private void Bielefeld_Click(object sender, RoutedEventArgs e)
        {
            LinksHelper.OpenUrl(new Uri("http://www.uni-bielefeld.de/"));
        }
        
        private void Tum_Click(object sender, RoutedEventArgs e)
        {
            LinksHelper.OpenUrl(new Uri("http://portal.mytum.de/welcome"));
        }
  

        
        


        
        
    }
}