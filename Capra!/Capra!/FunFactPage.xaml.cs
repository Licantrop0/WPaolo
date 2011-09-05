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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Resources;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System.Xml.Linq;

namespace Capra
{
    public partial class FunFactPage : PhoneApplicationPage
    {
        Random Rnd = new Random();
        List<FunFact> FunFacts;

        public FunFactPage()
        {
            InitializeComponent();

            //Carica i FunFacts
            XDocument XFunFacts = XDocument.Load("funFacts.xml");
            FunFacts = XFunFacts.Descendants("FunFact").Select(ff =>
                new FunFact(ff.Attribute("Type").Value, ff.Attribute("Text").Value)).ToList();

            if (Settings.TotCapre <= 10)
            {
                // sere
                // textProgress.Text = "Hai detto Capra! solo " + Settings.TotCapre + "/1000 volte! \nContinua per sbloccare gli extra!";
                textProgress.Text = "Hai detto Capra! solo " + Settings.TotCapre + " volte! \nContinua per sbloccare gli extra!";
                progressBar.Value = (double)(Settings.TotCapre);

                titleTextBox.Text = "";
                textFunFact.Text = "Non hai invocato abbastanza capre per poterne scoprire i segreti. Continua...";
   
            }
            else if ((Settings.TotCapre > 10) && (Settings.TotCapre <= 1000))
            {
                // sblocco dei contenuti solo ogni tot 
                int sbloccati = FunFacts.Count * Settings.TotCapre / 999;
                if (sbloccati == 0) sbloccati = 1;

                textProgress.Text = "Hai detto Capra! " + Settings.TotCapre 
                    + " volte! \n Contenuti extra "+ sbloccati + "/"+ FunFacts.Count +" sbloccati!";
                progressBar.Value = (double)(Settings.TotCapre);

                // ne mostro uno a caso
                int randomFact = Rnd.Next(sbloccati);
                titleTextBox.Text = FunFacts[randomFact].Type;
                textFunFact.Text = FunFacts[randomFact].Text; 
            }
            else
            {
                textProgress.Text = "Hai detto Capra! " + Settings.TotCapre + " volte! \nHai sbloccato tutto!";
                progressBar.Value = 1000.00;

                titleTextBox.Text = "MASTRO CAPRAIO";
                textFunFact.Text = "La tua conoscenza sulle capre ha raggiunto un livello tale da fare invidia a Wikipedia :) ";

                // non mostro piu' niente
                // o forse mostriamo qcosa? lupo possiamo mostrarli in un'altra pagina??
                // come aggiornamento semmai.. ci penseremo..
            }
            


        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                new MarketplaceDetailTask()
                {
                    ContentIdentifier = "5925f9d6-483d-e011-854c-00237de2db9e"
                }.Show();
            }
            catch (InvalidOperationException)
            { /*do nothing */ }
        }
    }
}