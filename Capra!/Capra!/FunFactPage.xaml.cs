using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace Capra
{
    public partial class FunFactPage : PhoneApplicationPage
    {
        Random Rnd = new Random();
        List<FunFact> FunFacts;

        public FunFactPage()
        {
            InitializeComponent();

            //Carica tutti i FunFacts
            XDocument XFunFacts = XDocument.Load("funFacts.xml");
            FunFacts = XFunFacts.Descendants("FunFact")
                .Select(ff => new FunFact(
                    ff.Attribute("Type").Value,
                    ff.Attribute("Text").Value)
                ).ToList();

            if (Settings.TotCapre <= 10)
            {
                textProgress.Text = "Hai detto Capra! solo " + Settings.TotCapre +
                    " volte!\nContinua per sbloccare gli extra!";
                
                progressBar.Value = (double)(Settings.TotCapre);

                titleTextBox.Text = "";
                textFunFact.Text = "Non hai invocato abbastanza capre per poterne scoprire i segreti. Continua...";

            }
            else if ((Settings.TotCapre > 10) && (Settings.TotCapre <= 999))
            {
                // sblocco dei contenuti solo ogni tot 
                int sbloccati = FunFacts.Count * Settings.TotCapre / 800;
                if (sbloccati == 0) sbloccati = 1;

                textProgress.Text = "Hai detto Capra! " + Settings.TotCapre
                    + " volte!\nContenuti extra " + sbloccati + "/" + FunFacts.Count + " sbloccati!";
                progressBar.Value = (double)(Settings.TotCapre);

                //ne mostro uno a caso tra i primi sbloccati
                var randomFact = FunFacts[Rnd.Next(sbloccati)];
                titleTextBox.Text = randomFact.Type;
                textFunFact.Text = randomFact.Text;
            }
            else
            {
                textProgress.Text = "Hai detto Capra! " + Settings.TotCapre + " volte!\nHai sbloccato tutto!";
                progressBar.Value = 1000.00;

                titleTextBox.Text = "MASTRO CAPRAIO";
                textFunFact.Text = "La tua conoscenza sulle capre ha raggiunto un livello tale da fare invidia a Wikipedia :)";
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