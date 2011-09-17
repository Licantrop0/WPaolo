using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Phone.Tasks;
using WPCommon.Helpers;

namespace Capra.ViewModel
{
    public class FunFactsViewModel : INotifyPropertyChanged
    {
        public List<FunFact> FunFacts { get; set; }
        private int sbloccati;
        private const int TotFunFacts = 56;

        public FunFactsViewModel()
        {
            if (DesignerProperties.IsInDesignTool)
            {
                FunFacts = new List<FunFact>();
                FunFacts.Add(new FunFact("TEST FunFact Type", "Test FunFact Text\nLorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat."));
                return;
            }

            sbloccati = TotFunFacts * Settings.TotCapre / 1000;
            if (sbloccati > 0)
            {
                FunFacts = XDocument.Load("funFacts.xml").Descendants("FunFact")
                    .Select(ff => new FunFact(ff.Attribute("Type").Value, ff.Attribute("Text").Value))
                    .Take(sbloccati).Reverse().ToList(); //La funzione reverse è geniale: ti fa sempre vedere l'ultimo sbloccato!
            }
            else
            {
                FunFacts = new List<FunFact>();
                FunFacts.Add(new FunFact(string.Empty, "Non hai invocato abbastanza capre per poterne scoprire i segreti. Continua..."));
            }
        }

        public string ProgressDescription
        {
            get
            {
                if (DesignerProperties.IsInDesignTool)
                {
                    return "Test Progress Description";
                }
                else if (sbloccati == 0)
                {
                    return "Hai detto Capra! solo " + Settings.TotCapre +
                        " volte!\nContinua per sbloccare gli extra!";
                }
                else if (Settings.TotCapre < 1000)
                {

                    return "Hai detto Capra! " + Settings.TotCapre +
                        " volte!\nContenuti extra " +
                        sbloccati + "/" + TotFunFacts + " sbloccati!";
                }
                else
                {
                    return "Hai detto Capra! " + Settings.TotCapre
                        + " volte!\nHai sbloccato tutto: sei un Mastro CAPRAIO!";
                }
            }
        }

        public double ProgressValue
        {
            get
            {
                return (double)Settings.TotCapre;
            }
        }

        private RelayCommand _buySgarbiMix;
        public RelayCommand BuySgarbiMix
        {
            get
            {
                return _buySgarbiMix ?? (_buySgarbiMix = new RelayCommand(buy =>
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
                }));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}