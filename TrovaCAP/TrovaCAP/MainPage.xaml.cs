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
using System.IO;


namespace TrovaCAP
{
    enum Step
    {
        scegliProvinciaOComune,
        selezionaProvincia,
        selezionaProvinciaComune,
        selezionaComune,
        selezionaFrazioneOVia,
    }


    public partial class MainPage : PhoneApplicationPage
    {
    #region private members

        private CAPDB _capDB;

        private bool _bLoading = true;

        Step _state;

        string _sProvinciaSelezionata;
        string _sComuneSelezionato;
        List<CAPRecord> _capRecords;
        string _sCapSelezionato;

    #endregion

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            ReadAndParseDataBase();

            _state = Step.scegliProvinciaOComune;

            GoStep();
            
        }

        private void GoStep()
        {
            switch(_state)
            {
                case Step.scegliProvinciaOComune:

                    messageBox.Text = "Seleziona una provincia o direttamente un comune";

                    break;

                default:
                    break;
            }
        }

        
        private void acbProvince_GotFocus(object sender, RoutedEventArgs e)
        {
            if (_state == Step.scegliProvinciaOComune)
            {
                // put off initial state
                if (acbProvince.Text == "--")
                    acbProvince.Text = "";

                acbProvince.ItemsSource = _capDB.Province.Keys;
                _state = Step.selezionaProvincia;
            }
        }

