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

        //===============================================================================
        //                        FUN FACTS
        //===============================================================================

        //private void showFunFacts()
        //{
        //    // sometimes instead of fun facts I'll show the achievements

        //    if ((Settings.TotCapre >= 10) && (Settings.TotCapre < 100) && (prizeIsShown == 0))
        //    {
        //        CapraImage.Source = new BitmapImage(new Uri("Images\\o10.png", UriKind.Relative));
        //        textObiettivo.Text = "Obiettivo sbloccato!"; textObiettivo.Visibility = Visibility.Visible;
        //        textFunFact.Text = "Hai detto almeno 10 volte Capra!"; textFunFact.Visibility = Visibility.Visible;
        //        titleFunFact.Visibility = Visibility.Collapsed;
        //        prizeIsShown = 1;
        //    }
        //    else if ((Settings.TotCapre >= 100) && (Settings.TotCapre < 500) && (prizeIsShown == 1))
        //    {
        //        CapraImage.Source = new BitmapImage(new Uri("Images\\o100.png", UriKind.Relative));
        //        textObiettivo.Text = "Obiettivo sbloccato!"; textObiettivo.Visibility = Visibility.Visible;
        //        textFunFact.Text = "Hai detto almeno 100 volte Capra!"; textFunFact.Visibility = Visibility.Visible;
        //        titleFunFact.Visibility = Visibility.Collapsed;
        //        prizeIsShown = 2;
        //    }
        //    else if ((Settings.TotCapre >= 500) && (Settings.TotCapre < 1000) && (prizeIsShown == 2))
        //    {
        //        CapraImage.Source = new BitmapImage(new Uri("Images\\o500.png", UriKind.Relative));
        //        textObiettivo.Text = "Obiettivo sbloccato!"; textObiettivo.Visibility = Visibility.Visible;
        //        textFunFact.Text = "Hai detto almeno 500 volte Capra!"; textFunFact.Visibility = Visibility.Visible;
        //        titleFunFact.Visibility = Visibility.Collapsed;
        //        prizeIsShown = 3;
        //    }
        //    else if ((Settings.TotCapre >= 1000) && (prizeIsShown == 3))
        //    {
        //        CapraImage.Source = new BitmapImage(new Uri("Images\\o1000.png", UriKind.Relative));
        //        textObiettivo.Text = "Obiettivi sbloccati!"; textObiettivo.Visibility = Visibility.Visible;
        //        textFunFact.Text = "Hai detto almeno 1000 volte Capra! Congratulazioni, hai sbloccato tutti gli obiettivi! Ora puoi fregiarti del prezioso titolo di Mastro Capraio!"; textFunFact.Visibility = Visibility.Visible;
        //        titleFunFact.Visibility = Visibility.Collapsed;
        //        prizeIsShown = 4;
        //    }
        //    else
        //    {
        //        this.Focus();
        //        int randomFact = Rnd.Next(FunFacts.Count);
        //        titleFunFact.Text = FunFacts[randomFact].Type; titleFunFact.Visibility = Visibility.Visible;
        //        textFunFact.Text = FunFacts[randomFact].Text; textFunFact.Visibility = Visibility.Visible;
        //        CapraImage.Source = new BitmapImage(new Uri("Images\\funFact.png", UriKind.Relative));
        //    }
        //    SblocButton.Visibility = Visibility.Visible;
        //    factIsShown = true;

        //}


        //===============================================================================
        //                        ACHIEVEMENTS
        //===============================================================================

        //private void SblocButton_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        //{
        //    CapraImage.Source = new BitmapImage(new Uri("Images\\o1000.png", UriKind.Relative));

        //    if (prizeIsShown == 4)
        //    {
        //        textFunFact.Text = "Congratulazioni, hai sbloccato tutti gli obiettivi e puoi fregiarti del titolo di Mastro Capraio. Ora sei anche il massimo esperto mondiale di Capre, e sei autorizzato a dare della capra a chiunque!";
        //        textObiettivo.Text = "Mastro Capraio";
        //    }
        //    else
        //    {
        //        textFunFact.Text = "Devi sbloccare tutti gli obiettivi per poterti fregiare del titolo di Mastro Capraio. Continua a chiamare Capra! C'è pieno di capre intorno a te!";
        //        textObiettivo.Text = "";
        //    }

        //    textObiettivo.Visibility = Visibility.Visible;
        //    textFunFact.Visibility = Visibility.Visible;
        //    titleFunFact.Visibility = Visibility.Collapsed;

        //}


    }
}