        private void acbProvince_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_state == Step.selezionaProvincia)
            {
                if (_capDB.Province.ContainsKey(acbProvince.Text))
                {
                    _sProvinciaSelezionata = acbProvince.Text;
                    acbComuni.ItemsSource = _capDB.Province[_sProvinciaSelezionata].Keys;
                    _state = Step.selezionaProvinciaComune;
                    messageBox.Text = "Ora seleziona un comune";
                }
                else
                {
                    acbProvince.Text = "";
                    messageBox.Text = "Provincia errata, riprova o seleziona direttamente un comune";
                    // <produci un suono fastidioso e manda un messaggio di errore>
                }
            }
        }

        private void acbComuni_GotFocus(object sender, RoutedEventArgs e)
        {
            if (_state == Step.scegliProvinciaOComune)
            {
                acbComuni.ItemsSource = _capDB.Comuni.Keys;
                _state = Step.selezionaComune;
            }
        }

        private void acbComuni_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_state == Step.selezionaProvinciaComune || _state == Step.selezionaComune)
            {
                Dictionary<string, Comune> comuniCandidati;
                
                if (_state == Step.selezionaProvinciaComune)
                {
                    comuniCandidati = _capDB.Province[_sProvinciaSelezionata];
                }
                else
                {
                    comuniCandidati = _capDB.Comuni;
                }

                if (comuniCandidati.ContainsKey(acbComuni.Text))
                {
                    _sComuneSelezionato = acbComuni.Text;
                    //_capRecords = _capDB.Province[_sProvinciaSelezionata][_sComuneSelezionato].CapRecords;
                    _capRecords = comuniCandidati[_sComuneSelezionato].CapRecords;

                    // se i CAPRecords ritornano un solo CAP ritornalo
                    var caps = from cr in _capRecords
                               select cr.Cap.CAPString;

                    int countCAP = caps.Distinct().Count();

                    if (countCAP == 1)
                    {
                        _sCapSelezionato = _capRecords[0].Cap.CAPString;
                    }
                    else
                    {
                        // bisogna selezionare o la frazione o la via...
                    }

                }
                else
                {
                    // <produci un suono fastidioso e manda un messaggio di errore>
                    int a;
                }
            }
        }


       
        private void ReadAndParseDataBase()
        {
            Dictionary<string, Dictionary<string, Comune>> dProvince = new Dictionary<string, Dictionary<string, Comune>>();
            Dictionary<string, Comune> dComuni = new Dictionary<string, Comune>();
            Dictionary<string, CAP> dCAPS = new Dictionary<string, CAP>();
            
            var resource = System.Windows.Application.GetResourceStream(new Uri("DB2.txt", UriKind.Relative));
            TextReader tr = new StreamReader(resource.Stream);

            string tmp;

            // read a line of text
            while ((tmp = tr.ReadLine()) != null)
            {
                // read a line on ASCII file, separate strings on comma char
                string[] words = tmp.Split(',');

                string sProvincia = Normalize(words[0]);

                string sComune1 = Normalize(words[1]);
                string sComune2 = Normalize(words[2]);

                string sFrazione1 = Normalize(words[3]);
                string sFrazione2 = Normalize(words[4]);
                string sFrazione = sFrazione2 == "" ? sFrazione1 : sFrazione1 + " - " + sFrazione2;

                string sToponimo1 = Normalize(words[5]);
                string sToponimo2 = Normalize(words[6]);
                string sToponimo = sToponimo2 == "" ? sToponimo1 : sToponimo1 + " - " + sToponimo2;

                string sDaugt = Normalize(words[7]);
                sToponimo = sToponimo == "" ? sToponimo : sToponimo + " (" + sDaugt + ")";

                string sCivico = Normalize(words[8]);

                string sCap = Normalize(words[9]);

                // insert CAP in CAP Dictionary if necessary
                if (!dCAPS.ContainsKey(sCap))
                {
                    dCAPS.Add(sCap, new CAP(sCap));
                }

                CAPRecord newRecord = new CAPRecord(sFrazione, sToponimo, sCivico);
                newRecord.Cap = dCAPS[sCap];

                // insert in province Dictionary if not already inserted
                if (!dProvince.ContainsKey(sProvincia))
                {
                    dProvince.Add(sProvincia, new Dictionary<string, Comune>());
                }

                // parse Comune1, Comune2 if necessary
                if (sComune2 == "")
                {
                    sComune1 = sComune1 + " (" + sProvincia + ")";
                }
                else
                {
                    string sTmp = sComune1;
                    sComune1 = sTmp + " - " + sComune2 + " (" + sProvincia + ")";
                    sComune2 = sComune2 + " - " + sTmp + " (" + sProvincia + ")";
                }

                // insert into comuni Dictionary if not already inserted, if comune name is bilungual insert two separate records
                if (!dComuni.ContainsKey(sComune1))
                {
                    if (sComune2 != "")
                    {
                        Comune c2 = new Comune();
                        c2.ComuneID = sComune2;
                        c2.CapRecords = new List<CAPRecord>();
                        dComuni.Add(c2.ComuneID, c2);

                        dProvince[sProvincia].Add(c2.ComuneID, c2);
                    }

                    Comune c1 = new Comune();
                    c1.ComuneID = sComune1;
                    c1.CapRecords = new List<CAPRecord>();
                    dComuni.Add(sComune1, c1);

                    dProvince[sProvincia].Add(c1.ComuneID, c1);
                }

                // insert CAP record into both indexes
                if (sComune2 != "")
                {
                    dComuni[sComune2].CapRecords.Add(newRecord);
                }

                dComuni[sComune1].CapRecords.Add(newRecord);
            }

            // incapsule CAP structures into the class CAPDB
            _capDB = new CAPDB(dProvince, dComuni, dCAPS);
        }

        private string Normalize(string s)
        {
            string sReturn;
            if (s == " '")
            {
                sReturn = "";
            }
            else
            {
                sReturn = s.Remove(0, 2);
                sReturn = sReturn.Remove(sReturn.Length - 1, 1);
            }

            return sReturn;
        }

        private void acbProvince_KeyDown(object sender, KeyEventArgs e)
        {
            // filter input
            if (acbProvince.Text.Length > 2)
            {
                acbProvince.Text = "";
                // <riproduci suono di errore ed un'animazione (ad esempio il controllo lampeggia di rosso)>
            }
        }

        


       

        
    }


}